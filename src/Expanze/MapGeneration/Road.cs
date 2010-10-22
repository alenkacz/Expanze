using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze
{
    class Road
    {
        private int number;
        public static int counter = 0;
        private Color pickRoadColor;
        private Matrix world;

        public Road(Matrix world)
        {
            this.world = world;

            number = ++counter;
            this.pickRoadColor = new Color(0.0f, this.number / 256.0f, 0.0f);
        }

        public void DrawPickableAreas()
        {
            Model m = GameState.map.getRectangleShape();
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.05f, 0.0f)) * Matrix.CreateScale(0.2f) * world;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = pickRoadColor.ToVector3();
                    effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }
    }
}
