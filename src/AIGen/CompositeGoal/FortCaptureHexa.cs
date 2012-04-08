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

        public FortCaptureHexa(IMapController map, double kPoints, int depth)
            : base(map, depth, "Capture hexa")
        {
            this.kPoints = kPoints;
            kCapture = 1 - kPoints;
            bestHexa = null;
            bestHexaIDs = new List<int>();
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
            IHexa hexa;
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
                    hexaNeighbour = hexa.GetIHexaNeighbour((RoadPos)loop1);

                    if (hexaNeighbour == null)
                        continue;

                    int count = 0;
                    foreach (int i in bestHexaIDs)
                    {
                        if (i == hexaNeighbour.GetID())
                            count++;
                    }

                    if(count > 1) // dont fight about hexa
                        continue;

                    tempDesirability = GetDesirablity(me, hexaNeighbour);
                    if (tempDesirability > bestDesirability)
                    {
                        bestDesirability = tempDesirability;
                        bestHexa = hexa.GetIHexaNeighbour((RoadPos)loop1);
                    }
                }
            }

            double points = (map.GetActionPoints(PlayerPoints.FortCaptureHexa) > 0) ? 1.0 : 0.0;
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

            if (hexa.GetCaptured() && hexa.GetCapturedIPlayer() != attacker)
            {
                return (enemySum + attackerSum) / 144.0f;
            }

            return (enemySum) / 144.0f;
        }
    }
}
