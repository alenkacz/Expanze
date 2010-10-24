﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Expanze
{
    /// <summary>
    /// Containing information about one single hexa
    /// </summary>
    class Hexa
    {
        public enum RoadPos { UpLeft, UpRight, MiddleLeft, MiddleRight, BottomLeft, BottomRight, Count };
        public enum TownPos { Up, UpLeft, UpRight, BottomLeft, BottomRight, Bottom, Count };

        int value;      // how many sources will player get
        int number;     // from counter, useable for picking
        Color pickHexaColor;
        private Boolean pickActive = false;         // if mouse is over pickable area
        private Boolean pickNewActive = false;      // if mouse is newly over pickable area
        private Boolean pickNewPress = false;       // if mouse press on pickable area newly
        private Boolean pickPress = false;          // if mouse press on pickable area
        private Boolean pickNewRelease = false;     // if mouse is newly release above pickable area

        private Settings.Types type = Settings.Types.Water;
        private Town[] towns;
        private Boolean[] townOwner;
        private Road[] roads;
        private Boolean[] roadOwner;
        private Matrix world;   // wordl position of Hex

        private static int counter = 0;    // how many hexas are created

        public Hexa() : this(0, Settings.Types.Water) { }

        public Hexa( int value , Settings.Types type)
        {
            this.number = ++counter;
            this.pickHexaColor = new Color(this.number / 256.0f, 0.0f, 0.0f);
            
            this.type = type;
            this.value = value;
            this.towns = new Town[(int) TownPos.Count];
            this.roads = new Road[(int)RoadPos.Count];
            this.townOwner = new Boolean[(int)TownPos.Count];
            this.roadOwner = new Boolean[(int)RoadPos.Count];
        }

        public void setWorld(Matrix m)
        {
            world = m;
        }

        public void CreateTownsAndRoads(Hexa[] neighbours)
        {
            if (type == Settings.Types.Nothing || type == Settings.Types.Water)
                return;

            ///////////////////////
            // Creating roads -> //
            ///////////////////////
            // Always make owns road or get reference from other roads (not sending reference to other hexas)
            if (neighbours[(int)RoadPos.UpLeft] == null ||
                neighbours[(int)RoadPos.UpLeft].getRoad(RoadPos.BottomRight) == null)
            {
                roadOwner[(int)RoadPos.UpLeft] = true;
                roads[(int)RoadPos.UpLeft] = new Road(Matrix.CreateRotationY(-(float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(-0.25f, 0.0f, 0.14f)) * world);
            }
            else
                roads[(int)RoadPos.UpLeft] = neighbours[(int)RoadPos.UpLeft].getRoad(RoadPos.BottomRight);

            if (neighbours[(int)RoadPos.UpRight] == null ||
                neighbours[(int)RoadPos.UpRight].getRoad(RoadPos.BottomLeft) == null)
            {
                roadOwner[(int)RoadPos.UpRight] = true;
                roads[(int)RoadPos.UpRight] = new Road(Matrix.CreateRotationY((float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(-0.25f, 0.0f, -0.14f)) * world);
            }
            else
                roads[(int)RoadPos.UpRight] = neighbours[(int)RoadPos.UpRight].getRoad(RoadPos.BottomLeft);

            if (neighbours[(int)RoadPos.MiddleLeft] == null ||
                neighbours[(int)RoadPos.MiddleLeft].getRoad(RoadPos.MiddleRight) == null)
            {
                roadOwner[(int)RoadPos.MiddleLeft] = true;
                roads[(int)RoadPos.MiddleLeft] = new Road(Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, 0.29f)) * world);
            }
            else
                roads[(int)RoadPos.MiddleLeft] = neighbours[(int)RoadPos.MiddleLeft].getRoad(RoadPos.MiddleRight);

            if (neighbours[(int)RoadPos.MiddleRight] == null ||
                neighbours[(int)RoadPos.MiddleRight].getRoad(RoadPos.MiddleLeft) == null)
            {
                roadOwner[(int)RoadPos.MiddleRight] = true;
                roads[(int)RoadPos.MiddleRight] = new Road(Matrix.CreateTranslation(new Vector3(0.0f, 0.0f, -0.28f)) * world);
            }
            else
                roads[(int)RoadPos.MiddleRight] = neighbours[(int)RoadPos.MiddleRight].getRoad(RoadPos.MiddleLeft);


            if (neighbours[(int)RoadPos.BottomLeft] == null ||
                neighbours[(int)RoadPos.BottomLeft].getRoad(RoadPos.UpRight) == null)
            {
                roadOwner[(int)RoadPos.BottomLeft] = true;
                roads[(int)RoadPos.BottomLeft] = new Road(Matrix.CreateRotationY((float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(0.25f, 0.0f, 0.14f)) * world);
            }
            else
                roads[(int)RoadPos.BottomLeft] = neighbours[(int)RoadPos.BottomLeft].getRoad(RoadPos.UpRight);

            if (neighbours[(int)RoadPos.BottomRight] == null ||
                neighbours[(int)RoadPos.BottomRight].getRoad(RoadPos.UpLeft) == null)
            {
                roadOwner[(int)RoadPos.BottomRight] = true;
                roads[(int)RoadPos.BottomRight] = new Road(Matrix.CreateRotationY(-(float)Math.PI / 3.0f) * Matrix.CreateTranslation(new Vector3(0.25f, 0.0f, -0.14f)) * world);
            }
            else
                roads[(int)RoadPos.BottomRight] = neighbours[(int)RoadPos.BottomRight].getRoad(RoadPos.UpLeft);
            
            ///////////////////////
            // Creating tows ->  //
            ///////////////////////
            if ((neighbours[(int)RoadPos.UpLeft] == null ||
                 neighbours[(int)RoadPos.UpLeft].getTown(TownPos.BottomRight) == null) &&
                (neighbours[(int)RoadPos.UpRight] == null ||
                 neighbours[(int)RoadPos.UpRight].getTown(TownPos.BottomLeft) == null))
            {
                townOwner[(int)TownPos.Up] = true;
                towns[(int)TownPos.Up] = new Town(Matrix.CreateTranslation(new Vector3(-0.32f, 0.0f, 0.0f)) * world);
            }
            else
                if (neighbours[(int)RoadPos.UpLeft] != null && neighbours[(int)RoadPos.UpLeft].getTown(TownPos.BottomRight) != null)
                    towns[(int)TownPos.Up] = neighbours[(int)RoadPos.UpLeft].getTown(TownPos.BottomRight);
                else
                    towns[(int)TownPos.Up] = neighbours[(int)RoadPos.UpRight].getTown(TownPos.BottomLeft);

            if ((neighbours[(int)RoadPos.BottomLeft] == null ||
                 neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.UpRight) == null) &&
                (neighbours[(int)RoadPos.BottomRight] == null ||
                 neighbours[(int)RoadPos.BottomRight].getTown(TownPos.UpLeft) == null))
            {
                townOwner[(int)TownPos.Bottom] = true;
                towns[(int)TownPos.Bottom] = new Town(Matrix.CreateTranslation(new Vector3(0.32f, 0.0f, 0.0f)) * world);
            }
            else
                towns[(int)TownPos.Bottom] = neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.UpRight);

            if ((neighbours[(int)RoadPos.UpRight] == null ||
                neighbours[(int)RoadPos.UpRight].getTown(TownPos.Bottom) == null) &&
               (neighbours[(int)RoadPos.MiddleRight] == null ||
                neighbours[(int)RoadPos.MiddleRight].getTown(TownPos.UpLeft) == null))
            {
                townOwner[(int)TownPos.UpRight] = true;
                towns[(int)TownPos.UpRight] = new Town(Matrix.CreateTranslation(new Vector3(-0.16f, 0.0f, -0.28f)) * world);
            }
            else
                if (neighbours[(int)RoadPos.UpRight] != null && neighbours[(int)RoadPos.UpRight].getTown(TownPos.Bottom) != null)
                    towns[(int)TownPos.UpRight] = neighbours[(int)RoadPos.UpRight].getTown(TownPos.Bottom);
                else
                    towns[(int)TownPos.UpRight] = neighbours[(int)RoadPos.MiddleRight].getTown(TownPos.UpLeft);

            if ((neighbours[(int)RoadPos.UpLeft] == null ||
                 neighbours[(int)RoadPos.UpLeft].getTown(TownPos.Bottom) == null) &&
                (neighbours[(int)RoadPos.MiddleLeft] == null ||
                 neighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.UpRight) == null))
            {
                townOwner[(int)TownPos.UpLeft] = true;
                towns[(int)TownPos.UpLeft] = new Town(Matrix.CreateTranslation(new Vector3(-0.16f, 0.0f, 0.28f)) * world);
            }
            else
                towns[(int)TownPos.UpLeft] = neighbours[(int)RoadPos.UpLeft].getTown(TownPos.Bottom);

            if ((neighbours[(int)RoadPos.BottomRight] == null ||
                neighbours[(int)RoadPos.BottomRight].getTown(TownPos.Up) == null) &&
               (neighbours[(int)RoadPos.MiddleRight] == null ||
                neighbours[(int)RoadPos.MiddleRight].getTown(TownPos.BottomLeft) == null))
            {
                townOwner[(int)TownPos.BottomRight] = true;
                towns[(int)TownPos.BottomRight] = new Town(Matrix.CreateTranslation(new Vector3(0.16f, 0.0f, -0.28f)) * world);
            }
            else
                if (neighbours[(int)RoadPos.BottomRight] != null && neighbours[(int)RoadPos.BottomRight].getTown(TownPos.Bottom) != null)
                    towns[(int)TownPos.BottomRight] = neighbours[(int)RoadPos.BottomRight].getTown(TownPos.Up);
                else
                    towns[(int)TownPos.BottomRight] = neighbours[(int)RoadPos.MiddleRight].getTown(TownPos.BottomLeft);

            if ((neighbours[(int)RoadPos.BottomLeft] == null ||
                neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.Up) == null) &&
               (neighbours[(int)RoadPos.MiddleLeft] == null ||
                neighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.BottomRight) == null))
            {
                townOwner[(int)TownPos.BottomLeft] = true;
                towns[(int)TownPos.BottomLeft] = new Town(Matrix.CreateTranslation(new Vector3(0.16f, 0.0f, 0.28f)) * world);
            }
            else
                if (neighbours[(int)RoadPos.BottomLeft] != null && neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.Bottom) != null)
                    towns[(int)TownPos.BottomLeft] = neighbours[(int)RoadPos.BottomLeft].getTown(TownPos.Up);
                else
                    towns[(int)TownPos.BottomLeft] = neighbours[(int)RoadPos.MiddleLeft].getTown(TownPos.BottomRight);
        }

        public void Draw(GameTime gameTime)
        {
            Model m = GameState.map.getHexaModel(type);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GameState.game.GraphicsDevice.RasterizerState = rasterizerState;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    if (pickActive)
                        effect.EmissiveColor = new Vector3(0.3f, 0.0f, 0.0f);
                    else
                        effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);

                    effect.World = Matrix.CreateRotationZ((float) Math.PI) * transforms[mesh.ParentBone.Index] * world;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }

            for (int loop1 = 0; loop1 < roads.Length; loop1++)
                if (roadOwner[loop1])
                    roads[loop1].Draw(gameTime);


            for (int loop1 = 0; loop1 < towns.Length; loop1++)
                if (townOwner[loop1])
                    towns[loop1].Draw(gameTime);

            Vector3 point3D = GameState.game.GraphicsDevice.Viewport.Project(new Vector3(-0.15f, 0.0f, 0.05f), GameState.projection, GameState.view, world);
            Vector2 point2D = new Vector2();
            point2D.X = point3D.X;
            point2D.Y = point3D.Y;
            SpriteBatch spriteBatch = GameState.spriteBatch;

            spriteBatch.Begin();
            spriteBatch.DrawString(GameState.gameFont, value + "", point2D, Color.DarkRed);
            spriteBatch.End();

            GameState.game.GraphicsDevice.BlendState = BlendState.Opaque;
            GameState.game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        public void DrawPickableAreas()
        {
            Model m = GameState.map.getCircleShape();
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.05f, 0.0f)) * Matrix.CreateScale(0.6f) * world;
            
            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = pickHexaColor.ToVector3(); //new Vector3((float) number / counter, (float) number / counter, (float) number / counter);
                    effect.World = transforms[mesh.ParentBone.Index] * mWorld;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }

            for (int loop1 = 0; loop1 < roads.Length; loop1++)
                if (roadOwner[loop1])
                    roads[loop1].DrawPickableAreas();


            for (int loop1 = 0; loop1 < towns.Length; loop1++)
                if (townOwner[loop1])
                    towns[loop1].DrawPickableAreas();
        }

        public void HandlePickableAreas(Color c)
        {
            for (int loop1 = 0; loop1 < roads.Length; loop1++)
                if (roadOwner[loop1])
                    roads[loop1].HandlePickableAreas(c);

            for (int loop1 = 0; loop1 < towns.Length; loop1++)
                if (townOwner[loop1])
                    towns[loop1].HandlePickableAreas(c);

            if (c == pickHexaColor)
            {
                if (!pickActive)
                    pickNewActive = true;
                pickActive = true;

                if (GameState.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!pickPress)
                        pickNewPress = true;
                    pickPress = true;
                }
                else
                {
                    if (pickPress)
                        pickNewRelease = true;
                    pickPress = false;
                }
            }
            else
            {
                pickActive = false;
                pickPress = false;
                pickNewActive = false;
                pickNewPress = false;
                pickNewRelease = false;
            }
        }

        public string getModelPath()
        {
            return Settings.mapPaths[(int)getType()];
        }

        public Settings.Types getType()
        {
            return this.type;
        }

        public Road getRoad(RoadPos roadPos)
        {
            return roads[(int)roadPos];
        }

        public Town getTown(TownPos townPos)
        {
            return towns[(int)townPos];
        }
    }
}
