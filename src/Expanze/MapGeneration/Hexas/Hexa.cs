using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private Boolean pickActive = false;

        private Settings.Types type = Settings.Types.Water;
        private Town[] towns;
        private Road[] roads;
        private Matrix world;   // wordl position of Hex

        private static Map map;
        private static int counter = 0;    // how many hexas are created

        public Hexa() : this(0, Settings.Types.Water) { }

        public Hexa( int value , Settings.Types type)
        {
            this.number = ++counter;
            this.pickHexaColor = new Color(this.number / 256.0f, 0.0f, 0.0f);
            
            this.type = type;
            this.value = value;
            this.towns = new Town[(int) TownPos.Count];
            this.roads = new Road[(int) RoadPos.Count];
        }

        public void setWorld(Matrix m)
        {
            world = m;
        }

        public static void setMap(Map map2)
        {
            map = map2;
        }

        public void CreateTownsAndRoads(Hexa[] neighbours)
        {
            if (type == Settings.Types.Nothing || type == Settings.Types.Water)
                return;

            // Always make owns road or get reference from other roads (not sending reference to other hexas)
            if (neighbours[(int)RoadPos.UpLeft] == null || 
                neighbours[(int)RoadPos.UpLeft].getRoad(RoadPos.BottomRight) == null)
                roads[(int)RoadPos.UpLeft] = new Road();
            else
                roads[(int)RoadPos.UpLeft] = neighbours[(int)RoadPos.UpLeft].getRoad(RoadPos.BottomRight);

            if (neighbours[(int)RoadPos.UpRight] == null ||
                neighbours[(int)RoadPos.UpRight].getRoad(RoadPos.BottomLeft) == null)
                roads[(int)RoadPos.UpRight] = new Road();
            else
                roads[(int)RoadPos.UpRight] = neighbours[(int)RoadPos.UpRight].getRoad(RoadPos.BottomLeft);

            if (neighbours[(int)RoadPos.MiddleLeft] == null ||
                neighbours[(int)RoadPos.MiddleLeft].getRoad(RoadPos.MiddleRight) == null)
                roads[(int)RoadPos.MiddleLeft] = new Road();
            else
                roads[(int)RoadPos.MiddleLeft] = neighbours[(int)RoadPos.MiddleLeft].getRoad(RoadPos.MiddleRight);

            if (neighbours[(int)RoadPos.MiddleRight] == null ||
                neighbours[(int)RoadPos.MiddleRight].getRoad(RoadPos.MiddleLeft) == null)
                roads[(int)RoadPos.MiddleRight] = new Road();
            else
                roads[(int)RoadPos.MiddleRight] = neighbours[(int)RoadPos.MiddleRight].getRoad(RoadPos.MiddleLeft);


            if (neighbours[(int)RoadPos.BottomLeft] == null ||
                neighbours[(int)RoadPos.BottomLeft].getRoad(RoadPos.UpRight) == null)
                roads[(int)RoadPos.BottomLeft] = new Road();
            else
                roads[(int)RoadPos.BottomLeft] = neighbours[(int)RoadPos.BottomLeft].getRoad(RoadPos.UpRight);

            if (neighbours[(int)RoadPos.BottomRight] == null ||
                neighbours[(int)RoadPos.BottomRight].getRoad(RoadPos.UpLeft) == null)
                roads[(int)RoadPos.BottomRight] = new Road();
            else
                roads[(int)RoadPos.BottomRight] = neighbours[(int)RoadPos.BottomRight].getRoad(RoadPos.UpLeft);
        }

        public void Draw(GameTime gameTime)
        {
            Model m = map.getHexaModel(type);
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    if (pickActive)
                        effect.EmissiveColor = new Vector3(1.0f, 0.0f, 0.0f);
                    else
                        effect.EmissiveColor = new Vector3(0.0f, 0.0f, 0.0f);

                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = GameState.view;
                    effect.Projection = GameState.projection;
                }
                mesh.Draw();
            }
        }

        public void DrawPickableAreas()
        {
            Model m = map.getCircleShape();
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix mWorld = Matrix.CreateTranslation(new Vector3(0.0f, 0.1f, 0.0f)) * Matrix.CreateScale(0.5f) * world;

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
        }

        public void HandlePickableAreas(Color c)
        {
            if (c == pickHexaColor)
            {
                pickActive = true;
            }
            else
            {
                pickActive = false;
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
