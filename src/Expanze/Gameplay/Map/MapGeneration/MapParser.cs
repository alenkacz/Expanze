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

        Dictionary<int,int> lowProductivityList = new Dictionary<int,int>();
        Dictionary<int, int> mediumProductivityList = new Dictionary<int, int>();
        Dictionary<int, int> highProductivityList = new Dictionary<int, int>();

        Dictionary<int, int> activeProductivityList;

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
            // TODO should be done programatically
            activeProductivityList = highProductivityList;

            //TODO user number parameter
            xDoc.Load("Content/Maps/" + "small" + ".xml");

            XmlNodeList productivities = xDoc.GetElementsByTagName("productivity");

            this.parseProductivity(productivities);

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
                    int hexanum = 0;

                    if (decideType(type.InnerText) != HexaKind.Nothing)
                    {
                        hexanum = getRandomProductivity();
                    }


                    map[i][j] = HexaCreator.create(decideType(type.InnerText), hexanum);
                }
                
            }

            return map;
        }

        /// <summary>
        /// Returns randomly selected productivity value from loaded list
        /// </summary>
        /// <returns>value of productivity</returns>
        private int getRandomProductivity()
        {
            System.Random generator = new System.Random();
            int index = 0;
            int type = 0;

            if (!ranOutOfProductivities())
            {

                while (true)
                {
                    // trying to find nonzero random type
                    index = generator.Next(activeProductivityList.Count);

                    if (activeProductivityList.ElementAt(index).Value > 0)
                    {
                        type = activeProductivityList.ElementAt(index).Key;
                        break;
                    }
                }

                // decrement number of available productivity numbers
                --activeProductivityList[type];

                return type;
            }

            return 0;
        }

        /// <summary>
        /// Checks whether there are some productivitiy points available - fix for indefinite loop, bad map design
        /// </summary>
        /// <returns>true if there are no productivity points left</returns>
        private bool ranOutOfProductivities()
        {
            foreach (int i in activeProductivityList.Values)
            {
                if (i > 0) return false;
            }

            return true;
        }

        private void parseProductivity(XmlNodeList xml)
        {
            foreach (XmlNode x in xml)
            {
                XmlAttributeCollection attCol = x.Attributes;
                XmlNodeList items = x.ChildNodes;

                foreach (XmlAttribute a in attCol)
                {
                    if (a.Value == "low" && a.Name == "type")
                    {
                        fillProductivityList(items,lowProductivityList);
                    }
                    else if (a.Value == "medium" && a.Name == "type")
                    {
                        fillProductivityList(items, mediumProductivityList);
                    }
                    else if (a.Value == "high" && a.Name == "type")
                    {
                        fillProductivityList(items, highProductivityList);
                    }
                    else
                    {
                        // TODO some exception should be triggered
                    }
                }
            }
        }

        private void fillProductivityList(XmlNodeList items, Dictionary<int,int> list)
        {
            foreach (XmlNode x in items)
            {
                XmlAttributeCollection attCol = x.Attributes;
                int productivity = 0;

                foreach (XmlAttribute a in attCol)
                {
                    if (a.Name == "value")
                    {
                        productivity = Int32.Parse(a.Value);
                    }
                }

                list.Add(productivity, Int32.Parse(x.InnerText));
            }
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
