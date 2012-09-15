using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;
using Expanze.Gameplay.Map.View;

namespace Expanze.Gameplay.Map
{
    class MountainsView : HexaView
    {
        Matrix mineMatrix;

        public MountainsView(HexaModel model, int x, int y, ModelView modelView)
            : base(model, x, y, modelView)
        {
       
        }

        private Model GetModel(int order, out bool cover)
        {
            Model m;
            int tempPos = (order + hexaRotation) % 6;
            if (BuildingKind.NoBuilding != model.getTown((CorePlugin.TownPos)order).GetBuildingKind(model.GetID()))
            {
                m = GameResources.Inst().GetMountainsSourceBuildingModel(tempPos);
                cover = false;
            }
            else
            {
                cover = true;
                if (tempPos == 5 || (tempPos == 4 && BuildingKind.NoBuilding != model.getTown((CorePlugin.TownPos)5).GetBuildingKind(model.GetID())))
                    return null;
                m = GameResources.Inst().GetMountainsCover(tempPos);
            }

            return m;
        }

        public override void DrawShadow(MapView mapView, Matrix shadow)
        {
            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                bool cover;
                Model m = GetModel(loop1, out cover);
                if (model == null || cover)
                    continue;

                mapView.DrawShadow(m, mineMatrix, shadow);
            }
        }

        public override void DrawBuildings(GameTime gameTime)
        {
            if (Settings.graphics == GraphicsQuality.LOW_GRAPHICS)
            {
                base.DrawBuildings(gameTime);
                return;
            }

            Matrix rotation;
            rotation = (hexaRotation == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (hexaRotation));
            mineMatrix = Matrix.CreateScale(0.00028f) * rotation * world;

            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                bool cover;
                Model m = GetModel(loop1, out cover);
                if (m == null)
                    continue;

                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.Alpha = 1.0f;
                        effect.LightingEnabled = true;
                        effect.AmbientLightColor = GameState.MaterialAmbientColor;
                        effect.DirectionalLight0.Direction = GameState.LightDirection;
                        effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                        effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                        effect.DirectionalLight0.Enabled = true;
                        effect.World = transforms[mesh.ParentBone.Index] * mineMatrix;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
