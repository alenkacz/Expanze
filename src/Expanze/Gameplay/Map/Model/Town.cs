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
        private Player playerOwner; /// owner of this town, if null no player owns town on this place
        private bool isBuild;       /// is there town or it is only place for possible town
        private Road[] roadNeighbour; /// two or 3 neighbours
        private Town[] townNeighbour; /// two or 3 neighbours
        private HexaModel[] hexaNeighbour; /// 1 - 3 neighbours
        private BuildingKind[] building; /// indeces corresponds with hexaNeighbour indeces

        private int townID;             /// unique ID of town place, from 1 to counter
        public static int counter = 0;  /// how many town places are on the map

        public int getTownID() { return townID; }
        public bool getIsBuild() { return isBuild; }
        public Player getPlayerOwner() { return playerOwner; }
        public ISourceAll getCost() { return Settings.costTown; }
        public IHexaGet getIHexaGet(int pos) { return getHexa(pos); }
        public HexaModel getHexa(int pos) { return hexaNeighbour[pos]; }
        public static int getTownCount() { return counter; }    // number of towns

        public Town()
        {
            townID = ++counter;
            isBuild = false;

            roadNeighbour = new Road[3];
            townNeighbour = new Town[3];
            hexaNeighbour = new HexaModel[3];
            building = new BuildingKind[3];
            for (int loop1 = 0; loop1 < building.Length; loop1++)
                building[loop1] = BuildingKind.NoBuilding;

            //building[1] = BuildingKind.SourceBuilding;

            playerOwner = null;
        }

        public int findBuildingByHexaID(int hexaID)
        {
            for (int loop1 = 0; loop1 < building.Length; loop1++)
            {
                if (hexaNeighbour[loop1] != null && hexaNeighbour[loop1].getID() == hexaID)
                    return loop1;
            }
            return -1;
        }

        public BuildingKind getBuildingKind(int hexaID)
        {
            if (!isBuild)
                return BuildingKind.NoBuilding;

            int buildingPos = findBuildingByHexaID(hexaID);


            return (buildingPos == -1) ? BuildingKind.NoBuilding : building[buildingPos];
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

        public void collectSources(Player player)
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
                        case HexaKind.Forest:
                            cost = cost + new SourceAll(amount, 0, 0, 0, 0);
                            break;

                        case HexaKind.Stone:
                            cost = cost + new SourceAll(0, amount, 0, 0, 0);
                            break;

                        case HexaKind.Cornfield:
                            cost = cost + new SourceAll(0, 0, amount, 0, 0);
                            break;

                        case HexaKind.Pasture:
                            cost = cost + new SourceAll(0, 0, 0, amount, 0);
                            break;

                        case HexaKind.Mountains:
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

        public BuildingBuildError canActivePlayerBuildBuildingInTown(int pos, SourceAll cost)
        {
            GameMaster gm = GameMaster.getInstance();
            if (gm.getState() == EGameState.StateGame)
            {
                if (building[pos] != BuildingKind.NoBuilding)
                    return BuildingBuildError.AlreadyBuild;

                if (gm.getActivePlayer() != playerOwner)
                    return BuildingBuildError.NotOwner;

                if (!cost.HasPlayerSources(gm.getActivePlayer()))
                    return BuildingBuildError.NoSources;

                if (cost == new SourceAll(0))
                {
                    // WHAT?
                }

                return BuildingBuildError.OK;
            }

            return BuildingBuildError.NoSources;
        }

        public TownBuildError CanActivePlayerBuildTown()
        {
            GameMaster gm = GameMaster.getInstance();
            if (gm.getState() == EGameState.StateGame)
            {
                Player activePlayer = gm.getActivePlayer();
                Boolean hasActivePlayerRoadNeighbour = false;

                foreach (Road road in roadNeighbour)
                {
                    if (road != null && road.getOwner() == activePlayer)
                        hasActivePlayerRoadNeighbour = true;
                }

                if (isBuild)
                    return TownBuildError.AlreadyBuild;
                if (HasTownBuildNeighbour())
                    return TownBuildError.OtherTownIsClose;
                if (!Settings.costTown.HasPlayerSources(activePlayer))
                    return TownBuildError.NoSources;
                if (!hasActivePlayerRoadNeighbour)
                    return TownBuildError.NoPlayerRoad;

                return TownBuildError.OK;
            }
            else
            {
                if (isBuild)
                    return TownBuildError.AlreadyBuild;
                if (HasTownBuildNeighbour())
                    return TownBuildError.OtherTownIsClose;

                return TownBuildError.OK;
            }
        }

        public void buildBuilding(int pos)
        {
            building[pos] = BuildingKind.SourceBuilding;
        }
    }
}