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

        public TownPromptItem(int townID, String title, String description, SourceAll cost, Texture2D icon) : base(title, description, cost, icon)
        {
            this.townID = townID;
        }

        public override void Execute()
        {
            GameState.map.BuildTown(townID);
        }
    }

    class TownView
    {
        private static int pickTownID = -1;
        private int townID;
        private bool isBuildView;       // Could be diffrent from model Town isBuild, first is in model true but it is not draw, it waits

        private Color pickTownColor;
        private PickVariables pickVars;
        private Town model;
        private Matrix world;

        public TownView(Town model, Matrix world)
        {
            this.model = model;
            this.townID = model.getTownID();
            this.pickTownColor = new Color(0.0f, 0.0f, townID / 256.0f);
            this.world = world;

            pickVars = new PickVariables(pickTownColor);
        }

        public int getTownID() { return townID; }
        public Town getTownModel() { return model; }
        public Boolean getIsMarked() { return pickTownID == townID; }
        public Boolean getPickNewPress() { return pickVars.pickNewPress; }

        public void setIsBuild(bool isBuild) { this.isBuildView = isBuild; }

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
                    WindowPromt wP = GameState.windowPromt;


                    switch (model.CanActivePlayerBuildTown())
                    {
                        case TownBuildError.AlreadyBuild:
                            wP.showAlert(Strings.ALERT_TITLE_TOWN_IS_BUILD);
                            break;
                        case TownBuildError.NoSources:
                            wP.showAlert(Strings.ALERT_TITLE_NOT_ENOUGH_SOURCES);
                            break;
                        case TownBuildError.NoPlayerRoad:
                            wP.showAlert(Strings.ALERT_TITLE_NO_ROAD_IS_CLOSE);
                            break;
                        case TownBuildError.OtherTownIsClose:
                            wP.showAlert(Strings.ALERT_TITLE_OTHER_TOWN_IS_CLOSE);
                            break;

                        case TownBuildError.OK :
                            if (GameMaster.getInstance().getState() == EGameState.StateGame)
                            {
                                PromptWindow.Inst().showPrompt(Strings.PROMPT_TITLE_BUILDING,
                                    new TownPromptItem(townID,
                                                       Strings.PROMT_TITLE_WANT_TO_BUILD_TOWN,
                                                       "",
                                                       Settings.costTown,
                                                       null));
                            }
                            else
                            {
                                GameState.map.BuildTown(townID);
                                //wP.showPromt(Strings.PROMT_TITLE_WANT_TO_BUILD_TOWN, wP.BuildTown, new SourceAll(0));
                                //wP.setArgInt1(townID);
                            }
                            break;
                    }
                }
            }
        }
    }
}
