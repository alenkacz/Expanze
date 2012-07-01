
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CorePlugin;

namespace Expanze.Gameplay.Map
{

    class PastureView : HexaView
    {
        private static Random rnd;

        public PastureView(HexaModel model, int x, int y)
            : base(model, x, y)
        {
            translateM = new Matrix[MAX_SHEEP];
            scaleM = new Matrix[MAX_SHEEP];
            objectMatrix = new Matrix[MAX_SHEEP];
            colorT = new Vector3[MAX_SHEEP];

            if (rnd == null)
                rnd = new Random();

            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                for (int loop2 = 0; loop2 < 4; loop2++)
                {
                    float shade = (float)rnd.NextDouble() / 10.0f;
                    colorT[loop2 + 4 * loop1] = new Vector3(shade, shade, shade);
                    scaleM[loop2 + 4 * loop1] = Matrix.CreateScale((float)(0.00065 - 0.00002 + 0.00004 * rnd.NextDouble()));
                    objectMatrix[loop2 + 4 * loop1] = scaleM[loop2 + 4 * loop1] * Matrix.CreateTranslation(new Vector3((float)(rnd.NextDouble() - 0.5) / 8.0f + 0.05f, 0, (float)(rnd.NextDouble() - 0.5) / 8.0f + 0.05f)) * Matrix.CreateRotationY((float)(Math.PI / 3.0 * ((6 - loop1) + 0.5 + 2) + Math.PI / 3.0 * rnd.NextDouble()));
                }
            }
        }

        public int MAX_SHEEP = 24;
        Matrix[] translateM;
        Matrix[] scaleM;
        Matrix[] objectMatrix;
        Vector3[] colorT;

        public void DrawSheeps(GameTime gameTime)
        {
            Model m = GameResources.Inst().GetTreeModel(1);

            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            int stepherds = 0;
            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                if (model.getTown((TownPos)loop1).GetBuildingKind(model.GetID()) == BuildingKind.SourceBuilding)
                    stepherds++;
            }

            if (stepherds == 0)
                return;

            for (int loop1 = 0; loop1 < translateM.Length; loop1++)
            {
                if (loop1 % 3 == 0 && stepherds >= 1 ||
                   loop1 % 3 == 1 && stepherds >= 3 ||
                   loop1 % 3 == 2 && stepherds >= 2)
                {
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
        }

        public override void DrawBuildings(GameTime gameTime)
        {
            base.DrawBuildings(gameTime);

            DrawSheeps(gameTime);
        }
    }
}
