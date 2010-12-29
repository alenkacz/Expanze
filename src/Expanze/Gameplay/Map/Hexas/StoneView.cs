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
        public StoneView(HexaModel model)
            : base(model)
        {

        }

        public override void DrawBuildings(GameTime gameTime)
        {
            GameResources gr = GameResources.Inst();
            Model[] m;
            m = new Model[2];
            Matrix rotation;
            rotation = (hexaID % 6 == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (hexaID % 6));
            Matrix tempMatrix = Matrix.CreateScale(0.00028f) * rotation;
            Matrix rotationMatrix = Matrix.Identity;
            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                int tempPos = (loop1 + hexaID) % 6;
                switch(model.getTown((CorePlugin.TownPos)loop1).getBuildingKind(model.GetID()))
                {
                    case BuildingKind.SourceBuilding :
                        m[0] = gr.getStoneSourceBuildingModel(tempPos);
                        rotationMatrix = Matrix.Identity;
                        break;
                    case BuildingKind.NoBuilding:
                        m[0] = gr.getStoneCover(tempPos);
                        rotationMatrix = Matrix.Identity;
                        break;
                    case BuildingKind.FortBuilding:
                        m[0] = gr.getStoneCover(tempPos);
                        m[1] = gr.getBuildingModel(BuildingModel.Fort);
                        rotationMatrix = (loop1 == 4) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * -(loop1 - 4));
                        rotationMatrix = Matrix.CreateScale(0.00028f) * rotationMatrix;
                        break;
                    case BuildingKind.MarketBuilding:
                        m[0] = gr.getStoneCover(tempPos);
                        m[1] = gr.getBuildingModel(BuildingModel.Market);
                        rotationMatrix = (loop1 == 4) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * -(loop1 - 4));
                        rotationMatrix = Matrix.CreateScale(0.00028f) * rotationMatrix;
                        break;
                    case BuildingKind.MonasteryBuilding:
                        m[0] = gr.getStoneCover(tempPos);
                        m[1] = gr.getBuildingModel(BuildingModel.Monastery);
                        rotationMatrix = (loop1 == 4) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * -(loop1 - 4));
                        rotationMatrix = Matrix.CreateScale(0.00028f) * rotationMatrix;
                        break;
                    default :
                        m[0] = null;
                        break;
                }

                for (int loop2 = 0; loop2 < 2; loop2++)
                {
                    if (m[loop2] == null)
                        break;

                    Matrix[] transforms = new Matrix[m[loop2].Bones.Count];
                    m[loop2].CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (ModelMesh mesh in m[loop2].Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.LightingEnabled = true;
                            effect.AmbientLightColor = GameState.MaterialAmbientColor;
                            effect.DirectionalLight0.Direction = GameState.LightDirection;
                            effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                            effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                            effect.DirectionalLight0.Enabled = true;
                            effect.World = transforms[mesh.ParentBone.Index] * ((loop2 == 1) ? rotationMatrix : tempMatrix) * world;
                            effect.View = GameState.view;
                            effect.Projection = GameState.projection;
                        }
                        mesh.Draw();
                    }
                }
            }
        }
    }
}
