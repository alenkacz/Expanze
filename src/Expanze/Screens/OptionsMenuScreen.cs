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


        static string[] resolution = new string[Settings.allResolutions.Length];

        static int currentResolution = 0;

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
            : base("Options")
        {
            fillResolutionsMenu();

            // Create our menu entries.
            resolutionMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry apply = new MenuEntry("Apply Changes");
            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            resolutionMenuEntry.Selected += UngulateMenuEntrySelected;
            apply.Selected += ApplyChangesSelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(resolutionMenuEntry);
            MenuEntries.Add(apply);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            resolutionMenuEntry.Text = "Resolution: " + resolution[currentResolution];
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void ApplyChangesSelected(object sender, PlayerIndexEventArgs e)
        {
            string selected = resolution[currentResolution];

            if (selected != resolutionToString(Settings.activeResolution))
            {
                //resolution was changed
                GraphicsDeviceManager gdm = Settings.GraphicsDeviceManager;
                Vector2 newRes = resolutionToVector(selected);
                gdm.PreferredBackBufferWidth = (int)newRes.X;
                gdm.PreferredBackBufferHeight = (int)newRes.Y;
                Settings.activeResolution = newRes;
                gdm.ApplyChanges();
            }
        }


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void UngulateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentResolution++;

            if (currentResolution >= resolution.Length)
                currentResolution = 0;

            SetMenuEntryText();
        }


        #endregion
    }
}
