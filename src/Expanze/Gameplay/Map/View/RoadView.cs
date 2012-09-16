using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Expanze.Gameplay.Map;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;
using Expanze.Utils.Music;

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
            if (GameState.map.GetMapController().BuildRoad(roadID) != null)
                MusicManager.Inst().PlaySound(SoundEnum.building);
            else
                MusicManager.Inst().PlaySound(SoundEnum.button2);
        }

        public override string TryExecute()
        {
            RoadModel road = GameState.map.GetRoadByID(roadID);
            RoadBuildError error = road.CanBuildRoad();
            switch (error)
            {
                case RoadBuildError.AlreadyBuild: return Strings.Inst().GetString(TextEnum.ALERT_TITLE_ROAD_IS_BUILD);
                case RoadBuildError.NoPlayerRoadOrTown: return Strings.Inst().GetString(TextEnum.ALERT_TITLE_NO_ROAD_OR_TOWN_IS_CLOSE);
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
        private PlayerInstanceView roadInstance;

        private static Vector3 colorBlack = new Vector3(0.0f, 0.0f, 0.0f);
        private static Vector3 colorTutorial = new Vector3(0.5f, 0.5f, 0.0f);
        private static Vector3 colorGoal = new Vector3(0.7f, 0.7f, 0.7f);
        private static Vector3 colorCanBuild = new Vector3(0.0f, 0.5f, 0);
        private static Vector3 colorCantBuild = new Vector3(0.5f, 0.0f, 0);

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

        public RoadView(RoadModel model, Matrix world, ModelView modelView)
        {
            this.modelView = modelView;
            this.model = model;
            this.roadID = model.GetRoadID();
            this.pickRoadColor = new Color(0.0f, 1.0f - roadID / 256.0f, 0.0f);
            this.world = Matrix.CreateTranslation(new Vector3(0.0f, 0.012f, 0.0f)) * Matrix.CreateScale(0.1f) * world;
            worldShape = Matrix.CreateTranslation(new Vector3(0.0f, 0.03f, 0.0f)) * Matrix.CreateScale(0.25f) * world;
            pickVars = new PickVariables(pickRoadColor);
            roadInstance = (PlayerInstanceView) modelView.AddInstance(GameResources.Inst().GetRoadModel(), new PlayerInstanceView(this.world, 0, 1));
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
                roadInstance.Visible = true;

                //if (roadID == 77)
                //  roadInstance.Visible = true;

                Player player = model.GetOwner();
                if (player == null)
                    player = GameMaster.Inst().GetActivePlayer();
                Vector3 color = player.GetColor().ToVector3();

                roadInstance.PlayerEmissiveColor = color * 0.5f;
                roadInstance.PlayerDiffuseColor = color * 0.9f;
                roadInstance.PlayerAmbientLightColor = color / 3.0f;

                roadInstance.NormalEmissiveColor = colorBlack;
                if (tutorialID == roadID)
                {
                    roadInstance.NormalEmissiveColor = colorTutorial;
                }
                // if player wants to build new Road, can he? Show it in red/green color
                if (model.GoalRoad && !isBuildView && !(pickVars.pickActive && gm.GetState() == EGameState.StateGame))
                {
                    roadInstance.NormalEmissiveColor = colorGoal;
                }

                if (pickVars.pickActive && !isBuildView)
                {
                    if (model.CanBuildRoad() != RoadBuildError.OK)
                    {
                        roadInstance.NormalEmissiveColor = colorCantBuild;
                    }
                    else
                        roadInstance.NormalEmissiveColor = colorCanBuild;
                }
            }
            else
            {
                roadInstance.Visible = false;
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
                    if(model.GetIsBuild())
                        MusicManager.Inst().PlaySound(SoundEnum.road);
                    PromptWindow.Inst().Show(PromptWindow.Mod.Buyer, Strings.Inst().GetString(TextEnum.HEXA_DUO), true);
                    PromptWindow.Inst().AddPromptItem(
                            new RoadPromptItem(roadID,
                                                Strings.Inst().GetString(TextEnum.PROMT_TITLE_WANT_TO_BUILD_ROAD),
                                                Strings.Inst().GetString(TextEnum.PROMPT_DESCRIPTION_WANT_TO_BUILD_ROAD),
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

        ModelView modelView;
    }
}
