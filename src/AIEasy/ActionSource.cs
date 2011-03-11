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
        ActiveState state;                // about which town, hexa etc it is about


        public ActionSource(DelAction action, PriceKind price)
        {
            this.action = action;
            this.price = price;
            state = new ActiveState();
        }

        public PriceKind GetPriceKind() { return price; }
        public DelAction GetAction() { return action; }
        public int GetSourceChange() { return sourceChange; }
        public void SetSourceChange(int sourceChange)
        {
            this.sourceChange = sourceChange;
        }
        public void SetState(ActiveState activeState) {
            state.activeLicenceKind = activeState.activeLicenceKind;
            state.activeRoad = activeState.activeRoad;
            state.activeSourceKind = activeState.activeSourceKind;
            state.activeTown = activeState.activeTown;
            state.activeTownPos = activeState.activeTownPos;
        }

        internal ActiveState GetState()
        {
            return state;
        }
    }
}
