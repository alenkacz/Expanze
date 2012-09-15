using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay.Map.View
{
    class InstanceView
    {
        bool visible;
        Matrix world;

        public InstanceView(Matrix world)
        {
            this.world = world;

            visible = true;
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
        
        public Matrix World
        {
            get { return world; }
        }

        internal virtual void UpdateEffect(BasicEffect effect, int meshNumber)
        {
            
        }
    }
}
