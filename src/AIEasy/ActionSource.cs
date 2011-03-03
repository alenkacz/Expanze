using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    class ActionSource
    {
        DelAction action;
        PriceKind price;
        int sourceChange;

        public ActionSource(DelAction action, PriceKind price)
        {
            this.action = action;
            this.price = price;
        }

        public PriceKind GetPriceKind() { return price; }
        public DelAction GetAction() { return action; }
        public int GetSourceChange() { return sourceChange; }
        public void SetSourceChange(int sourceChange)
        {
            this.sourceChange = sourceChange;
        }
    }
}
