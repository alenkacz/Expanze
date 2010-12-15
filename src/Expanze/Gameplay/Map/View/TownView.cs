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
            : base(title, description, source, isSourceCost, icon)
        {
            this.townID = townID;
        }

        public override void Execute()
        {
            GameState.map.GetMapController().BuildTown(townID);
        }

        public override string TryExecute()
        {
            Town town = GameState.map.GetTownByID(townID);
            TownBuildError error = town.CanActivePlayerBuildTown();
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

        private Color pickTownColor;
        private PickVariables pickVars;
        private Town model;
        private Matrix world;

        private bool[] buildingIsBuild; /// is building on 1-3 position build?

        public TownView(Town model, Matrix world)
        {
            this.model = model;
            this.townID = model.getTownID();
            this.pickTownColor = new Color(0.0f, 0.0f, townID / 256.0f);
            this.world = world;
            buildingIsBuild = new bool[3];
            for (int loop1 = 0; loop1 < buildingIsBuild.Length; loop1++)
                buildingIsBuild[loop1] = false;
            pickVars = new PickVariables(pickTownColor);
        }

        public int getTownID() { return townID; }
        public Town getTownModel() { return model; }
        public Boolean getIsMarked() { return pickTownID == townID; }
        public Boolean getPickNewPress() { return pickVars.pickNewPress; }

        public void setIsBuild(bool isBuild) { this.isBuildView = isBuild; }

        public bool getBuildingIsBuild(int hexaID)
        {
            return buildingIsBuild[model.findBuildingByHexaID(hexaID)];
        }

        public void setBuildingIsBuild(int pos, bool isBuild)
        {
            buildingIsBuild[pos] = isBuild;
        }

        public void draw(GameTime gameTime)
        {
            if (pickVars.pickActive || isBuildView)
            {
                Model m = GameResources.Inst().getTownModel();
                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                Matrix rotation;
                rotation = (townID % 6 == 0) ? Matrix.Identity : Matrix.CreateRotationY(((float)Math.PI / 3.0f) * (townID % 6));
                Matrix mWorld = rotation * Matrix.CreateTranslation(new Vector3(0.0f, 0.01f, 0.0f)) * Matrix.CreateScale(0.00032f) * world;

                int a = 0;

                Player player = model.getPlayerOwner();
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
                        
                        if (pickVars.pickActive && !isBuildView)
                        {
                            if (model.CanActivePlayerBuildTown() != TownBuildError.OK &&
                                !(model.getIsBuild() && !isBuildView))
                            {
                                if (a != 0)
                                    effect.EmissiveColor = new Vector3(0.5f, 0.0f, 0);
                            }
                            else
                            {
                                if (a != 0)
                                    effect.EmissiveColor = new Vector3(0, 0.5f, 0);
                            }
                        } else
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);

                        effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();

                    a++;
                }

                if (pickTownID == townID || (pickVars.pickActive && isBuildView))
                {
                    m = GameResources.Inst().getShape(GameResources.SHAPE_SPHERE);
                    mWorld = Matrix.CreateScale(0.0001f) * Matrix.CreateTranslation(new Vector3(0.0f, 0.15f, 0.0f)) * world;
                    foreach (ModelMesh mesh in m.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                            effect.View = GameState.view;
                            effect.Projection = GameState.projection;
                        }
                        mesh.Draw();
                    }
                }
            }
        }

        public void drawPickableAreas()
        {
            Model m = GameResources.Inst().getShape(GameResources.SHAPE_CIRCLE);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.04f, 0.0f)) * Matrix.CreateScale(0.22f) * world;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = pickTownColor.ToVector3();
                    effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }

        public void handlePickableAreas(Color c)
        {
            Map.SetPickVariables(c == pickTownColor, pickVars);

            // create new town?
            GameMaster gm = GameMaster.getInstance();
            if (pickVars.pickNewPress)
            {
                pickVars.pickNewPress = false;
                if (model.getIsBuild() && GameMaster.getInstance().getState() == EGameState.StateGame)
                {
                    if (pickTownID == townID)
                        pickTownID = -1;
                    else
                        pickTownID = townID;
                }
                else
                {
                        if (GameMaster.getInstance().getState() == EGameState.StateGame)
                        {
                            PromptWindow.Inst().showPrompt(PromptWindow.Mod.Buyer, Strings.HEXA_TRI, true);
                            PromptWindow.Inst().addPromptItem(
                                new TownPromptItem(townID,
                                                    Strings.PROMT_TITLE_WANT_TO_BUILD_TOWN,
                                                    Strings.PROMPT_DESCRIPTION_WANT_TO_BUILD_TOWN,
                                                    Settings.costTown, true, 
                                                    GameResources.Inst().getHudTexture(HUDTexture.IconTown)));
                        }
                        else
                        {
                            GameState.map.GetMapController().BuildTown(townID);
                        }
                }
            }
        }

        public static void resetTownView()
        {
            pickTownID = -1;
        }
    }
}
