using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze
{
    class MessageItem
    {
        private Texture2D icon;
        private String title;
        private String description;

        public MessageItem(String title, String description, Texture2D icon)
        {
            this.icon = icon;
            this.title = title;
            this.description = description;
        }

        public Texture2D getIcon() { return icon; }
        public String getTitle() { return title; }
        public String getDescription() { return description; }
    }
}
