using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;

namespace Expanze.Gameplay.Map
{
    class MountainsView : HexaView
    {
        public MountainsView(HexaModel model, int x, int y) : base(model, x, y)
        {

        }

        public override void DrawBuildings(GameTime gameTime)
        {

            Matrix rotation;
            rotation = (hexaRotation == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (hexaRotation));
            Matrix tempMatrix = Matrix.CreateScale(0.00028f) *rotation;

            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                Model m;
                int tempPos = (loop1 + hexaRotation) % 6;
                if (BuildingKind.NoBuilding != model.getTown((CorePlugin.TownPos)loop1).GetBuildingKind(model.GetID()))
                {
                    m = GameResources.Inst().GetMountainsSourceBuildingModel(tempPos);
                }
                else
                {
                    if (tempPos == 5 || (tempPos == 4 && BuildingKind.NoBuilding != model.getTown((CorePlugin.TownPos)5).GetBuildingKind(model.GetID())))
                        continue;
                    m = GameResources.Inst().GetMountainsCover(tempPos);
                }



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
