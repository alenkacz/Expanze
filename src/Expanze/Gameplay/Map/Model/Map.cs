﻿using System;
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
    class Map : GameComponent
    {
        private Vector3 eye, target, up;

        Game myGame;

        float aspectRatio;

        HexaModel[][] hexaMapModel;
        MapView mapView;
        MapController mapController;

        public Map(Game game)
        {
            myGame = game;
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
            hexaMapModel = getMap();

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
            if(!PromptWindow.Inst().getIsActive() &&
               !MarketComponent.getInstance().getIsActive())
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

            mapView.Update(gameTime);
        }

        public override void HandlePickableAreas(Color c)
        {
            if (GameMaster.getInstance().getPaused())
                return;

            if (GameMaster.getInstance().getActivePlayer().getComponentAI() != null)
                return;

            mapView.HandlePickableAreas(c);
        }

        public override void DrawPickableAreas()
        {
            if (GameMaster.getInstance().getPaused())
                return;

            mapView.DrawPickableAreas();
        }

        public override void Draw2D()
        {
            mapView.Draw2D();
        }

        public override void Draw(GameTime gameTime)
        {
            //if (GameMaster.getInstance().getPaused())
            //    return;

            myGame.GraphicsDevice.BlendState = BlendState.Opaque;
            myGame.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GameState.view = Matrix.CreateLookAt(eye, target, up);

            mapView.Draw(gameTime);

            base.Draw(gameTime);
        }

        public void NextTurn()
        {
            if (mapView.getIsViewQueueClear())
            {
                ItemQueue item = new ItemQueue(ItemKind.NextTurn, 0);
                mapView.AddToViewQueue(item);
            }
        }

        public HexaModel GetHexaModel(int i, int j)
        {
            return hexaMapModel[i][j];
        }

        public Town GetTownByID(int townID)
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

        public Road GetRoadByID(int roadID)
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
    }
}
