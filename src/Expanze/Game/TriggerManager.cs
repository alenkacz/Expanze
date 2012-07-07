using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    public enum TriggerType
    {
        NextTurn,
        MarketOpen,
        MarketClose,
        MarketFirstRow,
        MarketSecondRow,
        MarketChangeSources,
        TownBuild, 
        RoadBuild,
        TownChoose,
        Count
    }

    public class TriggerPair
    {
        TriggerType type;
        Trigger observer;

        public TriggerPair(TriggerType type, Trigger observer)
        {
            this.type = type;
            this.observer = observer;
        }

        public TriggerType Type
        {
            get { return type; }
        }

        public Trigger Observer
        {
            get { return observer; }
        }
    }

    public class TriggerManager
    {
        private static TriggerManager triggerManager;
        private List<Trigger>[] observers;
        private List<TriggerPair> dettachList;

        private TriggerManager()
        {
            observers = new List<Trigger>[(int) TriggerType.Count];
            dettachList = new List<TriggerPair>();

            for(int loop1 = 0; loop1 < observers.Length; loop1++)
                observers[loop1] = new List<Trigger>();
        }

        public static TriggerManager Inst()
        {
            if (triggerManager == null)
                triggerManager = new TriggerManager();

            return triggerManager;
        }

        public void TurnTrigger(TriggerType type, int restriction1)
        {
            foreach (Trigger observer in observers[(int)type])
            {
                if(observer.Restriction1() == restriction1)
                    observer.TurnOn();
            }
            Dettach();
        }

        public void TurnTrigger(TriggerType type)
        {
            foreach (Trigger observer in observers[(int)type])
            {
                observer.TurnOn();
            }
            Dettach();
        }

        public void Attach(Trigger observer, TriggerType type)
        {
            observers[(int)type].Add(observer);
        }

        private void Dettach()
        {
            foreach (TriggerPair pair in dettachList)
            {
                observers[(int) pair.Type].Remove(pair.Observer);
            }

            dettachList.Clear();
        }

        public void Dettach(Trigger observer, TriggerType type)
        {
            dettachList.Add(new TriggerPair(type, observer));
        }
    }
}
