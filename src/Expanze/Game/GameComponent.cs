using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Expanze
{
     public abstract class GameComponent
    {
         public virtual void Update(GameTime gameTime)
         {
         }
         public virtual void Draw(GameTime gameTime) { }
         public virtual void LoadContent() { }
         public virtual void UnloadContent() { }
         public virtual void Initialize() { }
    }
}
