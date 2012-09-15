using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Expanze.Gameplay.Map.View
{
    class ModelView
    {
        private Dictionary<Model, ViewItem> viewItems;

        public ModelView()
        {
            viewItems = new Dictionary<Model, ViewItem>();
        }

        public InstanceView AddInstance(Model model, InstanceView newInstance)
        {
            if (!viewItems.ContainsKey(model))
            {
                viewItems[model] = new ViewItem(model);
            }

            return viewItems[model].Add(newInstance);
        }

        public void ChangeVisibility(Model model, int id)
        {
            viewItems[model].ChangeVisibility(id);
        }

        public void Draw(GameTime gameTime)
        {
            foreach(ViewItem item in viewItems.Values)
            {
                item.Draw(gameTime);
            }
        }

        public void Clear()
        {
            viewItems.Clear();
        }
    }
}
