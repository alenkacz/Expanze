using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    static class Strings
    {
        // Menu strings

        public static string MENU_LOADING_LOADING = "Načítání obsahu";

        //

        public static string ALERT_TITLE_NOT_ENOUGH_SOURCES = "Nemáš dostatek surovin.";
        
        // Town building alerts
        public static string ALERT_TITLE_TOWN_IS_BUILD = "Město již postaveno.";
        public static string ALERT_TITLE_NO_ROAD_IS_CLOSE = "Nevede sem tvoje cesta.";
        public static string ALERT_TITLE_OTHER_TOWN_IS_CLOSE = "Jiné město v blízkosti.";

        // Road building alerts
        public static string ALERT_TITLE_ROAD_IS_BUILD = "Cesta již postavena.";
        public static string ALERT_TITLE_NO_ROAD_OR_TOWN_IS_CLOSE = "Žádná tvoje cesta či město v blízkosti.";

        // Special building alerts
        public static string ALERT_TITLE_MAX_UPGRADES = "Už máš zakoupené 3 pokroky.";

        // Source buildings building
        public static string HEXA_TRI = "Rozcestí";
        public static string HEXA_DUO = "Údolí";
        public static string HEXA_NAME_MOUNTAINS = "Rudnaté hory";
        public static string HEXA_NAME_PASTURE = "Pastvina";
        public static string HEXA_NAME_STONE = "Kamenná mohyla";
        public static string HEXA_NAME_FOREST = "Jehličnatý les";
        public static string HEXA_NAME_CORNFIELD = "Obilné pole";
        public static string HEXA_NAME_DESERT = "Poušť";


        public static string ALERT_TITLE_NOT_TOWN_OWNER = "To není tvé město.";
        public static string ALERT_TITLE_BUILDING_IS_BUILD = "Už tu stojí budova.";

        public static string PROMT_TITLE_WANT_TO_BUILD_TOWN = "Město";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_TOWN = "U města můžeš stavět chatrč pastevce, lom, důl, mlýn, pilu, či pevnost, tržiště, klášter.";
        public static string PROMT_TITLE_WANT_TO_BUILD_ROAD = "Cesta";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_ROAD = "Umožní rozšířit tvé území, stavět nová města na konci cest.";

        public static string PROMT_TITLE_WANT_TO_BUILD_MINE = "Důl na rudu";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_MINE = "Horníci budou pro tebe těžit z dolu rudu. Po vynalezení pokroku z Kláštera lze důl vylepšit, aby těžba byla ještě vyšší.";
        public static string PROMT_TITLE_WANT_TO_BUILD_QUARRY = "Kamenný lom";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_QUARRY = "Horníci budou těžit stavební kámen.  Po vynalezení pokroku z Kláštera jim to půjde líp.";
        public static string PROMT_TITLE_WANT_TO_BUILD_SAW = "Pila";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_SAW = "Dřevorubci natahají z lesa dřevo.  Po vynalezení pokroku z Kláštera bude těžba účinější.";
        public static string PROMT_TITLE_WANT_TO_BUILD_MILL = "Větrný mlýn";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_MILL = "Mlynář z pole získá spoustu obilí. Po vynalezení pokroku z Kláštera lze mlýn vylepšit, aby zisky byly vyšší.";
        public static string PROMT_TITLE_WANT_TO_BUILD_STEPHERD = "Chatrč pastevce";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_STEPHERD = "Pastevec ti bude dodávat mnoho oveček.  Po vynalezení pokroku z Kláštera budeš mít ovcí mnohem víc.";

        public static string PROMPT_TITLE_WANT_TO_BUILD_FORT = "Pevnost";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_FORT = "Z pevnosti můžeš poslat vojsko poničit nějaké pole, obsadit důl, zničit suroviny protihráči, nebo získat body za vojenskou přehlídku.";
        public static string PROMPT_TITLE_WANT_TO_BUILD_MARKET = "Tržiště";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_MARKET = "Na tržišti si můžeš koupit lepší směnný kurz pro výměnu surovin. Chceš-li měnit tři ku jedné, či dva ku jedné, tržiště je jasná volba.";
        public static string PROMPT_TITLE_WANT_TO_BUILD_MONASTERY = "Klášter";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_MONASTERY = "Mniši můžou vynaleznout lepší nástroje pro horníky, dřevorubce, zvýšit úrodnost obilných polí, urychlit práci pastevce.";
    }
}
