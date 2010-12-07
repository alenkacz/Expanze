﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    static class Strings
    {

        // Menu strings
        public static string MENU_HOT_SEAT_NO_AI = "Hráč";

        public static string MENU_PAUSE_GAME_ITEM_RESUME = "Zpět do hry";
        public static string MENU_PAUSE_GAME_ITEM_QUIT_GAME = "Ukončit hru";

        public static string MENU_LOADING_LOADING = "Načítání obsahu";

        //
        public static string ALERT_TITLE_THIS_IS_NOT_YOURS = "Tohle není tvoje. Můžeš pouze koukat.";
        public static string ALERT_TITLE_NOT_ENOUGH_SOURCES = "Nemáš dostatek surovin.";
        
        // Town building alerts
        public static string ALERT_TITLE_TOWN_IS_BUILD = "Město již postaveno.";
        public static string ALERT_TITLE_NO_ROAD_IS_CLOSE = "Nevede sem tvoje cesta.";
        public static string ALERT_TITLE_OTHER_TOWN_IS_CLOSE = "Jiné město v blízkosti.";

        // Road building alerts
        public static string ALERT_TITLE_ROAD_IS_BUILD = "Cesta již postavena.";
        public static string ALERT_TITLE_NO_ROAD_OR_TOWN_IS_CLOSE = "Žádná tvoje cesta či město v blízkosti.";

        // Special building alerts
        public static string ALERT_TITLE_ALREADY_HAVE_SECOND_UPGRADE = "Již máš koupen tento pokrok.";
        public static string ALERT_TITLE_NO_UPGRADE = "Prvně musíš vynaleznout pokrok v kláštěře.";
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
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_1_MINE = "Vozíky na rudu";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MINE = "Pořídíš-li horníkům nové vozíky, budou pracovat o 50% lépe";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_2_MINE = "Nové helmy";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MINE = "Horníci s novými helmami nebudou tak často na marodce. Zisky rudy o 100% lepší.";

        public static string PROMT_TITLE_WANT_TO_BUILD_QUARRY = "Kamenný lom";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_QUARRY = "Horníci budou těžit stavební kámen.  Po vynalezení pokroku z Kláštera jim to půjde líp.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_1_QUARRY = "Lepší krumpáče";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_QUARRY = "Pořídíš-li horníkům lepší krumpáče, budou pracovat o 50% lépe";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_2_QUARRY = "Trhavina";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_QUARRY = "Horníci s trhavinou dovedou divy. Zisky kamene o 100% lepší.";
       
        public static string PROMT_TITLE_WANT_TO_BUILD_SAW = "Pila";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_SAW = "Dřevorubci natahají z lesa dřevo.  Po vynalezení pokroku z Kláštera bude těžba účinější.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_1_SAW = "Sekery";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_SAW = "Pořídíš-li dřevorupcům sekery, budou pracovat o 50% lépe";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_2_SAW = "Tažní koně";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_SAW = "Tažní koně velice urychlí práci. Zisky dřeva o 100% lepší.";

        
        public static string PROMT_TITLE_WANT_TO_BUILD_MILL = "Větrný mlýn";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_MILL = "Mlynář z pole získá spoustu obilí. Po vynalezení pokroku z Kláštera lze mlýn vylepšit, aby zisky byly vyšší.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_1_MILL = "Zázračné hnojivo";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MILL = "S hnojivem ti roste obilí o 50% víc.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_2_MILL = "Nové lopaty";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MILL = "Mlýnské lopaty urychlí zpracování obilí o 100%.";
      
        public static string PROMT_TITLE_WANT_TO_BUILD_STEPHERD = "Chatrč pastevce";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_STEPHERD = "Pastevec ti bude dodávat mnoho oveček.  Po vynalezení pokroku z Kláštera budeš mít ovcí mnohem víc.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_1_STEPHERD = "Krmivo pro ovce";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_STEPHERD = "Jen tráva nestačí, s krmivem jich bude o 50% víc.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_2_STEPHERD = "Naslouchej ovcím";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_STEPHERD = "Pastevec se naučí ovcím naslouchat, bude jich dvakrát tolik.";

        public static string PROMPT_TITLE_WANT_TO_BUILD_FORT = "Pevnost";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_FORT = "Z pevnosti můžeš poslat vojsko poničit nějaké pole, obsadit důl, zničit suroviny protihráči, nebo získat body za vojenskou přehlídku.";
        public static string PROMPT_TITLE_WANT_TO_BUILD_MARKET = "Tržiště";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_MARKET = "Na tržišti si můžeš koupit lepší směnný kurz pro výměnu surovin. Chceš-li měnit tři ku jedné, či dva ku jedné, tržiště je jasná volba.";
        public static string PROMPT_TITLE_WANT_TO_BUILD_MONASTERY = "Klášter";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_MONASTERY = "Mniši můžou vynaleznout lepší nástroje pro horníky, dřevorubce, zvýšit úrodnost obilných polí, urychlit práci pastevce.";

        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_1 = "Výhradní výměnné právo na obilí";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_2 = "Výměnné právo na obilí druhého stupně";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1 = "Výhradní výměnné právo na maso";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2 = "Výměnné právo na maso druhého stupně";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_1 = "Výhradní výměnné právo na kámen";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_2 = "Výměnné právo na kámen druhého stupně";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1 = "Výhradní výměnné právo na dřevo";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2 = "Výměnné právo na dřevo druhého stupně";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_1 = "Výhradní výměnné právo na rudu";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_2 = "Výměnné právo na obilí druhého rudu";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_1 = "S touto listinou můžeš měnit 3 obilí za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_2 = "S touto listinou můžeš měnit 2 obilí za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1 = "S touto listinou můžeš měnit 3 masa za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2 = "S touto listinou můžeš měnit 2 masa za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_1 = "S touto listinou můžeš měnit 3 stavební kameny za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_2 = "S touto listinou můžeš měnit 2 stavební kameny za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1 = "S touto listinou můžeš měnit 3 dřeva za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2 = "S touto listinou můžeš měnit 2 dřeva za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_1 = "S touto listinou můžeš měnit 3 rudy za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_2 = "S touto listinou můžeš měnit 2 rudy za jednu jinou surovinu.";

        public static string PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_CAPTURE = "Obsadit pole";
        public static string PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_DESTROY_HEXA = "Poničit pole";
        public static string PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_SOURCES = "Zničit suroviny";
        public static string PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_PARADE = "Armádní přehlídka";

        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_CAPTURE = "Obsazené pole nebude dávat nikomu suroviny. Při pokusu obsadit stejné pole 2x bude pole uvolněno.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_DESTROY_HEXA = "Poničí tebou vybrané pole. (obilné pole, hory, les, pastvinu, či lom) Pole bude vynášet polovinu oproti běžnému stavu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES = "Zničí polovinu surovin jednoho ze soupeřů.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_PARADE = "Armádní přehlídka ti přinese slávu a zisk 3 bodů.";
    }
}
