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
    class RoadModel : IRoad
    {
        private Player playerOwner;     /// owner of road, if road is not build, owner is null
        private bool isBuild;           /// is build this road? or it is only place for road

        private int roadID;             /// unique ID for road, use for picking and for searching concreate roads
                                        /// roadID starts with one and the last one is equals counter
        public static int counter = 0;  /// count of possible roads in the map

        private TownModel[] neighbour;       /// every road must have two town neighbours

        public RoadModel()
        {
            roadID = ++counter;

            neighbour = new TownModel[2];
            isBuild = false;
            playerOwner = null;
        }

        public static void resetCounter() { counter = 0; }      // every new game have to be reseted

        public static int GetRoadCount() { return counter; }    // number of roads
        public Player getOwner() { return playerOwner; }
        public int GetRoadID() { return roadID; }
        public bool getIsBuild() { return isBuild; }

        public void SetTownNeighbours(TownModel one, TownModel two)
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
            foreach (TownModel town in neighbour)
            {
                if (town.HasPlayerRoadNeighbour(player) && (town.GetPlayerOwner() == player || !town.GetIsBuild()))
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
            foreach(TownModel town in neighbour)
            {
                if (town.GetPlayerOwner() == player)
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
            player.AddPoints(Settings.pointsRoad);
            player.AddBuilding(Building.Road);
        }

        /// <summary>
        /// Building road uses by AI
        /// </summary>
        public IRoad Build()
        {
            return GameState.map.GetMapController().BuildRoad(roadID);
        }

        /// <summary>
        /// Checks all conditions needed to be ok to build road.
        /// </summary>
        /// <returns>OK if there is not problem or name of problem.</returns>
        public RoadBuildError CanBuildRoad()
        {
            GameMaster gm = GameMaster.Inst();
            if (gm.GetState() == EGameState.StateGame)
            {
                Player activePlayer = gm.GetActivePlayer();

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
