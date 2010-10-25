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
         public virtual void DrawPickableAreas() { }
         public virtual void HandlePickableAreas(Color c) { }   // which color was last time under mouse cursor
         public virtual void Draw(GameTime gameTime) { }
         public virtual void Draw2D() { }
         public virtual void LoadContent() { }
         public virtual void UnloadContent() { }
         public virtual void Initialize() { }
    }
}
