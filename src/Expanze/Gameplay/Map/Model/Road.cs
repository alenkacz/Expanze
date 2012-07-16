using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CorePlugin;
using Expanze.Gameplay;

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

        private PathNode pathNode;      /// node used for finding path to road
        private bool goalRoad;               /// in campain, is it goal to build it
                                        
        public RoadModel()
        {
            roadID = ++counter;

            neighbour = new TownModel[2];
            pathNode = new PathNode();
            isBuild = false;
            playerOwner = null;
            goalRoad = false;
        }

        public static void ResetCounter() { counter = 0; }      // every new game have to be reseted

        public static int GetRoadCount() { return counter; }    // number of roads
        public Player GetOwner() { return playerOwner; }
        public IPlayer GetIOwner() { return playerOwner; }
        public int GetRoadID() { return roadID; }
        public bool GetIsBuild() { return isBuild; }
        public ITown[] GetITown() { return neighbour; }

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
                if (town.HasPlayerRoadNeighbour(player) && (town.GetOwner() == player || !town.GetIsBuild()))
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
                if (town.GetOwner() == player)
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
            player.AddRoad(this);
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
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_ALREADY_BUILD);
                    return RoadBuildError.AlreadyBuild;
                }
                if (!IsActivePlayersRoadOnEndOfRoad(activePlayer) && !IsActivePlayersTownOnEndOfRoad(activePlayer))
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_NO_PLAYER_ROAD_OR_TOWN);
                    return RoadBuildError.NoPlayerRoadOrTown;
                }
                if (!Settings.costRoad.HasPlayerSources(activePlayer))
                {
                    GameState.map.GetMapController().SetLastError(Strings.ERROR_NO_SOURCES);
                    return RoadBuildError.NoSources;
                }
                return RoadBuildError.OK;
            }

            return RoadBuildError.OK;
        }

        internal void ClearNodePath()
        {
            pathNode.Clear();
        }

        internal PathNode GetPathNode() { return pathNode; }
        internal void SetPathNode(int distance, TownModel ancestorTown, IRoad ancestorRoad)
        {
            pathNode.Set(distance, ancestorTown, ancestorRoad);
        }

        public bool GoalRoad
        {
            get
            {
                return goalRoad;
            }
            set
            {
                goalRoad = value;
            }
        }
    }
}
