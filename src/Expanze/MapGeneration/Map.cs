using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CorePlugin;

namespace Expanze
{
    /// <summary>
    /// Responsible for rendering game map
    /// </summary>
    class Map : GameComponent, IMapController
    {
        const int N_MODEL = 7;
        Model[] hexaModel;
        Model rectangleShape, circleShape;
        Model townModel, roadModel;

        public Vector3 eye, target, up;

        Game myGame;
        ContentManager content;

        float aspectRatio;

        Hexa[][] hexaMap;

        public Map(Game game)
        {
            myGame = game;
        }

        private void CreateHexaWorldMatrices()
        {
            Hexa.resetCounter();
            Road.resetCounter();
            Town.resetCounter();

            Matrix mWorld = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-0.49f * hexaMap.Length / 2.0f, 0.0f, -0.56f * hexaMap[0].Length / 2.0f)); ;
            for (int i = 0; i < hexaMap.Length; i++)
            {
                for (int j = 0; j < hexaMap[i].Length; j++)
                {
                    if (hexaMap[i][j] != null)
                    {
                        hexaMap[i][j].setWorld(mWorld);
                    }
                    mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, 0.56f));
                }
                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.49f, 0.0f, -0.28f - 0.56f * hexaMap[i].Length));
            }
        }

        private void CreateTownsAndRoads()
        {
            Hexa[] neighbours = new Hexa[6];

            for (int i = 0; i < hexaMap.Length; ++i)
            {
                  for (int j = 0; j < hexaMap[i].Length; ++j)
                  {
                      if (hexaMap[i][j] == null)
                          continue;

                      // UP LEFT
                      if (i >= 1 && hexaMap[i - 1].Length > j)
                          neighbours[(int)RoadPos.UpLeft] = hexaMap[i - 1][j];
                      else
                          neighbours[(int)RoadPos.UpLeft] = null;

                      // UP RIGHT
                      if (i >= 1 && j > 0 && hexaMap[i - 1].Length > j - 1)
                          neighbours[(int)RoadPos.UpRight] = hexaMap[i - 1][j - 1];
                      else
                          neighbours[(int)RoadPos.UpRight] = null;

                      // LEFT
                      if (hexaMap[i].Length > j + 1)
                          neighbours[(int)RoadPos.MiddleLeft] = hexaMap[i][j + 1];
                      else
                          neighbours[(int)RoadPos.MiddleLeft] = null;

                      // RIGHT
                      if (j - 1 >= 0)
                          neighbours[(int)RoadPos.MiddleRight] = hexaMap[i][j - 1];
                      else
                          neighbours[(int)RoadPos.MiddleRight] = null;

                      // BOTTOM LEFT
                      if (i + 1 < hexaMap.Length && hexaMap[i + 1].Length > j + 1)
                          neighbours[(int)RoadPos.BottomLeft] = hexaMap[i + 1][j + 1];
                      else
                          neighbours[(int)RoadPos.BottomLeft] = null;

                      // BOTTOM RIGHT
                      if (i + 1 < hexaMap.Length && hexaMap[i + 1].Length > j)
                          neighbours[(int)RoadPos.BottomRight] = hexaMap[i + 1][j];
                      else
                          neighbours[(int)RoadPos.BottomRight] = null;

                      hexaMap[i][j].CreateTownsAndRoads(neighbours);
                  }
            }
        }

        public override void Initialize()
        {
            hexaMap = getMap();
            CreateHexaWorldMatrices(); // have to be before Create Towns and roads
            CreateTownsAndRoads();
            for (int i = 0; i < hexaMap.Length; i++)
                for (int j = 0; j < hexaMap[i].Length; j++)
                    if (hexaMap[i][j] != null)
                    {
                        hexaMap[i][j].FindRoadNeighbours();
                        hexaMap[i][j].FindTownNeighbours();
                    }

            GameState.map = this;

            aspectRatio = myGame.GraphicsDevice.Viewport.Width / (float)myGame.GraphicsDevice.Viewport.Height;
            target = new Vector3(0.0f, 0.0f, 0.0f);
            up = new Vector3(0.1f, 0.8f, 0.0f);
            eye = new Vector3(0.4f, 1.5f, 0.0f);
            GameState.view = Matrix.CreateLookAt(eye, target, up);
            GameState.projection = Matrix.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(90), aspectRatio, 0.01f, 100.0f);
        }

        private Hexa[][] getMap()
        {
            MapParser parser = new MapParser();
            return parser.getMap();
        }

        public Model getHexaModel(HexaType type)
        {
            return hexaModel[(int)type];
        }

        public Model getTownModel() { return townModel; }
        public Model getRoadModel() { return roadModel; }

        public Model getRectangleShape()
        {
            return rectangleShape;
        }

        public Model getCircleShape()
        {
            return circleShape;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(myGame.Services, "Content");

            hexaModel = new Model[N_MODEL];
            hexaModel[(int)HexaType.Cornfield] = content.Load<Model>(Settings.mapPaths[(int) HexaType.Cornfield]);
            hexaModel[(int)HexaType.Desert] = content.Load<Model>(Settings.mapPaths[(int) HexaType.Desert]);
            hexaModel[(int)HexaType.Forest] = content.Load<Model>(Settings.mapPaths[(int)HexaType.Forest]);
            hexaModel[(int)HexaType.Mountains] = content.Load<Model>(Settings.mapPaths[(int)HexaType.Mountains]);
            hexaModel[(int)HexaType.Pasture] = content.Load<Model>(Settings.mapPaths[(int)HexaType.Pasture]);
            hexaModel[(int)HexaType.Stone] = content.Load<Model>(Settings.mapPaths[(int)HexaType.Stone]);
            hexaModel[(int)HexaType.Water] = content.Load<Model>(Settings.mapPaths[(int)HexaType.Water]);
            rectangleShape = content.Load<Model>("Shapes/rectangle");
            circleShape = content.Load<Model>("Shapes/circle");
            townModel = content.Load<Model>("Models/town");
            roadModel = content.Load<Model>("Models/road");
        }

        // active player gets on start of his turn sources from mining buildings
        public void getSources(Player player)
        {
            for (int i = 0; i < hexaMap.Length; i++)
                for (int j = 0; j < hexaMap[i].Length; j++)
                    if (hexaMap[i][j] != null)
                    {
                        hexaMap[i][j].CollectSources(player);
                    }
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

                if (eye.Y < 0.1f)
                    eye.Y = 0.1f;
                if(eye.Y > 2.8f)
                    eye.Y = 2.8f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (GameMaster.getInstance().getPaused())
                return;

            ChangeCamera();
        }

        public override void HandlePickableAreas(Color c)
        {
            if (GameMaster.getInstance().getPaused() || GameMaster.getInstance().getActivePlayer().getIsAI())
                return;

            for (int i = 0; i < hexaMap.Length; i++)
                for (int j = 0; j < hexaMap[i].Length; j++)
                    if (hexaMap[i][j] != null)
                        hexaMap[i][j].HandlePickableAreas(c);
        }

        public override void DrawPickableAreas()
        {
            if (GameMaster.getInstance().getPaused())
                return;

            for (int i = 0; i < hexaMap.Length; i++)
                for (int j = 0; j < hexaMap[i].Length; j++)
                    if (hexaMap[i][j] != null)
                        hexaMap[i][j].DrawPickableAreas();
        }

        public override void Draw2D()
        {
            //if (GameMaster.getInstance().getPaused())
            //    return;

            for (int i = 0; i < hexaMap.Length; i++)
            {
                for (int j = 0; j < hexaMap[i].Length; j++)
                {
                    if (hexaMap[i][j] != null)
                    {
                        hexaMap[i][j].Draw2D();
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

            for (int i = 0; i < hexaMap.Length; i++)
            {
                for (int j = 0; j < hexaMap[i].Length; j++)
                {
                    if (hexaMap[i][j] != null)
                    {
                        hexaMap[i][j].Draw(gameTime);
                        //hexaMap[i][j].DrawPickableAreas();
                    }
                }
            }

            base.Draw(gameTime);
        }


        private Town GetTownByID(int townID)
        {
            Town town = null;
            for (int i = 0; i < hexaMap.Length; i++)
                for (int j = 0; j < hexaMap[i].Length; j++)
                    if (hexaMap[i][j] != null)
                    {
                        town= hexaMap[i][j].GetTownByID(townID);
                        if (town != null)
                            return town;
                    }
            return null;
        }

        /// ********************************
        /// IMapController

        public EGameState GetState() { return GameMaster.getInstance().getState(); }

        public IHexaGet GetHexa(int x, int y)
        {
            return hexaMap[x][y];
        }


        public bool BuildTown(int townID)
        {
            Town town = GetTownByID(townID);

            GameMaster gm = GameMaster.getInstance();
            if (town.CanActivePlayerBuildTown())
            {
                town.BuildTown(gm.getActivePlayer());
                if (gm.getState() != EGameState.StateGame)
                {
                    gm.nextTurn();
                }
                else
                    gm.getActivePlayer().payForSomething(Settings.costTown);
                return true;
            }

            return false;
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
                    if (!pickVars.wasClickAnywhereLast)
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
