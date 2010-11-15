using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    static class Strings
    {
        public static string ALERT_TITLE_NOT_ENOUGH_SOURCES = "Nemáš dostatek surovin.";
        
        // Town building alerts
        public static string ALERT_TITLE_TOWN_IS_BUILD = "Město již postaveno.";
        public static string ALERT_TITLE_NO_ROAD_IS_CLOSE = "Nevede sem tvoje cesta.";
        public static string ALERT_TITLE_OTHER_TOWN_IS_CLOSE = "Jiné město v blízkosti.";

        // Road building alerts
        public static string ALERT_TITLE_ROAD_IS_BUILD = "Cesta již postavena.";
        public static string ALERT_TITLE_NO_ROAD_OR_TOWN_IS_CLOSE = "Žádná tvoje cesta či město v blízkosti";


        public static string PROMT_TITLE_WANT_TO_BUILD_TOWN = "Chceš postavit město?";
        public static string PROMT_TITLE_WANT_TO_BUILD_ROAD = "Chceš postavit cestu?";
    }
}
