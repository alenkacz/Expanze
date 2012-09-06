#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Expanze.Utils.Music;
#endregion

namespace Expanze
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry(Strings.Inst().GetString(TextEnum.MENU_PAUSE_GAME_ITEM_RESUME));
            MenuEntry restartGameMenuEntry = new MenuEntry(Strings.Inst().GetString(TextEnum.MENU_PAUSE_GAME_ITEM_RESTART));
            MenuEntry quitGameMenuEntry = new MenuEntry(Strings.Inst().GetString(TextEnum.MENU_PAUSE_GAME_ITEM_QUIT_GAME));
            
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            resumeGameMenuEntry.Selected += ResumeGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
            restartGameMenuEntry.Selected += OnCancel;
            restartGameMenuEntry.Selected += RestartGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(restartGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);   
      

        }


        #endregion

        #region Handle Input

        protected override void  OnCancel(PlayerIndex playerIndex)
        {
 	         base.OnCancel(playerIndex);
             GameMaster.Inst().SetPaused(false);
        }
     

        void ResumeGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameMaster.Inst().SetPaused(false);
        }

        void RestartGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameMaster.Inst().RestartGame();
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            string message = Strings.Inst().GetString(TextEnum.MENU_PAUSE_GAME_ARE_YOU_SURE);

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            MusicManager.Inst().PlaySong(SongEnum.menu);
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(0.7f);
            base.Draw(gameTime);
        }


        #endregion
    }
}
