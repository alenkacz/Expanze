﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace AIHard
{

    class Desirability
    {
        private static IMapController map;

        public static void SetMapController(IMapController mapController)
        {
            map = mapController;
        }

        public static double GetHasSources(PriceKind priceKind)
        {
            return GetHasSources(map.GetPrice(priceKind));
        }
        public static double GetHasSources(ISourceAll source)
        {
            int sourceToChange = map.CanChangeSourcesFor(source);
            double hasSourcesDesirability = 1.0 - ((sourceToChange > 100) ? 100 : sourceToChange) / 100.0;

            return hasSourcesDesirability;
        }

        public static bool HasSomeoneBuilding(Building building)
        {
            List<IPlayer> players = map.GetPlayerOthers();

            foreach (IPlayer player in players)
            {
                if (player.GetBuildingCount(building) > 0)
                    return true;
            }
            return false;
        }
    }
}
