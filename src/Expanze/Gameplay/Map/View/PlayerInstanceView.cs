using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Expanze.Gameplay.Map.View
{
    class PlayerInstanceView : InstanceView
    {
        Vector3 normalEmissiveColor;
        public Vector3 NormalEmissiveColor
        {
            set { normalEmissiveColor = value; }
        }
       
        Vector3 playerEmissiveColor;
        public Vector3 PlayerEmissiveColor
        {
            set { playerEmissiveColor = value; }
        }
        Vector3 playerDiffuseColor;
        public Vector3 PlayerDiffuseColor
        {
            set { playerDiffuseColor = value; }
        }
        Vector3 playerAmbientLightColor;
        public Vector3 PlayerAmbientLightColor
        {
            set { playerAmbientLightColor = value; }
        }
        int playerMesh1, playerMesh2;

        public PlayerInstanceView(Matrix world, int playerMesh1, int playerMesh2) : base(world)
        {
            this.playerMesh1 = playerMesh1;
            this.playerMesh2 = playerMesh2;
        }

        internal override void UpdateEffect(Microsoft.Xna.Framework.Graphics.BasicEffect effect, int meshNumber)
        {
            base.UpdateEffect(effect, meshNumber);

            if (meshNumber == playerMesh1 || meshNumber == playerMesh2)
            {
                effect.AmbientLightColor = playerAmbientLightColor;
                effect.DiffuseColor = playerDiffuseColor;
                effect.EmissiveColor = playerEmissiveColor;
            }
            else
            {
                effect.EmissiveColor = normalEmissiveColor;
            }
        }
    }
}
