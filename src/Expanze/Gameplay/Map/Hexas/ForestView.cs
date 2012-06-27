
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze.Gameplay.Map
{
    
    class ForestView : HexaView
    {
        private static Random rnd;

        public ForestView(HexaModel model, int x, int y)
            : base(model, x, y)
        {
            translateM = new Matrix[MAX_TREE];
            scaleM = new Matrix[MAX_TREE];
            objectMatrix = new Matrix[MAX_TREE];
            colorT = new Vector3[MAX_TREE];

            if(rnd == null)
            rnd = new Random();

            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                for (int loop2 = 0; loop2 < 8; loop2++)
                {
                    colorT[loop2 + 8 * loop1] = new Vector3(0.0f, (float)rnd.NextDouble() / 2.0f, 0.0f);
                    //translateM[loop1] = Matrix.CreateTranslation(new Vector3((float) (rnd.NextDouble() - 0.5) / 2.0f, 0, (float) (rnd.NextDouble() - 0.5) / 2.0f));
                    scaleM[loop2 + 8 * loop1] = Matrix.CreateScale((float)(0.001 - 0.0002 + 0.0004 * rnd.NextDouble()));
                    //objectMatrix[loop1] = scaleM[loop1] * translateM[loop1];

                    objectMatrix[loop2 + 8 * loop1] = scaleM[loop2 + 8 * loop1] * Matrix.CreateTranslation(new Vector3((float)(rnd.NextDouble() - 0.5) / 6.0f + 0.17f, 0, 0)) * Matrix.CreateRotationY((float)(Math.PI / 3.0 * ((6 - loop1) + 0.5 + 2) + Math.PI / 3.0 * rnd.NextDouble()));
                }
            }
        }

        public int MAX_TREE = 48;
        Matrix[] translateM;
        Matrix[] scaleM;
        Matrix[] objectMatrix;
        Vector3[] colorT;

        public void DrawTrees(GameTime gameTime)
        {
            Model m = GameResources.Inst().GetTreeModel(0);

            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            for (int loop1 = 0; loop1 < translateM.Length; loop1++)
            {
                if (model.getTown((CorePlugin.TownPos)(loop1 / 8)).GetBuildingKind(model.GetID()) != BuildingKind.NoBuilding)
                    continue;

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.LightingEnabled = true;
                        effect.AmbientLightColor = colorT[loop1];
                        // GameState.MaterialAmbientColor;
                        effect.DirectionalLight0.Direction = GameState.LightDirection;
                        effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                        effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                        effect.DirectionalLight0.Enabled = true;
                        effect.World = transforms[mesh.ParentBone.Index] * objectMatrix[loop1] * world;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();
                }
            }
        }

        public override void DrawBuildings(GameTime gameTime)
        {
            base.DrawBuildings(gameTime);

            DrawTrees(gameTime);
        }
    }
}
