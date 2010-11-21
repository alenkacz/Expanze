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
    class Road : IRoadGet
    {
        private Player playerOwner;     /// owner of road, if road is not build, owner is null
        private bool isBuild;           /// is build this road? or it is only place for road

        private int roadID;             /// unique ID for road, use for picking and for searching concreate roads
                                        /// roadID starts with one and the last one is equals counter
        public static int counter = 0;  /// count of possible roads in the map

        private Town[] neighbour;       /// every road must have two town neighbours

        public Road()
        {
            roadID = ++counter;

            neighbour = new Town[2];
            isBuild = false;
            playerOwner = null;
        }

        public static void resetCounter() { counter = 0; }      // every new game have to be reseted

        public static int getRoadCount() { return counter; }    // number of roads
        public Player getOwner() { return playerOwner; }
        public int getRoadID() { return roadID; }
        public bool getIsBuild() { return isBuild; }

        public void SetTownNeighbours(Town one, Town two)
        {
            neighbour[0] = one;
            neighbour[1] = two;
        }

        /// <summary>
        /// Checks if player has its road build on one of the ends
        /// of this road.
        /// </summary>
        /// <param name="player">Future road owner</param>
        /// <returns>True if there is at least one players road</returns>
        public Boolean IsActivePlayersRoadOnEndOfRoad(Player player)
        {
            foreach (Town town in neighbour)
            {
                if (town.HasPlayerRoadNeighbour(player))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if players town is build on one of the road ends.
        /// </summary>
        /// <param name="player">Future road owner</param>
        /// <returns>True if there is players town</returns>
        public Boolean IsActivePlayersTownOnEndOfRoad(Player player)
        {
            foreach(Town town in neighbour)
            {
                if (town.getPlayerOwner() == player)
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// It build town(only model, not view) and sets its owner.
        /// </summary>
        /// <param name="player">New owner of the road.</param>
        public void BuildRoad(Player player)
        {
            playerOwner = player;
            isBuild = true;
        }

        /// <summary>
        /// Checks all conditions needed to be ok to build road.
        /// </summary>
        /// <returns>OK if there is not problem or name of problem.</returns>
        public RoadBuildError CanActivePlayerBuildRoad()
        {
            GameMaster gm = GameMaster.getInstance();
            if (gm.getState() == EGameState.StateGame)
            {
                Player activePlayer = gm.getActivePlayer();

                if (isBuild)
                    return RoadBuildError.AlreadyBuild;
                if(!IsActivePlayersRoadOnEndOfRoad(activePlayer) && !IsActivePlayersTownOnEndOfRoad(activePlayer))
                    return RoadBuildError.NoPlayerRoadOrTown;
                if(!Settings.costRoad.HasPlayerSources(activePlayer))
                    return RoadBuildError.NoSources;
                return RoadBuildError.OK;
            }

            return RoadBuildError.OK;
        }
    }
}
