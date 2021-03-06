﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIEasy
{
    static class Fitness
    {
        static IMapController map;
        static AIEasy ai;

        public static void SetMapController(IMapController mapController, AIEasy aiEasy) 
        {
            map = mapController;
            ai = aiEasy;
        }

        /// <summary>
        /// Use in decisioning about capturing hexa
        /// </summary>
        /// <param name="attacker">Player with fort (me)</param>
        /// <param name="hexa">Target hexa</param>
        /// <returns></returns>
        public static float GetFitness(IPlayer attacker, IHexa hexa)
        {
            if (map.GetPlayerOthers().Count == 0)
                return 0.0f;

            if (hexa.GetCapturedIPlayer() == attacker)
                return 0.0f;

            int enemySum = 0;
            int attackerSum = hexa.GetNormalProductivity(attacker);

            foreach (IPlayer player in map.GetPlayerOthers())
            {
                if (player == attacker)
                {
                    enemySum += hexa.GetNormalProductivity(map.GetPlayerMe());
                } else
                    enemySum += hexa.GetNormalProductivity(player);
            }

            if(hexa.GetCaptured() && hexa.GetCapturedIPlayer() != attacker)
            {
                return (enemySum + attackerSum) / 144.0f;
            }

            return (enemySum) / 144.0f;
        }

        public static float GetFitness(IHexa hexa)
        {
            if (hexa == null)
                return 0.0f;

            float fitness;

            fitness = hexa.GetStartSource() / 24.0f;

            /*
             * Ore and stone is more useful than other sources 
             */
            if (hexa.GetKind() != HexaKind.Mountains &&
               hexa.GetKind() != HexaKind.Stone)
            {
                fitness *= 0.85f;
            }

            /*
             * Better than water
             */

            if (hexa.GetKind() == HexaKind.Desert)
                fitness = 0.15f;

            return fitness;
        }

        public static float GetFitness(IRoad road)
        {
            if (road == null)
                return 0.0f;

            float fitness = 0.0f;
            IRoad tempRoad;
            bool con;

            foreach (ITown town1 in road.GetITown())
            {
                // 
                if (town1.GetIOwner() != null)
                    continue;

                con = false;
                for (byte loop1 = 0; loop1 < 3; loop1++)
                {
                    tempRoad = town1.GetIRoad(loop1);
                    if (tempRoad != null &&
                        tempRoad.GetIOwner() == map.GetPlayerMe())
                    {
                        con = true;
                        break ;
                    }
                }
                if (con)
                    continue;

                if (town1.IsPossibleToBuildTown())
                    fitness += GetFitness(town1);

                for(byte loop1 = 0; loop1 < 3; loop1++)
                {
                    tempRoad = town1.GetIRoad(loop1);
                    if (tempRoad == null || tempRoad.GetIsBuild())
                        continue;

                    foreach(ITown town2 in tempRoad.GetITown())
                    {
                        if(town2 != town1 && town2.IsPossibleToBuildTown())
                            fitness += GetFitness(town2) / 2.5f;
                    }
                }
            }

            return fitness / 2.0f;
        }

        public static float GetFitness(IPlayer player)
        {
            if (map.GetPlayerOthers().Count == 0)
                return 0.0f;

            int sum = 0;
            int sourceHim;
            int sourceMe;
            ISourceAll source = player.GetSource();
            IPlayer me = map.GetPlayerMe();

            for (int loop1 = 0; loop1 < 5; loop1++)
            {
                sourceHim = source.Get((SourceKind)loop1);
                sourceMe = me.GetCollectSourcesNormal().Get((SourceKind)loop1);
                sum += (sourceMe <= 16) ? sourceHim * 2 : sourceHim;
            }

            float result = sum / 2000.0f;
            if (result > 1.0f)
                result = 1.0f;

            return result;
        }

        public static float GetFitness(ITown town)
        {
            if (town == null)
                return 0.0f;

            if(!town.IsPossibleToBuildTown())
                return 0.0f;

            float fitness = 0.0f;
            float multiplier = 1.0f;
            ISourceAll normal = map.GetPlayerMe().GetCollectSourcesNormal();
            bool[] haveKind = new bool[5];
            for (int loop1 = 0; loop1 < 5; loop1++)
                haveKind[loop1] = normal[loop1] != 0;

            IHexa tempHexa;
            for (byte loop1 = 0; loop1 < 3; loop1++)
            {
                tempHexa = town.GetIHexa(loop1);

                if (tempHexa.GetStartSource() == 0)
                    continue;

                if (!haveKind[normal.KindToInt(tempHexa.GetSourceKind())])
                {
                    haveKind[normal.KindToInt(tempHexa.GetSourceKind())] = true;
                    multiplier = 3.5f;
                }
                else
                    multiplier = 1.0f;

                fitness += Fitness.GetFitness(tempHexa) * multiplier / 9.0f;
            }

            return fitness;
        }
    }
}
