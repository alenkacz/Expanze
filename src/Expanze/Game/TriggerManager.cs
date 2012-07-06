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
        MarketFirstRow,
        MarketSecondRow,
        MarketChangeSources,
        Count
    }

    public class TriggerManager
    {
        private static TriggerManager triggerManager;
        private List<Trigger>[] observers;

        private TriggerManager()
        {
            observers = new List<Trigger>[(int) TriggerType.Count];
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
        }

        public void TurnTrigger(TriggerType type)
        {
            foreach (Trigger observer in observers[(int)type])
            {
                observer.TurnOn();
            }
        }

        public void Attach(Trigger observer, TriggerType type)
        {
            observers[(int)type].Add(observer);
        }

        public void Dettach(Trigger observer, TriggerType type)
        {
            observers[(int)type].Remove(observer);
        }
    }
}
