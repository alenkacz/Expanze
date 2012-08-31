using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;
using AIGen;

namespace AIGen
{
    class FortCaptureHexa : CompositeGoal
    {
        IHexa bestHexa;

        List<int> bestHexaIDs;

        double kCapture;
        double kPoints;

        public FortCaptureHexa(IMapController map, int k, int depth)
            : base(map, depth, "Capture hexa")
        {
            kPoints = k / 100.0f;
            kCapture = 1 - kPoints;
            bestHexa = null;
            bestHexaIDs = new List<int>();

            if (map.GetActionPoints(PlayerPoints.FortCaptureHexa) > 0 && map.GetTurnNumber() > 10)
                kPoints += 0.1;
        }

        public override void Init()
        {
            if (bestHexa == null)
                GetDesirability();

            AddSubgoal(new RaiseSources(map, PriceKind.ACaptureHexa, depth + 1));
            AddSubgoal(new FortCaptureHexaAtom(map, bestHexa, depth + 1));
            bestHexaIDs.Add(bestHexa.GetID());

            bestHexa = null;
        }

        public override double GetDesirability()
        {
            IPlayer me = map.GetPlayerMe();
            List<IFort> forts = me.GetFort();
            if (forts.Count == 0)
                return 0.0f;

            IHexa hexa;
            IHexa hexa1Neighbour;
            IHexa hexaNeighbour;
            double tempDesirability;
            double bestDesirability = 0.0;

            foreach (IFort fort in forts)
            {
                hexa = map.GetIHexaByID(fort.GetHexaID());

                if (hexa == null)
                    continue;

                for (int loop1 = 0; loop1 < 6; loop1++)
                {
                    hexa1Neighbour = hexa.GetIHexaNeighbour((RoadPos)loop1);
                    if (hexa1Neighbour == null)
                        continue;

                    for (int loop2 = 0; loop2 < 6; loop2++)
                    {
                        hexaNeighbour = hexa1Neighbour.GetIHexaNeighbour((RoadPos) loop2);

                        if (hexaNeighbour == null || hexaNeighbour.GetKind() == HexaKind.Desert || hexaNeighbour.GetKind() == HexaKind.Water || hexaNeighbour.GetKind() == HexaKind.Null)
                            continue;

                        if (map.CanCaptureHexa(hexaNeighbour) == CaptureHexaError.TooFarFromFort)
                            continue;

                        int count = 0;
                        foreach (int i in bestHexaIDs)
                        {
                            if (i == hexaNeighbour.GetID())
                                count++;
                        }

                        if (count > 1) // dont fight about hexa
                            continue;

                        tempDesirability = GetDesirablity(me, hexaNeighbour);
                        if (tempDesirability > bestDesirability)
                        {
                            bestDesirability = tempDesirability;
                            bestHexa = hexaNeighbour;
                        }
                    }
                }
            }

            if (bestHexa == null)
                return 0.0f;

            double points = 0.0;
            if (map.GetActionPoints(PlayerPoints.FortCaptureHexa) - map.GetPlayerMe().GetPoints()[(int)PlayerPoints.FortCaptureHexa] > 0)
                points = 1.0;

            return bestDesirability * kCapture + points * kPoints;
        }

        public double GetDesirablity(IPlayer attacker, IHexa hexa)
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
                }
                else
                    enemySum += hexa.GetNormalProductivity(player);
            }

            if (hexa.GetKind() == HexaKind.Mountains || hexa.GetKind() == HexaKind.Stone)
                enemySum = (int)(enemySum * 1.05);
            else if (hexa.GetKind() == HexaKind.Forest)
                enemySum = (int)(enemySum * 0.8);
            else if (hexa.GetKind() == HexaKind.Desert)
                return 0.0;

            if (hexa.GetCaptured() && hexa.GetCapturedIPlayer() != attacker)
            {
                return (enemySum + attackerSum) / 144.0f;
            }

            return (enemySum) / 144.0f;
        }
    }
}
