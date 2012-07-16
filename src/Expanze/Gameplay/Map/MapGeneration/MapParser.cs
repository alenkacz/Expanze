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
        System.Random generator = new System.Random();

        Dictionary<int,int> lowProductivityList;
        Dictionary<int, int> mediumProductivityList;
        Dictionary<int, int> highProductivityList;

        Dictionary<string, int> normalMapTypes;
        Dictionary<string, int> wastelandMapTypes;
        Dictionary<string, int> lowlandMapTypes;

        Dictionary<int, int> activeProductivityList;
        Dictionary<string, int> activeMapTypes;

        int[] rndIndexTypes;
        int rndIndexTypesIndex;
        int[] rndIndexProductivity;
        int rndIndexProductivityIndex;

        public MapParser()
        {
            
        }

        public HexaModel[][] getMap(string mapSize, string mapType, string mapWealth) 
        {
            return this.parse(mapSize,mapType,mapWealth);
        }

        private void Init()
        {
            xDoc = new XmlDocument();

            lowProductivityList = new Dictionary<int, int>();
            mediumProductivityList = new Dictionary<int, int>();
            highProductivityList = new Dictionary<int, int>();

            normalMapTypes = new Dictionary<string, int>();
            wastelandMapTypes = new Dictionary<string, int>();
            lowlandMapTypes = new Dictionary<string, int>();
        }

        public HexaModel[][] parseCampaignMap(string mapSource)
        {
            HexaModel[][] map;
            xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/" + mapSource);

            XmlNodeList rows = xDoc.GetElementsByTagName("row");
            map = new HexaModel[rows.Count][];

            for (int i = 0; i < rows.Count; ++i)
            {
                map[i] = new HexaModel[rows[i].ChildNodes.Count];
                XmlNodeList hexas = rows[i].ChildNodes;

                for (int loop1 = 0; loop1 < hexas.Count; ++loop1)
                {
                    HexaKind type = HexaKind.Nothing;
                    int hexanum = 0;
                    foreach (XmlAttribute attribute in hexas[loop1].Attributes)
                    {
                        if (attribute.Name == "type")
                            type = decideType(attribute.Value);
                        if (attribute.Name == "productivity")
                            hexanum = Convert.ToInt32(attribute.Value);
                    }

                    map[i][loop1] = HexaCreator.create(type, hexanum);
                }

            }           
            return map;
        }

        public HexaModel[][] parse(string mapSize, string mapType, string mapWealth)
        {
            HexaModel[][] map;

            Init();

            

            //TODO user number parameter
            xDoc.Load("Content/Maps/" + mapSize + ".xml");

            decideProductivity(mapWealth);
            decideMapType(mapType);

            XmlNodeList productivities = xDoc.GetElementsByTagName("productivity");
            XmlNodeList mapTypes = xDoc.GetElementsByTagName("mapType");

            this.parseProductivity(productivities);
            this.parseMapTypes(mapTypes);

            rndIndexTypesIndex = 0;
            rndIndexProductivityIndex = 0;

            if (rndIndexTypes == null)
            {
                rndIndexTypes = new int[200];
                
                for (int loop1 = 0; loop1 < rndIndexTypes.Length; loop1++)
                {
                    rndIndexTypes[loop1] = loop1 % 6;
                }
                ShuffleArray(rndIndexTypes);
                
                rndIndexProductivity = new int[200];
                for (int loop1 = 0; loop1 < rndIndexProductivity.Length; loop1++)
                {
                    rndIndexProductivity[loop1] = loop1 % 5;
                }
                ShuffleArray(rndIndexProductivity);
            }

            XmlNodeList rows = xDoc.GetElementsByTagName("row");
            map = new HexaModel[rows.Count][];

            for (int i = 0; i < rows.Count; ++i)
            {
                map[i] = new HexaModel[rows[i].ChildNodes.Count];
                XmlNodeList hexas = rows[i].ChildNodes;

                for (int j = 0; j < hexas.Count; ++j)
                {
                    bool isEmpty = false;
                    bool isWater = false;

                    foreach (XmlAttribute a in hexas[j].Attributes)
                    {
                        if(a.Name == "type" && a.Value == "space") isEmpty = true;
                        if(a.Name == "type" && a.Value == "water") isWater = true;
                    }

                    HexaKind type;
                    int hexanum = 0;

                    if (!isEmpty && !isWater)
                    {
                        type = getRandomType();

                        if (type == HexaKind.Desert)
                        {
                            hexanum = 0;
                        }
                        else
                        {
                            hexanum = getRandomProductivity();
                        }
                    }
                    else
                    {
                        if (isWater)
                            type = HexaKind.Water;
                        else
                            type = HexaKind.Nothing;
                    }


                    map[i][j] = HexaCreator.create(type, hexanum);
                }
                
            }

            return map;
        }

        private void decideProductivity(string s) {

            if (s == "low")
            {
                activeProductivityList = lowProductivityList;
            }
            else if (s == "medium")
            {
                activeProductivityList = mediumProductivityList;
            }
            else
            {
                activeProductivityList = highProductivityList;
            }
        }

        private void decideMapType(string s) {
            if (s == "normal")
            {
                activeMapTypes = normalMapTypes;
            }
            else if (s == "lowland")
            {
                activeMapTypes = lowlandMapTypes;
            }
            else
            {
                activeMapTypes = wastelandMapTypes;
            }
        }

        /// <summary>
        /// Returns randomly selected productivity value from loaded list
        /// </summary>
        /// <returns>value of productivity</returns>
        private int getRandomProductivity()
        {
            //int index = 0;
            int type = 0;

            if (!ranOutOfProductivities())
            {
                while(true)
                {
                    rndIndexProductivityIndex = (rndIndexProductivityIndex + 1) % rndIndexProductivity.Length;
                    if (activeProductivityList.ElementAt(rndIndexProductivity[rndIndexProductivityIndex]).Value > 0)
                    {
                        type = activeProductivityList.ElementAt(rndIndexProductivity[rndIndexProductivityIndex]).Key;
                        break;
                    }
                }
                /*
                while (true)
                {
                    // trying to find nonzero random type
                    index = generator.Next(activeProductivityList.Count);

                    if (activeProductivityList.ElementAt(index).Value > 0)
                    {
                        type = activeProductivityList.ElementAt(index).Key;
                        break;
                    }
                }*/

                // decrement number of available productivity numbers
                --activeProductivityList[type];

                return type;
            }

            return 0;
        }

        /// <summary>
        /// Returns randomly selected productivity value from loaded list
        /// </summary>
        /// <returns>value of productivity</returns>
        private HexaKind getRandomType()
        {
            //int index = 0;
            string type = "";

            if (!ranOutOfMapTypes())
            {
                while (true)
                {
                    rndIndexTypesIndex = (rndIndexTypesIndex + 1) % rndIndexTypes.Length;
                    if (activeMapTypes.ElementAt(rndIndexTypes[rndIndexTypesIndex]).Value > 0)
                    {
                        type = activeMapTypes.ElementAt(rndIndexTypes[rndIndexTypesIndex]).Key;
                        break;
                    }
                }

                /*
                while (true)
                {
                    // trying to find nonzero random type
                    index = generator.Next(activeMapTypes.Count);

                    if (activeMapTypes.ElementAt(index).Value > 0)
                    {
                        type = activeMapTypes.ElementAt(index).Key;
                        break;
                    }
                }*/

                // decrement number of available productivity numbers
                --activeMapTypes[type];

                return decideType(type);
            }

            return HexaKind.Nothing;
        }



        private void ShuffleArray(int[] rndIndex)
        {
            int id1, id2;

            for (int loop1 = 0; loop1 < rndIndex.Length * 3; loop1++)
            {
                id1 = generator.Next() % rndIndex.Length;
                id2 = generator.Next() % rndIndex.Length;

                int temp = rndIndex[id1];
                rndIndex[id1] = rndIndex[id2];
                rndIndex[id2] = temp;
            }
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

        /// <summary>
        /// Checks whether there are some map type points available - fix for indefinite loop, bad map design
        /// </summary>
        /// <returns>true if there are no productivity points left</returns>
        private bool ranOutOfMapTypes()
        {
            foreach (int i in activeMapTypes.Values)
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

        private void parseMapTypes(XmlNodeList xml)
        {
            foreach (XmlNode x in xml)
            {
                XmlAttributeCollection attCol = x.Attributes;
                XmlNodeList items = x.ChildNodes;

                foreach (XmlAttribute a in attCol)
                {
                    if (a.Value == "normal" && a.Name == "type")
                    {
                        fillMapTypesList(items, normalMapTypes);
                    }
                    else if (a.Value == "lowland" && a.Name == "type")
                    {
                        fillMapTypesList(items, lowlandMapTypes);
                    }
                    else if (a.Value == "wasteland" && a.Name == "type")
                    {
                        fillMapTypesList(items, wastelandMapTypes);
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

        private void fillMapTypesList(XmlNodeList items, Dictionary<string, int> list)
        {
            foreach (XmlNode x in items)
            {
                XmlAttributeCollection attCol = x.Attributes;
                string hexaType = "";

                foreach (XmlAttribute a in attCol)
                {
                    if (a.Name == "type")
                    {
                        hexaType = a.Value;
                    }
                }

                list.Add(hexaType, Int32.Parse(x.InnerText));
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
                case "space":
                    return HexaKind.Nothing;
                default :
                    return HexaKind.Null;
            }
        }
    }
}
