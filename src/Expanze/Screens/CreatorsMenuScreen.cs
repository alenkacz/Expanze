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

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CreatorsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            alenaMenuEntry = new MenuEntry(string.Empty);
            lukasMenuEntry = new MenuEntry(string.Empty);
            pavlaMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Zpět");

            // Hook up menu event handlers.
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(lukasMenuEntry);
            MenuEntries.Add(alenaMenuEntry);
            MenuEntries.Add(pavlaMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            pavlaMenuEntry.Text = "Lukáš Beran - teamleader";
            alenaMenuEntry.Text = "Alena Varkočková - programátorka";
            lukasMenuEntry.Text = "Pavla Balíková - grafička";
        }


        #endregion

        #region Handle Input



        #endregion
    }
}
