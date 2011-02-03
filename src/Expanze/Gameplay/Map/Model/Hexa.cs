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
    class HexaModel : IHexa
    {
        private int startSource;      /// how many sources will player get
        private bool sourceDisaster;  /// is on hex disaster 
        private int turnDisaster;     /// how many turns disaster will last
        private bool sourceMiracle;   /// is on hex miracle
        private int turnMiracle;      /// how many turns miracle will last

        private bool captureIs;       /// has been hexa captured from fort?
        private Player capturePlayer; /// who has captured hexa

        /// destroying was in the game
        private bool hexaDestroy;      /// was hexa destroyed from fort
        private int turnDestroy;      /// how many turns will be hexa destroyed

        private int hexaID;             /// from counter, useful for picking
        private int x;                  /// x coord in map
        private int y;                  /// y coord in map

        private static int counter = 0;    /// how many hexas are created
        private static int hexaIDFort;     /// from which hexa are soldiers send to capturing/destroying hexas?
        SourceAll sourceBuildingCost;      /// source cost of building according hexa kind

        private HexaKind kind = HexaKind.Water;
        private HexaModel[] hexaNeighbours;      /// neighbours of hexa, to index use RoadPos
        private TownModel[] towns;               /// possible towns on hexa, to index use Town Pos
        private Boolean[] townOwner;             /// was this town made by this hexa? if was, this hexa will draw it, handle picking, get sources...
        private RoadModel[] roads;               /// possible roads on hexa, to index use RoadPos
        private Boolean[] roadOwner;             /// was this road made by this hexa? if was, this hexa will draw it, handle picking...



        public HexaModel() : this(0, HexaKind.Water, new SourceAll(0)) { }

        public HexaModel(int value, HexaKind type, SourceAll sourceBuildingCost)
        {
            if (type == HexaKind.Water)
            {
                this.hexaID = -1;
            } else
                this.hexaID = ++counter;

            this.sourceBuildingCost = sourceBuildingCost;
            this.kind = type;
            this.startSource = value;
            this.towns = new TownModel[(int) TownPos.Count];
            this.roads = new RoadModel[(int)RoadPos.Count];
            this.townOwner = new Boolean[(int)TownPos.Count];
            this.roadOwner = new Boolean[(int)RoadPos.Count];

            sourceDisaster = false;
            sourceMiracle = false;
            hexaDestroy = false;
            captureIs = false;
            capturePlayer = null;
        }

        public static void ResetCounter() { counter = 0; }
        public static int GetHexaCount() { return counter; }
        public static int GetHexaIDFort() { return hexaIDFort; }
        public static void SetHexaIDFort(int hexaID) { hexaIDFort = hexaID; }
        public bool GetCaptured() { return captureIs; }
        public Player GetCapturedPlayer() { return capturePlayer; }

        public void Update(GameTime gameTime)
        {
        }

        public void NextTurn()
        {
            if (sourceDisaster)
            {
                turnDisaster--;
                if (turnDisaster == 0)
                    sourceDisaster = false;
            }

            if (sourceMiracle)
            {
                turnMiracle--;
                if (turnMiracle == 0)
                    sourceMiracle = false;
            }

            if (hexaDestroy)
            {
                turnDestroy--;
                if (turnDestroy == 0)
                    hexaDestroy = false;
            }
        }

        public void CreateTownsAndRoads(HexaModel[] neighboursModel, HexaView hexaView, HexaView[] neighboursView)
        {
            hexaNeighbours = new HexaModel[neighboursModel.Length];
            for (int loop1 = 0; loop1 < neighboursModel.Length; loop1++)
                hexaNeighbours[loop1] = neighboursModel[loop1];

            if (kind == HexaKind.Nothing || kind == HexaKind.Water)
                return;

            ///////////////////////
            // Creating roads -> //
            ///////////////////////
            // Always make owns road or get reference from other roads (not sending reference to other hexas)
            if (neighboursModel[(int)RoadPos.UpLeft] == null ||
                neighboursModel[(int)RoadPos.UpLeft].GetRoad(RoadPos.BottomRight) == null)
            {
                roadOwner[(int)RoadPos.UpLeft] = true;
                roads[(int)RoadPos.UpLeft] = new RoadModel();
                hexaView.CreateRoadView(RoadPos.UpLeft, Matrix.CreateRotationY(-(float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(-0.25f, 0.0f, 0.14f)));
            }
            else
            {
                roads[(int)RoadPos.UpLeft] = neighboursModel[(int)RoadPos.UpLeft].GetRoad(RoadPos.BottomRight);
                hexaView.SetRoadView(RoadPos.UpLeft, neighboursView[(int)RoadPos.UpLeft].GetRoadView(RoadPos.BottomRight));
            }

            if (neighboursModel[(int)RoadPos.UpRight] == null ||
                neighboursModel[(int)RoadPos.UpRight].GetRoad(RoadPos.BottomLeft) == null)
            {
                roadOwner[(int)RoadPos.UpRight] = true;
                roads[(int)RoadPos.UpRight] = new RoadModel();
                hexaView.CreateRoadView(RoadPos.UpRight, Matrix.CreateRotationY((float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(-0.25f, 0.0f, -0.14f)));
            }
            else
            {
                roads[(int)RoadPos.UpRight] = neighboursModel[(int)RoadPos.UpRight].GetRoad(RoadPos.BottomLeft);
                hexaView.SetRoadView(RoadPos.UpRight, neighboursView[(int)RoadPos.UpRight].GetRoadView(RoadPos.BottomLeft));
            }

            if (neighboursModel[(int)RoadPos.MiddleLeft] == null ||
                neighboursModel[(int)RoadPos.MiddleLeft].GetRoad(RoadPos.MiddleRight) == null)
            {
                roadOwner[(int)RoadPos.MiddleLeft] = true;
                roads[(int)RoadPos.MiddleLeft] = new RoadModel();
                hexaView.CreateRoadView(RoadPos.MiddleLeft, Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, 0.29f)));
            }
            else
            {
                roads[(int)RoadPos.MiddleLeft] = neighboursModel[(int)RoadPos.MiddleLeft].GetRoad(RoadPos.MiddleRight);
                hexaView.SetRoadView(RoadPos.MiddleLeft, neighboursView[(int)RoadPos.MiddleLeft].GetRoadView(RoadPos.MiddleRight));
            }

            if (neighboursModel[(int)RoadPos.MiddleRight] == null ||
                neighboursModel[(int)RoadPos.MiddleRight].GetRoad(RoadPos.MiddleLeft) == null)
            {
                roadOwner[(int)RoadPos.MiddleRight] = true;
                roads[(int)RoadPos.MiddleRight] = new RoadModel();
                hexaView.CreateRoadView(RoadPos.MiddleRight, Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, -0.28f)));
            }
            else
            {
                roads[(int)RoadPos.MiddleRight] = neighboursModel[(int)RoadPos.MiddleRight].GetRoad(RoadPos.MiddleLeft);
                hexaView.SetRoadView(RoadPos.MiddleRight, neighboursView[(int)RoadPos.MiddleRight].GetRoadView(RoadPos.MiddleLeft));
            }

            if (neighboursModel[(int)RoadPos.BottomLeft] == null ||
                neighboursModel[(int)RoadPos.BottomLeft].GetRoad(RoadPos.UpRight) == null)
            {
                roadOwner[(int)RoadPos.BottomLeft] = true;
                roads[(int)RoadPos.BottomLeft] = new RoadModel();
                hexaView.CreateRoadView(RoadPos.BottomLeft, Matrix.CreateRotationY((float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(0.25f, 0.0f, 0.14f)));
            }
            else
            {
                roads[(int)RoadPos.BottomLeft] = neighboursModel[(int)RoadPos.BottomLeft].GetRoad(RoadPos.UpRight);
                hexaView.SetRoadView(RoadPos.BottomLeft, neighboursView[(int)RoadPos.BottomLeft].GetRoadView(RoadPos.UpRight));
            }
            if (neighboursModel[(int)RoadPos.BottomRight] == null ||
                neighboursModel[(int)RoadPos.BottomRight].GetRoad(RoadPos.UpLeft) == null)
            {
                roadOwner[(int)RoadPos.BottomRight] = true;
                roads[(int)RoadPos.BottomRight] = new RoadModel();
                hexaView.CreateRoadView(RoadPos.BottomRight, Matrix.CreateRotationY(-(float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(0.25f, 0.0f, -0.14f)));
            }
            else
            {
                roads[(int)RoadPos.BottomRight] = neighboursModel[(int)RoadPos.BottomRight].GetRoad(RoadPos.UpLeft);
                hexaView.SetRoadView(RoadPos.BottomRight, neighboursView[(int)RoadPos.BottomRight].GetRoadView(RoadPos.UpLeft));
            }

            ///////////////////////
            // Creating tows ->  //
            ///////////////////////
            if ((neighboursModel[(int)RoadPos.UpLeft] == null ||
                 neighboursModel[(int)RoadPos.UpLeft].getTown(TownPos.BottomRight) == null) &&
                (neighboursModel[(int)RoadPos.UpRight] == null ||
                 neighboursModel[(int)RoadPos.UpRight].getTown(TownPos.BottomLeft) == null))
            {
                townOwner[(int)TownPos.Up] = true;
                towns[(int)TownPos.Up] = new TownModel();
                hexaView.CreateTownView(TownPos.Up, Matrix.CreateTranslation(new Vector3(-0.32f, 0.0f, 0.0f)));
            }
            else
            {
                if (neighboursModel[(int)RoadPos.UpLeft] != null && neighboursModel[(int)RoadPos.UpLeft].getTown(TownPos.BottomRight) != null)
                {
                    towns[(int)TownPos.Up] = neighboursModel[(int)RoadPos.UpLeft].getTown(TownPos.BottomRight);
                    hexaView.SetTownView(TownPos.Up, neighboursView[(int)RoadPos.UpLeft].GetTownView(TownPos.BottomRight));
                }
                else
                {
                    towns[(int)TownPos.Up] = neighboursModel[(int)RoadPos.UpRight].getTown(TownPos.BottomLeft);
                    hexaView.SetTownView(TownPos.Up, neighboursView[(int)RoadPos.UpRight].GetTownView(TownPos.BottomLeft));
                }
            }

            if ((neighboursModel[(int)RoadPos.BottomLeft] == null ||
                 neighboursModel[(int)RoadPos.BottomLeft].getTown(TownPos.UpRight) == null) &&
                (neighboursModel[(int)RoadPos.BottomRight] == null ||
                 neighboursModel[(int)RoadPos.BottomRight].getTown(TownPos.UpLeft) == null))
            {
                townOwner[(int)TownPos.Bottom] = true;
                towns[(int)TownPos.Bottom] = new TownModel();
                hexaView.CreateTownView(TownPos.Bottom, Matrix.CreateTranslation(new Vector3(0.32f, 0.0f, 0.0f)));
            }
            else
            {
                towns[(int)TownPos.Bottom] = neighboursModel[(int)RoadPos.BottomLeft].getTown(TownPos.UpRight);
                hexaView.SetTownView(TownPos.Bottom, neighboursView[(int)RoadPos.BottomLeft].GetTownView(TownPos.UpRight));
            }

            if ((neighboursModel[(int)RoadPos.UpRight] == null ||
                neighboursModel[(int)RoadPos.UpRight].getTown(TownPos.Bottom) == null) &&
               (neighboursModel[(int)RoadPos.MiddleRight] == null ||
                neighboursModel[(int)RoadPos.MiddleRight].getTown(TownPos.UpLeft) == null))
            {
                townOwner[(int)TownPos.UpRight] = true;
                towns[(int)TownPos.UpRight] = new TownModel();
                hexaView.CreateTownView(TownPos.UpRight, Matrix.CreateTranslation(new Vector3(-0.16f, 0.0f, -0.28f)));
            }
            else
            {
                if (neighboursModel[(int)RoadPos.UpRight] != null && neighboursModel[(int)RoadPos.UpRight].getTown(TownPos.Bottom) != null)
                {
                    towns[(int)TownPos.UpRight] = neighboursModel[(int)RoadPos.UpRight].getTown(TownPos.Bottom);
                    hexaView.SetTownView(TownPos.UpRight, neighboursView[(int)RoadPos.UpRight].GetTownView(TownPos.Bottom));
                }
                else
                {
                    towns[(int)TownPos.UpRight] = neighboursModel[(int)RoadPos.MiddleRight].getTown(TownPos.UpLeft);
                    hexaView.SetTownView(TownPos.UpRight, neighboursView[(int)RoadPos.MiddleRight].GetTownView(TownPos.UpLeft));
                }
            }

            if ((neighboursModel[(int)RoadPos.UpLeft] == null ||
                 neighboursModel[(int)RoadPos.UpLeft].getTown(TownPos.Bottom) == null) &&
                (neighboursModel[(int)RoadPos.MiddleLeft] == null ||
                 neighboursModel[(int)RoadPos.MiddleLeft].getTown(TownPos.UpRight) == null))
            {
                townOwner[(int)TownPos.UpLeft] = true;
                towns[(int)TownPos.UpLeft] = new TownModel();
                hexaView.CreateTownView(TownPos.UpLeft, Matrix.CreateTranslation(new Vector3(-0.16f, 0.0f, 0.28f)));
            }
            else
            {
                towns[(int)TownPos.UpLeft] = neighboursModel[(int)RoadPos.UpLeft].getTown(TownPos.Bottom);
                hexaView.SetTownView(TownPos.UpLeft, neighboursView[(int)RoadPos.UpLeft].GetTownView(TownPos.Bottom));
            }

            if ((neighboursModel[(int)RoadPos.BottomRight] == null ||
                neighboursModel[(int)RoadPos.BottomRight].getTown(TownPos.Up) == null) &&
               (neighboursModel[(int)RoadPos.MiddleRight] == null ||
                neighboursModel[(int)RoadPos.MiddleRight].getTown(TownPos.BottomLeft) == null))
            {
                townOwner[(int)TownPos.BottomRight] = true;
                towns[(int)TownPos.BottomRight] = new TownModel();
                hexaView.CreateTownView(TownPos.BottomRight, Matrix.CreateTranslation(new Vector3(0.16f, 0.0f, -0.28f)));
            }
            else
            {
                if (neighboursModel[(int)RoadPos.BottomRight] != null && neighboursModel[(int)RoadPos.BottomRight].getTown(TownPos.Bottom) != null)
                {
                    towns[(int)TownPos.BottomRight] = neighboursModel[(int)RoadPos.BottomRight].getTown(TownPos.Up);
                    hexaView.SetTownView(TownPos.BottomRight, neighboursView[(int)RoadPos.BottomRight].GetTownView(TownPos.Up));
                }
                else
                {
                    towns[(int)TownPos.BottomRight] = neighboursModel[(int)RoadPos.MiddleRight].getTown(TownPos.BottomLeft);
                    hexaView.SetTownView(TownPos.BottomRight, neighboursView[(int)RoadPos.MiddleRight].GetTownView(TownPos.BottomLeft));
                }
            }
            if ((neighboursModel[(int)RoadPos.BottomLeft] == null ||
                neighboursModel[(int)RoadPos.BottomLeft].getTown(TownPos.Up) == null) &&
               (neighboursModel[(int)RoadPos.MiddleLeft] == null ||
                neighboursModel[(int)RoadPos.MiddleLeft].getTown(TownPos.BottomRight) == null))
            {
                townOwner[(int)TownPos.BottomLeft] = true;
                towns[(int)TownPos.BottomLeft] = new TownModel();
                hexaView.CreateTownView(TownPos.BottomLeft, Matrix.CreateTranslation(new Vector3(0.16f, 0.0f, 0.28f)));
            }
            else
            {
                if (neighboursModel[(int)RoadPos.BottomLeft] != null && neighboursModel[(int)RoadPos.BottomLeft].getTown(TownPos.Bottom) != null)
                {
                    towns[(int)TownPos.BottomLeft] = neighboursModel[(int)RoadPos.BottomLeft].getTown(TownPos.Up);
                    hexaView.SetTownView(TownPos.BottomLeft, neighboursView[(int)RoadPos.BottomLeft].GetTownView(TownPos.Up));
                }
                else
                {
                    towns[(int)TownPos.BottomLeft] = neighboursModel[(int)RoadPos.MiddleLeft].getTown(TownPos.BottomRight);
                    hexaView.SetTownView(TownPos.BottomLeft, neighboursView[(int)RoadPos.MiddleLeft].GetTownView(TownPos.BottomRight));
                }
            }
        }

        public void FindTownNeighbours()
        {
            TownModel thirdTown = null;
            RoadModel thirdRoad = null;
            for (int loop1 = 0; loop1 < towns.Length; loop1++)
            {
                if (townOwner[loop1])
                {
                    thirdTown = null;
                    thirdRoad = null;

                    switch ((TownPos)loop1)
                    {
                        case TownPos.Up :
                            if (hexaNeighbours[(int)RoadPos.UpLeft] != null && hexaNeighbours[(int)RoadPos.UpLeft].GetKind() != HexaKind.Water)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.UpLeft].getTown(TownPos.UpRight);
                                thirdRoad = hexaNeighbours[(int)RoadPos.UpLeft].GetRoad(RoadPos.MiddleRight);
                            }
                            else if (hexaNeighbours[(int)RoadPos.UpRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.UpRight].getTown(TownPos.UpLeft);
                                thirdRoad = hexaNeighbours[(int)RoadPos.UpRight].GetRoad(RoadPos.MiddleLeft);
                            }
                            towns[loop1].SetRoadNeighbours(roads[(int)RoadPos.UpLeft], roads[(int)RoadPos.UpRight], thirdRoad);
                            towns[loop1].SetTownNeighbours(towns[(int)TownPos.UpLeft], towns[(int)TownPos.UpRight], thirdTown);
                            towns[loop1].SetHexaNeighbours(this, hexaNeighbours[(int)RoadPos.UpLeft], hexaNeighbours[(int)RoadPos.UpRight]);
                            break;

                        case TownPos.Bottom:
                            if (hexaNeighbours[(int)RoadPos.BottomLeft] != null && hexaNeighbours[(int)RoadPos.BottomLeft].GetKind() != HexaKind.Water)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.BottomLeft].getTown(TownPos.BottomRight);
                                thirdRoad = hexaNeighbours[(int)RoadPos.BottomLeft].GetRoad(RoadPos.MiddleRight);
                            }
                            else if (hexaNeighbours[(int)RoadPos.BottomRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.BottomRight].getTown(TownPos.BottomLeft);
                                thirdRoad = hexaNeighbours[(int)RoadPos.BottomRight].GetRoad(RoadPos.MiddleLeft);
                            }
                            towns[loop1].SetRoadNeighbours(roads[(int)RoadPos.BottomLeft], roads[(int)RoadPos.BottomRight], thirdRoad);
                            towns[loop1].SetTownNeighbours(towns[(int)TownPos.BottomLeft], towns[(int)TownPos.BottomRight], thirdTown);
                            towns[loop1].SetHexaNeighbours(this, hexaNeighbours[(int)RoadPos.BottomLeft], hexaNeighbours[(int)RoadPos.BottomRight]);
                            break;

                        case TownPos.UpLeft:
                            if (hexaNeighbours[(int)RoadPos.MiddleLeft] != null && hexaNeighbours[(int)RoadPos.MiddleLeft].GetKind() != HexaKind.Water)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.Up);
                                thirdRoad = hexaNeighbours[(int)RoadPos.MiddleLeft].GetRoad(RoadPos.UpRight);
                            }
                            else if (hexaNeighbours[(int)RoadPos.UpLeft] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.UpLeft].getTown(TownPos.BottomLeft);
                                thirdRoad = hexaNeighbours[(int)RoadPos.UpLeft].GetRoad(RoadPos.BottomLeft);
                            }
                            towns[loop1].SetRoadNeighbours(roads[(int)RoadPos.UpLeft], roads[(int)RoadPos.MiddleLeft], thirdRoad);
                            towns[loop1].SetTownNeighbours(towns[(int)TownPos.Up], towns[(int)TownPos.BottomLeft], thirdTown);
                            towns[loop1].SetHexaNeighbours(this, hexaNeighbours[(int)RoadPos.UpLeft], hexaNeighbours[(int)RoadPos.MiddleLeft]);
                            break;

                        case TownPos.UpRight:
                            if (hexaNeighbours[(int)RoadPos.MiddleRight] != null && hexaNeighbours[(int)RoadPos.MiddleRight].GetKind() != HexaKind.Water)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.MiddleRight].getTown(TownPos.Up);
                                thirdRoad = hexaNeighbours[(int)RoadPos.MiddleRight].GetRoad(RoadPos.UpLeft);
                            }
                            else if (hexaNeighbours[(int)RoadPos.UpRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.UpRight].getTown(TownPos.BottomRight);
                                thirdRoad = hexaNeighbours[(int)RoadPos.UpRight].GetRoad(RoadPos.BottomRight);
                            }
                            towns[loop1].SetRoadNeighbours(roads[(int)RoadPos.UpRight], roads[(int)RoadPos.MiddleRight], thirdRoad);
                            towns[loop1].SetTownNeighbours(towns[(int)TownPos.Up], towns[(int)TownPos.BottomRight], thirdTown);
                            towns[loop1].SetHexaNeighbours(this, hexaNeighbours[(int)RoadPos.UpRight], hexaNeighbours[(int)RoadPos.MiddleRight]);
                            break;

                        case TownPos.BottomLeft:
                            if (hexaNeighbours[(int)RoadPos.MiddleLeft] != null && hexaNeighbours[(int)RoadPos.MiddleLeft].GetKind() != HexaKind.Water)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.Bottom);
                                thirdRoad = hexaNeighbours[(int)RoadPos.MiddleLeft].GetRoad(RoadPos.BottomRight);
                            }
                            else if (hexaNeighbours[(int)RoadPos.BottomLeft] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.BottomLeft].getTown(TownPos.UpLeft);
                                thirdRoad = hexaNeighbours[(int)RoadPos.BottomLeft].GetRoad(RoadPos.UpLeft);
                            }
                            towns[loop1].SetRoadNeighbours(roads[(int)RoadPos.BottomLeft], roads[(int)RoadPos.MiddleLeft], thirdRoad);
                            towns[loop1].SetTownNeighbours(towns[(int)TownPos.Bottom], towns[(int)TownPos.UpLeft], thirdTown);
                            towns[loop1].SetHexaNeighbours(this, hexaNeighbours[(int)RoadPos.BottomLeft], hexaNeighbours[(int)RoadPos.MiddleLeft]);
                            break;

                        case TownPos.BottomRight:
                            if (hexaNeighbours[(int)RoadPos.BottomRight] != null && hexaNeighbours[(int)RoadPos.BottomRight].GetKind() != HexaKind.Water)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.BottomRight].getTown(TownPos.UpRight);
                                thirdRoad = hexaNeighbours[(int)RoadPos.BottomRight].GetRoad(RoadPos.UpRight);
                            } else
                            if (hexaNeighbours[(int)RoadPos.MiddleRight] != null)
                            {
                                thirdTown = hexaNeighbours[(int)RoadPos.MiddleRight].getTown(TownPos.Bottom);
                                thirdRoad = hexaNeighbours[(int)RoadPos.MiddleRight].GetRoad(RoadPos.BottomLeft);
                            }
                             
                            towns[loop1].SetRoadNeighbours(roads[(int)RoadPos.BottomRight], roads[(int)RoadPos.MiddleRight], thirdRoad);
                            towns[loop1].SetTownNeighbours(towns[(int)TownPos.Bottom], towns[(int)TownPos.UpRight], thirdTown);
                            towns[loop1].SetHexaNeighbours(this, hexaNeighbours[(int)RoadPos.BottomRight], hexaNeighbours[(int)RoadPos.MiddleRight]);
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

        public int GetStartSource() { return startSource; }
        public int GetCurrentSource()
        {
            float multiply = 1.0f;
            if (sourceMiracle)
                multiply *= 1.5f;
            else if (sourceDisaster)
                multiply *= 0.5f;

            if (hexaDestroy)
                multiply *= 0.5f;

            return (int) (startSource * multiply);
        }

        public HexaKind GetKind()
        {
            return this.kind;
        }

        public RoadModel GetRoad(RoadPos roadPos)
        {
            return roads[(int)roadPos];
        }

        public RoadModel GetRoadByID(int roadID)
        {
            for (int loop1 = 0; loop1 < roads.Length; loop1++)
                if (roadOwner[loop1])
                {
                    if (roadID == roads[loop1].GetRoadID())
                        return roads[loop1];
                }

            return null;
        }

        public TownModel getTown(TownPos townPos)
        {
            return towns[(int)townPos];
        }

        public ITown GetITown(TownPos townPos)
        {
            return towns[(int)townPos];
        }

        public TownModel GetTownByID(int townID)
        {
            for (int loop1 = 0; loop1 < towns.Length; loop1++)
                if (townOwner[loop1])
                {
                    if (townID == towns[loop1].GetTownID())
                        return towns[loop1];
                }

            return null;
        }

        public int GetID() { return hexaID; }
        public Boolean GetRoadOwner(int i) { return roadOwner[i]; }
        public Boolean GetTownOwner(int i) { return townOwner[i]; }
        public HexaModel GetHexaNeighbour(int i) { return hexaNeighbours[i]; }
        public SourceAll GetSourceBuildingCost() { return sourceBuildingCost; }
        public void SetCoord(int x, int y) { this.x = x; this.y = y; }


        public bool IsInFortRadius()
        {
            /// You cant destroy or capture desert
            if (kind == HexaKind.Desert)
                return false;

            if (hexaIDFort == hexaID)
                return true;


            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                if (hexaNeighbours[loop1].GetID() == hexaIDFort)
                    return true;
            }
            return false;
        }

        public void Capture(Player player)
        {
            if (!captureIs)
            {
                capturePlayer = player;
            }
            else
            {
                capturePlayer = null;
            }

            captureIs = !captureIs;
        }

        public void Destroy()
        {
            hexaDestroy = true;
            turnDestroy = GameMaster.Inst().GetPlayerCount() * 4;
        }

        public void ApplyEvent(Gameplay.RndEvent rndEvent)
        {
            if (rndEvent.getHexaKind() == kind)
            {
                if (rndEvent.getIsPositive())
                {
                    sourceMiracle = true;
                    turnMiracle = GameMaster.Inst().GetPlayerCount() * 3;
                    sourceDisaster = false;
                }
                else
                {
                    sourceDisaster = true;
                    turnDisaster = GameMaster.Inst().GetPlayerCount() * 3;
                    sourceMiracle = false;
                }
            }
        }
    }
}
