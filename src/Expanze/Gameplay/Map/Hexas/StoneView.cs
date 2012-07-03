using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;

namespace Expanze.Gameplay.Map
{
    class StoneView : HexaView
    {
        public StoneView(HexaModel model, int x, int y)
            : base(model, x, y)
        {

        }

        public override void DrawBuildings(GameTime gameTime)
        {
            base.DrawBuildings(gameTime);
            GameResources gr = GameResources.Inst();
            Model m;

            Matrix rotation;
            rotation = (hexaRotation == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * hexaRotation);
            Matrix tempMatrix = Matrix.CreateScale(0.00028f) * rotation;
            Matrix rotationMatrix = Matrix.Identity;
            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                int tempPos = (loop1 + hexaRotation) % 6;
                switch(model.getTown((CorePlugin.TownPos)loop1).GetBuildingKind(model.GetID()))
                {
                    case BuildingKind.NoBuilding:
                        m = gr.GetStoneCover(tempPos);
                        rotationMatrix = Matrix.Identity;
                        break;
                    default :
                        m = null;
                        break;
                }

                if (m == null)
                      continue;

                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.LightingEnabled = true;
                        effect.AmbientLightColor = GameState.MaterialAmbientColor;
                        effect.DirectionalLight0.Direction = GameState.LightDirection;
                        effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                        effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                        effect.DirectionalLight0.Enabled = true;
                        effect.World = transforms[mesh.ParentBone.Index] * tempMatrix * world;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
