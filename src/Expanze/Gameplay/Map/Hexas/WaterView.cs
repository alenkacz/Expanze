
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Expanze.Gameplay.Map.View;
using Microsoft.Xna.Framework.Graphics;

namespace Expanze.Gameplay.Map
{
    class WaterView : HexaView
    {
        Texture2D waterTexture;
        Matrix rotation;

        public WaterView(HexaModel model, Texture2D waterTexture, Matrix rotation, int x, int y)
            : base(model, x, y)
        {
            this.waterTexture = waterTexture;
            this.rotation = rotation;
        }

        public override void Draw(GameTime gameTime)
        {
            if (Settings.graphics != GraphicsQuality.HIGH_GRAPHICS)
            {
                bool hasNoWaterNeighbour = false;
                for(int loop1 = 0; loop1 < 6; loop1++)
                {
                    HexaModel hexaModel = model.GetHexaNeighbour(loop1);
                    if(hexaModel != null)
                    {
                        CorePlugin.HexaKind kind = hexaModel.GetKind();
                        if (kind != CorePlugin.HexaKind.Water && kind != CorePlugin.HexaKind.Null && kind != CorePlugin.HexaKind.Nothing)
                        {
                            hasNoWaterNeighbour = true;
                            break;
                        }
                    }
                }

                if(!hasNoWaterNeighbour)
                    return;
            }

            Model m = GameResources.Inst().GetHexaModel(CorePlugin.HexaKind.Water);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            GameState.game.GraphicsDevice.RasterizerState = GameState.rasterizerState;
            //GameState.game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //GameState.game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            
            //rotation = (hexaID % 6 == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (hexaID % 6));
            Matrix tempMatrix = Matrix.CreateScale(0.00028f) * rotation;

            for(int loop1 = 0; loop1 < m.Meshes.Count; loop1++)
            {
                ModelMesh mesh = m.Meshes[loop1];
                
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Texture = waterTexture;
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = GameState.MaterialAmbientColor;
                    effect.DirectionalLight0.Direction = GameState.LightDirection;
                    effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                    effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                    effect.DirectionalLight0.Enabled = true;

                    effect.World = transforms[mesh.ParentBone.Index] * tempMatrix * world;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }

            //GameState.game.GraphicsDevice.BlendState = BlendState.Opaque;
            //GameState.game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        public override void DrawBuildings(GameTime gameTime)
        {
            // nothing
        }

        public override TownView GetTownByID(int townID)
        {
            return null;
        }

        public override RoadView GetRoadViewByID(int roadID)
        {
            return null;
        }
    }
}
