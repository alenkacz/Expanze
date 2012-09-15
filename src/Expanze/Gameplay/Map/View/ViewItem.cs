﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Expanze.Gameplay.Map.View
{
    class ViewItem
    {
        bool valid;
        int validItems;
        int visibleItems;

        Model model;

        InstanceView[] instance;

        Matrix[] transforms;

        private static int VALID_ITEMS = 50;

        public ViewItem(Model model)
        {
            this.model = model;
            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            valid = false;
            validItems = 0;
            visibleItems = 0;

            instance = new InstanceView[ViewItem.VALID_ITEMS];
        }

        protected virtual void ResizeArrays()
        {
            InstanceView[] newInstance = new InstanceView[instance.Length * 2];

            for (int loop1 = 0; loop1 < instance.Length; loop1++)
            {
                newInstance[loop1] = instance[loop1];
            }

            instance = newInstance;
        }

        public InstanceView Add(InstanceView newInstance)
        {
            valid = false;
            if (validItems >= instance.Length)
            {
                ResizeArrays();
            }

            instance[validItems] = newInstance;

            visibleItems++;
            validItems++;

            return newInstance;
        }

        public void ChangeVisibility(int id)
        {
            instance[id].Visible = !instance[id].Visible;
            if (instance[id].Visible)
                visibleItems++;
            else
                visibleItems--;

            valid = false;
        }

        public void Draw(GameTime gameTime)
        {
            int meshNumber = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 1.0f;
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.Direction = GameState.LightDirection;
                    effect.DirectionalLight0.DiffuseColor = GameState.LightDiffusionColor;
                    effect.DirectionalLight0.SpecularColor = GameState.LightSpecularColor;
                    effect.DirectionalLight0.Enabled = true;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                    for (int loop1 = 0; loop1 < validItems; loop1++)
                    {
                        if (instance[loop1].Visible)
                        {
                            effect.World = transforms[mesh.ParentBone.Index] * instance[loop1].World;
                            instance[loop1].UpdateEffect(effect, meshNumber);
                            mesh.Draw();
                        }
                    }
                }
                meshNumber++;
            }
        }
    }
}
