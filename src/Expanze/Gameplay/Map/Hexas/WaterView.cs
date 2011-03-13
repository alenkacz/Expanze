
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
        Model waterModel;
        Matrix rotation;

        public WaterView(HexaModel model, Model waterModel, Matrix rotation)
            : base(model)
        {
            this.waterModel = waterModel;
            this.rotation = rotation;
        }

        public override void Draw(GameTime gameTime)
        {
            Model m = waterModel;
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            //rasterizerState.FillMode = FillMode.WireFrame;
            GameState.game.GraphicsDevice.RasterizerState = rasterizerState;

            //rotation = (hexaID % 6 == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (hexaID % 6));
            Matrix tempMatrix = Matrix.CreateScale(0.00028f) * rotation;

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

                    effect.World = transforms[mesh.ParentBone.Index] * tempMatrix * world;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
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
