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


        static string[] resolution = new string[Settings.allResolutions.Length];
        static string[] fullscreen = {Strings.MENU_COMMON_NO, Strings.MENU_COMMON_YES};

        static int currentResolution = 0;
        static bool isFullscreen = false;

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
            resolutionMenuEntry = new MenuEntry(string.Empty);
            fullscreenMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry apply = new MenuEntry(Strings.MENU_OPTION_ACTIVATE_CHANGES);
            MenuEntry back = new MenuEntry(Strings.MENU_COMMON_BACK);

            // Hook up menu event handlers.
            resolutionMenuEntry.Selected += ResolutionMenuEntrySelected;
            fullscreenMenuEntry.Selected += FullscreenMenuEntrySelected;
            apply.Selected += ApplyChangesSelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(resolutionMenuEntry);
            MenuEntries.Add(fullscreenMenuEntry);
            MenuEntries.Add(apply);
            MenuEntries.Add(back);

            isFullscreen = Settings.isFullscreen;

            SetMenuEntryText();
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            resolutionMenuEntry.Text = Strings.MENU_OPTION_RESOLUTION + ": " + resolution[currentResolution];
            fullscreenMenuEntry.Text = Strings.MENU_OPTION_FULLSCREEN + ": " + fullscreen[(isFullscreen) ? 1 : 0];
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void ApplyChangesSelected(object sender, PlayerIndexEventArgs e)
        {
            string selected = resolution[currentResolution];
            bool fullscreen = isFullscreen;
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

        #endregion
    }
}
