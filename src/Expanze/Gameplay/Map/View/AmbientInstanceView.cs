using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay.Map.View
{
    class AmbientInstanceView : InstanceView
    {
        Vector3 ambientLightColor;

        public Vector3 AmbientLightColor
        {
            set { 
                ambientLightColor.X = value.X;
                ambientLightColor.Y = value.Y;
                ambientLightColor.Z = value.Z; 
            }
        }

        public AmbientInstanceView(Matrix world)
            : base(world)
        {
            ambientLightColor = new Vector3();
        }

        internal override void UpdateEffect(BasicEffect effect, int meshNumber)
        {
            effect.AmbientLightColor = ambientLightColor;
        }
    }
}
