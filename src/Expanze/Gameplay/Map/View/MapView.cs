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
                        switch (hexaMapModel[i][j].getKind())
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
                hexaMapModel[i][j].getKind() != HexaKind.Water &&
                hexaMapModel[i][j].getKind() != HexaKind.Nothing)
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

        public void DrawWaterHexa(Matrix mWorld)
        {
            Matrix tempMatrix = Matrix.CreateScale(0.00028f);
            Model m = GameResources.Inst().getHexaModel(HexaKind.Water);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = GameState.MaterialAmbientColor;
                    effect.DirectionalLight0.Direction = GameState.LightDirection;
                    effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                    effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                    effect.DirectionalLight0.Enabled = true;

                    effect.World = transforms[mesh.ParentBone.Index] * tempMatrix * mWorld;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
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
            
            Matrix mWorld = Matrix.Identity;
            float dx = 0.591f;  // copyied from creating matrices
            float dy = 0.512f;
            HexaView tempView;
            
            int rowWidth = 0;
            const int hexaBorder = 14;

            for (int i = 0; i < hexaMapView.Length; i++)
            {
                rowWidth = hexaMapView[i].Length;
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    tempView = hexaMapView[i][j];
                    if (tempView != null)
                    {
                        mWorld = hexaMapView[i][j].getWorldMatrix();
                        break;
                    }
                    rowWidth--;
                }

                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx * -hexaBorder));
                for (int loop2 = -hexaBorder; loop2 <= hexaBorder; loop2++)
                {
                    DrawWaterHexa(mWorld);
                    if(loop2 == -1)
                        mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx * rowWidth));

                    mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx));
                }
            }

            mWorld = hexaMapView[0][0].getWorldMatrix() * Matrix.CreateTranslation(new Vector3(-dy, 0.0f, dx / 2.0f - hexaBorder * dx));

            int maxRowWidth = hexaBorder * 2 + 3;
            for (int loop1 = 0; loop1 < maxRowWidth; loop1++)
            {
                for (int loop2 = 0; loop2 < maxRowWidth; loop2++)
                {
                    DrawWaterHexa(mWorld);
                    mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx));
                }
                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(-dy, 0.0f, -dx / 2.0f - dx * (maxRowWidth - ((loop1 % 2 == 0) ? 1 : 0))));
            }

            mWorld = hexaMapView[hexaMapView.Length - 1][hexaMapView[hexaMapView.Length - 1].Length - rowWidth].getWorldMatrix() * Matrix.CreateTranslation(new Vector3(dy, 0.0f, dx / 2.0f - hexaBorder * dx));
            for (int loop1 = 0; loop1 < maxRowWidth; loop1++)
            {
                for (int loop2 = 0; loop2 < maxRowWidth; loop2++)
                {
                    DrawWaterHexa(mWorld);
                    mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx));
                }
                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(dy, 0.0f, -dx / 2.0f - dx * (maxRowWidth - ((loop1 % 2 == 0) ? 1 : 0))));
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

        public void BuildBuildingView(int townID, int pos)
        {
            TownView townView = GetTownViewByID(townID);
            townView.setBuildingIsBuild(pos, true);
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
