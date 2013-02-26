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
using Expanze.Utils;
using Expanze.Utils.Music;

namespace Expanze.Gameplay.Map
{
    /// <summary>
    /// Responsible for rendering game map
    /// </summary>
    class Map : GameComponent
    {
        private Vector3 eye, target, up;

        Game game;

        HexaModel[][] hexaMapModel;
        MapView mapView;
        MapController mapController;

        public Map(Game game)
        {
            this.game = game;
            mapView = new MapView(this);
            mapController = new MapController(this, mapView);
        }

        public MapController GetMapController() { return mapController; }
        public MapView GetMapView() { return mapView; }
        public HexaModel[][] GetHexaMapModel() { return hexaMapModel; }

        private void CreateHexaWorldMatrices()
        {
            mapView.CreateHexaWorldMatrices();
        }

        private void CreateTownsAndRoads()
        {
            HexaModel[] neighboursModel = new HexaModel[6];
            HexaView[] neighboursView = new HexaView[6];

            HexaView[][] hexaMapView = mapView.GetHexaMapView();

            for (int i = 0; i < hexaMapModel.Length; ++i)
            {
                  for (int j = 0; j < hexaMapModel[i].Length; ++j)
                  {
                      if (hexaMapModel[i][j] == null)
                          continue;

                      hexaMapModel[i][j].SetCoord(i, j);

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
                      mapView.GetHexaMapView()[i][j].SetHexaNeighbours(neighboursView);
                  }
            }
        }

        public override void Initialize()
        {
            Initialize(true);
        }

        public void Initialize(bool newMap)
        {
            HexaModel.ResetCounter();
            RoadModel.ResetCounter();
            TownModel.ResetCounter();

            hexaMapModel = GetMap(newMap);
            
            mapView.Initialize();

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

            target = new Vector3(0.0f, 0.0f, 0.0f);
            up = new Vector3(0.1f, 0.8f, 0.0f);
            eye = new Vector3(0.4f, 1.5f, 0.0f);
            GameState.SecretAmbientColor = new Vector3(0.0f, 0.0f, 0.0f);
            GameState.MaterialAmbientColor = new Vector3(0.1f, 0.1f, 0.1f);
            GameState.LightDirection = new Vector3(-1.0f, -0.5f, 0.0f);
            GameState.view = Matrix.CreateLookAt(eye, target, up);
            float aspectRatio = game.GraphicsDevice.Viewport.Width / (float)game.GraphicsDevice.Viewport.Height;
            GameState.projection = Matrix.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(90), aspectRatio, 0.01f, 100.0f);

            mapController.Init();
        }

        MapParser parser;

        private HexaModel[][] GetMap(bool newMap)
        {
            if(parser == null || newMap)
              parser = new MapParser();
            GameSettings gs = GameMaster.Inst().GetGameSettings();
            if (GameMaster.Inst().GetMapSource() == null)
            {
                HexaModel[][] tempMap = MapGenerator.GenerateScenarioMap(gs);
                GameMaster.Inst().InitPlayers();
                return tempMap;
            }
            else
                return parser.parseCampaignMap(GameMaster.Inst().GetMapSource());
        }

        public override void LoadContent()
        {

        }

        // active player gets on start of his turn sources from mining buildings
        public void getSources(Player player)
        {
            player.AddSources(new SourceAll(0), TransactionState.TransactionStart);
            player.ClearCollectSources();
            for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                    if (hexaMapModel[i][j] != null)
                    {
                        hexaMapModel[i][j].CollectSources(player);
                    }
            player.AddSources(new SourceAll(0), TransactionState.TransactionEnd);
        }

        public void ChangeCamera(GameTime gameTime)
        {
            {

                if (GameState.CurrentMouseState.RightButton == ButtonState.Pressed)
                {
                    float dx = (GameState.CurrentMouseState.X - GameState.LastMouseState.X) / 3000.0f * gameTime.ElapsedGameTime.Milliseconds;
                    eye.Z += dx;
                    target.Z += dx;

                    float dy = (GameState.CurrentMouseState.Y - GameState.LastMouseState.Y) / 3000.0f * gameTime.ElapsedGameTime.Milliseconds;
                    eye.X -= dy;
                    target.X -= dy;
                }

                float cameraVelAcc = 700.0f;

                if (InputManager.Inst().GetGameAction("game", "cameraleft").IsPressed())
                {
                    float dx = gameTime.ElapsedGameTime.Milliseconds / cameraVelAcc;
                    eye.Z += dx;
                    target.Z += dx;
                }
                else if (InputManager.Inst().GetGameAction("game", "cameraright").IsPressed())
                {
                    float dx = gameTime.ElapsedGameTime.Milliseconds / cameraVelAcc;
                    eye.Z -= dx;
                    target.Z -= dx;
                }

                if (InputManager.Inst().GetGameAction("game", "cameraup").IsPressed())
                {
                    float dy = gameTime.ElapsedGameTime.Milliseconds / cameraVelAcc;
                    eye.X -= dy;
                    target.X -= dy;
                } else
                    if (InputManager.Inst().GetGameAction("game", "cameradown").IsPressed())
                {
                    float dy = gameTime.ElapsedGameTime.Milliseconds / cameraVelAcc;
                    eye.X += dy;
                    target.X += dy;
                }
            }

            float yMax = 2.4f;
            //float yMin = 0.2f;


            // dolu mapa
            float eyeBonus = hexaMapModel.Length * 0.04f;
            if (eye.X > 0.6f + (yMax - eye.Y) / 2 + eyeBonus)
            {
                target.X = 0.2f + (yMax - eye.Y) / 2 + eyeBonus;
                eye.X = 0.5999f + (yMax - eye.Y) / 2 + eyeBonus;
            }

            // nahoru
            if (eye.X < -1.1f - (yMax - eye.Y))
            {
                target.X = -1.5f - (yMax - eye.Y);
                eye.X = -1.1f - (yMax - eye.Y);
            }

            int max = 0;
            int maxID = 0;
            for (int loop1 = 0; loop1 < hexaMapModel.Length; loop1++)
                if (hexaMapModel[loop1].Length > max)
                {
                    max = hexaMapModel[loop1].Length;
                    maxID = loop1;
                }
            // vpravo
            eyeBonus = hexaMapModel[maxID].Length * 0.06f;
            if (eye.Z > 0.4 + (yMax - eye.Y) + eyeBonus)
            {
                eye.Z = 0.4f + (yMax - eye.Y) + eyeBonus;
                target.Z = 0.4f + (yMax - eye.Y + eyeBonus);
            }
            if (eye.Z < -0.4 - (yMax - eye.Y) - eyeBonus)
            {
                eye.Z = -0.4f - (yMax - eye.Y) - eyeBonus;
                target.Z = -0.4f - (yMax - eye.Y) - eyeBonus;
            }
            //target = new Vector3(0.0f, 0.0f, 0.0f);
            //eye = new Vector3(0.4f, 1.5f, 0.0f);

            if (GameState.CurrentMouseState.ScrollWheelValue - GameState.LastMouseState.ScrollWheelValue != 0)
            {
                eye.Y -= (GameState.CurrentMouseState.ScrollWheelValue - GameState.LastMouseState.ScrollWheelValue) / 6000.0f * gameTime.ElapsedGameTime.Milliseconds;
            }
            if (InputManager.Inst().GetGameAction("game", "cameratop").IsPressed())
            {
                eye.Y += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            }
            if (InputManager.Inst().GetGameAction("game", "camerabottom").IsPressed())
            {
                eye.Y -= gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            }

            if (eye.Y < 0.2f)
                eye.Y = 0.2f;
            if (eye.Y > 2.4f)
                eye.Y = 2.4f;

            GameState.view = Matrix.CreateLookAt(eye, target, up);
        }

        float lightAngle = 0;
        public void ChangeLight(GameTime gameTime)
        {
            lightAngle += gameTime.ElapsedGameTime.Milliseconds / 450.0f;
            if (lightAngle > 360.0f)
                lightAngle -= 360.0f;

            float tempAngle = lightAngle;
            if (tempAngle > 180.0f)
                tempAngle -= 180.0f;
            GameState.SunHeight = (1.0f - Math.Abs(tempAngle - 90) / 90.0f);
            GameState.LightDirection = new Vector3(-1.0f, -0.49f - GameState.SunHeight / 5.0f, (float)Math.Cos(MathHelper.ToRadians(lightAngle)) * 1.4f);
            GameState.ShadowDirection = new Vector3(-1.0f, -0.9f - GameState.SunHeight / 2.0f, GameState.LightDirection.Z);
            GameState.ShadowDirection.Normalize();
            float tempF = (float) Math.Abs(Math.Cos(MathHelper.ToRadians(lightAngle))) / 4.0f;
            GameState.LightDiffusionColor = new Vector3(0.99f, 0.99f - tempF, 0.99f - tempF);
            GameState.LightSpecularColor = new Vector3(0.4f, 0.2f, 0.2f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (GameMaster.Inst().GetPaused())
                return;

            ChangeCamera(gameTime);
            ChangeLight(gameTime);
            ChangeVolume(gameTime);

            mapView.Update(gameTime);

            for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                    if (hexaMapModel[i][j] != null)
                        hexaMapModel[i][j].Update(gameTime);
        }

        int showMusic = 0;
        int showSound = 0;
        private void ChangeVolume(GameTime gameTime)
        {
            showMusic -= gameTime.ElapsedGameTime.Milliseconds;
            showSound -= gameTime.ElapsedGameTime.Milliseconds;
            int time = 1000;

            if (showMusic < 0)
            {
                Settings.showMusicVolume = false;
                showMusic = 0;
            }

            if (showSound < 0)
            {
                Settings.showSoundVolume = false;
                showSound = 0;
            }

            if (InputManager.Inst().GetGameAction("game", "sounddown").IsPressed())
            {
                Settings.soundVolume -= gameTime.ElapsedGameTime.Milliseconds * 0.3f;
                if (Settings.soundVolume < 0.0f)
                    Settings.soundVolume = 0.0f;

                showSound = time;
                Settings.showSoundVolume = true;
            }
            if (InputManager.Inst().GetGameAction("game", "musicdown").IsPressed())
            {
                Settings.musicVolume -= gameTime.ElapsedGameTime.Milliseconds * 0.3f;
                if (Settings.musicVolume < 0.0f)
                    Settings.musicVolume = 0.0f;

                Settings.showMusicVolume = true;
                showMusic = time;

                MusicManager.Inst().SetMusicVolume(Settings.musicVolume);
            }

            if (InputManager.Inst().GetGameAction("game", "soundup").IsPressed())
            {
                Settings.soundVolume += gameTime.ElapsedGameTime.Milliseconds * 0.3f;
                if (Settings.soundVolume > 1000.0f)
                    Settings.soundVolume = 1000.0f;

                showSound = time;
                Settings.showSoundVolume = true;
            }

            if (InputManager.Inst().GetGameAction("game", "musicup").IsPressed())
            {
                Settings.musicVolume += gameTime.ElapsedGameTime.Milliseconds * 0.3f;
                if (Settings.musicVolume > 1000.0f)
                    Settings.musicVolume = 1000.0f;

                Settings.showMusicVolume = true;
                showMusic = time;

                MusicManager.Inst().SetMusicVolume(Settings.musicVolume);
            }
        }

        public override void HandlePickableAreas(Color c)
        {
            if (GameMaster.Inst().GetPaused())
                return;

            if (GameMaster.Inst().GetActivePlayer().GetComponentAI() != null)
                return;

            mapView.HandlePickableAreas(c);
        }

        public override void DrawPickableAreas()
        {
            if (GameMaster.Inst().GetPaused())
                return;

            mapView.DrawPickableAreas();
        }

        public override void Draw2D()
        {
            mapView.Draw2D();
            GameMaster.Inst().Draw2D();
        }

        public override void Draw(GameTime gameTime)
        {
            //if (GameMaster.getInstance().getPaused())
            //    return;

            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            mapView.Draw(gameTime);

            base.Draw(gameTime);
        }

        public void NextTurn()
        {
            if (mapView.getIsViewQueueClear())
            {
                ItemQueue item = new ItemQueue(mapView);
                mapView.AddToViewQueue(item);
            }

            for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                    if (hexaMapModel[i][j] != null)
                        hexaMapModel[i][j].NextTurn();
        }

        public HexaModel GetHexaModel(int i, int j)
        {
            if (i >= 0 && i < hexaMapModel.Length &&
               j >= 0 && j < hexaMapModel[i].Length)
                return hexaMapModel[i][j];
            else
                return null;
        }

        public HexaModel GetHexaByID(int hexaID)
        {
            for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                    if (hexaMapModel[i][j] != null && hexaMapModel[i][j].GetID() == hexaID)
                    {
                        return hexaMapModel[i][j];
                    }
            return null;
        }

        public TownModel GetTownByID(int townID)
        {
            TownModel town = null;
            if (hexaMapModel == null)
                return null;

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

        public RoadModel GetRoadByID(int roadID)
        {
            RoadModel road = null;
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

        

        /// ********************************
        /// IMapController

        

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

        public void ApplyEvent(RndEvent rndEvent)
        {
           for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                    if (hexaMapModel[i][j] != null)
                    {
                        hexaMapModel[i][j].ApplyEvent(rndEvent);
                    }
        }

        internal void FreeCapturedHexa(Player activePlayer)
        {
            for (int i = 0; i < hexaMapModel.Length; i++)
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                {
                    if (hexaMapModel[i][j] != null)
                    {
                        if (activePlayer == hexaMapModel[i][j].GetCapturedPlayer())
                        {
                            hexaMapModel[i][j].SetFree();
                        }
                    }
                }
        }
    }
}
