﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorePlugin;

namespace Expanze.Gameplay.Map
{
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
                Model m = GameState.map.getTownModel();
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
                        effect.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);
                        effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.1f, 0.0f);
                        effect.DirectionalLight0.Enabled = true;

                        if (a == 1 || a == 2)
                        {
                            effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);
                            effect.DiffuseColor = color * 0.9f;
                            effect.AmbientLightColor = new Vector3(0.0533f, 0.0988f, 0.1819f);
                        }
                        else
                        {
                            //effect.DiffuseColor = new Vector3(0.64f, 0.64f, 0.64f);
                        }
                        /*
                        if (pickVars.pickActive && !isBuildView)
                        {
                            if (model.CanActivePlayerBuildTown() != TownBuildError.OK &&
                                !(model.getIsBuild() && !isBuildView))
                                effect.DiffuseColor = new Vector3(1, 0.0f, 0);
                            else
                                effect.DiffuseColor = new Vector3(0, 1.0f, 0);
                        }*/

                        effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                        effect.View = GameState.view;
                        effect.Projection = GameState.projection;
                    }
                    mesh.Draw();

                    a++;
                }

                if (pickTownID == townID)
                {
                    m = GameState.map.getShape(Map.SHAPE_SPHERE);
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
            Model m = GameState.map.getShape(Map.SHAPE_CIRCLE);
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
                    if (GameMaster.getInstance().getState() == EGameState.StateGame)
                    {
                        wP.showPromt(Strings.PROMT_TITLE_WANT_TO_BUILD_TOWN, wP.BuildTown, Settings.costTown);
                        wP.setArgInt1(townID);
                    }
                    else
                    {
                        wP.showPromt(Strings.PROMT_TITLE_WANT_TO_BUILD_TOWN, wP.BuildTown, new SourceAll(0));
                        wP.setArgInt1(townID);
                    }
                }
            }
        }
    }
}
