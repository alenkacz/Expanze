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
        TownUnchoose,
        HexaBuild,
        BuildingBuild,
        MessageClose,
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
        private List<TriggerPair> attachList;
        bool inForEach;

        private TriggerManager()
        {
            observers = new List<Trigger>[(int) TriggerType.Count];
            dettachList = new List<TriggerPair>();
            attachList = new List<TriggerPair>();
            inForEach = false;

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
            inForEach = true;
            foreach (Trigger observer in observers[(int)type])
            {
                if(observer.Restriction1() == restriction1)
                    observer.TurnOn();
            }
            inForEach = false;
            Dettach();
            Attach();
        }

        public void TurnTrigger(TriggerType type)
        {
            inForEach = true;
            foreach (Trigger observer in observers[(int)type])
            {
                observer.TurnOn();
            }
            inForEach = false;
            Dettach();
            Attach();
        }

        public void Attach(Trigger observer, TriggerType type)
        {
            attachList.Add(new TriggerPair(type, observer));
            if (!inForEach)
                Attach();
        }

        public void Attach()
        {
            foreach (TriggerPair pair in attachList)
            {
                observers[(int)pair.Type].Add(pair.Observer);
            }

            attachList.Clear();
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
            if (!inForEach)
                Dettach();
        }
    }
}
