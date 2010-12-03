using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CorePlugin;

namespace Expanze
{
    class MapParser
    {
        XmlDocument xDoc;

        public MapParser()
        {
            xDoc = new XmlDocument();
        }

        public HexaModel[][] getMap() 
        {
            int number = getRandomMap();
            return this.parse(number);
        }

        public HexaModel[][] parse(int number)
        {
            HexaModel[][] map;

            //TODO user number parameter
            xDoc.Load("Content/Maps/" + "big" + ".xml");

            XmlNodeList rows = xDoc.GetElementsByTagName("row");
            map = new HexaModel[rows.Count][];

            String a = rows[1].InnerText;

            for (int i = 0; i < rows.Count; ++i)
            {
                map[i] = new HexaModel[rows[i].ChildNodes.Count];
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
            return 2;
        }

        /// <summary>
        /// Returns number of randomly chosen
        /// </summary>
        /// <returns></returns>
        private int getRandomMap()
        {
            //TODO write some actual code here
            System.Random generator = new System.Random();
            return 1; // generator.Next(getNumberOfMaps() - 1) + 1;
        }

        /// <summary>
        /// Returns type of the hexas
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private HexaKind decideType(String type)
        {
            switch (type)
            {
                case "cornfield":
                    return HexaKind.Cornfield;
                case "forest":
                    return HexaKind.Forest;
                case "desert":
                    return HexaKind.Desert;
                case "mountains":
                    return HexaKind.Mountains;
                case "pasture":
                    return HexaKind.Pasture;
                case "stone":
                    return HexaKind.Stone;
                case "water":
                    return HexaKind.Water;
                case "nothing" :
                    return HexaKind.Nothing;
                default :
                    return HexaKind.Null;
            }
        }
    }
}
