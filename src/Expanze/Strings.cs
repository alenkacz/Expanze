using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    static class Strings
    {

        // Menu strings
        public static string MENU_COMMON_BACK = "Zpět";
        public static string MENU_COMMON_ARE_YOU_SURE = "Jste si jisti, že chcete hru ukončit?";
        public static string MENU_COMMON_YES = "Ano";
        public static string MENU_COMMON_NO = "Ne";

        public static string MENU_MAIN_HOT_SEAT = "Hot seat";
        public static string MENU_MAIN_QUICK_GAME = "Rychlá hra";
        public static string MENU_MAIN_OPTION = "Nastavení";
        public static string MENU_MAIN_CREATORS = "Autoři";
        public static string MENU_MAIN_EXIT = "Konec";

        public static string MENU_HOT_SEAT_NO_AI = "Hráč";
        public static string[] MENU_HOT_SEAT_NAMES = { "Pedro de Mendoza", "Raimundus Lullus", "Hernando de Soto", "Francisco Pizarro", "Diego de Almagro", "Juan de la Cosa" };
        public static string MENU_HOT_SEAT_POINTS = "Počet bodů";
        public static string MENU_HOT_SEAT_MAP_TYPE = "Druh mapy";
        public static string MENU_HOT_SEAT_MAP_SIZE = "Velikost mapy";
        public static string MENU_HOT_SEAT_MAP_WEALTH = "Bohatství surovin";

        public static string MENU_PAUSE_GAME_ITEM_RESUME = "Zpět do hry";
        public static string MENU_PAUSE_GAME_ITEM_QUIT_GAME = "Ukončit hru";
        public static string MENU_PAUSE_GAME_ARE_YOU_SURE = "Opravdu chcete ukončit hru?";

        public static string MENU_LOADING_LOADING = "Načítání obsahu";

        public static string MENU_GAME_LOADING_TITLE = "Načítání";
        public static string MENU_GAME_LOADING_HUD = "textur";
        public static string MENU_GAME_LOADING_HEXAS = "hex";
        public static string MENU_GAME_LOADING_BUILDINGS = "budov těžby";
        public static string MENU_GAME_LOADING_SPECIAL_BUILDINGS = "speciálních budov";
        public static string MENU_GAME_LOADING_MAP = "mapy";

        public static string MENU_OPTION_TITLE = "Nastavení";
        public static string MENU_OPTION_ACTIVATE_CHANGES = "Aktivovat změny";
        public static string MENU_OPTION_RESOLUTION = "Rozlišení";
        public static string MENU_OPTION_FULLSCREEN = "Fullscreen";

        public static string MENU_CREATORS_LUKAS = "Lukáš Beran - teamleader, programátor";
        public static string MENU_CREATORS_ALENA = "Alena Varkočková - programátorka";
        public static string MENU_CREATORS_PAVLA = "Pavla Balíková - grafička";

        public static string GAME_SETTINGS_MAP_SIZE_SMALL = "Malá";
        public static string GAME_SETTINGS_MAP_SIZE_MEDIUM = "Střední";
        public static string GAME_SETTINGS_MAP_SIZE_BIG = "Velká";
        public static string GAME_SETTINGS_MAP_WEALTH_LOW = "Nízké";
        public static string GAME_SETTINGS_MAP_WEALTH_MEDIUM = "Střední";
        public static string GAME_SETTINGS_MAP_WEALTH_HIGH = "Vysoké";
        public static string GAME_SETTINGS_MAP_TYPE_NORMAL = "Normální";
        public static string GAME_SETTINGS_MAP_TYPE_LOWLAND = "Nížiny";
        public static string GAME_SETTINGS_MAP_TYPE_WASTELAND = "Pustina";

        public static string GAME_ALERT_TITLE_GAME_STARTED = "Expanze začíná!";
        public static string GAME_ALERT_DESCRIPTION_GAME_STARTED = "Rozestavění prvních měst je u konce, boj o území může začít.";
        public static string GAME_ALERT_TITLE_AI_EXCEPTION = "O jednoho míň";
        public static string GAME_ALERT_DESCRIPTION_AI_EXCEPTION = "vzdává hru a již dále nebude pokračovat v expanzi ostrova.";

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
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MINE = "Pořídíš-li horníkům nové vozíky, budou pracovat o 50% lépe.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_2_MINE = "Nové helmy";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MINE = "Horníci s novými helmami nebudou tak často na marodce. Zisky rudy o 100% lepší.";

        public static string PROMT_TITLE_WANT_TO_BUILD_QUARRY = "Kamenný lom";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_QUARRY = "Horníci budou těžit stavební kámen.  Po vynalezení pokroku z Kláštera jim to půjde líp.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_1_QUARRY = "Lepší krumpáče";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_QUARRY = "Pořídíš-li horníkům lepší krumpáče, budou pracovat o 50% lépe.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_2_QUARRY = "Trhavina";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_QUARRY = "Horníci s trhavinou dovedou divy. Zisky kamene o 100% lepší.";
       
        public static string PROMT_TITLE_WANT_TO_BUILD_SAW = "Pila";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_SAW = "Dřevorubci natahají z lesa dřevo.  Po vynalezení pokroku z Kláštera bude těžba účinější.";
        public static string PROMPT_TITLE_WANT_TO_UPGRADE_1_SAW = "Sekery";
        public static string PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_SAW = "Pořídíš-li dřevorubcům sekery, budou pracovat o 50% lépe.";
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
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_FORT = "Z pevnosti můžeš poslat vojsko zničit suroviny protihráči, nebo získat body za vojenskou přehlídku.";
        public static string PROMPT_TITLE_WANT_TO_BUILD_MARKET = "Tržiště";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_MARKET = "Na tržišti si můžeš koupit lepší směnný kurz pro výměnu surovin. Chceš-li měnit tři ku jedné, či dva ku jedné, tržiště je jasná volba.";
        public static string PROMPT_TITLE_WANT_TO_BUILD_MONASTERY = "Klášter";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUILD_MONASTERY = "Mniši můžou vynaleznout lepší nástroje pro horníky, dřevorubce, zvýšit úrodnost obilných polí, urychlit práci pastevce.";

        public static string PROMPT_DESCRIPTION_ALL_UPGRADES_INVENTED = "Vynalezl jsi již všechny dostupné pokroky. Nyní můžeš vybavit novými vynálezy své budovy a tím zlepšit jejich efektivitu.";
        public static string PROMPT_DESCRIPTION_ALL_LICENCES_BOUGHT = "Koupil jsi již všechny dostupné licence umožňující výhodnější směnu jednotlivých surovin. Je na čase toho využít a vyhrát!";
        public static string PROMPT_DESCRIPTION_ALL_UPGRADES_USED = "S dvěma vylepšeními ti vynáší tato budova dvojnásobek oproti základní budově. Budovu již nelze dále vylepšovat.";

        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_1 = "Výhradní výměnné právo na obilí";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_2 = "Výměnné právo na obilí druhého stupně";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1 = "Výhradní výměnné právo na ovce";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2 = "Výměnné právo na ovce druhého stupně";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_1 = "Výhradní výměnné právo na kámen";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_2 = "Výměnné právo na kámen druhého stupně";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1 = "Výhradní výměnné právo na dřevo";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2 = "Výměnné právo na dřevo druhého stupně";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_1 = "Výhradní výměnné právo na rudu";
        public static string PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_2 = "Výměnné právo na obilí druhého rudu";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_1 = "S touto listinou můžeš měnit 3 obilí za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_2 = "S touto listinou můžeš měnit 2 obilí za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1 = "S touto listinou můžeš měnit 3 ovce za jednu jinou surovinu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2 = "S touto listinou můžeš měnit 2 ovce za jednu jinou surovinu.";
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

        public static string PROMPT_DESCTIPTION_MESSAGE_FORT_ACTION_PARADE = "Předvedl jsi majestátnou přehlídku, těžce se bude překonávat. Získal jsi 3 body.";

        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_CAPTURE = "Obsazené pole nebude dávat nikomu suroviny. Při pokusu obsadit stejné pole 2x bude pole uvolněno.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_DESTROY_HEXA = "Poničí tebou vybrané pole. (obilné pole, hory, les, pastvinu, či lom) Pole bude vynášet polovinu oproti běžnému stavu.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES = "Zničí polovinu surovin jednoho ze soupeřů.";
        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_PARADE = "Armádní přehlídka ti přinese slávu a zisk 3 bodů.";

        public static string PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES_CHOISING_PLAYER = "Chceš aby tento hráč přišel o své suroviny? Zvědové zjistili, že má tolik surovin :";
    
        // EVENTS //

        // DISASTERS //

        public static string MESSAGE_TITLE_DISASTER = "Pohroma";
        public static string MESSAGE_DESCRIPTION_DISASTER_CORNFIELD = "Zemi postihla extrémní sucha. Výnosy z polí budou poloviční.";
        public static string MESSAGE_DESCRIPTION_DISASTER_PASTURE = "Vlci napadli stádo ovcí. Pastevci ti jich budou dodávat o polovinu méně.";
        public static string MESSAGE_DESCRIPTION_DISASTER_STONE = "Sesuvy kamení dělají práci nebezpečnou a horníci nepracují. Těžba bude poloviční.";
        public static string MESSAGE_DESCRIPTION_DISASTER_FOREST = "Požár zachvátil lesy v zemi. Těžba dřeva musí být omezena na polovinu.";
        public static string MESSAGE_DESCRIPTION_DISASTER_MOUNTAINS = "V dolech straší a horníci se bojí. Budou těžit jen ti nejodvážnější. Zisky poloviční.";

        // MIRACLES //

        public static string MESSAGE_DESCRIPTION_MIRACLE_CORNFIELD = "Teplé počasí polím přálo, obilí rostlo mnohem víc, rovnou o polovinu víc.";
        public static string MESSAGE_DESCRIPTION_MIRACLE_PASTURE = "Ovcím se narodilo spoustu mláďat, bude jich o polovinu více.";
        public static string MESSAGE_DESCRIPTION_MIRACLE_STONE = "Nikdo neví, co se stalo. Těžba jde teď mnohem lépe, je zrychlena o polovinu.";
        public static string MESSAGE_DESCRIPTION_MIRACLE_FOREST = "Silný vítr vyvrátil mnoho stromů. Těžba dřeva bude rychlejší o polovinu.";
        public static string MESSAGE_DESCRIPTION_MIRACLE_MOUNTAINS = "Nalezeny nové žíly, může se těžit naplno. Zisky rud budou o polovinu vyšší.";

        public static string MESSAGE_TITLE_MIRACLE = "Zázrak";

        // Medals //
        public static string MESSAGE_TITLE_MEDAL_TOWN = "Medaile za expanzi";
        public static string MESSAGE_TITLE_MEDAL_ROAD = "Medaile cestovatele";
        public static string MESSAGE_TITLE_MEDAL_MARKET = "Medaile obchodníka";
        public static string MESSAGE_TITLE_MEDAL_FORT = "Medaile válečníka";
        public static string MESSAGE_TITLE_MEDAL_MONASTERY = "Medaile učence";
        public static string MESSAGE_TITLE_MEDAL_SAW = "Medaile pily";
        public static string MESSAGE_TITLE_MEDAL_MILL = "Medaile mlýna";
        public static string MESSAGE_TITLE_MEDAL_QUARRY = "Medaile lomu";
        public static string MESSAGE_TITLE_MEDAL_STEPHERD = "Medaile pastevce";
        public static string MESSAGE_TITLE_MEDAL_MINE = "Medaile dolu";

        public static string MESSAGE_DESCRIPTION_MEDAL_TOWN = "Získá ji ten, který má nejvíce měst, avšak minimálně 5.";
        public static string MESSAGE_DESCRIPTION_MEDAL_ROAD = "Získá ji ten, jenž postaví nejvíce cest, avšak nejméně 10.";
        public static string MESSAGE_DESCRIPTION_MEDAL_MARKET = "Získá ji ten, kdo alespoň 3 tržiště má a pokud nikdo jiný jich nemá víc.";
        public static string MESSAGE_DESCRIPTION_MEDAL_FORT = "Máš-li alespoň tři pevnosti a zároveň víc pevností než kdokoliv jiný, medaile je tvá.";
        public static string MESSAGE_DESCRIPTION_MEDAL_MONASTERY = "Potřebuješ postavit 3 kláštery a mít jich víc než tví soupeři.";
        public static string MESSAGE_DESCRIPTION_MEDAL_SAW = "Postav víc pil než ostatní, avšak minimálně tři a tato medaile je tvá.";
        public static string MESSAGE_DESCRIPTION_MEDAL_MILL = "Postav víc mlýnů než ostatní, avšak minimálně tři a tato medaile je tvá.";
        public static string MESSAGE_DESCRIPTION_MEDAL_QUARRY = "Postav víc lomů než ostatní, avšak minimálně tři a tato medaile je tvá.";
        public static string MESSAGE_DESCRIPTION_MEDAL_STEPHERD = "Postav víc chatrčí pastevce než ostatní, avšak minimálně tři a tato medaile je tvá.";
        public static string MESSAGE_DESCRIPTION_MEDAL_MINE = "Postav víc dolů na rudu než ostatní, avšak minimálně tři a tato medaile je tvá.";
    }
}
