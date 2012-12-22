using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Expanze.Gameplay.Map.View
{
    class PlayerInstanceView : InstanceView
    {
        protected Vector3 normalEmissiveColor;
        public Vector3 NormalEmissiveColor
        {
            set { 
                normalEmissiveColor.X = value.X;
                normalEmissiveColor.Y = value.Y;
                normalEmissiveColor.Z = value.Z; 
            }
        }
       
        protected Vector3 playerEmissiveColor;
        public Vector3 PlayerEmissiveColor
        {
            set { 
                playerEmissiveColor.X = value.X;
                playerEmissiveColor.Y = value.Y;
                playerEmissiveColor.Z = value.Z;
            }
        }
        protected Vector3 playerDiffuseColor;
        public Vector3 PlayerDiffuseColor
        {
            set { 
                playerDiffuseColor.X = value.X;
                playerDiffuseColor.Y = value.Y;
                playerDiffuseColor.Z = value.Z;
            }
        }
        protected Vector3 playerAmbientLightColor;
        public Vector3 PlayerAmbientLightColor
        {
            set {
                playerAmbientLightColor.X = value.X;
                playerAmbientLightColor.Y = value.Y;
                playerAmbientLightColor.Z = value.Z;
            }
        }
        protected int playerMesh1, playerMesh2;

        public PlayerInstanceView(Matrix world, int playerMesh1, int playerMesh2) : base(world)
        {
            this.playerMesh1 = playerMesh1;
            this.playerMesh2 = playerMesh2;

            playerAmbientLightColor = new Vector3();
            playerDiffuseColor = new Vector3();
            playerEmissiveColor = new Vector3();
            normalEmissiveColor = new Vector3();
        }

        internal override void UpdateEffect(Microsoft.Xna.Framework.Graphics.BasicEffect effect, int meshNumber)
        {
            if ((meshNumber != playerMesh1 &&
                 meshNumber != playerMesh2) ||
                Settings.graphics == GraphicsQuality.LOW_GRAPHICS && normalEmissiveColor.LengthSquared() < 0.01)
            {
                effect.AmbientLightColor = playerAmbientLightColor;
                effect.DiffuseColor = playerDiffuseColor;
                effect.EmissiveColor = playerEmissiveColor;
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
