using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CorePlugin;

namespace Expanze
{
    class Town : ITownGet
    {
        private Player playerOwner;
        private bool isBuild;
        private Road[] roadNeighbour; // two or 3 neighbours
        private Town[] townNeighbour; // two or 3 neighbours
        private HexaModel[] hexaNeighbour; // 1 - 3 neighbours

        private int townID;
        public static int counter = 0;

        public int getTownID() { return townID; }
        public bool getIsBuild() { return isBuild; }
        public Player getPlayerOwner() { return playerOwner; }
        public ISourceAll getCost() { return Settings.costTown; }

        public Town()
        {
            townID = ++counter;
            isBuild = false;

            roadNeighbour = new Road[3];
            townNeighbour = new Town[3];
            hexaNeighbour = new HexaModel[3];

            playerOwner = null;
        }

        public static void resetCounter() { counter = 0; }

        public void setRoadNeighbours(Road road1, Road road2, Road road3)
        {
            roadNeighbour[0] = road1;
            roadNeighbour[1] = road2;
            roadNeighbour[2] = road3;
        }

        public void setTownNeighbours(Town town1, Town town2, Town town3)
        {
            townNeighbour[0] = town1;
            townNeighbour[1] = town2;
            townNeighbour[2] = town3;
        }

        public void setHexaNeighbours(HexaModel hexa1, HexaModel hexa2, HexaModel hexa3)
        {
            hexaNeighbour[0] = hexa1;
            hexaNeighbour[1] = hexa2;
            hexaNeighbour[2] = hexa3;
        }

        public void CollectSources(Player player)
        {
            if (playerOwner != player)
                return;

            SourceAll cost = new SourceAll(0);
            int amount;

            foreach (HexaModel hexa in hexaNeighbour)
            {
                if (hexa != null)
                {
                    amount = hexa.getValue();

                    switch (hexa.getType())
                    {
                        case HexaType.Forest:
                            cost = cost + new SourceAll(amount, 0, 0, 0, 0);
                            break;

                        case HexaType.Stone:
                            cost = cost + new SourceAll(0, amount, 0, 0, 0);
                            break;

                        case HexaType.Cornfield :
                            cost = cost + new SourceAll(0, 0, amount, 0, 0);
                            break;

                        case HexaType.Pasture:
                            cost = cost + new SourceAll(0, 0, 0, amount, 0);
                            break;

                        case HexaType.Mountains:
                            cost = cost + new SourceAll(0, 0, 0, 0, amount);
                            break;
                    }
                }
            }
            player.addSources(cost, TransactionState.TransactionMiddle);
        }

        public void BuildTown(Player player)
        {
            playerOwner = player;
            isBuild = true;
        }

        public Boolean HasPlayerRoadNeighbour(Player player)
        {
            foreach (Road road in roadNeighbour)
            {
                if (road != null)
                {
                    if (road.getOwner() == player)
                        return true;
                }
            }
            return false;
        }

        // has someone already built town next to this spot?
        public Boolean HasTownBuildNeighbour()
        {
            for (int loop1 = 0; loop1 < townNeighbour.Length; loop1++)
            {
                if (townNeighbour[loop1] != null)
                {
                    if (townNeighbour[loop1].getIsBuild())
                        return true;
                }
            }

            return false;
        }

        public Boolean CanActivePlayerBuildTown()
        {
            GameMaster gm = GameMaster.getInstance();
            if (gm.getState() == EGameState.StateGame)
            {
                Player activePlayer = gm.getActivePlayer();
                Boolean hasActivePlayerRoadNeighbour = false;

                foreach(Road road in roadNeighbour)
                {
                    if (road != null && road.getOwner() == activePlayer)
                        hasActivePlayerRoadNeighbour = true;
                }

                return !isBuild && !HasTownBuildNeighbour() && Settings.costTown.HasPlayerSources(activePlayer) && hasActivePlayerRoadNeighbour;
            } else
                return !isBuild && !HasTownBuildNeighbour();
        }
    }
}
