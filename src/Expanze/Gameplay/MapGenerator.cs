using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePlugin;

namespace Expanze.Gameplay
{
    static class MapGenerator
    {
        static Random rnd;

        private static HexaModel[][] GenerateBase(GameSettings gs)
        {
            int[] sizeArray = new int[10];
            int[] posArray = new int[10];
            int[] waterArray = new int[10];

            int hexCount = 0;
            int hexMax = 5 + 9 * gs.PlayerCount / 2;

            int pos = 0;
            int size = rnd.Next(3) + 1;
            int row = 0;

            double waterChance = (gs.GetMapType() == MapType.LITTLE_ISLANDS) ? 0.6 : 0.9;
            double waterMinWidth = (gs.GetMapType() == MapType.LITTLE_ISLANDS) ? 3 : 5;

            int mid = -1;

            for (int loop1 = 0; loop1 < sizeArray.Length - 1; loop1++)
            {
                double rndNumber = rnd.NextDouble();
                if (rndNumber > ((size < 3)  ? 0.3 : 0.6) && size <= 3 + gs.PlayerCount / 2)
                    size += 1;
                else if (rndNumber < ((size > 4) ? 0.6 : 0.3) && size - pos >= 3)
                    size -= 1;
                if (rnd.NextDouble() > 0.75)
                {
                    if (rndNumber > 0.6 && size <= 3 + gs.PlayerCount)
                        size += 1;
                    else if (rndNumber < 0.3 && size >= 3)
                        size -= 1;
                }

                if (loop1 > 1 && rnd.NextDouble() > 0.55)
                    pos++;

                if (loop1 != 0)
                {
                    hexCount += size;
                }

                if (mid < 0 && hexCount > hexMax / 2 + 1 && gs.GetMapType() == MapType.TWO_ISLANDS)
                {
                    mid = loop1;
                    hexCount -= size;
                }

                switch (gs.GetMapType())
                {
                    case MapType.ISLAND:
                    case MapType.TWO_ISLANDS:
                        waterArray[loop1] = (rnd.NextDouble() > 0.75) ? 1 : 0;
                        break;
                    case MapType.LITTLE_ISLANDS:
                        waterArray[loop1] = rnd.Next(size) + 1; 
                        break;
                }

                sizeArray[loop1] = size + pos + waterArray[loop1];
                posArray[loop1] = pos;
                row = loop1 + 1;

                if (hexCount >= hexMax && loop1 > 2)
                {
                    sizeArray[loop1] -= (hexCount - hexMax) / 2;
                    sizeArray[loop1 + 1] = sizeArray[loop1];
                    posArray[loop1 + 1] = posArray[loop1];
                    row++;
                    break;
                }
            }
            HexaModel[][] map = new HexaModel[row][];

            for (int loop1 = 0; loop1 < map.Length; loop1++)
            {
                int width = sizeArray[loop1];
                int origWidth = sizeArray[loop1];
                
                if (loop1 > 0 && sizeArray[loop1 - 1] > sizeArray[loop1])
                {
                    width = sizeArray[loop1 - 1];
                    sizeArray[loop1] = width;
                }
                if (loop1 < sizeArray.Length - 1 && sizeArray[loop1 + 1] > sizeArray[loop1])
                {
                    width = sizeArray[loop1 + 1];
                    sizeArray[loop1] = width;
                }

                map[loop1] = new HexaModel[width + 2 + posArray[loop1]];

                bool lastTime = false;
                for (int loop2 = 0; loop2 < map[loop1].Length; loop2++)
                {
                    if (loop1 == 0 || loop1 == map.Length - 1 || loop2 <= posArray[loop1] || loop2 > origWidth ||
                        (loop1 == mid && gs.GetMapType() == MapType.TWO_ISLANDS))
                    {
                        if (loop2 < posArray[loop1])
                            map[loop1][loop2] = null;
                        else
                            map[loop1][loop2] = new WaterHexa(false, false);
                    }
                    else
                    {
                        if (waterArray[loop1] > 0 && loop2 > 2 && loop2 < map[loop1].Length - 3 && rnd.NextDouble() < 0.7 && !lastTime)
                        {
                            map[loop1][loop2] = new WaterHexa(false, false);
                            waterArray[loop1]--;
                            lastTime = true;
                        }
                        else {
                            string type = "?+";
                            double rndNumber = rnd.NextDouble();
                            switch (gs.GetMapSource())
                            {
                                case MapSource.NORMAL :
                                    if (rndNumber > 0.93)
                                        type = "desert";
                                    else
                                        type = "?";
                                    break;
                                case MapSource.LOWLAND:
                                    if (rndNumber > 0.93)
                                        type = "desert";
                                    else if (rndNumber > 0.75)
                                        type = "2";
                                    else
                                        type = "3";
                                    break;
                                case MapSource.WASTELAND:
                                    if (rndNumber > 0.88)
                                        type = "desert";
                                    else if (rndNumber > 0.70)
                                        type = "3";
                                    else
                                        type = "2";
                                    break;
                            }
                            map[loop1][loop2] = HexaCreator.create(type, "?", false, false);
                            lastTime = false;
                        }
                    }
                }
            }

            return map;
        }

        public static HexaModel[][] GenerateScenarioMap(GameSettings gs)
        {
            rnd = GameMaster.Inst().RandomGenerator;

            GenerateSetup(gs);
            HexaModel[][] map = GenerateBase(gs);

            for (int loop1 = 0; loop1 < map.Length; loop1++)
            {
                for (int loop2 = 0; loop2 < map[loop1].Length; loop2++)
                {
                    if (map[loop1][loop2] == null)
                        continue;

                    switch (gs.GetMapKind())
                    {
                        case MapKind.VISIBLE:
                            map[loop1][loop2].SecretKind = false;
                            break;
                        case MapKind.HALF:
                            map[loop1][loop2].SecretKind = (GameMaster.Inst().GetRandomNumber() > 0.45) ? false : true;
                            break;
                        case MapKind.HIDDEN:
                            map[loop1][loop2].SecretKind = true;
                            break;
                    }

                    switch (gs.GetMapProductivity())
                    {
                        case MapProductivity.VISIBLE:
                            map[loop1][loop2].SecretProductivity = false;
                            break;
                        case MapProductivity.HALF:
                            map[loop1][loop2].SecretProductivity = (GameMaster.Inst().GetRandomNumber() > 0.45) ? false : true;
                            break;
                        case MapProductivity.HIDDEN:
                            map[loop1][loop2].SecretProductivity = true;
                            break;
                    }
                }
            }

            return map;
        }

        private static void GenerateSetup(GameSettings gs)
        {
            GameMaster.Inst().ResetGameSettings();
            Settings.pointsTown = 8 - (gs.PlayerCount + 2) / 3;
            Settings.maxTurn = 45 + 5 * gs.PlayerCount;
            for (int loop1 = 0; loop1 < 3; loop1++)
            {
                switch (rnd.Next(12))
                {
                    case 0:
                        Settings.pointsRoad = 5 + rnd.Next(5);
                        break;
                    case 1:
                        switch (gs.GetMapSource())
                        {
                            case MapSource.LOWLAND:
                                Settings.pointsSaw = 3 + rnd.Next(3);
                                break;
                            case MapSource.WASTELAND:
                                Settings.pointsQuarry = 3 + rnd.Next(3);
                                break;
                            case MapSource.NORMAL:
                                Settings.pointsSaw = rnd.Next(4);
                                Settings.pointsMill = rnd.Next(4);
                                Settings.pointsMine = rnd.Next(4);
                                break;
                        }
                        break;
                    case 2:
                        switch (gs.GetMapSource())
                        {
                            case MapSource.LOWLAND:
                                Settings.pointsCorn = 8 + rnd.Next(5) * 8;
                                break;
                            case MapSource.WASTELAND:
                                Settings.pointsOre = 8 + rnd.Next(5) * 8;
                                break;
                            case MapSource.NORMAL:
                                Settings.pointsWood = rnd.Next(8) * 8;
                                Settings.pointsMeat = rnd.Next(8) * 8;
                                Settings.pointsStone = rnd.Next(8) * 8;
                                break;
                        }
                        break;
                    case 3:
                        switch (rnd.Next(3))
                        {
                            case 0:
                                Settings.pointsFortCapture = 3 + rnd.Next(3);
                                break;
                            case 1:
                                Settings.pointsFortParade = 3 + rnd.Next(3);
                                break;
                            case 2:
                                Settings.pointsFortSteal = 3 + rnd.Next(3);
                                break;
                        }
                        break;

                    case 4:
                        Settings.pointsMarket = rnd.Next(3);
                        Settings.pointsMarketLvl2 = rnd.Next(3);
                        break;

                    case 5:
                        Settings.pointsUpgradeLvl2 = rnd.Next(3);
                        Settings.pointsMonastery = rnd.Next(3);
                        break;
                }
            }
        }
    }
}
