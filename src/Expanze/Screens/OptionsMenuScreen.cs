#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System.Xml;
#endregion

namespace Expanze
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry resolutionMenuEntry;
        MenuEntry fullscreenMenuEntry;
        MenuEntry languageMenuEntry;
        MenuEntry difficultyMenuEntry;
        MenuEntry apply;
        

        static string[] resolution = new string[Settings.allResolutions.Length];
        static string[] fullscreen = {Strings.Inst().GetString(TextEnum.MENU_COMMON_NO), Strings.Inst().GetString(TextEnum.MENU_COMMON_YES)};
        
        string[] languages;
        string[] languageCodes;

        static int currentResolution = 0;
        static bool isFullscreen = false;
        static int activeLanguage = 0;
        static int activeDifficulty = 0;

        #endregion

        #region Initialization

        private void fillResolutionsMenu() {
            int i = 0;
            foreach( Vector2 res in Settings.allResolutions) {
                resolution[i] = resolutionToString(res);
                if(res.X == Settings.activeResolution.X && res.Y == Settings.activeResolution.Y) {
                    currentResolution = i;
                }
                i++;
            }
        }

        public string resolutionToString(Vector2 res)
        {
            return res.X + "x" + res.Y;
        }

        public Vector2 resolutionToVector(string res)
        {
            string[] s = res.Split("x".ToCharArray());
            return new Vector2(int.Parse(s[0]),int.Parse(s[1]));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Nastavení")
        {
            fillResolutionsMenu();

            // Create our menu entries.
            difficultyMenuEntry = new MenuEntry(string.Empty);
            resolutionMenuEntry = new MenuEntry(string.Empty);
            fullscreenMenuEntry = new MenuEntry(string.Empty);
            languageMenuEntry = new MenuEntry(string.Empty);
            apply = new MenuEntry(Strings.Inst().GetString(TextEnum.MENU_OPTION_ACTIVATE_CHANGES));

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/texts.xml");
            XmlNodeList languageList = xDoc.GetElementsByTagName("language");
            languages = new string[languageList.Count];
            languageCodes = new string[languageList.Count];
            for(int loop1 = 0; loop1 < languageList.Count; loop1++)
            {
                languages[loop1] = languageList[loop1].FirstChild.InnerText;
                languageCodes[loop1] = languageList[loop1].LastChild.InnerText;

                if (languageCodes[loop1] == Strings.Inst().Language)
                    activeLanguage = loop1;
            }
            activeDifficulty = (int) Settings.difficulty;

            SetMenuEntryText();

            // Hook up menu event handlers.
            resolutionMenuEntry.Selected += ResolutionMenuEntrySelected;
            fullscreenMenuEntry.Selected += FullscreenMenuEntrySelected;
            difficultyMenuEntry.Selected += DifficultyMenuEntrySelected;
            languageMenuEntry.Selected += LanguageMenuEntrySelected;
            apply.Selected += ApplyChangesSelected;

            // Add entries to the menu.
            MenuEntries.Add(resolutionMenuEntry);
            MenuEntries.Add(fullscreenMenuEntry);
            MenuEntries.Add(difficultyMenuEntry);
            MenuEntries.Add(languageMenuEntry);
            MenuEntries.Add(apply);

            isFullscreen = Settings.isFullscreen;

            SetMenuEntryText();
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            resolutionMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_OPTION_RESOLUTION) + ": " + resolution[currentResolution];
            fullscreen[0] = Strings.Inst().GetString(TextEnum.MENU_COMMON_NO);
            fullscreen[1] = Strings.Inst().GetString(TextEnum.MENU_COMMON_YES);
            fullscreenMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_OPTION_FULLSCREEN) + ": " + fullscreen[(isFullscreen) ? 1 : 0];
            string[] difficulty = { Strings.Inst().GetString(TextEnum.MENU_OPTION_DIFFICULTY_EASY), Strings.Inst().GetString(TextEnum.MENU_OPTION_DIFFICULTY_NORMAL) };
            difficultyMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_OPTION_DIFFICULTY) + ": " + difficulty[activeDifficulty];
            languageMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_OPTION_LANGUAGE) + ": " + languages[activeLanguage];
            apply.Text = Strings.Inst().GetString(TextEnum.MENU_OPTION_ACTIVATE_CHANGES);
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void ApplyChangesSelected(object sender, PlayerIndexEventArgs e)
        {
            GameState.hotSeatScreen = new HotSeatScreen();

            string selected = resolution[currentResolution];
            bool fullscreen = isFullscreen;
            Strings.Inst().LoadTexts(languageCodes[activeLanguage], languages[activeLanguage]);
            SetMenuEntryText();
            GraphicsDeviceManager gdm = Settings.GraphicsDeviceManager;

            if (selected != resolutionToString(Settings.activeResolution))
            {
                if (fullscreen != Settings.isFullscreen)
                {
                    //fullscreen settings has changed
                    gdm.IsFullScreen = fullscreen;
                    Settings.isFullscreen = fullscreen;
                }

                //resolution was changed
                Vector2 newRes = resolutionToVector(selected);
                gdm.PreferredBackBufferWidth = (int)newRes.X;
                gdm.PreferredBackBufferHeight = (int)newRes.Y;
                Settings.activeResolution = newRes;
                Settings.scaleChange();
                gdm.ApplyChanges();
            }
            else if (fullscreen != Settings.isFullscreen)
            {
                //only fullscreen settings has changes

                gdm.IsFullScreen = fullscreen;
                Settings.isFullscreen = fullscreen;
                gdm.ApplyChanges();
            }

            Settings.difficulty = (Settings.Difficulty)activeDifficulty;
        }


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void ResolutionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentResolution++;

            if (currentResolution >= resolution.Length)
                currentResolution = 0;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void FullscreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            isFullscreen = !isFullscreen;
            SetMenuEntryText();
        }

        void DifficultyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            activeDifficulty++;
            string[] difficulty = { Strings.Inst().GetString(TextEnum.MENU_OPTION_DIFFICULTY_EASY), Strings.Inst().GetString(TextEnum.MENU_OPTION_DIFFICULTY_NORMAL) };
            if (activeDifficulty >= difficulty.Length)
                activeDifficulty = 0;

            SetMenuEntryText();
        }

        void LanguageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            activeLanguage++;
            if (activeLanguage >= languages.Length)
                activeLanguage = 0;

            languageMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_OPTION_LANGUAGE) + ": " + languages[activeLanguage];
        }

        #endregion
    }
}
