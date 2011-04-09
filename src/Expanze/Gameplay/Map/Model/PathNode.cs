using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze.Gameplay
{
    class PathNode
    {
        internal const int INFINITY = Int32.MaxValue;
        
        TownModel ancestorTown;
        IRoad ancestorRoad;
        int distance;

        static bool validData = false;
        static IPlayer playerReference;

        public static bool GetIsValid() { return validData;}
        public static void SetIsValid(bool valid) { validData = valid; }
        public static IPlayer GetPlayerReference() { return playerReference; }
        public static void SetPlayerReference(IPlayer player) { playerReference = player; }
        public int GetDistance() { return distance; }

        public void Clear()
        {
            ancestorTown = null;
            ancestorRoad = null;
            distance = INFINITY;
        }

        internal void Set(int distance, TownModel ancestorTown, IRoad ancestorRoad)
        {
            this.distance = distance;
            this.ancestorTown = ancestorTown;
            this.ancestorRoad = ancestorRoad;
        }

        internal TownModel GetAncestorTown() { return ancestorTown; }
        internal IRoad GetAncestorRoad() { return ancestorRoad; }
    }
}
