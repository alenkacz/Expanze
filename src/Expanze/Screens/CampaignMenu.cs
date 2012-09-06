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
using System;
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
            int campID = 0;
            if (subFiles.Length > 0)
            {
                foreach (FileInfo subFile in subFiles)
                {
                    string name = subFile.Name;
                    if(name.StartsWith("cam"))
                    {
                        campID++;
                        xDoc.Load(subFile.FullName);
                        XmlNodeList nameNode = xDoc.GetElementsByTagName("name");
                        foreach (XmlNode language in nameNode[0].ChildNodes)
                        {
                            if (language.LocalName == Strings.Inst().Language)
                            {
                                mapnamesource.Add(language.InnerText, name);
                                levelMenuItem = new MenuEntry(language.InnerText, GameResources.Inst().GetFont(EFont.MedievalBigest));
                                levelMenuItem.Selected += CampaignLevelSelected;
                                if (campID > Settings.campaign + 1)
                                {
                                    levelMenuItem.Disabled = true;
                                    levelMenuItem.ColorHover = Color.OrangeRed;
                                    levelMenuItem.ColorNormal = Color.OrangeRed;
                                }
                                MenuEntries.Add(levelMenuItem);
                            }
                        }
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
            String levelStr = src;
            int level = Int32.Parse(levelStr.Substring(8, 2));
            Settings.level = level;
            GameMaster.Inst().SetMapSource(src);// PrepareCampaignMap(src);

            // now is used for AI
            GameLoadScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(false));
            ScreenManager.RemoveScreen(this);
        }

        #endregion
    }
}
