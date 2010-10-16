using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Expanze
{
    class MapParser
    {
        XmlDocument xDoc;

        public MapParser()
        {
            xDoc = new XmlDocument();
        }

        public Hexa[][] getMap() 
        {
            int number = getRandomMap();
            return this.parse(number);
        }

        public Hexa[][] parse(int number)
        {
            Hexa[][] map;

            //TODO user number parameter
            xDoc.Load("Content/Maps/1.xml");

            XmlNodeList rows = xDoc.GetElementsByTagName("row");
            map = new Hexa[rows.Count][];

            String a = rows[1].InnerText;

            for (int i = 0; i < rows.Count; ++i)
            {
                map[i] = new Hexa[rows[i].ChildNodes.Count];
                XmlNodeList hexas = rows[i].ChildNodes;

                for (int j = 0; j < hexas.Count; ++j)
                {
                    XmlNode type = hexas[j].SelectSingleNode("type");
                    XmlNode hexanum = hexas[j].SelectSingleNode("number");

                    map[i][j] = HexaCreator.create(decideType(type.InnerText), int.Parse(hexanum.InnerText));
                }
                
            }

            return map;
        }

        /// <summary>
        /// Counts number of Maps in a folder
        /// </summary>
        /// <returns>Actual number of maps</returns>
        public int getNumberOfMaps()
        {
            //TODO write some actual code here
            return 1;
        }

        /// <summary>
        /// Returns number of randomly chosen
        /// </summary>
        /// <returns></returns>
        private int getRandomMap()
        {
            //TODO write some actual code here

            return 1;
        }

        /// <summary>
        /// Returns type of the hexas
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Settings.Types decideType(String type)
        {
            switch (type)
            {
                case "cornfield":
                    return Settings.Types.Cornfield;
                case "forest":
                    return Settings.Types.Forest;
                case "desert":
                    return Settings.Types.Desert;
                case "mountaint":
                    return Settings.Types.Mountains;
                case "pasture":
                    return Settings.Types.Pasture;
                case "stone":
                    return Settings.Types.Stone;
                default:
                    return Settings.Types.Water;
            }
        }
    }
}
