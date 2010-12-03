﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Expanze.Gameplay.Map;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;

namespace Expanze.Gameplay.Map.View
{
    class RoadPromptItem : PromptItem
    {
        int roadID;

        public RoadPromptItem(int roadID, String title, String description, SourceAll cost, Texture2D icon)
            : base(title, description, cost, icon)
        {
            this.roadID = roadID;
        }

        public override void Execute()
        {
            GameState.map.GetMapController().BuildRoad(roadID);
        }
    }

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
                Model m = GameResources.Inst().getRoadModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.01f, 0.0f)) * Matrix.CreateScale(0.019f) * world;

                int a = 0;

                Player player = model.getOwner();
                if (player == null)
                    player = GameMaster.getInstance().getActivePlayer();
                Vector3 color = player.getColor().ToVector3();
                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.LightingEnabled = true;
                        effect.DirectionalLight0.Direction = GameState.LightDirection;
                        effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                        effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                        effect.DirectionalLight0.Enabled = true;

                        // is it model part which is for flags? They have to be in player colors
                        if (a % 5 == 1 || a % 5 == 2 || a % 5 == 3 || a == 4 || a == 5 || a == 15 || a == 14)
                        {
                            effect.EmissiveColor = color * 0.5f;
                            effect.DiffuseColor = color * 0.9f;
                            effect.AmbientLightColor = color / 3.0f;
                        }
                        else
                        {
                            /*
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                            if (a == 20)
                                effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                            else
                                effect.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                            effect.AmbientLightColor = new Vector3(0.7f, 0.7f, 0.7f);*/
                        }

                        // if player wants to build new Road, can he? Show it in red/green color
                        
                        if (pickVars.pickActive && !isBuildView)
                        {
                            if (model.CanActivePlayerBuildRoad() != RoadBuildError.OK)
                            {
                                if (!(a % 5 == 1 || a % 5 == 2 || a % 5 == 3 || a == 4 || a == 5 || a == 15 || a == 14))
                                    effect.EmissiveColor = new Vector3(0.5f, 0.0f, 0);
                            }
                            else
                                if (!(a % 5 == 1 || a % 5 == 2 || a % 5 == 3 || a == 4 || a == 5 || a == 15 || a == 14))
                                    effect.EmissiveColor = new Vector3(0, 0.5f, 0);
                        } else
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);

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
            Model m = GameResources.Inst().getShape(GameResources.SHAPE_RECTANGLE);
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
                if (GameMaster.getInstance().getState() == EGameState.StateGame)
                {
                    WindowPromt wP = GameState.windowPromt;

                    switch (model.CanActivePlayerBuildRoad())
                    {
                        case RoadBuildError.NoSources:
                            wP.showAlert(Strings.ALERT_TITLE_NOT_ENOUGH_SOURCES);
                            break;
                        case RoadBuildError.AlreadyBuild:
                            wP.showAlert(Strings.ALERT_TITLE_ROAD_IS_BUILD);
                            break;
                        case RoadBuildError.NoPlayerRoadOrTown:
                            wP.showAlert(Strings.ALERT_TITLE_NO_ROAD_OR_TOWN_IS_CLOSE);
                            break;
                        case RoadBuildError.OK :
                            PromptWindow.Inst().showPrompt(Strings.PROMPT_TITLE_BUILDING, false);
                            PromptWindow.Inst().addPromptItem(
                                    new RoadPromptItem(roadID,
                                                       Strings.PROMT_TITLE_WANT_TO_BUILD_ROAD,
                                                       "",
                                                       Settings.costRoad,
                                                       null));
                            break;
                    }
                }
            }
        }
    }
}
