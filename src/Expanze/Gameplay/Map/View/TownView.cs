using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;

namespace Expanze.Gameplay.Map
{
    class TownPromptItem : PromptItem
    {
        int townID;

        public TownPromptItem(int townID, String title, String description, SourceAll source, bool isSourceCost, Texture2D icon)
            : base(title, description, source, isSourceCost, false, icon)
        {
            this.townID = townID;
        }

        public override void Execute()
        {
            GameState.map.GetMapController().BuildTown(townID);
        }

        public override string TryExecute()
        {
            TownModel town = GameState.map.GetTownByID(townID);
            TownBuildError error = town.CanBuildTown();
            switch (error)
            {
                case TownBuildError.AlreadyBuild: return Strings.ALERT_TITLE_TOWN_IS_BUILD;
                case TownBuildError.NoPlayerRoad: return Strings.ALERT_TITLE_NO_ROAD_IS_CLOSE;
                case TownBuildError.OtherTownIsClose: return Strings.ALERT_TITLE_OTHER_TOWN_IS_CLOSE;
                case TownBuildError.NoSources: return "";
            }
            return base.TryExecute();
        }
    }

    class TownItemQueue : ItemQueue
    {
        int townID;

        public TownItemQueue(MapView mapView, int townID) : base(mapView)
        {
            this.townID = townID;
        }

        public override void Execute()
        {
            mapView.BuildTownView(townID);
            base.Execute();
        }
    }

    class TownView
    {
     
        private static int pickTownID;
        private int townID;
        private bool isBuildView;       // Could be diffrent from model Town isBuild, first is in model true but it is not draw, it waits
        private int townRotation;

        private Color pickTownColor;
        private PickVariables pickVars;
        private TownModel model;
        private Matrix world;

        private static int tutorialID;

        private bool[] buildingIsBuild; /// is building on 1-3 position build?
                                        /// 
        public static void SetPickTownID(int id) { pickTownID = id; HexaView.SetActiveHexaID(-1); }
        public static int GetPickTownID() { return pickTownID; }

        public TownView(TownModel model, Matrix world)
        {
            this.model = model;
            this.townID = model.GetTownID();
            townRotation = GameMaster.Inst().GetRandomInt(6);
            this.pickTownColor = new Color(0.0f, 0.0f, 1.0f - townID / 256.0f);

            Matrix rotation;
            rotation = (townRotation == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (townRotation));
            this.world = rotation * Matrix.CreateTranslation(new Vector3(0.0f, 0.01f, 0.0f)) * Matrix.CreateScale(0.00032f) * world;

            buildingIsBuild = new bool[3];
            for (int loop1 = 0; loop1 < buildingIsBuild.Length; loop1++)
                buildingIsBuild[loop1] = false;
            pickVars = new PickVariables(pickTownColor);
        }

        public Matrix getWorld() { return world; }
        public int getTownID() { return townID; }
        public TownModel getTownModel() { return model; }
        public Boolean getIsMarked() { return pickTownID == townID; }
        public Boolean getPickNewPress() { return pickVars.pickNewPress; }

        public void setIsBuild(bool isBuild) { this.isBuildView = isBuild; }

        public bool getBuildingIsBuild(int hexaID)
        {
            return buildingIsBuild[model.FindBuildingByHexaID(hexaID)];
        }

        public void setBuildingIsBuild(int pos, bool isBuild)
        {
            buildingIsBuild[pos] = isBuild;
        }

        public void Draw(GameTime gameTime)
        {
            if (pickVars.pickActive || isBuildView || model.GoalTown || townID == tutorialID)
            {
                Model m = GameResources.Inst().GetTownModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                int a = 0;

                Player player = model.GetOwner();
                if (player == null)
                    player = GameMaster.Inst().GetActivePlayer();
                Vector3 color = player.GetColor().ToVector3();

                int sum = 0;
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

                        if (a == 0)
                        {
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                            effect.DiffuseColor = color * 0.9f;
                            effect.AmbientLightColor = color * 0.3f;
                        }
                        else if(a == 2)
                        {
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                            effect.DiffuseColor = color * 0.6f;
                            effect.AmbientLightColor = color * 0.3f;
                            //effect.DiffuseColor = new Vector3(0.64f, 0.64f, 0.64f);
                        }

                        effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                        if (townID == tutorialID)
                        {
                            effect.EmissiveColor = new Vector3(0.7f, 0.7f, 0.0f);
                        }

                        if (model.GoalTown && !isBuildView && !pickVars.pickActive)
                        {
                            effect.EmissiveColor = new Vector3(0.8f, 0.8f, 0.8f);
                        }

                        if (pickVars.pickActive && !isBuildView)
                        {
                            if (model.CanBuildTown() != TownBuildError.OK &&
                                !(model.GetIsBuild() && !isBuildView))
                            {
                                if (a != 0)
                                    effect.EmissiveColor = new Vector3(0.5f, 0.0f, 0);
                            }
                            else
                            {
                                if (a != 0)
                                    effect.EmissiveColor = new Vector3(0, 0.5f, 0);
                            }
                        }
                            

                        effect.World = transforms[mesh.ParentBone.Index] * world;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();

                    a++;
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        sum += part.PrimitiveCount;
                    }
                }

                sum++;

                if (pickTownID == townID || (pickVars.pickActive && isBuildView))
                {
                    m = GameResources.Inst().GetShape(GameResources.SHAPE_SPHERE);

                    foreach (ModelMesh mesh in m.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.World = transforms[mesh.ParentBone.Index] * world;
                            effect.View = GameState.view;
                            effect.Projection = GameState.projection;
                        }
                        mesh.Draw();
                    }
                }
            }
        }

        public void DrawPickableAreas()
        {
            Model m = GameResources.Inst().GetShape(GameResources.SHAPE_CIRCLE);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = pickTownColor.ToVector3();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }

        public void HandlePickableAreas(Color c)
        {
            Map.SetPickVariables(c == pickTownColor, pickVars);

            if (pickVars.pickActive)
                Settings.activeTown = townID;

            // create new town?
            GameMaster gm = GameMaster.Inst();
            if (pickVars.pickNewPress)
            {
                pickVars.pickNewPress = false;
                if (model.GetIsBuild() && GameMaster.Inst().GetState() == EGameState.StateGame)
                {
                    if (pickTownID == townID)
                    {
                        TriggerManager.Inst().TurnTrigger(TriggerType.TownUnchoose, townID);
                        gm.SetTargetPlayer(gm.GetActivePlayer());
                        SetPickTownID(-1);
                    }
                    else
                    {
                        TriggerManager.Inst().TurnTrigger(TriggerType.TownChoose, townID);
                        gm.SetTargetPlayer(model.GetOwner());
                        SetPickTownID(townID);
                    }
                }
                else
                {
                        if (GameMaster.Inst().GetState() == EGameState.StateGame)
                        {
                            PromptWindow.Inst().Show(PromptWindow.Mod.Buyer, Strings.HEXA_TRI, true);
                            PromptWindow.Inst().AddPromptItem(
                                new TownPromptItem(townID,
                                                    Strings.PROMT_TITLE_WANT_TO_BUILD_TOWN,
                                                    Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_TOWN,
                                                    Settings.costTown, true, 
                                                    GameResources.Inst().GetHudTexture(HUDTexture.IconTown)));
                        }
                        else
                        {
                            GameState.map.GetMapController().BuildTown(townID);
                        }
                }
            }
        }

        public static void ResetTownView()
        {
            SetPickTownID(-1);
        }

        public static int TutorialID {
            get
            {
                return tutorialID;
            }
            set
            {
                tutorialID = value;
            }
        }

        internal void DrawShadow(MapView mapView, Matrix shadow)
        {
            if (isBuildView)
            {
                Model m = GameResources.Inst().GetTownModel();
                mapView.DrawShadow(m, world, shadow);
            }
        }
    }
}
