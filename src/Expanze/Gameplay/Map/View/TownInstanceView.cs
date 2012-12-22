using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Expanze.Gameplay.Map.View
{
    class TownInstanceView : PlayerInstanceView
    {
        public TownInstanceView(Matrix world, int playerMesh1, int playerMesh2)
            : base(world, playerMesh1, playerMesh2)
        {
            roofDiffusiveColor = new Vector3();
        }

        Vector3 roofDiffusiveColor;
        public Vector3 RoofDiffusiveColor
        {
            set {
                roofDiffusiveColor.X = value.X;
                roofDiffusiveColor.Y = value.Y;
                roofDiffusiveColor.Z = value.Z; 
            }
        }

        internal override void UpdateEffect(Microsoft.Xna.Framework.Graphics.BasicEffect effect, int meshNumber)
        {
            if (meshNumber == playerMesh1 || meshNumber == playerMesh2 ||
                (Settings.graphics == GraphicsQuality.LOW_GRAPHICS && normalEmissiveColor.LengthSquared() < 0.01))
            {
                effect.AmbientLightColor = playerAmbientLightColor;
                effect.EmissiveColor = playerEmissiveColor;

                if (meshNumber == playerMesh1)
                    effect.DiffuseColor = playerDiffuseColor;
                else
                    effect.DiffuseColor = roofDiffusiveColor;
            }
            else
            {
                effect.EmissiveColor = normalEmissiveColor;
                effect.DiffuseColor = new Vector3(0.8f);
                effect.AmbientLightColor = new Vector3(0.0f);
            }
        }
    }
}
