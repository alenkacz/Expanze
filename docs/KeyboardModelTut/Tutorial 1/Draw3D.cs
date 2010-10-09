using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace tutorial
{
    class Draw3D : DrawableGameComponent
    {
        Model myModel;

        public Matrix world;
        public Matrix view;
        public Matrix projection;
        public Vector3 eye, target, up;
        public float angle;

        Game myGame;

        float aspectRatio;

        public Draw3D(Game game)
            : base(game)
        {
            myGame = game;
        }

        public override void Initialize()
        {
            base.Initialize();
            aspectRatio = myGame.GraphicsDevice.Viewport.Width / (float)myGame.GraphicsDevice.Viewport.Height;
            eye = new Vector3(-5.0f, -5.0f, 0.0f);
            target = new Vector3(0.0f, 0.0f, 0.0f);
            up = new Vector3(0.0f, 1.0f, 0.0f);
            angle = 0.0f;
            view = Matrix.CreateLookAt(eye, target, up);
            world = Matrix.Identity;
            projection = Matrix.CreatePerspectiveFieldOfView((float) MathHelper.ToRadians(90), aspectRatio, 1.0f, 100.0f);
        }

        protected override void LoadContent()
        {
            myModel = Game.Content.Load<Model>("sword");
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            angle = angle + gameTime.ElapsedGameTime.Milliseconds / 1000.0f * 3.0f;
            if (angle > 360.0f)
                angle -= 360.0f;
            eye = new Vector3((float) (5.0f * Math.Cos(angle)), 0.0f, 5.0f * (float) Math.Sin(angle));
            view = Matrix.CreateLookAt(eye, target, up);
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }

        }
    }
}
