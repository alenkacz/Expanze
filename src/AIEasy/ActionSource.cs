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
        int sourceChange;
        ActiveState state;                // about which town, hexa etc it is about
        GetPrice getPrice;

        public ActionSource(DelAction action, PriceKind price) : this(action, () => { return price;})
        {

        }

        public ActionSource(DelAction action, GetPrice getPrice)
        {
            this.action = action;
            this.getPrice = getPrice;
            state = new ActiveState();
        }

        public PriceKind GetPriceKind() { return getPrice(); }
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
            state.activeSourceBuildingKind = activeState.activeSourceBuildingKind;
            state.activeUpgradeKind = activeState.activeUpgradeKind;
            state.activePlayer = activeState.activePlayer;
            state.activeHexa = activeState.activeHexa;
        }

        internal ActiveState GetState()
        {
            return state;
        }
    }
}
