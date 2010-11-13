using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay.Map
{
    class MountainsView : HexaView
    {
        private HexaModel hexaModel;

        public MountainsView(HexaModel model) : base(model)
        {

        }

        public override void DrawBuildings(GameTime gameTime)
        {
            Model[] covers = GameState.map.getMountainsCover();
            foreach (Model m in covers)
            {
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index] * world;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
