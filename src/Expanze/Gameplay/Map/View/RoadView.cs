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

        public RoadPromptItem(int roadID, String title, String description, SourceAll source, bool isSourceCost, Texture2D icon)
            : base(title, description, source, isSourceCost, false, icon)
        {
            this.roadID = roadID;
        }

        public override void Execute()
        {
            GameState.map.GetMapController().BuildRoad(roadID);
        }

        public override string TryExecute()
        {
            RoadModel road = GameState.map.GetRoadByID(roadID);
            RoadBuildError error = road.CanBuildRoad();
            switch (error)
            {
                case RoadBuildError.AlreadyBuild: return Strings.ALERT_TITLE_ROAD_IS_BUILD;
                case RoadBuildError.NoPlayerRoadOrTown: return Strings.ALERT_TITLE_NO_ROAD_OR_TOWN_IS_CLOSE;
                case RoadBuildError.NoSources: return "";
            }
            return base.TryExecute();
        }
    }

    class RoadItemQueue : ItemQueue
    {
        int roadID;

        public RoadItemQueue(MapView mapView, int roadID)
            : base(mapView)
        {
            this.roadID = roadID;
        }

        public override void Execute()
        {
            mapView.BuildRoadView(roadID);
            base.Execute();
        }
    }

    class RoadView
    {
        private bool isBuildView;

        private Color pickRoadColor;
        private PickVariables pickVars;
        private Matrix world;
        private Matrix worldShape;

        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }
        private int roadID;
        private RoadModel model;

        internal RoadModel Model
        {
            get { return model; }
        }

        private static int tutorialID;

        public static int TutorialID
        {
            get { return RoadView.tutorialID; }
            set { RoadView.tutorialID = value; }
        }

        public RoadView(RoadModel model, Matrix world)
        {
            this.model = model;
            this.roadID = model.GetRoadID();
            this.pickRoadColor = new Color(0.0f, 1.0f - roadID / 256.0f, 0.0f);
            this.world = Matrix.CreateTranslation(new Vector3(0.0f, 0.012f, 0.0f)) * Matrix.CreateScale(0.1f) * world;
            worldShape = Matrix.CreateTranslation(new Vector3(0.0f, 0.03f, 0.0f)) * Matrix.CreateScale(0.25f) * world;
            pickVars = new PickVariables(pickRoadColor);
            isBuildView = false;
        }

        public int getRoadID() { return roadID; }
        public void setIsBuild(bool isBuild) { isBuildView = isBuild; }

        public void Draw(GameTime gameTime)
        {
            GameMaster gm = GameMaster.Inst();
            if ((pickVars.pickActive && gm.GetState() == EGameState.StateGame) || isBuildView ||
                model.GoalRoad || tutorialID == roadID)
            {
                Model m = GameResources.Inst().GetRoadModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                int a = 0;

                Player player = model.GetOwner();
                if (player == null)
                    player = GameMaster.Inst().GetActivePlayer();
                Vector3 color = player.GetColor().ToVector3();

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.Alpha = 1.0f;
                        effect.LightingEnabled = true;
                        effect.DirectionalLight0.Direction = GameState.LightDirection;
                        effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                        effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                        effect.DirectionalLight0.Enabled = true;

                        effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                        if (tutorialID == roadID)
                        {
                            effect.EmissiveColor = new Vector3(0.5f, 0.5f, 0.0f);
                        }
                        // if player wants to build new Road, can he? Show it in red/green color
                        if (model.GoalRoad && !isBuildView && !(pickVars.pickActive && gm.GetState() == EGameState.StateGame))
                        {
                            effect.EmissiveColor = new Vector3(0.7f, 0.7f, 0.7f);
                        }

                        if (pickVars.pickActive && !isBuildView)
                        {
                            if (model.CanBuildRoad() != RoadBuildError.OK)
                            {
                                effect.EmissiveColor = new Vector3(0.5f, 0.0f, 0);
                            }
                            else
                                effect.EmissiveColor = new Vector3(0, 0.5f, 0);
                        }

                        // is it model part which is for flags? They have to be in player colors
                        if (a == 3)
                        {
                            effect.EmissiveColor = color * 0.5f;
                            effect.DiffuseColor = color * 0.9f;
                            effect.AmbientLightColor = color / 3.0f;
                        }

                        effect.World = transforms[mesh.ParentBone.Index] * world;
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
            Model m = GameResources.Inst().GetShape(GameResources.SHAPE_RECTANGLE);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.EmissiveColor = pickRoadColor.ToVector3();
                    effect.World = transforms[mesh.ParentBone.Index] * worldShape;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }

        public void HandlePickableAreas(Color c)
        {
            Map.SetPickVariables(c == pickRoadColor, pickVars);
            
            if(pickVars.pickActive)
                Settings.activeRoad = roadID;

            if (pickVars.pickNewPress)
            {
                pickVars.pickNewPress = false;
                if (GameMaster.Inst().GetState() == EGameState.StateGame)
                {
                    PromptWindow.Inst().Show(PromptWindow.Mod.Buyer, Strings.HEXA_DUO, true);
                    PromptWindow.Inst().AddPromptItem(
                            new RoadPromptItem(roadID,
                                                Strings.PROMT_TITLE_WANT_TO_BUILD_ROAD,
                                                Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_ROAD,
                                                Settings.costRoad, true,
                                                GameResources.Inst().GetHudTexture(HUDTexture.IconRoad)));             
                }
            }
        }

        internal void DrawShadow(MapView mapView, Matrix shadow)
        {
            if (isBuildView)
            {
                Model m = GameResources.Inst().GetRoadModel();
                mapView.DrawShadow(m, world, shadow);
            }
        }
    }
}
