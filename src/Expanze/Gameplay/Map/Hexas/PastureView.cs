
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CorePlugin;
using Expanze.Gameplay.Map.View;

namespace Expanze.Gameplay.Map
{

    class PastureView : HexaView
    {
        private static Random rnd;

        public PastureView(HexaModel model, int x, int y, ModelView modelView)
            : base(model, x, y, modelView)
        {
            translateM = new Matrix[MAX_SHEEP];
            scaleM = new Matrix[MAX_SHEEP];
            objectMatrix = new Matrix[MAX_SHEEP];
            colorT = new Vector3[MAX_SHEEP];
            sheeps = new InstanceView[MAX_SHEEP];

            if (rnd == null)
                rnd = new Random();
        }

        public int MAX_SHEEP = 24;
        Matrix[] translateM;
        Matrix[] scaleM;
        Matrix[] objectMatrix;
        InstanceView[] sheeps;
        Vector3[] colorT;
        int stepherds = 0;

        public void DrawSheeps(GameTime gameTime)
        {
            Model m = GameResources.Inst().GetTreeModel(1);

            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            stepherds = 0;
            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                if (model.getTown((TownPos)loop1).GetBuildingKind(model.GetID()) == BuildingKind.SourceBuilding)
                    stepherds++;
            }

            for (int loop1 = 0; loop1 < translateM.Length; loop1++)
            {
                if (loop1 % 3 == 0 && stepherds >= 1 ||
                   loop1 % 3 == 1 && stepherds >= 3 ||
                   loop1 % 3 == 2 && stepherds >= 2)
                {
                    sheeps[loop1].Visible = true;
                }
                else
                    sheeps[loop1].Visible = false;
            }
        }

        public override void DrawShadow(MapView mapView, Matrix shadow)
        {
            base.DrawShadow(mapView, shadow);

            if (model.SecretKind && !GameMaster.Inst().LastHumanPlayer.GetIsDiscovered(hexaID))
            {
                return;
            }

            Model m = GameResources.Inst().GetTreeModel(5);
            mapView.DrawShadow(m, worldM, shadow);

            if (stepherds == 0)
                return;

            m = GameResources.Inst().GetTreeModel(1);
            for (int loop1 = 0; loop1 < translateM.Length; loop1++)
            {
                if (loop1 % 3 == 0 && stepherds >= 1 ||
                   loop1 % 3 == 1 && stepherds >= 3 ||
                   loop1 % 3 == 2 && stepherds >= 2)
                {
                    mapView.DrawShadow(m, objectMatrix[loop1], shadow);
                }
            }
        }

        public override void SetWorld(Matrix m)
        {
            base.SetWorld(m);
            Model sheepModel = GameResources.Inst().GetTreeModel(1);
            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                for (int loop2 = 0; loop2 < 4; loop2++)
                {
                    float shade = (float)rnd.NextDouble() / 10.0f;
                    colorT[loop2 + 4 * loop1] = new Vector3(shade, shade, shade);
                    scaleM[loop2 + 4 * loop1] = Matrix.CreateScale((float)(0.00065 - 0.00002 + 0.00004 * rnd.NextDouble()));
                    objectMatrix[loop2 + 4 * loop1] = scaleM[loop2 + 4 * loop1] * Matrix.CreateTranslation(new Vector3((float)(rnd.NextDouble() - 0.5) / 8.0f + 0.05f, 0, (float)(rnd.NextDouble() - 0.5) / 8.0f + 0.05f)) * Matrix.CreateRotationY((float)(Math.PI / 3.0 * ((6 - loop1) + 0.5 + 2) + Math.PI / 3.0 * rnd.NextDouble()));
                    objectMatrix[loop2 + 4 * loop1] = objectMatrix[loop2 + 4 * loop1] * world;

                    sheeps[loop2 + 4 * loop1] = modelView.AddInstance(sheepModel, new InstanceView(objectMatrix[loop2 + 4 * loop1]));
                }
            }
        }
        public override void DrawBuildings(GameTime gameTime)
        {
            base.DrawBuildings(gameTime);

            if (Settings.graphics == GraphicsQuality.LOW_GRAPHICS)
                return;

            DrawSheeps(gameTime);

            Model m = GameResources.Inst().GetTreeModel(5);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 1.0f;
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = GameState.MaterialAmbientColor;
                    effect.DirectionalLight0.Direction = GameState.LightDirection;
                    effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                    effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                    effect.DirectionalLight0.Enabled = true;
                    effect.World = transforms[mesh.ParentBone.Index] * worldM;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }

        }
    }
}
