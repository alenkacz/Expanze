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
        public MountainsView(HexaModel model) : base(model)
        {

        }

        public override void DrawBuildings(GameTime gameTime)
        {

            Matrix rotation;
            rotation = (hexaID % 6 == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (hexaID % 6));
            Matrix tempMatrix = Matrix.CreateScale(0.00027f) *rotation;

            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                Model m;
                int tempPos = (loop1 + hexaID) % 6;
                if (BuildingKind.NoBuilding != model.getTown((CorePlugin.TownPos)loop1).getBuildingKind(model.getID()))
                {
                    m = GameState.map.getMountainsSourceBuildingModel(tempPos);
                }
                else
                {
                    if (tempPos == 5 || (tempPos == 4 && BuildingKind.NoBuilding == model.getTown((CorePlugin.TownPos)(((int) CorePlugin.TownPos.BottomLeft + hexaID) % 6)).getBuildingKind(model.getID())))
                        continue;
                    m = GameState.map.getMountainsCover(tempPos);
                }



                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
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
