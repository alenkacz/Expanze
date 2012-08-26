using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;
using System.Xml;
using Expanze.Utils.Genetic;

namespace Expanze
{
    public enum GraphicsQuality {
        LOW_GRAPHICS,
        MEDIUM_GRAPHICS,
        HIGH_GRAPHICS
    }

    static class Settings
    {
        public static Vector2[] allResolutions = { new Vector2(800, 600), new Vector2(1024, 768), new Vector2(1280, 800), new Vector2(1366, 768), new Vector2(1440, 900) };
        public static Vector2 maximumResolution = new Vector2(1440,900);

        /// indexed by enum Types
        public static String[] hexaSrcPath = new String[] { "Models/yellowhex", "Models/greenhex", "Models/greyhex", "Models/brownhex", "Models/redhex", "Models/orangehex", "Models/simplehex" };

        /// resources which get player on start
        public static SourceAll startResources = new SourceAll(100);

        /// costs of infrastructure
        public static SourceAll costTown = new SourceAll(60, 60, 30, 60, 30);
        public static SourceAll costRoad = new SourceAll(0, 0, 50, 0, 50);
        public static SourceAll costMine = new SourceAll(0, 50, 50, 0, 0);
        public static SourceAll costSaw = new SourceAll(0, 50, 0, 0, 50);
        public static SourceAll costMill = new SourceAll(0, 0, 50, 50, 0);
        public static SourceAll costStephard = new SourceAll(50, 0, 0, 0, 50);
        public static SourceAll costQuarry = new SourceAll(50, 0, 0, 50, 0);
        public static SourceAll costFort = new SourceAll(50, 50, 0, 50, 0);
        public static SourceAll costMarket = new SourceAll(30, 100, 0, 0, 0);
        public static SourceAll costMonastery = new SourceAll(60, 0, 50, 20, 0);
        // costs of actions
        public static SourceAll costFortParade = new SourceAll(50, 50, 50, 80, 0);
        public static SourceAll costFortCrusade = new SourceAll(80, 80, 0, 80, 0);
        public static SourceAll costFortSources = new SourceAll(0, 0, 0, 100, 50);
        public static SourceAll costFortCapture = new SourceAll(80, 80, 0, 50, 50);
        // costs of market upgrades
        public static SourceAll costMarketCorn1 = new SourceAll(90, 0, 0, 0, 0);
        public static SourceAll costMarketCorn2 = new SourceAll(60, 60, 0, 0, 0);
        public static SourceAll costMarketMeat1 = new SourceAll(0, 90, 0, 0, 0);
        public static SourceAll costMarketMeat2 = new SourceAll(0, 60, 0, 0, 60);
        public static SourceAll costMarketStone1 = new SourceAll(0, 0, 90, 0, 0);
        public static SourceAll costMarketStone2 = new SourceAll(60, 0, 60, 0, 0);
        public static SourceAll costMarketWood1 = new SourceAll(0, 0, 0, 90, 0);
        public static SourceAll costMarketWood2 = new SourceAll(0, 0, 60, 60, 0);
        public static SourceAll costMarketOre1 = new SourceAll(0, 0, 0, 0, 90);
        public static SourceAll costMarketOre2 = new SourceAll(0, 0, 0, 60, 60);
        // costs of monastery upgrades
        public static SourceAll costMonasteryCorn1 = new SourceAll(0, 0, 40, 40, 0);
        public static SourceAll costMonasteryCorn2 = new SourceAll(40, 40, 0, 0, 40);
        public static SourceAll costMonasteryMeat1 = new SourceAll(40, 0, 0, 0, 40);
        public static SourceAll costMonasteryMeat2 = new SourceAll(0, 40, 40, 40, 0);
        public static SourceAll costMonasteryStone1 = new SourceAll(40, 0, 0, 40, 0);
        public static SourceAll costMonasteryStone2 = new SourceAll(0, 40, 40, 0, 40);
        public static SourceAll costMonasteryWood1 = new SourceAll(0, 40, 0, 0, 40);
        public static SourceAll costMonasteryWood2 = new SourceAll(40, 0, 40, 40, 0);
        public static SourceAll costMonasteryOre1 = new SourceAll(0, 40, 40, 0, 0);
        public static SourceAll costMonasteryOre2 = new SourceAll(40, 0, 0, 40, 40);

        public static double captureFreeChance = 0.25;
        // goals
        public static int maxTurn = 50;
        public static int[] winPoints = {10, 15, 20};
        public static int pointsTown = 0; /// points for new town
        public static int pointsRoad = 1; /// points for new road
        public static int pointsFort = 0; /// points for new fort
        public static int pointsMonastery = 0; /// points for new monastery
        public static int pointsMarket = 0;    /// points for new market
        public static int pointsMedal = 0;     /// points for medal
        public static int pointsFortParade = 0; /// points for parade from fort
        public static int pointsUpgradeLvl1 = 0;
        public static int pointsMarketLvl1 = 0;
        public static int pointsUpgradeLvl2 = 0;
        public static int pointsMarketLvl2 = 0;
        public static int pointsMill = 0;
        public static int pointsStepherd = 0;
        public static int pointsSaw = 0;
        public static int pointsQuarry = 0;
        public static int pointsMine = 0;
        public static int pointsFortCapture = 0;
        public static int pointsFortSteal = 0;
        public static int pointsOre = 0;
        public static int pointsCorn = 0;
        public static int pointsMeat = 0;
        public static int pointsStone = 0;
        public static int pointsWood = 0;
        public static List<int> goalRoad;
        public static List<int> goalTown;
        public static int goalRoadID = 0;
        public static int goalTownID = 0;

        public static bool banRandomEvents = false;
        public static bool banChangeSources = false;
        public static bool banMonastery = false;
        public static bool banFort = false;
        public static bool banMarket = false;
        public static bool banSecondLicence = false;
        public static bool banSecondUpgrade = false;
        public static bool banFortCrusade = false;
        public static bool banFortParade = false;
        public static bool banFortCaptureHexa = false;
        public static bool banFortStealSources = false;

        public static GraphicsQuality graphics = GraphicsQuality.HIGH_GRAPHICS;
        public static Color colorHoverItem = Color.BurlyWood;
        public static Color colorHovorCorner = Color.AntiqueWhite;
        public static Color colorPassiveItem = Color.White;
        public static Color colorDisableItem = Color.LightSlateGray;
        public static Color colorMainText = Color.AntiqueWhite;
        public static Color colorMenuText = Color.BurlyWood;

        public static Vector2 activeResolution = new Vector2(800,600);

        public static float getScale()
        {
            return activeResolution.X / maximumResolution.X;
        }

        public static GraphicsDeviceManager GraphicsDeviceManager = null;
        public static Game Game = null;

        public static bool isFullscreen = false;
        public static Dictionary<String, int[][]> hero;

        public static Matrix spriteScale = Matrix.CreateScale(Settings.activeResolution.X / Settings.maximumResolution.X, Settings.activeResolution.Y / Settings.maximumResolution.Y, 1);

        public static void scaleChange()
        {
            spriteScale = Matrix.CreateScale(Settings.activeResolution.X / Settings.maximumResolution.X, Settings.activeResolution.Y / Settings.maximumResolution.Y, 1);
        }

        public static Vector2 scale(Vector2 size)
        {
            return new Vector2(size.X * spriteScale.M11,size.Y * spriteScale.M22);
        }

        public static int scaleW(float w)
        {
            return (int)(w * spriteScale.M11);
        }

        public static int UnScaleW(float w)
        {
            return (int)(w / spriteScale.M11);
        }

        public static int scaleH(float h)
        {
            return (int)(h * spriteScale.M22);
        }

        public static int UnScaleH(float h)
        {
            return (int)(h / spriteScale.M22);
        }

        //position of player name on the gamescreen
        public static Vector2 playerNamePosition = new Vector2(Settings.maximumResolution.X - scaleW(500), scaleH(15));
        public static Vector2 playerColorPosition = new Vector2(Settings.maximumResolution.X - scaleW(200), scaleH(28));
        public static Vector2 playerColorSize = new Vector2(scaleW(50), scaleH(50));

        //possible colors 
        public static List<Color> playerColors = new List<Color> { Color.Red, Color.Blue, Color.Yellow, Color.White, Color.Green, Color.DarkMagenta };
        public static List<string> playerColorNames = new List<string> { "Red", "Blue", "Yellow", "White", "Green", "DarkMagenta" };
    
        //possible game modes
        public static List<String> PlayerState = new List<String>
        {
            "Player"
        };

        // help vars
        public static int activeTown = 0;
        public static int activeRoad = 0;
        public static int activeHexa = 0;
        public static string playerName = "Victoria Murden";

        public static void LoadAllSettings()
        {
            LoadSettings();
            LoadHeroes();
        }

        public static void SaveSettings()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/settings.xml");

            xDoc.GetElementsByTagName("fullscreen")[0].InnerText = (Settings.isFullscreen) ? "true" : "false";
            xDoc.GetElementsByTagName("screen-width")[0].InnerText = activeResolution.X + "";
            xDoc.GetElementsByTagName("screen-height")[0].InnerText = activeResolution.Y + "";

            xDoc.GetElementsByTagName("language-code")[0].InnerText = Strings.Inst().Language;
            xDoc.GetElementsByTagName("language-name")[0].InnerText = Strings.Inst().LanguageName;

            xDoc.Save("Content/Maps/settings.xml");
        }

        private static void LoadSettings()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/settings.xml");

            Settings.isFullscreen = xDoc.GetElementsByTagName("fullscreen")[0].InnerText == "true";
            int screenWidth = Convert.ToInt32(xDoc.GetElementsByTagName("screen-width")[0].InnerText);
            int screenHeight = Convert.ToInt32(xDoc.GetElementsByTagName("screen-height")[0].InnerText);
            Settings.activeResolution = new Vector2(screenWidth, screenHeight);
            scaleChange();

            Strings.Inst().LoadTexts(xDoc.GetElementsByTagName("language-code")[0].InnerText, xDoc.GetElementsByTagName("language-name")[0].InnerText);
        }

        private static void LoadHeroes()
        {
            hero = new Dictionary<string, int[][]>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/heroes.xml");

            XmlNodeList heroes = xDoc.GetElementsByTagName("hero");
            string defaultPersonality = "";

            foreach (XmlNode heroNode in heroes)
            {
                string name = heroNode.FirstChild.InnerText;
                string personality = heroNode.LastChild.InnerText;
                if (defaultPersonality.Length == 0)
                    defaultPersonality = personality;
                if (personality.Length == 0)
                    personality = defaultPersonality;

                hero.Add(name, Genetic.ParsePersonality(personality));
            }
        }
    }
}
