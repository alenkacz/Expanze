#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
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
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry hotseatMenuEntry = new MenuEntry(Strings.MENU_MAIN_HOT_SEAT);
            MenuEntry quickMenuEntry = new MenuEntry(Strings.MENU_MAIN_QUICK_GAME);
            MenuEntry settingsMenuEntry = new MenuEntry(Strings.MENU_MAIN_OPTION);
            MenuEntry creatorsMenuEntry = new MenuEntry(Strings.MENU_MAIN_CREATORS);
            MenuEntry exitMenuEntry = new MenuEntry(Strings.MENU_MAIN_EXIT);

            // Hook up menu event handlers.
            hotseatMenuEntry.Selected += HotseatMenuEntrySelected;
            quickMenuEntry.Selected += QuickMenuEntrySelected;
            settingsMenuEntry.Selected += SettingsMenuEntrySelected;
            creatorsMenuEntry.Selected += CreatorsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(quickMenuEntry);
            MenuEntries.Add(hotseatMenuEntry);
            MenuEntries.Add(settingsMenuEntry);
            MenuEntries.Add(creatorsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void HotseatMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // clearing all players in case of several game in one program launch

            if (GameState.hotSeatScreen == null)
                GameState.hotSeatScreen = new HotSeatScreen();

            ScreenManager.AddScreen(GameState.hotSeatScreen, e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void QuickMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // reset to the default state
            GameMaster.Inst().PrepareQuickGame();

            // now is used for AI
            GameLoadScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(false));
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void SettingsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void CreatorsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CreatorsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            string message = Strings.MENU_COMMON_ARE_YOU_SURE;

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
