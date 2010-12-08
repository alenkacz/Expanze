using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expanze.Gameplay.Map.View;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze.Gameplay.Map
{
    class MapView
    {
        HexaView[][] hexaMapView;
        ViewQueue viewQueue;
        Map map;

        public MapView(Map map)
        {
            this.map = map;
            viewQueue = new ViewQueue(this);
        }

        public HexaView[][] GetHexaMapView() { return hexaMapView; }

        public void Initialize()
        {
            viewQueue.Clear();

            HexaModel[][] hexaMapModel = map.GetHexaMapModel();

            hexaMapView = new HexaView[hexaMapModel.Length][];
            for (int i = 0; i < hexaMapModel.Length; i++)
            {
                hexaMapView[i] = new HexaView[hexaMapModel[i].Length];
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                {
                    if (hexaMapModel[i][j] != null)
                    {
                        switch (hexaMapModel[i][j].getType())
                        {
                            case HexaKind.Mountains:
                                hexaMapView[i][j] = new MountainsView(hexaMapModel[i][j]);
                                break;
                            case HexaKind.Cornfield:
                                hexaMapView[i][j] = new CornfieldView(hexaMapModel[i][j]);
                                break;
                            case HexaKind.Stone:
                                hexaMapView[i][j] = new StoneView(hexaMapModel[i][j]);
                                break;
                            default:
                                hexaMapView[i][j] = new HexaView(hexaMapModel[i][j]);
                                break;
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            viewQueue.Update(gameTime);
        }

        public void HandlePickableAreas(Color c)
        {
            for (int i = 0; i < hexaMapView.Length; i++)
                for (int j = 0; j < hexaMapView[i].Length; j++)
                    if (hexaMapView[i][j] != null)
                        hexaMapView[i][j].HandlePickableAreas(c);
        }

        public void DrawPickableAreas()
        {
            for (int i = 0; i < hexaMapView.Length; i++)
                for (int j = 0; j < hexaMapView[i].Length; j++)
                    if (hexaMapView[i][j] != null)
                        hexaMapView[i][j].DrawPickableAreas();
        }

        public void Draw2D()
        {
            for (int i = 0; i < hexaMapView.Length; i++)
            {
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    if (hexaMapView[i][j] != null)
                    {
                        hexaMapView[i][j].Draw2D();
                    }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < hexaMapView.Length; i++)
            {
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    if (hexaMapView[i][j] != null)
                    {
                        hexaMapView[i][j].Draw(gameTime);
                    }
                }
            }
        }

        public void CreateHexaWorldMatrices()
        {
            HexaModel.resetCounter();
            Road.resetCounter();
            Town.resetCounter();
            TownView.resetTownView();

            float dx = 0.592f;
            float dy = 0.513f;
            Matrix mWorld = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-dy * hexaMapView.Length / 2.0f, 0.0f, -dx * hexaMapView[0].Length / 2.0f)); ;
            for (int i = 0; i < hexaMapView.Length; i++)
            {
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    if (hexaMapView[i][j] != null)
                    {
                        hexaMapView[i][j].setWorld(mWorld);
                    }
                    mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx));
                }
                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(dy, 0.0f, -dx / 2.0f - dx * hexaMapView[i].Length));
            }
        }

        public void AddToViewQueue(ItemQueue item)
        {
            viewQueue.Add(item);
        }

        public bool getIsViewQueueClear()
        {
            return viewQueue.getIsClear();
        }

        public void BuildTownView(int townID)
        {
            TownView townView = GetTownViewByID(townID);
            townView.setIsBuild(true);
        }

        public void BuildRoadView(int roadID)
        {
            RoadView roadView = GetRoadViewByID(roadID);
            roadView.setIsBuild(true);
        }

        private TownView GetTownViewByID(int townID)
        {
            TownView town = null;
            for (int i = 0; i < hexaMapView.Length; i++)
                for (int j = 0; j < hexaMapView[i].Length; j++)
                    if (hexaMapView[i][j] != null)
                    {
                        town = hexaMapView[i][j].GetTownByID(townID);
                        if (town != null)
                            return town;
                    }
            return null;
        }

        private RoadView GetRoadViewByID(int roadID)
        {
            RoadView road = null;
            for (int i = 0; i < hexaMapView.Length; i++)
                for (int j = 0; j < hexaMapView[i].Length; j++)
                    if (hexaMapView[i][j] != null)
                    {
                        road = hexaMapView[i][j].GetRoadViewByID(roadID);
                        if (road != null)
                            return road;
                    }
            return null;
        }
    }
}
