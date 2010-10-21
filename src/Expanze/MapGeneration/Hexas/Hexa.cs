using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    /// <summary>
    /// Containing information about one single hexa
    /// </summary>
    class Hexa
    {
        public enum RoadPos { UpLeft, UpRight, MiddleLeft, MiddleRight, BottomLeft, BottomRight, Count };
        public enum TownPos { Up, UpLeft, UpRight, BottomLeft, BottomRight, Bottom, Count };

        int value;
        private Settings.Types type = Settings.Types.Water;
        private Town[] towns;
        private Road[] roads;


        public Hexa() : this(0, Settings.Types.Water) { }

        public Hexa( int value , Settings.Types type)
        {
            this.type = type;
            this.value = value;
            this.towns = new Town[(int) TownPos.Count];
            this.roads = new Road[(int) RoadPos.Count];
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
