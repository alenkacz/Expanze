using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Expanze
{
    class Road
    {
        private int number;
        public static int counter = 0;
        private Color pickRoadColor;
        private Boolean pickActive = false;         // if mouse is over pickable area
        private Boolean pickNewActive = false;      // if mouse is newly over pickable area
        private Boolean pickNewPress = false;       // if mouse press on pickable area newly
        private Boolean pickPress = false;          // if mouse press on pickable area
        private Boolean pickNewRelease = false;     // if mouse is newly release above pickable area
        private Matrix world;

        public Road(Matrix world)
        {
            this.world = world;

            number = ++counter;
            this.pickRoadColor = new Color(0.0f, counter / 256.0f, 0.0f);
        }

        public void Draw(GameTime gameTime)
        {
            if (pickActive)
            {
                Model m = GameState.map.getRoadModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.01f, 0.0f)) * Matrix.CreateScale(0.3f) * world;

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                        effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();
                }
            }
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
                    effect.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.EmissiveColor = pickRoadColor.ToVector3();
                    effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }

        public void HandlePickableAreas(Color c)
        {
            if (c == pickRoadColor)
            {
                if (!pickActive)
                    pickNewActive = true;
                pickActive = true;

                if (GameState.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!pickPress)
                        pickNewPress = true;
                    pickPress = true;
                }
                else
                {
                    if (pickPress)
                        pickNewRelease = true;
                    pickPress = false;
                }
            }
            else
            {
                pickActive = false;
                pickPress = false;
                pickNewActive = false;
                pickNewPress = false;
                pickNewRelease = false;
            }
        }
    }
}
