
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay.Map
{
    class CornfieldView : HexaView
    {
        public CornfieldView(HexaModel model, int x, int y) : base(model, x, y)
        {
        }

        public override void DrawShadow(MapView mapView, Matrix shadow)
        {
            base.DrawShadow(mapView, shadow);

            if (model.SecretKind && !GameMaster.Inst().LastHumanPlayer.GetIsDiscovered(hexaID))
            {
                return;
            }

            Model m;
            m = GameResources.Inst().GetTreeModel(3);
            mapView.DrawShadow(m, worldM, shadow);
            m = GameResources.Inst().GetTreeModel(4);
            mapView.DrawShadow(m, worldM, shadow);
        }

        public override void DrawBuildings(GameTime gameTime)
        {
            base.DrawBuildings(gameTime);

            if (Settings.graphics == GraphicsQuality.LOW_GRAPHICS)
                return;

            Model[] models = new Model[2];
            models[0] = GameResources.Inst().GetTreeModel(3);
            models[1] = GameResources.Inst().GetTreeModel(4);

            foreach (Model m in models)
            {
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
                        effect.World = transforms[mesh.ParentBone.Index] * worldM;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
