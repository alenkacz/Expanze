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
    /// <summary>
    /// Containing information about one single hexa
    /// </summary>
    class HexaModel : IHexaGet
    {
        int value;      // how many sources will player get
        int hexaID;     // from counter, useable for picking
        private static int counter = 0;    // how many hexas are created

        private HexaType type = HexaType.Water;
        private HexaModel[] hexaNeighbours;      // neighbours of hexa, to index use RoadPos
        private Town[] towns;               // possible towns on hexa, to index use Town Pos
        private Boolean[] townOwner;        // was this town made by this hexa? if was, this hexa will draw it, handle picking, get sources...
        private Road[] roads;               // possible roads on hexa, to index use RoadPos
        private Boolean[] roadOwner;        // was this road made by this hexa? if was, this hexa will draw it, handle picking...

        public HexaModel() : this(0, HexaType.Water) { }

        public HexaModel(int value, HexaType type)
        {
            this.hexaID = ++counter;
            
            
            this.type = type;
            this.value = value;
            this.towns = new Town[(int) TownPos.Count];
            this.roads = new Road[(int)RoadPos.Count];
            this.townOwner = new Boolean[(int)TownPos.Count];
            this.roadOwner = new Boolean[(int)RoadPos.Count];
        }

        public static void resetCounter() { counter = 0; }

        public void CreateTownsAndRoads(HexaModel[] neighbours, HexaView hexaView)
        {
            hexaNeighbours = new HexaModel[neighbours.Length];
            for (int loop1 = 0; loop1 < neighbours.Length; loop1++)
                hexaNeighbours[loop1] = neighbours[loop1];

            if (type == HexaType.Nothing || type == HexaType.Water)
                return;

            ///////////////////////
            // Creating roads -> //
            ///////////////////////
            // Always make owns road or get reference from other roads (not sending reference to other hexas)
            if (neighbours[(int)RoadPos.UpLeft] == null ||
                neighbours[(int)RoadPos.UpLeft].getRoad(RoadPos.BottomRight) == null)
            {
                roadOwner[(int)RoadPos.UpLeft] = true;
                roads[(int)RoadPos.UpLeft] = new Road();
                hexaView.createRoadView(RoadPos.UpLeft, Matrix.CreateRotationY(-(float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(-0.25f, 0.0f, 0.14f)));
            }
            else
                roads[(int)RoadPos.UpLeft] = neighbours[(int)RoadPos.UpLeft].getRoad(RoadPos.BottomRight);

            if (neighbours[(int)RoadPos.UpRight] == null ||
                neighbours[(int)RoadPos.UpRight].getRoad(RoadPos.BottomLeft) == null)
            {
                roadOwner[(int)RoadPos.UpRight] = true;
                roads[(int)RoadPos.UpRight] = new Road();
                hexaView.createRoadView(RoadPos.UpRight, Matrix.CreateRotationY((float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(-0.25f, 0.0f, -0.14f)));
            }
            else
                roads[(int)RoadPos.UpRight] = neighbours[(int)RoadPos.UpRight].getRoad(RoadPos.BottomLeft);

            if (neighbours[(int)RoadPos.MiddleLeft] == null ||
                neighbours[(int)RoadPos.MiddleLeft].getRoad(RoadPos.MiddleRight) == null)
            {
                roadOwner[(int)RoadPos.MiddleLeft] = true;
                roads[(int)RoadPos.MiddleLeft] = new Road();
                hexaView.createRoadView(RoadPos.MiddleLeft, Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, 0.29f)));
            }
            else
                roads[(int)RoadPos.MiddleLeft] = neighbours[(int)RoadPos.MiddleLeft].getRoad(RoadPos.MiddleRight);

            if (neighbours[(int)RoadPos.MiddleRight] == null ||
                neighbours[(int)RoadPos.MiddleRight].getRoad(RoadPos.MiddleLeft) == null)
            {
                roadOwner[(int)RoadPos.MiddleRight] = true;
                roads[(int)RoadPos.MiddleRight] = new Road();
                hexaView.createRoadView(RoadPos.MiddleRight, Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, -0.28f)));
            }
            else
                roads[(int)RoadPos.MiddleRight] = neighbours[(int)RoadPos.MiddleRight].getRoad(RoadPos.MiddleLeft);


            if (neighbours[(int)RoadPos.BottomLeft] == null ||
                neighbours[(int)RoadPos.BottomLeft].getRoad(RoadPos.UpRight) == null)
            {
                roadOwner[(int)RoadPos.BottomLeft] = true;
                roads[(int)RoadPos.BottomLeft] = new Road();
                hexaView.createRoadView(RoadPos.BottomLeft, Matrix.CreateRotationY((float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(0.25f, 0.0f, 0.14f)));
            }
            else
                roads[(int)RoadPos.BottomLeft] = neighbours[(int)RoadPos.BottomLeft].getRoad(RoadPos.UpRight);

            if (neighbours[(int)RoadPos.BottomRight] == null ||
                neighbours[(int)RoadPos.BottomRight].getRoad(RoadPos.UpLeft) == null)
            {
                roadOwner[(int)RoadPos.BottomRight] = true;
                roads[(int)RoadPos.BottomRight] = new Road();
                hexaView.createRoadView(RoadPos.BottomRight, Matrix.CreateRotationY(-(float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(0.25f, 0.0f, -0.14f)));
            }
            else
                roads[(int)RoadPos.BottomRight] = neighbours[(int)RoadPos.BottomRight].getRoad(RoadPos.UpLeft);
            
            ///////////////////////
            // Creating tows ->  //
            ///////////////////////
            if ((neighbours[(int)RoadPos.UpLeft] == null ||
                 neighbours[(int)RoadPos.UpLeft].getTown(TownPos.BottomRight) == null) &&
                (neighbours[(int)RoadPos.UpRight] == null ||
                 neighbours[(int)RoadPos.UpRight].getTown(TownPos.BottomLeft) == null))
            {
                townOwner[(int)TownPos.Up] = true;
                towns[(int)TownPos.Up] = new Town();
                hexaView.createTownView(TownPos.Up, Matrix.CreateTranslation(new Vector3(-0.32f, 0.0f, 0.0f)));
            }
            else
                if (neighbours[(int)RoadPos.UpLeft] != null && neighbours[(int)RoadPos.UpLeft].getTown(TownPos.BottomRight) != null)
                    towns[(int)TownPos.Up] = neighbours[(int)RoadPos.UpLeft].getTown(TownPos.BottomRight);
                else
                    towns[(int)TownPos.Up] = neighbours[(int)RoadPos.UpRight].getTown(TownPos.BottomLeft);

            if ((neighbours[(int)RoadPos.BottomLeft] == null ||
                 neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.UpRight) == null) &&
                (neighbours[(int)RoadPos.BottomRight] == null ||
                 neighbours[(int)RoadPos.BottomRight].getTown(TownPos.UpLeft) == null))
            {
                townOwner[(int)TownPos.Bottom] = true;
                towns[(int)TownPos.Bottom] = new Town();
                hexaView.createTownView(TownPos.Bottom, Matrix.CreateTranslation(new Vector3(0.32f, 0.0f, 0.0f)));
            }
            else
                towns[(int)TownPos.Bottom] = neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.UpRight);

            if ((neighbours[(int)RoadPos.UpRight] == null ||
                neighbours[(int)RoadPos.UpRight].getTown(TownPos.Bottom) == null) &&
               (neighbours[(int)RoadPos.MiddleRight] == null ||
                neighbours[(int)RoadPos.MiddleRight].getTown(TownPos.UpLeft) == null))
            {
                townOwner[(int)TownPos.UpRight] = true;
                towns[(int)TownPos.UpRight] = new Town();
                hexaView.createTownView(TownPos.UpRight, Matrix.CreateTranslation(new Vector3(-0.16f, 0.0f, -0.28f))); 
            }
            else
                if (neighbours[(int)RoadPos.UpRight] != null && neighbours[(int)RoadPos.UpRight].getTown(TownPos.Bottom) != null)
                    towns[(int)TownPos.UpRight] = neighbours[(int)RoadPos.UpRight].getTown(TownPos.Bottom);
                else
                    towns[(int)TownPos.UpRight] = neighbours[(int)RoadPos.MiddleRight].getTown(TownPos.UpLeft);

            if ((neighbours[(int)RoadPos.UpLeft] == null ||
                 neighbours[(int)RoadPos.UpLeft].getTown(TownPos.Bottom) == null) &&
                (neighbours[(int)RoadPos.MiddleLeft] == null ||
                 neighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.UpRight) == null))
            {
                townOwner[(int)TownPos.UpLeft] = true;
                towns[(int)TownPos.UpLeft] = new Town();
                hexaView.createTownView(TownPos.UpLeft, Matrix.CreateTranslation(new Vector3(-0.16f, 0.0f, 0.28f)));
            }
            else
                towns[(int)TownPos.UpLeft] = neighbours[(int)RoadPos.UpLeft].getTown(TownPos.Bottom);

            if ((neighbours[(int)RoadPos.BottomRight] == null ||
                neighbours[(int)RoadPos.BottomRight].getTown(TownPos.Up) == null) &&
               (neighbours[(int)RoadPos.MiddleRight] == null ||
                neighbours[(int)RoadPos.MiddleRight].getTown(TownPos.BottomLeft) == null))
            {
                townOwner[(int)TownPos.BottomRight] = true;
                towns[(int)TownPos.BottomRight] = new Town();
                hexaView.createTownView(TownPos.BottomRight, Matrix.CreateTranslation(new Vector3(0.16f, 0.0f, -0.28f))); 
            }
            else
                if (neighbours[(int)RoadPos.BottomRight] != null && neighbours[(int)RoadPos.BottomRight].getTown(TownPos.Bottom) != null)
                    towns[(int)TownPos.BottomRight] = neighbours[(int)RoadPos.BottomRight].getTown(TownPos.Up);
                else
                    towns[(int)TownPos.BottomRight] = neighbours[(int)RoadPos.MiddleRight].getTown(TownPos.BottomLeft);

            if ((neighbours[(int)RoadPos.BottomLeft] == null ||
                neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.Up) == null) &&
               (neighbours[(int)RoadPos.MiddleLeft] == null ||
                neighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.BottomRight) == null))
            {
                townOwner[(int)TownPos.BottomLeft] = true;
                towns[(int)TownPos.BottomLeft] = new Town();
                hexaView.createTownView(TownPos.BottomLeft, Matrix.CreateTranslation(new Vector3(0.16f, 0.0f, 0.28f)));
            }
            else
                if (neighbours[(int)RoadPos.BottomLeft] != null && neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.Bottom) != null)
                    towns[(int)TownPos.BottomLeft] = neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.Up);
                else
                    towns[(int)TownPos.BottomLeft] = neighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.BottomRight);
        }

        public void FindTownNeighbours()
        {
            Town thirdTown = null;
            Road thirdRoad = null;
            for (int loop1 = 0; loop1 < towns.Length; loop1++)
            {
                if (townOwner[loop1])
                {
                    thirdTown = null;
                    thirdRoad = null;

                    switch ((TownPos)loop1)
                    {
                        case TownPos.Up :
                            if(hexaNeighbours[(int) RoadPos.UpLeft] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.UpLeft].getTown(TownPos.UpRight);
                                thirdRoad = hexaNeighbours[(int)RoadPos.UpLeft].getRoad(RoadPos.MiddleRight);
                            }
                            else if (hexaNeighbours[(int)RoadPos.UpRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.UpRight].getTown(TownPos.UpLeft);
                                thirdRoad = hexaNeighbours[(int)RoadPos.UpRight].getRoad(RoadPos.MiddleLeft);
                            }
                            towns[loop1].setRoadNeighbours(roads[(int)RoadPos.UpLeft], roads[(int)RoadPos.UpRight], thirdRoad);
                            towns[loop1].setTownNeighbours(towns[(int)TownPos.UpLeft], towns[(int)TownPos.UpRight], thirdTown);
                            towns[loop1].setHexaNeighbours(this, hexaNeighbours[(int)RoadPos.UpLeft], hexaNeighbours[(int)RoadPos.UpRight]);
                            break;

                        case TownPos.Bottom:
                            if (hexaNeighbours[(int)RoadPos.BottomLeft] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.BottomLeft].getTown(TownPos.BottomRight);
                                thirdRoad = hexaNeighbours[(int)RoadPos.BottomLeft].getRoad(RoadPos.MiddleRight);
                            }
                            else if (hexaNeighbours[(int)RoadPos.BottomRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.BottomRight].getTown(TownPos.BottomLeft);
                                thirdRoad = hexaNeighbours[(int)RoadPos.BottomRight].getRoad(RoadPos.MiddleLeft);
                            }
                            towns[loop1].setRoadNeighbours(roads[(int)RoadPos.BottomLeft], roads[(int)RoadPos.BottomRight], thirdRoad);
                            towns[loop1].setTownNeighbours(towns[(int)TownPos.BottomLeft], towns[(int)TownPos.BottomRight], thirdTown);
                            towns[loop1].setHexaNeighbours(this, hexaNeighbours[(int)RoadPos.BottomLeft], hexaNeighbours[(int)RoadPos.BottomRight]);
                            break;

                        case TownPos.UpLeft:
                            if (hexaNeighbours[(int)RoadPos.MiddleLeft] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.Up);
                                thirdRoad = hexaNeighbours[(int)RoadPos.MiddleLeft].getRoad(RoadPos.UpRight);
                            }
                            else if (hexaNeighbours[(int)RoadPos.UpLeft] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.UpLeft].getTown(TownPos.BottomLeft);
                                thirdRoad = hexaNeighbours[(int)RoadPos.UpLeft].getRoad(RoadPos.BottomLeft);
                            }
                            towns[loop1].setRoadNeighbours(roads[(int)RoadPos.UpLeft], roads[(int)RoadPos.MiddleLeft], thirdRoad);
                            towns[loop1].setTownNeighbours(towns[(int)TownPos.Up], towns[(int)TownPos.BottomLeft], thirdTown);
                            towns[loop1].setHexaNeighbours(this, hexaNeighbours[(int)RoadPos.UpLeft], hexaNeighbours[(int)RoadPos.MiddleLeft]);
                            break;

                        case TownPos.UpRight:
                            if (hexaNeighbours[(int)RoadPos.MiddleRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.MiddleRight].getTown(TownPos.Up);
                                thirdRoad = hexaNeighbours[(int)RoadPos.MiddleRight].getRoad(RoadPos.UpLeft);
                            }
                            else if (hexaNeighbours[(int)RoadPos.UpRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.UpRight].getTown(TownPos.BottomRight);
                                thirdRoad = hexaNeighbours[(int)RoadPos.UpRight].getRoad(RoadPos.BottomRight);
                            }
                            towns[loop1].setRoadNeighbours(roads[(int)RoadPos.UpRight], roads[(int)RoadPos.MiddleRight], thirdRoad);
                            towns[loop1].setTownNeighbours(towns[(int)TownPos.Up], towns[(int)TownPos.BottomRight], thirdTown);
                            towns[loop1].setHexaNeighbours(this, hexaNeighbours[(int)RoadPos.UpRight], hexaNeighbours[(int)RoadPos.MiddleRight]);
                            break;

                        case TownPos.BottomLeft:
                            if (hexaNeighbours[(int)RoadPos.MiddleLeft] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.Bottom);
                                thirdRoad = hexaNeighbours[(int)RoadPos.MiddleLeft].getRoad(RoadPos.BottomRight);
                            }
                            else if (hexaNeighbours[(int)RoadPos.BottomLeft] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.BottomLeft].getTown(TownPos.UpLeft);
                                thirdRoad = hexaNeighbours[(int)RoadPos.BottomLeft].getRoad(RoadPos.UpLeft);
                            }
                            towns[loop1].setRoadNeighbours(roads[(int)RoadPos.BottomLeft], roads[(int)RoadPos.MiddleLeft], thirdRoad);
                            towns[loop1].setTownNeighbours(towns[(int)TownPos.Bottom], towns[(int)TownPos.UpLeft], thirdTown);
                            towns[loop1].setHexaNeighbours(this, hexaNeighbours[(int)RoadPos.BottomLeft], hexaNeighbours[(int)RoadPos.MiddleLeft]);
                            break;

                        case TownPos.BottomRight:
                            if (hexaNeighbours[(int)RoadPos.MiddleRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.MiddleRight].getTown(TownPos.Bottom);
                                thirdRoad = hexaNeighbours[(int)RoadPos.MiddleRight].getRoad(RoadPos.BottomLeft);
                            }
                            else if (hexaNeighbours[(int)RoadPos.BottomRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.BottomRight].getTown(TownPos.UpRight);
                                thirdRoad = hexaNeighbours[(int)RoadPos.BottomRight].getRoad(RoadPos.UpRight);
                            }
                            towns[loop1].setRoadNeighbours(roads[(int)RoadPos.BottomRight], roads[(int)RoadPos.MiddleRight], thirdRoad);
                            towns[loop1].setTownNeighbours(towns[(int)TownPos.Bottom], towns[(int)TownPos.UpRight], thirdTown);
                            towns[loop1].setHexaNeighbours(this, hexaNeighbours[(int)RoadPos.BottomRight], hexaNeighbours[(int)RoadPos.MiddleRight]);
                            break;
                    }
                }
            }
        }

        public void FindRoadNeighbours()
        {
            for (int loop1 = 0; loop1 < roads.Length; loop1++)
            {
                if (roadOwner[loop1])
                {
                    switch ((RoadPos) loop1)
                    {
                        case RoadPos.UpLeft :
                            roads[loop1].SetTownNeighbours(towns[(int) TownPos.UpLeft], towns[(int) TownPos.Up]);
                            break;

                        case RoadPos.UpRight:
                            roads[loop1].SetTownNeighbours(towns[(int)TownPos.UpRight], towns[(int)TownPos.Up]);
                            break;

                        case RoadPos.BottomLeft:
                            roads[loop1].SetTownNeighbours(towns[(int)TownPos.BottomLeft], towns[(int)TownPos.Bottom]);
                            break;

                        case RoadPos.BottomRight:
                            roads[loop1].SetTownNeighbours(towns[(int)TownPos.BottomRight], towns[(int)TownPos.Bottom]);
                            break;

                        case RoadPos.MiddleLeft:
                            roads[loop1].SetTownNeighbours(towns[(int)TownPos.UpLeft], towns[(int)TownPos.BottomLeft]);
                            break;

                        case RoadPos.MiddleRight:
                            roads[loop1].SetTownNeighbours(towns[(int)TownPos.UpRight], towns[(int)TownPos.BottomRight]);
                            break;
                    }
                }
            }
        }

        public void CollectSources(Player player)
        {
            for (int loop1 = 0; loop1 < towns.Length; loop1++)
                if (townOwner[loop1])
                    towns[loop1].CollectSources(player);
        }

        public string getModelPath()
        {
            return Settings.mapPaths[(int)getType()];
        }

        public int getValue() { return value; }
        public HexaType getType()
        {
            return this.type;
        }

        public Road getRoad(RoadPos roadPos)
        {
            return roads[(int)roadPos];
        }

        public Road GetRoadByID(int roadID)
        {
            for (int loop1 = 0; loop1 < roads.Length; loop1++)
                if (roadOwner[loop1])
                {
                    if (roadID == roads[loop1].getRoadID())
                        return roads[loop1];
                }

            return null;
        }

        public Town getTown(TownPos townPos)
        {
            return towns[(int)townPos];
        }

        public ITownGet getITown(TownPos townPos)
        {
            return towns[(int)townPos];
        }

        public Town GetTownByID(int townID)
        {
            for (int loop1 = 0; loop1 < towns.Length; loop1++)
                if (townOwner[loop1])
                {
                    if (townID == towns[loop1].getTownID())
                        return towns[loop1];
                }

            return null;
        }

        public int getID() { return hexaID; }
        public Boolean getRoadOwner(int i) { return roadOwner[i]; }
        public Boolean getTownOwner(int i) { return townOwner[i]; }
    }
}
