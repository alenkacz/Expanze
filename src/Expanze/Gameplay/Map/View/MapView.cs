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
        ModelView view;
        Map map;
        Dictionary<String, String> waterTexture;

        public MapView(Map map)
        {
            this.map = map;
            viewQueue = new ViewQueue(this);
            view = new ModelView();
            waterTexture = new Dictionary<string, string>();
            MakeDictionaryForWaterTextures();
        }

        public HexaView[][] GetHexaMapView() { return hexaMapView; }

        public void Initialize()
        {
            viewQueue.Clear();
            view.Clear();

            HexaModel[][] hexaMapModel = map.GetHexaMapModel();

            hexaMapView = new HexaView[hexaMapModel.Length][];
            for (int i = 0; i < hexaMapModel.Length; i++)
            {
                hexaMapView[i] = new HexaView[hexaMapModel[i].Length];
                for (int j = 0; j < hexaMapModel[i].Length; j++)
                {
                    if (hexaMapModel[i][j] != null)
                    {
                        switch (hexaMapModel[i][j].GetKind())
                        {
                            case HexaKind.Mountains:
                                hexaMapView[i][j] = new MountainsView(hexaMapModel[i][j], i, j, view);
                                break;
                            case HexaKind.Cornfield:
                                hexaMapView[i][j] = new CornfieldView(hexaMapModel[i][j], i, j, view);
                                break;
                            case HexaKind.Stone:
                                hexaMapView[i][j] = new StoneView(hexaMapModel[i][j], i, j, view);
                                break;
                            case HexaKind.Water:
                                hexaMapView[i][j] = getWaterView(i, j);
                                break;
                            case HexaKind.Forest:
                                hexaMapView[i][j] = new ForestView(hexaMapModel[i][j], i, j, view);
                                break;
                            case HexaKind.Pasture:
                                hexaMapView[i][j] = new PastureView(hexaMapModel[i][j], i, j, view);
                                break;
                            case HexaKind.Desert:
                                hexaMapView[i][j] = new DesertView(hexaMapModel[i][j], i, j, view);
                                break;

                            default:
                                hexaMapView[i][j] = new HexaView(hexaMapModel[i][j], i, j, view);
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
                hexaMapModel[i][j].GetKind() != HexaKind.Water &&
                hexaMapModel[i][j].GetKind() != HexaKind.Nothing)
            {
                return true;
            }
            return false;
        }

        void MakeDictionaryForWaterTextures()
        {
            waterTexture["000000"] = "00";
            waterTexture["000001"] = "21";
            waterTexture["000010"] = "31";
            waterTexture["000011"] = "32";
            waterTexture["000100"] = "41";
            waterTexture["000101"] = "59";
            waterTexture["000110"] = "42";
            waterTexture["000111"] = "33";
            waterTexture["001000"] = "51";
            waterTexture["001001"] = "28";
            waterTexture["001010"] = "09";
            waterTexture["001011"] = "211";
            waterTexture["001100"] = "52";
            waterTexture["001101"] = "512";
            waterTexture["001110"] = "43";
            waterTexture["001111"] = "34";
            waterTexture["010000"] = "01";
            waterTexture["010001"] = "39";
            waterTexture["010010"] = "38";
            waterTexture["010011"] = "312";
            waterTexture["010100"] = "19";
            waterTexture["010101"] = "010";
            waterTexture["010110"] = "311";
            waterTexture["010111"] = "313";
            waterTexture["011000"] = "02";
            waterTexture["011001"] = "511";
            waterTexture["011010"] = "012";
            waterTexture["011011"] = "37";
            waterTexture["011100"] = "53";
            waterTexture["011101"] = "513";
            waterTexture["011110"] = "44";
            waterTexture["011111"] = "35";
            waterTexture["100000"] = "11";
            waterTexture["100001"] = "22";
            waterTexture["100010"] = "49";
            waterTexture["100011"] = "23";
            waterTexture["100100"] = "18";
            waterTexture["100101"] = "111";
            waterTexture["100110"] = "412";
            waterTexture["100111"] = "24";
            waterTexture["101000"] = "29";
            waterTexture["101001"] = "212";
            waterTexture["101010"] = "010";
            waterTexture["101011"] = "213";
            waterTexture["101100"] = "411";
            waterTexture["101101"] = "27";
            waterTexture["101110"] = "413";
            waterTexture["101111"] = "25";
            waterTexture["110000"] = "12";
            waterTexture["110001"] = "13";
            waterTexture["110010"] = "011";
            waterTexture["110011"] = "14";
            waterTexture["110100"] = "112";
            waterTexture["110101"] = "113";
            waterTexture["110110"] = "17";
            waterTexture["110111"] = "15";
            waterTexture["111000"] = "03";
            waterTexture["111001"] = "04";
            waterTexture["111010"] = "013";
            waterTexture["111011"] = "05";
            waterTexture["111100"] = "54";
            waterTexture["111101"] = "55";
            waterTexture["111110"] = "45";
            waterTexture["111111"] = "06";
        }

        WaterView getWaterView(int i, int j)
        {
            HexaModel[][] hexaMapModel = map.GetHexaMapModel();
            int neighbourNumber = 0;

            String neighbours = "";
            if (IsSourceHex(i - 1, j))
            {
                neighbourNumber += 1;
                //upLeft = true;
                neighbours += "1";
            }
            else neighbours += "0";

            if (IsSourceHex(i - 1, j - 1))
            {
                neighbourNumber += 1;
                //upRight = true;
                neighbours += "1";
            }
            else neighbours += "0";

            if (IsSourceHex(i, j - 1))
            {
                neighbourNumber += 1;
                //right = true;
                neighbours += "1";
            }
            else neighbours += "0";

            if (IsSourceHex(i + 1, j))
            {
                neighbourNumber += 1;
                //bottomRight = true;
                neighbours += "1";
            }
            else neighbours += "0";

            if (IsSourceHex(i + 1, j + 1))
            {
                neighbourNumber += 1;
                //bottomLeft = true;
                neighbours += "1";
            }
            else neighbours += "0";

            if (IsSourceHex(i, j + 1))
            {
                neighbourNumber += 1;
                //left = true;
                neighbours += "1";
            }
            else neighbours += "0";

            String value = waterTexture[neighbours];
            int rotationNumber = value[0] - '0';
            int textureNumber = Convert.ToInt32(value.Substring(1));
            Matrix rotation = Matrix.Identity;
            if (rotationNumber > 0)
            {
                rotation = Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (rotationNumber));
            }
            Texture2D t = GameResources.Inst().GetHexaTexture(textureNumber);
            
            return new WaterView(hexaMapModel[i][j], t, rotation, i, j, view);
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
            Model m = GameResources.Inst().GetHexaModel(HexaKind.Water);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Texture = GameResources.Inst().GetHexaTexture(0);
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
                        if(hexaMapView[i][j].IsOnScreen())
                            hexaMapView[i][j].Draw(gameTime);
                    }
                }
            }

            view.Draw(gameTime);

            if (Settings.graphics != GraphicsQuality.HIGH_GRAPHICS)
                return;

            GameState.game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // Draw on screen if 0 is the stencil buffer value   
            
            GameState.game.GraphicsDevice.ReferenceStencil = 0;
            DepthStencilState depthStencilState = new DepthStencilState();
            depthStencilState.ReferenceStencil = 0;
            depthStencilState.StencilEnable = true;
            depthStencilState.StencilFunction = CompareFunction.Equal;
            depthStencilState.StencilPass = StencilOperation.Increment;
            GameState.game.GraphicsDevice.DepthStencilState = depthStencilState;
        
            GameState.game.GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0, 0);

            Vector3 planeNormal = new Vector3(0, -1, 0);
            planeNormal.Normalize();
            Vector3 light = GameState.ShadowDirection;
            light.Normalize();
            Matrix shadow = Matrix.CreateShadow(light, new Plane(planeNormal, 0.001f));// *Matrix.CreateTranslation(GameState.ShadowDirection.X / 150.0f, 0, GameState.ShadowDirection.Y / 150.0f);

            for (int i = 0; i < hexaMapView.Length; i++)
            {
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    if (hexaMapView[i][j] != null)
                    {
                        if (hexaMapView[i][j].IsOnScreen())
                            hexaMapView[i][j].DrawShadow(this, shadow);
                    }
                }
            }
            GameState.game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GameState.game.GraphicsDevice.BlendState = BlendState.Opaque;
            
            DrawWater();
        }

        public void DrawShadow(Model m, Matrix world, Matrix shadow, int banID)
        {
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            int meshNumber = 0;
            foreach (ModelMesh mesh in m.Meshes)
            {
                if (meshNumber++ == banID)
                    continue;

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 0.25f + (1.0f - GameState.SunHeight) / 2.5f;
                    effect.LightingEnabled = true;
                    effect.EmissiveColor = Vector3.Zero;
                    effect.AmbientLightColor = Vector3.Zero;
                    effect.DirectionalLight0.Enabled = false;
                    effect.World = transforms[mesh.ParentBone.Index] * world * shadow;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }

        public void DrawShadow(Model m, Matrix world, Matrix shadow)
        {
            DrawShadow(m, world, shadow, -1);
        }

        private void DrawWater()
        {
            Matrix mWorld = Matrix.Identity;
            float dx = 0.591f;  // copyied from creating matrices
            float dy = 0.512f;
            HexaView tempView;

            //int maxRowWidth = 0;
            int rowWidth = 0;
            const int hexaBorder = 7;

            for (int i = 0; i < hexaMapView.Length; i++)
            {
                //if (hexaMapView[i].Length > maxRowWidth)
                //    maxRowWidth = hexaMapView[i].Length;

                rowWidth = hexaMapView[i].Length;
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    tempView = hexaMapView[i][j];
                    if (tempView != null)
                    {
                        mWorld = hexaMapView[i][j].GetWorldMatrix();
                        break;
                    }
                    rowWidth--;
                }

                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx * -hexaBorder));
                for (int loop2 = -hexaBorder; loop2 <= hexaBorder; loop2++)
                {
                    DrawWaterHexa(mWorld);
                    if (loop2 == -1)
                        mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx * rowWidth));

                    mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx));
                }
            }

            mWorld = hexaMapView[0][0].GetWorldMatrix() * Matrix.CreateTranslation(new Vector3(-dy, 0.0f, dx / 2.0f - hexaBorder * dx));

            int maxRowWidth = hexaBorder * 2 + rowWidth + 2;
            for (int loop1 = 0; loop1 < hexaBorder; loop1++)
            {
                for (int loop2 = 0; loop2 < maxRowWidth; loop2++)
                {
                    DrawWaterHexa(mWorld);
                    mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx));
                }
                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(-dy, 0.0f, -dx / 2.0f - dx * (maxRowWidth - ((loop1 % 2 == 0) ? 1 : 0))));
            }
            maxRowWidth -= 2;

            mWorld = hexaMapView[hexaMapView.Length - 1][hexaMapView[hexaMapView.Length - 1].Length - rowWidth].GetWorldMatrix() * Matrix.CreateTranslation(new Vector3(dy, 0.0f, dx / 2.0f - hexaBorder * dx));
            for (int loop1 = 0; loop1 < 3; loop1++)
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
            TownView.ResetTownView();
            HexaView.SetActiveHexaID(-1);

            float dx = 0.591f;
            float dy = 0.512f;
            Matrix mWorld = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-dy * hexaMapView.Length / 2.0f, 0.0f, -dx * hexaMapView[0].Length / 2.0f)); ;
            for (int i = 0; i < hexaMapView.Length; i++)
            {
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    if (hexaMapView[i][j] != null)
                    {
                        hexaMapView[i][j].SetWorld(mWorld);
                    }
                    mWorld = mWorld * Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, dx));
                }
                mWorld = mWorld * Matrix.CreateTranslation(new Vector3(dy, 0.0f, -dx / 2.0f - dx * hexaMapView[i].Length));
            }
        }

        public void AddToViewQueue(ItemQueue item)
        {
            AddToViewQueue(item, false);
        }
        public void AddToViewQueue(ItemQueue item, bool forceIt)
        {
            viewQueue.Add(item, forceIt);
        }

        public bool getIsViewQueueClear()
        {
            return viewQueue.IsClear();
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

        public TownView GetTownViewByID(int townID)
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

        public HexaView GetHexaViewByID(int hexaID)
        {
            for (int i = 0; i < hexaMapView.Length; i++)
                for (int j = 0; j < hexaMapView[i].Length; j++)
                {
                    if (hexaMapView[i][j] != null && hexaMapView[i][j].HexaID == hexaID)
                        return hexaMapView[i][j];
                }

            return null;
        }

        public RoadView GetRoadViewByID(int roadID)
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
