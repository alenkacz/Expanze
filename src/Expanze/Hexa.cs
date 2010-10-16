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
    class Hexa : GameComponent
    {
        const int N_MODEL = 7;
        Model[] hexaModel;
        Matrix[] world;

        public Matrix view;
        public Matrix projection;
        public Vector3 eye, target, up;
        public float angle;

        Game myGame;
        ContentManager content;

        float aspectRatio;

        public Hexa(Game game)
        {
            myGame = game;
        }

        public override void Initialize()
        {
            aspectRatio = myGame.GraphicsDevice.Viewport.Width / (float)myGame.GraphicsDevice.Viewport.Height;
            eye = new Vector3(-5.0f, -5.0f, 0.0f);
            target = new Vector3(0.0f, 0.0f, 0.0f);
            up = new Vector3(0.0f, 1.0f, 0.0f);
            angle = 0.0f;
            view = Matrix.CreateLookAt(eye, target, up);

            world = new Matrix[N_MODEL];
            world[0] = Matrix.Identity;
            Vector3 t = new Vector3(0.51f, 0.0f, 0.28f);
            world[1] = Matrix.CreateTranslation(t);
            t = new Vector3(-0.51f, 0.0f, 0.28f);
            world[2] = Matrix.CreateTranslation(t);
            t = new Vector3(0.51f, 0.0f, -0.28f);
            world[3] = Matrix.CreateTranslation(t);
            t = new Vector3(-0.51f, 0.0f, -0.28f);
            world[4] = Matrix.CreateTranslation(t);
            t = new Vector3(0.0f, 0.0f, -0.55f);
            world[5] = Matrix.CreateTranslation(t) * Matrix.CreateScale(0.3f);
            t = new Vector3(0.0f, 0.0f, 0.55f);
            world[6] = Matrix.CreateTranslation(t);
            projection = Matrix.CreatePerspectiveFieldOfView((float) MathHelper.ToRadians(90), aspectRatio, 1.0f, 100.0f);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(myGame.Services, "Content");

            hexaModel = new Model[N_MODEL];
            hexaModel[0] = content.Load<Model>("Models/redhex");
            hexaModel[1] = content.Load<Model>("Models/greenhex");
            hexaModel[2] = content.Load<Model>("Models/whitehex");
            hexaModel[3] = content.Load<Model>("Models/brownhex");
            hexaModel[4] = content.Load<Model>("Models/bluehex");
            hexaModel[5] = content.Load<Model>("Models/mill");
            hexaModel[6] = content.Load<Model>("Models/bluehex");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            angle = angle + gameTime.ElapsedGameTime.Milliseconds / 1000.0f * 1.0f;
            if (angle > 360.0f)
                angle -= 360.0f;
            eye = new Vector3((float)(2.5f * Math.Cos(angle)), 2.0f, 2.5f * (float)Math.Sin(angle));
        }

        public override void Draw(GameTime gameTime)
        {
            myGame.GraphicsDevice.BlendState = BlendState.Opaque;
            myGame.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            view = Matrix.CreateLookAt(eye, target, up);
            for (int loop1 = 0; loop1 < N_MODEL; loop1++)
            {
                Matrix[] transforms = new Matrix[hexaModel[loop1].Bones.Count];
                hexaModel[loop1].CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in hexaModel[loop1].Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index] * world[loop1];
                        effect.View = view;
                        effect.Projection = projection;
                    }

                    mesh.Draw();
                }
            }
            base.Draw(gameTime);
        }
    }
    
}
