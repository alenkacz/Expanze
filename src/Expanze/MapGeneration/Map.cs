using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Expanze
{
    /// <summary>
    /// Responsible for rendering game map
    /// </summary>
    class Map : GameComponent
    {
        const int N_MODEL = 7;
        Model[] hexaModel;
        Model rectangleShape, circleShape;

        public Vector3 eye, target, up;
        public float angle;

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
            Matrix mWorld = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-0.50f * hexaMap.Length / 2.0f, 0.0f, -0.56f * hexaMap[0].Length / 2.0f)); ;
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
                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.50f, 0.0f, -0.28f - 0.56f * hexaMap[i].Length));
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
                          neighbours[(int)Hexa.RoadPos.UpLeft] = hexaMap[i - 1][j];
                      else
                          neighbours[(int)Hexa.RoadPos.UpLeft] = null;

                      // UP RIGHT
                      if (i >= 1 && j > 0 && hexaMap[i - 1].Length > j - 1)
                          neighbours[(int)Hexa.RoadPos.UpRight] = hexaMap[i - 1][j - 1];
                      else
                          neighbours[(int)Hexa.RoadPos.UpRight] = null;

                      // LEFT
                      if (hexaMap[i].Length > j + 1)
                          neighbours[(int)Hexa.RoadPos.MiddleLeft] = hexaMap[i][j + 1];
                      else
                          neighbours[(int)Hexa.RoadPos.MiddleLeft] = null;

                      // RIGHT
                      if (j - 1 >= 0)
                          neighbours[(int)Hexa.RoadPos.MiddleRight] = hexaMap[i][j - 1];
                      else
                          neighbours[(int)Hexa.RoadPos.MiddleRight] = null;

                      // BOTTOM LEFT
                      if (i + 1 < hexaMap.Length && hexaMap[i + 1].Length > j + 1)
                          neighbours[(int)Hexa.RoadPos.BottomLeft] = hexaMap[i + 1][j + 1];
                      else
                          neighbours[(int)Hexa.RoadPos.BottomLeft] = null;

                      // BOTTOM RIGHT
                      if (i + 1 < hexaMap.Length && hexaMap[i + 1].Length > j)
                          neighbours[(int)Hexa.RoadPos.BottomRight] = hexaMap[i + 1][j];
                      else
                          neighbours[(int)Hexa.RoadPos.BottomRight] = null;

                      hexaMap[i][j].CreateTownsAndRoads(neighbours);
                  }
            }
        }

        public override void Initialize()
        {
            hexaMap = getMap();
            CreateTownsAndRoads();
            CreateHexaWorldMatrices();
            Hexa.setMap(this);

            aspectRatio = myGame.GraphicsDevice.Viewport.Width / (float)myGame.GraphicsDevice.Viewport.Height;
            target = new Vector3(0.0f, 0.0f, 0.0f);
            up = new Vector3(0.0f, 1.0f, 0.0f);
            eye = new Vector3(0.4f, 1.5f, 0.0f);
            GameState.view = Matrix.CreateLookAt(eye, target, up);
            GameState.projection = Matrix.CreatePerspectiveFieldOfView((float)MathHelper.ToRadians(90), aspectRatio, 1.0f, 100.0f);
        }

        private Hexa[][] getMap()
        {
            MapParser parser = new MapParser();
            return parser.getMap();
        }

        public Model getHexaModel(Settings.Types type)
        {
            return hexaModel[(int)type];
        }

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
            hexaModel[(int)Settings.Types.Cornfield] = content.Load<Model>(Settings.mapPaths[(int) Settings.Types.Cornfield]);
            hexaModel[(int)Settings.Types.Desert] = content.Load<Model>(Settings.mapPaths[(int) Settings.Types.Desert]);
            hexaModel[(int)Settings.Types.Forest] = content.Load<Model>(Settings.mapPaths[(int)Settings.Types.Forest]);
            hexaModel[(int)Settings.Types.Mountains] = content.Load<Model>(Settings.mapPaths[(int)Settings.Types.Mountains]);
            hexaModel[(int)Settings.Types.Pasture] = content.Load<Model>(Settings.mapPaths[(int)Settings.Types.Pasture]);
            hexaModel[(int)Settings.Types.Stone] = content.Load<Model>(Settings.mapPaths[(int)Settings.Types.Stone]);
            hexaModel[(int)Settings.Types.Water] = content.Load<Model>(Settings.mapPaths[(int)Settings.Types.Water]);
            rectangleShape = content.Load<Model>("Shapes/rectangle");
            circleShape = content.Load<Model>("Shapes/circle");
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
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ChangeCamera();

            //angle = angle + gameTime.ElapsedGameTime.Milliseconds / 1000.0f * 1.0f;
            if (angle > 360.0f)
                angle -= 360.0f;
        }

        public override void HandlePickableAreas(Color c)
        {
            for (int i = 0; i < hexaMap.Length; i++)
                for (int j = 0; j < hexaMap[i].Length; j++)
                    if (hexaMap[i][j] != null)
                        hexaMap[i][j].HandlePickableAreas(c);
        }

        public override void DrawPickableAreas()
        {
            for (int i = 0; i < hexaMap.Length; i++)
                for (int j = 0; j < hexaMap[i].Length; j++)
                    if (hexaMap[i][j] != null)
                        hexaMap[i][j].DrawPickableAreas();
        }

        public override void Draw(GameTime gameTime)
        {
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
                    }
                }
            }

            base.Draw(gameTime);
        }
    }
}
