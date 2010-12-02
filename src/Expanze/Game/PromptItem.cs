﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze
{
    /// <summary>
    /// This "structure" collect sources about one sellable item.
    /// For example for Mine, Monastery, Fort but also for technology from Monastery
    /// </summary>
    class PromptItem
    {
        String title;           // item title
        String description;     // description
        SourceAll cost;         // cost in all sources
        Texture2D icon;         // image icon of item

        public SourceAll getCost() { return cost; }
        public String getTitle() { return title; }
        public Texture2D getIcon() { return icon; }
        public String getDescription() { return description; }

        public PromptItem(String title, String description, SourceAll cost, Texture2D icon)
        {
            this.title = title;
            this.description = description;
            this.cost = cost;
            this.icon = icon;
        }

        public virtual void Execute() // what should be done if someone click on "buy/ok" button
        {

        }
    }
}