using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CorePlugin;
using Expanze.Gameplay.Map;
using Expanze.Gameplay.Map.View;

namespace Expanze.Gameplay.Map
{
    /// <summary>
    /// Responsible for rendering game map
    /// </summary>
    class Map : GameComponent, IMapController
    {
        private Vector3 eye, target, up;

        Game myGame;
        ViewQueue viewQueue;

        float aspectRatio;

        HexaModel[][] hexaMapModel;
        HexaView[][] hexaMapView;

        public Map(Game game)
        {
            myGame = game;
            viewQueue = new ViewQueue(this);
        }

        public bool getIsViewQueueClear()
        {
            return viewQueue.getIsClear();
        }

        private void CreateHexaWorldMatrices()
        {
            HexaModel.resetCounter();
            Road.resetCounter();
            Town.resetCounter();

            float dx = 0.592f;
            float dy = 0.513f;
            Matrix mWorld = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-dy * hexaMapModel.Length / 2.0f, 0.0f, -dx * hexaMapModel[0].Length / 2.0f)); ;
            for (int i = 0; i < hexaMapModel.Length; i++)
            {
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                {
                    if (hexaMapView[i][j] != null)
                    {
                        hexaMapView[i][j].setWorld(mWorld);
                    }
                    mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx));
                }
                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(dy, 0.0f, -dx / 2.0f - dx * hexaMapModel[i].Length));
            }
        }

        private void CreateTownsAndRoads()
        {
            HexaModel[] neighboursModel = new HexaModel[6];
            HexaView[] neighboursView = new HexaView[6];

            for (int i = 0; i < hexaMapModel.Length; ++i)
            {
                  for (int j = 0; j < hexaMapModel[i].Length; ++j)
                  {
                      if (hexaMapModel[i][j] == null)
                          continue;

                      // UP LEFT
                      if (i >= 1 && hexaMapModel[i - 1].Length > j)
                      {
                          neighboursModel[(int)RoadPos.UpLeft] = hexaMapModel[i - 1][j];
                          neighboursView[(int)RoadPos.UpLeft] = hexaMapView[i - 1][j];
                      }
                      else
                      {
                          neighboursModel[(int)RoadPos.UpLeft] = null;
                          neighboursView[(int)RoadPos.UpLeft] = null;
                      }

                      // UP RIGHT
                      if (i >= 1 && j > 0 && hexaMapModel[i - 1].Length > j - 1)
                      {
                          neighboursModel[(int)RoadPos.UpRight] = hexaMapModel[i - 1][j - 1];
                          neighboursView[(int)RoadPos.UpRight] = hexaMapView[i - 1][j - 1];
                      }
                      else
                      {
                          neighboursModel[(int)RoadPos.UpRight] = null;
                          neighboursView[(int)RoadPos.UpRight] = null;
                      }

                      // LEFT
                      if (hexaMapModel[i].Length > j + 1)
                      {
                          neighboursModel[(int)RoadPos.MiddleLeft] = hexaMapModel[i][j + 1];
                          neighboursView[(int)RoadPos.MiddleLeft] = hexaMapView[i][j + 1];
                      }
                      else
                      {
                          neighboursModel[(int)RoadPos.MiddleLeft] = null;
                          neighboursView[(int)RoadPos.MiddleLeft] = null;
                      }

                      // RIGHT
                      if (j - 1 >= 0)
                      {
                          neighboursModel[(int)RoadPos.MiddleRight] = hexaMapModel[i][j - 1];
                          neighboursView[(int)RoadPos.MiddleRight] = hexaMapView[i][j - 1];
                      }
                      else
                      {
                          neighboursModel[(int)RoadPos.MiddleRight] = null;
                          neighboursView[(int)RoadPos.MiddleRight] = null;
                      }

                      // BOTTOM LEFT
                      if (i + 1 < hexaMapModel.Length && hexaMapModel[i + 1].Length > j + 1)
                      {
                          neighboursModel[(int)RoadPos.BottomLeft] = hexaMapModel[i + 1][j + 1];
                          neighboursView[(int)RoadPos.BottomLeft] = hexaMapView[i + 1][j + 1];
                      }
                      else
                      {
                          neighboursModel[(int)RoadPos.BottomLeft] = null;
                          neighboursView[(int)RoadPos.BottomLeft] = null;
                      }

                      // BOTTOM RIGHT
                      if (i + 1 < hexaMapModel.Length && hexaMapModel[i + 1].Length > j)
                      {
                          neighboursModel[(int)RoadPos.BottomRight] = hexaMapModel[i + 1][j];
                          neighboursView[(int)RoadPos.BottomRight] = hexaMapView[i + 1][j];
                      }
                      else
                      {
                          neighboursModel[(int)RoadPos.BottomRight] = null;
                          neighboursView[(int)RoadPos.BottomRight] = null;
                      }

                      hexaMapModel[i][j].CreateTownsAndRoads(neighboursModel, hexaMapView[i][j], neighboursView);
                  }
            }
        }

        public override void Initialize()
        {
            viewQueue.Clear();

            hexaMapModel = getMap();
            hexaMapView = new HexaView[hexaMapModel.Length][];
            for (int i = 0; i < hexaMapModel.Length; i++)
            {
                hexaMapView[i] = new HexaView[hexaMapModel[i].Length];
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                {
                    if (hexaMapModel[i][j] != null)
                    {
                        switch (hexaMapModel[i][j].getType())
                        {
                            case HexaKind.Mountains :
                                hexaMapView[i][j] = new MountainsView(hexaMapModel[i][j]);
                                break;
                            case HexaKind.Cornfield :
                                hexaMapView[i][j] = new CornfieldView(hexaMapModel[i][j]);
                                break;
                            case HexaKind.Stone :
                                hexaMapView[i][j] = new StoneView(hexaMapModel[i][j]);
                                break;
                            default :
                                hexaMapView[i][j] = new HexaView(hexaMapModel[i][j]);
                                break;
                        }             
                    }
                }
            }

            CreateHexaWorldMatrices(); // have to be before Create Towns and roads
            CreateTownsAndRoads();
            for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                    if (hexaMapModel[i][j] != null)
                    {
                        hexaMapModel[i][j].FindRoadNeighbours();
                        hexaMapModel[i][j].FindTownNeighbours();
                    }

            GameState.map = this;

            aspectRatio = myGame.GraphicsDevice.Viewport.Width / (float)myGame.GraphicsDevice.Viewport.Height;
            target = new Vector3(0.0f, 0.0f, 0.0f);
            up = new Vector3(0.1f, 0.8f, 0.0f);
            eye = new Vector3(0.4f, 1.5f, 0.0f);
            GameState.MaterialAmbientColor = new Vector3(0.2f, 0.2f, 0.2f);
            GameState.LightDirection = new Vector3(-1.0f, -0.5f, 0.0f);
            GameState.view = Matrix.CreateLookAt(eye, target, up);
            GameState.projection = Matrix.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(90), aspectRatio, 0.01f, 100.0f);

            GameMaster.getInstance().StartTurn();
        }

        private HexaModel[][] getMap()
        {
            MapParser parser = new MapParser();
            return parser.getMap();
        }

        public override void LoadContent()
        {

        }

        // active player gets on start of his turn sources from mining buildings
        public void getSources(Player player)
        {
            player.addSources(new SourceAll(0), TransactionState.TransactionStart);
            for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                    if (hexaMapModel[i][j] != null)
                    {
                        hexaMapModel[i][j].CollectSources(player);
                    }
            player.addSources(new SourceAll(0), TransactionState.TransactionEnd);
        }

        public void ChangeCamera()
        {
            if (GameState.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                float dx = (GameState.CurrentMouseState.X - GameState.LastMouseState.X) / 100.0f;
                eye.Z += dx;
                target.Z += dx;

                float dy = (GameState.CurrentMouseState.Y - GameState.LastMouseState.Y) / 100.0f;
                eye.X -= dy;
                target.X -= dy;
            }

            if (GameState.CurrentMouseState.ScrollWheelValue - GameState.LastMouseState.ScrollWheelValue != 0)
            {
                eye.Y += (GameState.CurrentMouseState.ScrollWheelValue - GameState.LastMouseState.ScrollWheelValue) / 500.0f;

                if (eye.Y < 0.2f)
                    eye.Y = 0.2f;
                if(eye.Y > 2.8f)
                    eye.Y = 2.8f;
            }
        }

        float lightAngle = 0;
        public void ChangeLight(GameTime gameTime)
        {
            lightAngle += gameTime.ElapsedGameTime.Milliseconds / 30.0f;
            if (lightAngle > 360.0f)
                lightAngle -= 360.0f;
            GameState.LightDirection = new Vector3(-1.0f, -0.5f, (float) Math.Cos(MathHelper.ToRadians(lightAngle)));
            float tempF = (float) Math.Abs(Math.Cos(MathHelper.ToRadians(lightAngle))) / 4.0f;
            GameState.LightDiffusionColor = new Vector3(1.0f, 1.0f - tempF, 1.0f - tempF);
            GameState.LightSpecularColor = new Vector3(0.4f, 0.2f, 0.2f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (GameMaster.getInstance().getPaused())
                return;

            ChangeCamera();
            ChangeLight(gameTime);

            viewQueue.Update(gameTime);
        }

        public override void HandlePickableAreas(Color c)
        {
            if (GameMaster.getInstance().getPaused())
                return;

            if (GameMaster.getInstance().getActivePlayer().getComponentAI() != null)
                return;

            for (int i = 0; i < hexaMapView.Length; i++)
                for (int j = 0; j < hexaMapView[i].Length; j++)
                    if (hexaMapView[i][j] != null)
                        hexaMapView[i][j].HandlePickableAreas(c);
        }

        public override void DrawPickableAreas()
        {
            if (GameMaster.getInstance().getPaused())
                return;

            for (int i = 0; i < hexaMapView.Length; i++)
                for (int j = 0; j < hexaMapView[i].Length; j++)
                    if (hexaMapView[i][j] != null)
                        hexaMapView[i][j].DrawPickableAreas();
        }

        public override void Draw2D()
        {
            //if (GameMaster.getInstance().getPaused())
            //    return;

            for (int i = 0; i < hexaMapView.Length; i++)
            {
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    if (hexaMapView[i][j] != null)
                    {
                        hexaMapView[i][j].Draw2D();
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //if (GameMaster.getInstance().getPaused())
            //    return;

            myGame.GraphicsDevice.BlendState = BlendState.Opaque;
            myGame.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GameState.view = Matrix.CreateLookAt(eye, target, up);

            for (int i = 0; i < hexaMapView.Length; i++)
            {
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    if (hexaMapView[i][j] != null)
                    {
                        hexaMapView[i][j].Draw(gameTime);
                        //hexaMapView[i][j].DrawPickableAreas();
                    }
                }
            }

            base.Draw(gameTime);
        }

        public void NextTurn()
        {
            if (viewQueue.getIsClear())
            {
                ItemQueue item = new ItemQueue(ItemKind.NextTurn, 0);
                viewQueue.Add(item);
            }
        }

        private TownView GetTownViewByID(int townID)
        {
            TownView town = null;
            for (int i = 0; i < hexaMapView.Length; i++)
                for (int j = 0; j < hexaMapView[i].Length; j++)
                    if (hexaMapView[i][j] != null)
                    {
                        town = hexaMapView[i][j].GetTownByID(townID);
                        if (town != null)
                            return town;
                    }
            return null;
        }

        private RoadView GetRoadViewByID(int roadID)
        {
            RoadView road = null;
            for (int i = 0; i < hexaMapView.Length; i++)
                for (int j = 0; j < hexaMapView[i].Length; j++)
                    if (hexaMapView[i][j] != null)
                    {
                        road = hexaMapView[i][j].GetRoadViewByID(roadID);
                        if (road != null)
                            return road;
                    }
            return null;
        }

        private Town GetTownByID(int townID)
        {
            Town town = null;
            for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                    if (hexaMapModel[i][j] != null)
                    {
                        town= hexaMapModel[i][j].GetTownByID(townID);
                        if (town != null)
                            return town;
                    }
            return null;
        }

        public ITownGet GetITownGetByID(int townID)
        {
            return GetTownByID(townID);
        }

        public IRoadGet GetIRoadGetByID(int roadID)
        {
            return GetRoadByID(roadID);
        }

        private Road GetRoadByID(int roadID)
        {
            Road road = null;
            for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                    if (hexaMapModel[i][j] != null)
                    {
                        road = hexaMapModel[i][j].GetRoadByID(roadID);
                        if (road != null)
                            return road;
                    }
            return null;
        }

        // View building //
        /// ///////////////

        public void BuildTownView(int townID)
        {
            TownView townView = GetTownViewByID(townID);
            townView.setIsBuild(true);
        }

        public void BuildRoadView(int roadID)
        {
            RoadView roadView = GetRoadViewByID(roadID);
            roadView.setIsBuild(true);
        }

        /// ********************************
        /// IMapController

        public int GetMaxRoadID() { return Road.getRoadCount(); }
        public int GetMaxTownID() { return Town.getTownCount(); }
        public EGameState GetState() { return GameMaster.getInstance().getState(); }

        public IHexaGet GetHexa(int x, int y)
        {
            return hexaMapModel[x][y];
        }

        public void BuyUpgradeInSpecialBuilding(int townID, int hexaID, UpgradeKind upgradeKind, int upgradeNumber)
        {
            GameMaster gm = GameMaster.getInstance();
            Town town = GetTownByID(townID);
            SpecialBuilding building = town.getSpecialBuilding(hexaID);
            building.BuyUpgrade(upgradeKind, upgradeNumber);
            gm.getActivePlayer().payForSomething(building.getUpgradeCost(upgradeKind, upgradeNumber));
        }

        public RoadBuildError BuildRoad(int roadID)
        {
            Road road = GetRoadByID(roadID);

            GameMaster gm = GameMaster.getInstance();
            RoadBuildError error = road.CanActivePlayerBuildRoad();
            if (error == RoadBuildError.OK)
            {
                road.BuildRoad(gm.getActivePlayer());

                ItemQueue item = new ItemQueue(ItemKind.BuildRoad, roadID);
                viewQueue.Add(item);

                gm.getActivePlayer().payForSomething(Settings.costRoad);
            }

            return error;
        }

        public TownBuildError BuildTown(int townID)
        {
            Town town = GetTownByID(townID);

            GameMaster gm = GameMaster.getInstance();
            TownBuildError error = town.CanActivePlayerBuildTown();
            if (error == TownBuildError.OK)
            {
                town.BuildTown(gm.getActivePlayer());

                ItemQueue item = new ItemQueue(ItemKind.BuildTown, townID);
                viewQueue.Add(item);

                if (gm.getState() != EGameState.StateGame)
                {
                    SourceAll source = new SourceAll(0);
                    HexaModel hexa;
                    gm.getActivePlayer().addSources(new SourceAll(0), TransactionState.TransactionStart);
                    for(int loop1 = 0; loop1 < 3; loop1++)
                    {
                        if ((hexa = town.getHexa(loop1)) != null)
                        {
                            switch(hexa.getType())
                            {
                                case HexaKind.Cornfield :
                                    source = Settings.costMill; break;
                                case HexaKind.Forest:
                                    source = Settings.costSaw; break;
                                case HexaKind.Mountains:
                                    source = Settings.costMine; break;
                                case HexaKind.Pasture:
                                    source = Settings.costStephard; break;
                                case HexaKind.Stone:
                                    source = Settings.costQuarry; break;
                                default :
                                    source = new SourceAll(0); break;
                            }
                            gm.getActivePlayer().addSources(source,TransactionState.TransactionMiddle);
                        }
                    }
                    gm.getActivePlayer().addSources(new SourceAll(0), TransactionState.TransactionEnd);

                    if(!gm.getActivePlayer().getIsAI())
                        gm.NextTurn();
                }
                else
                    gm.getActivePlayer().payForSomething(Settings.costTown);
            }

            return error;
        }

        public BuildingBuildError buildBuildingInTown(int townID, int hexaID, BuildingKind kind)
        {
            GameMaster gm = GameMaster.getInstance();
            Town town = GetTownByID(townID);
            int buildingPos = town.findBuildingByHexaID(hexaID);
            HexaModel hexa = town.getHexa(buildingPos);

            SourceAll buildingCost = new SourceAll(0);
            switch (kind)
            {
                case BuildingKind.SourceBuilding:
                    buildingCost = hexa.getSourceBuildingCost();
                    break;

                case BuildingKind.FortBuilding:
                    buildingCost = Settings.costFort;
                    break;
            }

            BuildingBuildError error = town.canActivePlayerBuildBuildingInTown(buildingPos, buildingCost);
            if (error == BuildingBuildError.OK)
            {
                town.buildBuilding(buildingPos, kind);
                gm.getActivePlayer().payForSomething(buildingCost);
            }

            return error;
        }

        /// IHexaGet
        /// ********************************

        public static void SetPickVariables(Boolean isPickColor, PickVariables pickVars)
        {
            pickVars.wasClickAnywhereLast = pickVars.wasClickAnywhereNow;
            pickVars.wasClickAnywhereNow = GameState.CurrentMouseState.LeftButton == ButtonState.Pressed;
            if (isPickColor)
            {
                if (!pickVars.pickActive)
                    pickVars.pickNewActive = true;
                pickVars.pickActive = true;

                if (GameState.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!pickVars.wasClickAnywhereLast && !pickVars.pickPress)
                        pickVars.pickNewPress = true;
                    pickVars.pickPress = true;
                }
                else
                {
                    if (pickVars.pickPress)
                        pickVars.pickNewRelease = true;
                    pickVars.pickPress = false;
                }
            }
            else
            {
                pickVars.pickActive = false;
                pickVars.pickPress = false;
                pickVars.pickNewActive = false;
                pickVars.pickNewPress = false;
                pickVars.pickNewRelease = false;
            }
        }
    }
}
