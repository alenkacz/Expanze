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
using System.IO;
using System.Xml;
using System.Collections.Generic;
#endregion

namespace Expanze
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class CampaignMenuScreen : MenuScreen
    {
        #region Initialization

        XmlDocument xDoc;
        Dictionary<string, string> mapnamesource;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public CampaignMenuScreen()
            : base("Campaign Menu")
        {
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Content\\Maps");
            FileInfo[] subFiles = di.GetFiles();
            xDoc = new XmlDocument();
            mapnamesource = new Dictionary<string, string>();
            MenuEntry levelMenuItem;
            if (subFiles.Length > 0)
            {
                foreach (FileInfo subFile in subFiles)
                {
                    string name = subFile.Name;
                    if(name.StartsWith("cam"))
                    {
                        xDoc.Load(subFile.FullName);
                        XmlNodeList nameNode = xDoc.GetElementsByTagName("name");
                        mapnamesource.Add(nameNode[0].InnerText, name);
                        levelMenuItem = new MenuEntry(nameNode[0].InnerText);
                        levelMenuItem.Selected += CampaignLevelSelected;
                        MenuEntries.Add(levelMenuItem);
                    }
                }
            }
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void CampaignLevelSelected(object sender, PlayerIndexEventArgs e)
        {
            // reset to the default state
            MenuEntry menu = (MenuEntry)sender;
            string src;
            mapnamesource.TryGetValue(menu.Text, out src);
            GameMaster.Inst().PrepareCampaignMap(src);

            // now is used for AI
            GameLoadScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(false));
            ScreenManager.RemoveScreen(this);
        }

        #endregion
    }
}
