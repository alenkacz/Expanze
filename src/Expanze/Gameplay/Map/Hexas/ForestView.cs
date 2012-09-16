
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
    
    class ForestView : HexaView
    {
        private static Random rnd;

        public ForestView(HexaModel model, int x, int y, ModelView modelView)
            : base(model, x, y, modelView)
        {
            treeInstance = new AmbientInstanceView[MAX_TREE];

            if(rnd == null)
            rnd = new Random();
        }

        public int MAX_TREE = 48;
        AmbientInstanceView[] treeInstance;

        public void DrawTrees(GameTime gameTime)
        {
            for (int loop1 = 0; loop1 < treeInstance.Length; loop1++)
            {
                if (model.getTown((CorePlugin.TownPos)(loop1 / 8)).GetBuildingKind(model.GetID()) != BuildingKind.NoBuilding)
                    treeInstance[loop1].Visible = false;
                else
                    treeInstance[loop1].Visible = true;
            }
        }

        public override void DrawShadow(MapView mapView, Matrix shadow)
        {
            base.DrawShadow(mapView, shadow);

            if (model.SecretKind && !GameMaster.Inst().LastHumanPlayer.GetIsDiscovered(hexaID))
            {
                return;
            }

            Model m = GameResources.Inst().GetTreeModel(0);

            for (int loop1 = 0; loop1 < treeInstance.Length; loop1++)
            {
                if (model.getTown((CorePlugin.TownPos)(loop1 / 8)).GetBuildingKind(model.GetID()) != BuildingKind.NoBuilding)
                    continue;

                mapView.DrawShadow(m, treeInstance[loop1].World, shadow);
            }
        }

        public override void DrawBuildings(GameTime gameTime)
        {
            base.DrawBuildings(gameTime);

            if (Settings.graphics == GraphicsQuality.LOW_GRAPHICS)
            {
                for (int loop1 = 0; loop1 < treeInstance.Length; loop1++)
                {
                    treeInstance[loop1].Visible = false;
                }
                return;
            }

            DrawTrees(gameTime);
        }

        public override void SetWorld(Matrix m)
        {
            base.SetWorld(m);

            for (int loop1 = 0; loop1 < 6; loop1++)
            {
                for (int loop2 = 0; loop2 < 8; loop2++)
                {
                    //translateM[loop1] = Matrix.CreateTranslation(new Vector3((float) (rnd.NextDouble() - 0.5) / 2.0f, 0, (float) (rnd.NextDouble() - 0.5) / 2.0f));
                    Matrix scale  = Matrix.CreateScale((float)(0.001 - 0.0002 + 0.0004 * rnd.NextDouble()));
                    //objectMatrix[loop1] = scaleM[loop1] * translateM[loop1];

                    Matrix objectM = scale * Matrix.CreateTranslation(new Vector3((float)(rnd.NextDouble() - 0.5) / 6.0f + 0.17f, 0, 0)) * Matrix.CreateRotationY((float)(Math.PI / 3.0 * ((6 - loop1) + 0.5 + 2) + Math.PI / 3.0 * rnd.NextDouble()));
                    objectM = objectM * world;

                    treeInstance[loop2 + 8 * loop1] = (AmbientInstanceView)modelView.AddInstance(GameResources.Inst().GetTreeModel(0), new AmbientInstanceView(objectM));
                    treeInstance[loop2 + 8 * loop1].AmbientLightColor = new Vector3(0.0f, (float)rnd.NextDouble() / 2.0f, 0.0f);
                }
            }
        }
    }
}
