﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze.Gameplay
{
    public class Statistic
    {
        public enum Kind { Points, Towns, Roads, Medals, Market, Monastery, Fort, Count }
        
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
                case Kind.Points: return Strings.MENU_GRAPH_POINTS;
                case Kind.Towns: return Strings.MENU_GRAPH_TOWNS;
                case Kind.Roads: return Strings.MENU_GRAPH_ROADS;
                case Kind.Medals: return Strings.MENU_GRAPH_MEDALS;
                case Kind.Monastery: return Strings.MENU_GRAPH_MONASTERY;
                case Kind.Fort: return Strings.MENU_GRAPH_FORT;
                case Kind.Market: return Strings.MENU_GRAPH_MARKET;
            }
            return "";
        }

        public void AddStat(Kind kind, int amount, int turn)
        {
            if(turn >= MAX_TURNS)
                throw new Exception("Statistic can have only 150 columns");

            statistic[(int)kind][turn] += amount;
        }
    }
}