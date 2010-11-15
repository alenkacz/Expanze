using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Expanze.Gameplay.Map;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;

namespace Expanze.Gameplay.Map.View
{
    class RoadView
    {
        private bool isBuildView;

        private Color pickRoadColor;
        private PickVariables pickVars;
        private Matrix world;
        private int roadID;
        private Road model;

        public RoadView(Road model, Matrix world)
        {
            this.model = model;
            this.roadID = model.getRoadID();
            this.pickRoadColor = new Color(0.0f, roadID / 256.0f, 0.0f);
            this.world = world;
            pickVars = new PickVariables(pickRoadColor);
            isBuildView = false;
        }

        public int getRoadID() { return roadID; }
        public void setIsBuild(bool isBuild) { isBuildView = isBuild; }

        public void Draw(GameTime gameTime)
        {
            GameMaster gm = GameMaster.getInstance();
            if ((pickVars.pickActive && gm.getState() == EGameState.StateGame) || isBuildView)
            {
                Model m = GameState.map.getRoadModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.01f, 0.0f)) * Matrix.CreateScale(0.023f) * world;

                int a = 0;

                Player player = model.getOwner();
                if (player == null)
                    player = GameMaster.getInstance().getActivePlayer();
                Vector3 color = player.getColor().ToVector3();
                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.DirectionalLight0.Enabled = false;
                        effect.DirectionalLight1.Enabled = true;
                        effect.DirectionalLight1.DiffuseColor = new Vector3(0.3f, 0.3f, 0.3f);

                        // is it model part which is for flags? They have to be in player colors
                        if (a % 5 == 1 || a % 5 == 2 || a % 5 == 3 || a == 4 || a == 5 || a == 15 || a == 14)
                        {
                            effect.EmissiveColor = color * 0.5f;
                            effect.DiffuseColor = color * 0.9f;
                            effect.AmbientLightColor = color / 3.0f;
                        }
                        else
                        {
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                            if (a == 20)
                                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                            else
                                effect.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                            effect.AmbientLightColor = new Vector3(0.7f, 0.7f, 0.7f);
                        }

                        // if player wants to build new Road, can he? Show it in red/green color
                        if (pickVars.pickActive && !isBuildView)
                        {
                            if (!model.CanActivePlayerBuildRoad())
                                effect.DiffuseColor = new Vector3(1, 0.0f, 0);
                            else
                                effect.DiffuseColor = new Vector3(0, 1.0f, 0);
                        }

                        effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    a++;
                    mesh.Draw();
                }
            }
        }

        public void DrawPickableAreas()
        {
            Model m = GameState.map.getShape(Map.SHAPE_RECTANGLE);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.03f, 0.0f)) * Matrix.CreateScale(0.25f) * world;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.EmissiveColor = pickRoadColor.ToVector3();
                    effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }

        public void HandlePickableAreas(Color c)
        {
            Map.SetPickVariables(c == pickRoadColor, pickVars);

            if (pickVars.pickNewPress)
            {
                WindowPromt wP = GameState.windowPromt;
                if (model.CanActivePlayerBuildRoad())
                {
                    wP.showPromt(Strings.PROMT_TITLE_WANT_TO_BUILD_ROAD, wP.BuildRoad, Settings.costRoad);
                    wP.setArgInt1(roadID);
                }
                else
                {
                    wP.showAlert(Strings.ALERT_TITLE_NOT_ENOUGH_SOURCES);
                }
            }
        }
    }
}
