using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expanze.Gameplay.Map.View;
using Microsoft.Xna.Framework;
using CorePlugin;
using Microsoft.Xna.Framework.Graphics;

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
                            case HexaKind.Water:
                                hexaMapView[i][j] = getWaterView(i, j);
                                break;
                            default:
                                hexaMapView[i][j] = new HexaView(hexaMapModel[i][j]);
                                break;
                        }
                    }
                }
            }
        }

        private bool IsSourceHex(int i, int j)
        {
            HexaModel[][] hexaMapModel = map.GetHexaMapModel();
            if (i >= 0 && i < hexaMapModel.Length &&
                j >= 0 && j < hexaMapModel[i].Length &&
                hexaMapModel[i][j] != null && 
                hexaMapModel[i][j].getType() != HexaKind.Water &&
                hexaMapModel[i][j].getType() != HexaKind.Nothing)
            {
                return true;
            }
            return false;
        }

        WaterView getWaterView(int i, int j)
        {
            HexaModel[][] hexaMapModel = map.GetHexaMapModel();
            int neighbours = 0;
            bool upLeft = false;
            bool upRight = false;
            bool left = false;
            bool right = false;
            bool bottomLeft = false;
            bool bottomRight = false;

            if (IsSourceHex(i - 1, j))
            {
                neighbours += 1;
                upLeft = true;
            }

            if (IsSourceHex(i - 1, j - 1))
            {
                neighbours += 1;
                upRight = true;
            }

            if (IsSourceHex(i, j + 1))
            {
                neighbours += 1;
                left = true;
            }

            if (IsSourceHex(i, j - 1))
            {
                neighbours += 1;
                right = true;
            }

            if (IsSourceHex(i + 1, j + 1))
            {
                neighbours += 1;
                bottomLeft = true;
            }

            if (IsSourceHex(i + 1, j))
            {
                neighbours += 1;
                bottomRight = true;
            }

            Matrix rotation = Matrix.Identity;

            if (right && upRight)
            {
            }
            else 
                if (upLeft && upRight)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (1));
            }
            else if (upLeft && left)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (2));
            }
            else if (bottomLeft && left)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (3));
            }
            else if (bottomRight && bottomLeft)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (4));
            }
            else
                if (right)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (5));
            } else
                if (upLeft)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (1));
            }
            else
                if (left)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (2));
            }
            else
            if (bottomLeft)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (3));
            }
            else
            if (bottomRight)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (4));
            }

            Model m = GameResources.Inst().getHexaModel(HexaKind.Water + neighbours);

            return new WaterView(hexaMapModel[i][j], m, rotation);
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

            float dx = 0.591f;
            float dy = 0.512f;
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
