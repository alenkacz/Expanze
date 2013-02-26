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
    class CreatorsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry alenaMenuEntry;
        MenuEntry lukasMenuEntry;
        MenuEntry pavlaMenuEntry;
        MenuEntry vaclavMenuEntry;
        MenuEntry adamMenuEntry;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CreatorsMenuScreen()
            : base(Strings.Inst().GetString(TextEnum.MENU_OPTION_TITLE))
        {
            // Create our menu entries.
            alenaMenuEntry = new MenuEntry(string.Empty);
            pavlaMenuEntry = new MenuEntry(string.Empty);
            lukasMenuEntry = new MenuEntry(string.Empty);
            vaclavMenuEntry = new MenuEntry(string.Empty);
            adamMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry(Strings.Inst().GetString(TextEnum.MENU_COMMON_BACK));

            // Hook up menu event handlers.
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(lukasMenuEntry);
            MenuEntries.Add(alenaMenuEntry);
            MenuEntries.Add(pavlaMenuEntry);
            MenuEntries.Add(vaclavMenuEntry);
            MenuEntries.Add(adamMenuEntry);
            //MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            lukasMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_CREATORS_LUKAS);
            alenaMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_CREATORS_ALENA);
            pavlaMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_CREATORS_PAVLA);
            vaclavMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_CREATORS_VACLAV);
            adamMenuEntry.Text = Strings.Inst().GetString(TextEnum.MENU_CREATORS_ADAM);
        }


        #endregion
    }
}
