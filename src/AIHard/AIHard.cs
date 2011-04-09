using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using CorePlugin.Attributes;

namespace AIHard
{
    [PluginAttributeAI("Goal driven AI")]
    class AIHard : IComponentAI
    {
        IMapController mapController;
        ThinkGoal thinkGoal;

        public String GetAIName()
        {
            return "AI těžká";
        }

        public void InitAIComponent(IMapController mapController)
        {
            this.mapController = mapController;
            thinkGoal = new ThinkGoal(mapController);
        }

        public void ResolveAI()
        {
            thinkGoal.Init();
            while (thinkGoal.Process() != GoalState.EndTurn)
                ;
        }

        public IComponentAI Clone()
        {
            IComponentAI component = new AIHard();
            return component;
        }
    }
}
