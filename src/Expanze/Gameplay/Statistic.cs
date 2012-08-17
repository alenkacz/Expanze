using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.Gameplay
{
    public class Statistic
    {
        public enum Kind { Points, Towns, Roads, Medals, Market, Licences, Monastery, Upgrades, Fort, Actions, SumSources, Count }
        
        private const int MAX_TURNS = 150;
        int[][] statistic;

        public Statistic()
        {
            statistic = new int[(int)Kind.Count][];
            for (int loop1 = 0; loop1 < (int)Kind.Count; loop1++)
            {
                statistic[loop1] = new int[MAX_TURNS];
                for (int loop2 = 0; loop2 < MAX_TURNS; loop2++)
                {
                    statistic[loop1][loop2] = 0;
                }
            }
        }

        public int[][] GetStat() { return statistic; }

        public static string GetGraphName(Kind kind)
        {
            switch (kind)
            {
                case Kind.Points: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_POINTS);
                case Kind.Towns: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_TOWNS);
                case Kind.Roads: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_ROADS);
                case Kind.Medals: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_MEDALS);
                case Kind.Monastery: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_MONASTERY);
                case Kind.Fort: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_FORT);
                case Kind.Market: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_MARKET);
                case Kind.Actions: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_ACTION);
                case Kind.Licences: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_LICENCE);
                case Kind.Upgrades: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_UPGRADE);
                case Kind.SumSources: return Strings.Inst().GetString(TextEnum.MENU_GRAPH_SUMSOURCES);

                default: throw new Exception("Statistic kind doesnt exist.");
            }
        }

        public void AddStat(Kind kind, int amount, int turn)
        {
            if(turn >= MAX_TURNS)
                throw new Exception("Statistic can have only " + MAX_TURNS + " columns");

            statistic[(int)kind][turn] += amount;
        }
    }
}
