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
    class Road
    {
        private Player playerOwner;
        private bool isBuild;

        private int roadID;
        public static int counter = 0;

        private Town[] neighbour; // every road must have two neighbour

        public Road()
        {
            roadID = ++counter;

            neighbour = new Town[2];
            isBuild = false;
            playerOwner = null;
        }

        public static void resetCounter() { counter = 0; }

        public Player getOwner() { return playerOwner; }
        public int getRoadID() { return roadID; }
        public bool getIsBuild() { return isBuild; }

        public void SetTownNeighbours(Town one, Town two)
        {
            neighbour[0] = one;
            neighbour[1] = two;
        }

        public Boolean IsActivePlayersRoadOnEndOfRoad(Player player)
        {
            foreach (Town town in neighbour)
            {
                if (town.HasPlayerRoadNeighbour(player))
                    return true;
            }

            return false;
        }

        public Boolean IsActivePlayersTownOnEndOfRoad(Player player)
        {
            foreach(Town town in neighbour)
            {
                if (town.getPlayerOwner() == player)
                    return true;
            }

            return false;
        }

        public void BuildRoad(Player player)
        {
            playerOwner = player;
            isBuild = true;
        }

        public Boolean CanActivePlayerBuildRoad()
        {
            GameMaster gm = GameMaster.getInstance();
            if (gm.getState() == EGameState.StateGame)
            {
                Player activePlayer = gm.getActivePlayer();
                if (!isBuild &&
                    (IsActivePlayersRoadOnEndOfRoad(activePlayer) || IsActivePlayersTownOnEndOfRoad(activePlayer)) &&
                    Settings.costRoad.HasPlayerSources(activePlayer))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
