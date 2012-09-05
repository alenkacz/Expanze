using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Expanze
{
    public enum TextEnum
    {
        // Menu strings
        MENU_COMMON_BACK,
        MENU_COMMON_ARE_YOU_SURE,
        MENU_COMMON_YES,
        MENU_COMMON_NO,

        MENU_MAIN_CAMPAIGN,
        MENU_MAIN_CONTINUE,
        MENU_MAIN_HOT_SEAT,
        MENU_MAIN_QUICK_GAME,
        MENU_MAIN_OPTION,
        MENU_MAIN_CREATORS,
        MENU_MAIN_EXIT,

        MENU_HOT_SEAT_NO_AI,
        MENU_HOT_SEAT_AI,
        MENU_HOT_SEAT_MAP_TYPE,
        MENU_HOT_SEAT_MAP_SOURCE,
        MENU_HOT_SEAT_MAP_SECRET_PRODUCTIVITY,
        MENU_HOT_SEAT_MAP_SECRET_KIND,

        MENU_PAUSE_GAME_ITEM_RESUME,
        MENU_PAUSE_GAME_ITEM_QUIT_GAME,
        MENU_PAUSE_GAME_ITEM_RESTART,
        MENU_PAUSE_GAME_ARE_YOU_SURE,

        MENU_LOADING_LOADING,

        MENU_GAME_LOADING_TITLE,
        MENU_GAME_LOADING_HUD,
        MENU_GAME_LOADING_HEXAS,
        MENU_GAME_LOADING_BUILDINGS,
        MENU_GAME_LOADING_SPECIAL_BUILDINGS,
        MENU_GAME_LOADING_MAP,

        MENU_OPTION_TITLE,
        MENU_OPTION_ACTIVATE_CHANGES,
        MENU_OPTION_RESOLUTION,
        MENU_OPTION_FULLSCREEN,
        MENU_OPTION_LANGUAGE,
        MENU_OPTION_DIFFICULTY,
        MENU_OPTION_DIFFICULTY_EASY,
        MENU_OPTION_DIFFICULTY_NORMAL,

        MENU_CREATORS_LUKAS,
        MENU_CREATORS_ALENA,
        MENU_CREATORS_PAVLA,

        MENU_GRAPH_POINTS,
        MENU_GRAPH_TOWNS,
        MENU_GRAPH_ROADS,
        MENU_GRAPH_MEDALS,
        MENU_GRAPH_FORT,
        MENU_GRAPH_MONASTERY,
        MENU_GRAPH_MARKET,
        MENU_GRAPH_LICENCE,
        MENU_GRAPH_UPGRADE,
        MENU_GRAPH_ACTION,
        MENU_GRAPH_SUMSOURCES,

        GAME_SETTINGS_MAP_PRODUCTIVITY_HIDDEN,
        GAME_SETTINGS_MAP_PRODUCTIVITY_HALF,
        GAME_SETTINGS_MAP_PRODUCTIVITY_VISIBLE,
        GAME_SETTINGS_MAP_KIND_HIDDEN,
        GAME_SETTINGS_MAP_KIND_HALF,
        GAME_SETTINGS_MAP_KIND_VISIBLE,
        GAME_SETTINGS_MAP_SOURCE_NORMAL,
        GAME_SETTINGS_MAP_SOURCE_LOWLAND,
        GAME_SETTINGS_MAP_SOURCE_WASTELAND,
        GAME_SETTINGS_MAP_TYPE_ISLAND,
        GAME_SETTINGS_MAP_TYPE_2_ISLANDS,
        GAME_SETTINGS_MAP_TYPE_SMALL_ISLANDS,

        GAME_ALERT_TITLE_GAME_STARTED,
        GAME_ALERT_DESCRIPTION_GAME_STARTED,
        GAME_ALERT_TITLE_AI_EXCEPTION,
        GAME_ALERT_DESCRIPTION_AI_EXCEPTION,
        GAME_ALERT_TITLE_NEXT_TURN_BAD_STATE,
        GAME_ALERT_DESCRIPTION_NEXT_TURN_BAD_STATE,
        GAME_ALERT_TITLE_AI_IS_THINKING,
        GAME_ALERT_DESCRIPTION_AI_IS_THINKING,
        GAME_ALERT_TITLE_MARKET_BAD_TURN,
        GAME_ALERT_DESCRIPTION_MARKET_BAD_TURN,
        GAME_ALERT_TITLE_SOMEONE_STEAL,
        GAME_ALERT_DESCRIPTION_SOMEONE_STEAL1,
        GAME_ALERT_DESCRIPTION_SOMEONE_STEAL2,
        GAME_ALERT_TITLE_HUMAN_NEXT_TURN,
        GAME_ALERT_DESTRIPTION_HUMAN_NEXT_TURN,
        //
        ALERT_TITLE_THIS_IS_NOT_YOURS,
        ALERT_TITLE_NOT_ENOUGH_SOURCES,
        
        // Town building alerts
        ALERT_TITLE_TOWN_IS_BUILD,
        ALERT_TITLE_NO_ROAD_IS_CLOSE,
        ALERT_TITLE_OTHER_TOWN_IS_CLOSE,

        // Road building alerts
        ALERT_TITLE_ROAD_IS_BUILD,
        ALERT_TITLE_NO_ROAD_OR_TOWN_IS_CLOSE,

        // Special building alerts
        ALERT_TITLE_ALREADY_HAVE_SECOND_UPGRADE,
        ALERT_TITLE_NO_UPGRADE,
        ALERT_TITLE_MAX_UPGRADES,

        // Source buildings building
        HEXA_TRI,
        HEXA_DUO,
        HEXA_NAME_MOUNTAINS,
        HEXA_NAME_PASTURE,
        HEXA_NAME_STONE,
        HEXA_NAME_FOREST,
        HEXA_NAME_CORNFIELD,
        HEXA_NAME_DESERT,

        GOAL_CORN_PART1,
        GOAL_CORN_PART2,
        GOAL_MEAT_PART1,
        GOAL_MEAT_PART2,
        GOAL_STONE_PART1,
        GOAL_STONE_PART2,
        GOAL_WOOD_PART1,
        GOAL_WOOD_PART2,
        GOAL_ORE_PART1,
        GOAL_ORE_PART2,
        GOAL_TOWNID_PART1,
        GOAL_TOWNID_PART2,
        GOAL_ROADID_PART1,
        GOAL_ROADID_PART2,
        GOAL_TOWN_PART1,
        GOAL_TOWN_PART2,
        GOAL_ROAD_PART1,
        GOAL_ROAD_PART2,
        GOAL_MONASTERY_PART1,
        GOAL_MONASTERY_PART2,
        GOAL_FORT_PART1,
        GOAL_FORT_PART2,
        GOAL_MARKET_PART1,
        GOAL_MARKET_PART2,
        GOAL_STEPHERD_PART1,
        GOAL_STEPHERD_PART2,
        GOAL_MINE_PART1,
        GOAL_MINE_PART2,
        GOAL_SAW_PART1,
        GOAL_SAW_PART2,
        GOAL_MILL_PART1,
        GOAL_MILL_PART2,
        GOAL_QUARRY_PART1,
        GOAL_QUARRY_PART2,
        GOAL_UPGRADE1_PART1,
        GOAL_UPGRADE1_PART2,
        GOAL_UPGRADE2_PART1,
        GOAL_UPGRADE2_PART2,
        GOAL_LICENCE1_PART1,
        GOAL_LICENCE1_PART2,
        GOAL_LICENCE2_PART1,
        GOAL_LICENCE2_PART2,
        GOAL_FORT_PARADE_PART1,
        GOAL_FORT_PARADE_PART2,
        GOAL_FORT_CRUSADE_PART1,
        GOAL_FORT_CRUSADE_PART2,
        GOAL_FORT_STEAL_PART1,
        GOAL_FORT_STEAL_PART2,
        GOAL_FORT_CAPTURE_PART1,
        GOAL_FORT_CAPTURE_PART2,

        ALERT_TITLE_NOT_TOWN_OWNER,
        ALERT_TITLE_BUILDING_IS_BUILD,

        PROMT_TITLE_WANT_TO_BUILD_TOWN,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_TOWN,
        PROMT_TITLE_WANT_TO_BUILD_ROAD,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_ROAD,

        PROMT_TITLE_WANT_TO_BUILD_MINE,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_MINE,
        PROMPT_TITLE_WANT_TO_UPGRADE_1_MINE,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MINE,
        PROMPT_TITLE_WANT_TO_UPGRADE_2_MINE,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MINE,

        PROMT_TITLE_WANT_TO_BUILD_QUARRY,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_QUARRY,
        PROMPT_TITLE_WANT_TO_UPGRADE_1_QUARRY,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_QUARRY,
        PROMPT_TITLE_WANT_TO_UPGRADE_2_QUARRY,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_QUARRY,
       
        PROMT_TITLE_WANT_TO_BUILD_SAW,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_SAW,
        PROMPT_TITLE_WANT_TO_UPGRADE_1_SAW,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_SAW,
        PROMPT_TITLE_WANT_TO_UPGRADE_2_SAW,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_SAW,

        
        PROMT_TITLE_WANT_TO_BUILD_MILL,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_MILL,
        PROMPT_TITLE_WANT_TO_UPGRADE_1_MILL,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_MILL,
        PROMPT_TITLE_WANT_TO_UPGRADE_2_MILL,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_MILL,
      
        PROMT_TITLE_WANT_TO_BUILD_STEPHERD,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_STEPHERD,
        PROMPT_TITLE_WANT_TO_UPGRADE_1_STEPHERD,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_1_STEPHERD,
        PROMPT_TITLE_WANT_TO_UPGRADE_2_STEPHERD,
        PROMPT_DESCRIPTION_WANT_TO_UPGRADE_2_STEPHERD,

        PROMPT_TITLE_WANT_TO_BUILD_FORT,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_FORT,
        PROMPT_TITLE_WANT_TO_BUILD_MARKET,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_MARKET,
        PROMPT_TITLE_WANT_TO_BUILD_MONASTERY,
        PROMPT_DESCRIPTION_WANT_TO_BUILD_MONASTERY,

        PROMPT_DESCRIPTION_ALL_UPGRADES_INVENTED,
        PROMPT_DESCRIPTION_ALL_LICENCES_BOUGHT,
        PROMPT_DESCRIPTION_ALL_UPGRADES_USED,

        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_1,
        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_CORN_2,
        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1,
        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2,
        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_1,
        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_STONE_2,
        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1,
        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2,
        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_1,
        PROMPT_TITLE_WANT_TO_BUY_MARKET_UPGRADE_ORE_2,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_1,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_CORN_2,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_1,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_MEAT_2,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_1,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_STONE_2,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_1,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_WOOD_2,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_1,
        PROMPT_DESCRIPTION_WANT_TO_BUY_MARKET_UPGRADE_ORE_2,

        PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_CAPTURE,
        PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_CRUSADE,
        PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_SOURCES,
        PROMPT_TITLE_WANT_TO_BUY_FORT_ACTION_PARADE,

        PROMPT_DESCTIPTION_MESSAGE_FORT_ACTION_PARADE,

        PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_CAPTURE,
        PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_CRUSADE,
        PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES,
        PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_PARADE,

        PROMPT_DESCRIPTION_WANT_TO_BUY_FORT_ACTION_SOURCES_CHOISING_PLAYER,
    
        // EVENTS //

        // DISASTERS //

        MESSAGE_TITLE_DISASTER,
        MESSAGE_DESCRIPTION_DISASTER_CORNFIELD,
        MESSAGE_DESCRIPTION_DISASTER_PASTURE,
        MESSAGE_DESCRIPTION_DISASTER_STONE,
        MESSAGE_DESCRIPTION_DISASTER_FOREST,
        MESSAGE_DESCRIPTION_DISASTER_MOUNTAINS,

        // MIRACLES //

        MESSAGE_DESCRIPTION_MIRACLE_CORNFIELD,
        MESSAGE_DESCRIPTION_MIRACLE_PASTURE,
        MESSAGE_DESCRIPTION_MIRACLE_STONE,
        MESSAGE_DESCRIPTION_MIRACLE_FOREST,
        MESSAGE_DESCRIPTION_MIRACLE_MOUNTAINS,

        MESSAGE_TITLE_MIRACLE,

        MESSAGE_TITLE_MESSAGE_ON,
        MESSAGE_DESCRIPTION_MESSAGE_ON,

        // Medals //
        MESSAGE_TITLE_MEDAL_TOWN,
        MESSAGE_TITLE_MEDAL_ROAD,
        MESSAGE_TITLE_MEDAL_MARKET,
        MESSAGE_TITLE_MEDAL_FORT,
        MESSAGE_TITLE_MEDAL_MONASTERY,
        MESSAGE_TITLE_MEDAL_SAW,
        MESSAGE_TITLE_MEDAL_MILL,
        MESSAGE_TITLE_MEDAL_QUARRY,
        MESSAGE_TITLE_MEDAL_STEPHERD,
        MESSAGE_TITLE_MEDAL_MINE,

        MESSAGE_DESCRIPTION_MEDAL_TOWN,
        MESSAGE_DESCRIPTION_MEDAL_ROAD,
        MESSAGE_DESCRIPTION_MEDAL_MARKET,
        MESSAGE_DESCRIPTION_MEDAL_FORT,
        MESSAGE_DESCRIPTION_MEDAL_MONASTERY,
        MESSAGE_DESCRIPTION_MEDAL_SAW,
        MESSAGE_DESCRIPTION_MEDAL_MILL,
        MESSAGE_DESCRIPTION_MEDAL_QUARRY,
        MESSAGE_DESCRIPTION_MEDAL_STEPHERD,
        MESSAGE_DESCRIPTION_MEDAL_MINE,

        MESSAGE_TITLE_MARKET_NOT_SOURCES,
        MESSAGE_TITLE_MARKET_BUY_IT,
        MESSAGE_TITLE_MARKET_CHANGE_SOURCES,
        MESSAGE_DESCRIPTION_MARKET_NOT_SOURCES,
        MESSAGE_DESCRIPTION_MARKET_BUY_IT,
        MESSAGE_DESCRIPTION_MARKET_CHANGE_SOURCES,

        MESSAGE_TITLE_END_GAME,
        MESSAGE_DESCRIPTION_END_GAME_WIN,
        MESSAGE_DESCRIPTION_END_GAME_LOOSE1,
        MESSAGE_DESCRIPTION_END_GAME_LOOSE2,

        ERROR_NO_SOURCES,
        ERROR_NOT_ENOUGHT_FROM_SOURCE,
        ERROR_THERE_IS_NO_TOWN,
        ERROR_THERE_IS_NO_BUILDING,
        ERROR_INVALID_ROAD_ID,
        ERROR_INVALID_TOWN_ID,
        ERROR_NO_BUILDING_FOR_WATER,
        ERROR_INVALID_HEXA_ID,
        ERROR_NO_SOURCE_BUILDING_FOR_DESERT,
        ERROR_NO_SPECIAL_BUIDLING_FOR_MOUNTAINS,
        ERROR_BAN_SECOND_UPGRADE,
        ERROR_HAVE_SECOND_UPGRADE,
        ERROR_BAN_SECOND_LICENCE,
        ERROR_HAVE_SECOND_LICENCE,
        ERROR_MAX_UPGRADES,
        ERROR_MAX_LICENCES,
        ERROR_TOO_FAR_FROM_FORT,
        ERROR_NO_FORT,
        ERROR_BAN_FORT_CAPTURE_HEXA,
        ERROR_BAN_FORT_STEAL_SOURCES,
        ERROR_BAN_FORT_SHOW_PARADE,
        ERROR_BAN_MARKET,
        ERROR_BAN_MONASTERY,
        ERROR_BAN_FORT,
        ERROR_ALREADY_BUILD,
        ERROR_NO_OWNER,
        ERROR_OTHER_TOWN_IS_TOO_CLOSE,
        ERROR_NO_PLAYER_ROAD,
        ERROR_YOU_HAVE_BUILT_TOWN_THIS_TURN,
        ERROR_NO_PLAYER_ROAD_OR_TOWN,
        YOU_DONT_HAVE_FIRST_UPGRADE,

        MENU_VICTORY_SCREEN_GRAPH,
        MENU_VICTORY_SCREEN_STATISTIC_AFTER,
        MENU_VICTORY_SCREEN_TURN,

        TIP_GAME_1,
        TIP_GAME_2,
        TIP_GAME_3,
        TIP_GAME_4,
        TIP_GAME_5,
        TIP_GAME_6,
        TIP_GAME_7,
        TIP_GAME_8,
        TIP_GAME_9,
        TIP_GAME_10,
        COUNT
    }

    class Strings
    {
        private string[] playerNames =  { "Pedro de Mendoza", "Raimundus Lullus", "Hernando de Soto", "Francisco Pizarro", "Diego de Almagro", "Juan de la Cosa", "Francis Drake", "Willem Barents", "Willem Barents", "Vasco Núńez", "Abel Tasman", "Ibn Battúta", "Tolomeo Dias", "Kira Salak", "Frank Cole", "Michael Asher", "Robyn Davidson", "Lee Spence", "Rein Messner", "Robert Ballard", "Valentina Teresh", "Isabella Bird", "Xu Fu", "Dicuil", "Erik the Red", "Zheng he", "Piri Reis",
                                           "Luis Váez", "Samuel Champlain", "Carl Linnaeus", "Alessandro Malas", "Alex Humboldt" , "Mungo Park" , "Sacagawea", "Charles Wilkes", "John Rae" , "Otto Sverdrup", "Tom Crean", "Helen Thayer", "Jonê County", "Victoria Murden", "Emil Holub"}; 
        public string[] PlayerNames
        {
            get { return playerNames; }
        }

        private string[] texts;

        private string languageName;

        public string LanguageName
        {
            get { return languageName; }
            set { languageName = value; }
        }

        private string language;

        public string Language
        {
            get { return language; }
            set { language = value; }
        }
        private string defaultLanguage;

        public string DefaultLanguage
        {
            get { return defaultLanguage; }
            set { defaultLanguage = value; }
        }
        private static Strings instance = null;

        public static Strings Inst()
        {
            if (instance == null)
            {
                instance = new Strings();
            }

            return instance;
        }

        private Strings()
        {
            texts = new string[(int) TextEnum.COUNT];

            defaultLanguage = "cz";
            LoadTexts("eng", "English");
        }

        public void LoadTexts(string language, string languageName)
        {
            this.language = language;
            this.languageName = languageName;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/texts.xml");

            XmlNodeList textList = xDoc.GetElementsByTagName("text");
            foreach (XmlNode textNode in textList)
            {
                int id = (int) Enum.Parse(typeof(TextEnum), textNode.Attributes[0].InnerText, true);

                string bestText = null;
                string defaultText = null;

                foreach (XmlNode text in textNode)
                {
                    if(text.LocalName == language && text.InnerText.Length > 0)
                    {
                        bestText = text.InnerText;
                        break;
                    }
                    else if (text.LocalName == defaultLanguage && text.InnerText.Length > 0)
                    {
                        defaultText = text.InnerText;
                    } else if(defaultText == null)
                        defaultText = text.InnerText;
                }

                if (bestText != null)
                {
                    texts[id] = bestText;
                }
                else
                    texts[id] = defaultText;
            }
        }

        public string[] GetPlayerNames()
        {
            return playerNames;
        }

        public string GetString(TextEnum id)
        {
            return texts[(int) id];
        }
    }
}
