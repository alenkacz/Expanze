//#define CHANGE_FIRST_2_PLAYERS
//#define CHANGE_MAP_EVERY_TURN
#define TURN_ON_RANDOM_EVENTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;
using Expanze.Gameplay.Map;
using System.Threading;
using Expanze.Gameplay;
using Microsoft.Xna.Framework.Graphics;
using Expanze.Utils;
using Expanze.Utils.Genetic;
using System.Xml;

namespace Expanze
{
    public enum EFortState { Normal, DestroyingHexa, CapturingHexa };

    class GameMaster
    {
        private int playerCount;
        private List<Player> players = new List<Player>();
        private int turnNumber;

        private Player[] medailOwner;

        private Player activePlayer;
        private Player targetPlayer;
        private Player lastHumanPlayer;

        internal Player LastHumanPlayer
        {
            get { return lastHumanPlayer; }
        }

        private int activePlayerIndex;
        private EGameState state;
        private EFortState fortState;
        private bool hasBuiltTown;          /// In first 2 rounds each player can build only one town

        // when is game paused and player see paused menu, he cant build towers etc
        private bool paused;
        // used for open paused menu
        private bool pausedNew;
        private bool tutorial;

        private bool winnerNew;

        private Map map;

        private Thread actualAIThread;
        private const int AI_TIME = 5000;   // this is time which have each plugin each turn to resolve AI
        private int actualAITime;           // how much time has AI before it will be aborted
        private bool hasAIThreadStarted;

        private Random randomNumber;

        private static GameMaster instance = null;

        private GameSettings gameSettings;
        private string mapSource;

        private int gameCount;
#if GENETIC
        private Genetic genetic;
        private double fitness;
        private int[][] chromozone;
        private int[][][] bosses;
        bool lastWinner;
        int lastTurnNumber;

        private int geneticPopulation;
        private int geneticFitnessRound;
#endif

        public static GameMaster Inst()
        {
            if (instance == null)
            {
                instance = new GameMaster();
            }

            return instance;
        }

        /// <summary>
        /// Private constructor because of the Singleton
        /// </summary>
        private GameMaster() 
        {
            randomNumber = new Random();
            gameCount = 0;
        }

        public void AddToPlayerCount(int i) { 
            playerCount += i;
            if (playerCount == 1)
                winnerNew = true;
        }

        public bool StartGame()
        {
            gameCount++;
            tutorial = false;
            PrepareCampaignScenario();

            playerCount = players.Count;

            medailOwner = new Player[(int) Building.Count];
            for (int loop1 = 0; loop1 < players.Count; loop1++)
            {
                medailOwner[loop1] = null;
            }

            IComponentAI AI;
            int[][] setArray;

            lastHumanPlayer = players[0];
            for(int loop1 = 0; loop1 < players.Count; loop1++)
            {
                setArray = players[loop1].GetPersonality();
                players[loop1] = new Player(players[loop1].GetName(), players[loop1].GetColor(), players[loop1].GetComponentAI(), loop1, setArray, players[loop1].GetGen());
                AI = players[loop1].GetComponentAI();

                if (AI != null) // its computer then
                {
#if GENETIC
                    // GENETICS ALG
                    if (players[loop1].GetGen() &&
                        (gameCount - 1) % geneticFitnessRound == 0)
                    {
                        chromozone = genetic.GetChromozone();
                        setArray = chromozone;

                        // uncomment next line for finding your fitness
                        //players[loop1] = new Player(players[loop1].GetName(), players[loop1].GetColor(), null, loop1, setArray, players[loop1].GetGen());
                    }

                    if (!players[loop1].GetGen())
                    {
                        if (bosses == null)
                            setArray = genetic.GetBestOnePersonality(); // reference AI is the best AI in all population
                        else
                            setArray = bosses[(gameCount - 1) % geneticFitnessRound]; // reference AI is contant
                    }
                    // GENETICS 
#endif

                    AI.InitAIComponent(map.GetMapController(), setArray);
                }
                else
                {
                    lastHumanPlayer = players[loop1];
                }
            }

            state = EGameState.BeforeGame;
            PrepareCampaignMap();
            
            activePlayerIndex = 0;
            activePlayer = players.ElementAt(activePlayerIndex);
            targetPlayer = activePlayer;

            pausedNew = false;
            paused = false;
            winnerNew = false;

            hasAIThreadStarted = false;

            if(players[0].GetBuildingCount(Building.Town) == 0)
                state = EGameState.StateFirstTown;
            else
                state = EGameState.StateGame;

            fortState = EFortState.Normal;

            randomNumber = new System.Random();
            hasBuiltTown = false;
            turnNumber = 1;

            if (playerCount == 1)
            {
                Settings.banFortStealSources = true;
                Settings.banFortCaptureHexa = true;
            }
            if (Settings.pointsFortParade == 0)
                Settings.banFortParade = true;
            if (Settings.banChangeSources)
                Settings.banMarket = true;

            PromptWindow.Inst().Deactive();
            Message.Inst().ClearMessages();
            MarketComponent.Inst().SetIsActive(false);
            PathNode.SetIsValid(false);

            if(tutorial)
                Tutorial.Inst().TurnOn();
            StartTurn(false);
            return true;
        }

        public void PlayerWantMedail(Player player, Building medal)
        {
            /* no medals
            int minCount = 10;
            int pointsForMedal = Settings.pointsMedal;
            if (pointsForMedal == 0)
                return;

            switch(medal)
            {
                case Building.Town :
                    minCount = 5;
                    break;
                case Building.Road :
                    minCount = 10;
                    break;
                case Building.Market :
                case Building.Fort :
                case Building.Monastery :
                    minCount = 1;
                    break;
                default :
                    minCount = 4;
                    break;
            }

            if((medailOwner[(int) medal] == null && player.GetBuildingCount(medal) >= minCount) ||
               (minCount != 1 && /// medal for special buildings
                medailOwner[(int) medal] != null && player.GetBuildingCount(medal) > medailOwner[(int) medal].GetBuildingCount(medal)))
            {
                if (medailOwner[(int)medal] != null)
                {
                    medailOwner[(int)medal].RemovePoints(PlayerPoints.Medal);
                    medailOwner[(int)medal].GetStatistic().AddStat(Statistic.Kind.Medals, -1, turnNumber);
                }
                medailOwner[(int)medal] = player;
                medailOwner[(int)medal].GetStatistic().AddStat(Statistic.Kind.Medals, 1, turnNumber);
                player.AddPoints(PlayerPoints.Medal);
                Message.Inst().Show(GetMedalTitle(medal),GetMedalDescription(medal),GetMedaileIcon(medal));
            }*/
        }

        public void SetMapSource(string source)
        {
            mapSource = source;
        }
        public string GetMapSource()
        {
            return mapSource;
        }
        public void SetGameSettings(int points, string mapType, string mapSize, string mapWealth)
        {
            gameSettings = new GameSettings(points,mapType,mapSize,mapWealth);
        }


        private void PrepareCampaignSetup(XmlDocument xDoc)
        {
            SetDefaultSettings();
            XmlNodeList nameNode = xDoc.GetElementsByTagName("tutorial");
            if (nameNode.Count > 0)
            {
                Tutorial.Inst().Initialize(nameNode[0].InnerText + ".xml");
                tutorial = true;
            }
            else
            {
                tutorial = false;
                Tutorial.Inst().ClearAll();
            }

            XmlNodeList points = xDoc.GetElementsByTagName("points")[0].ChildNodes;
            foreach (XmlNode goal in points)
            {
                int value = Convert.ToInt32(goal.InnerText);
                switch (goal.Name)
                {
                    case "WinPoint": /* ignore it */ break;
                    case "Town": Settings.pointsTown = value; break;
                    case "Road": Settings.pointsRoad = value; break;
                    case "Fort": Settings.pointsFort = value; break;
                    case "Monastery": Settings.pointsMonastery = value; break;
                    case "Market": Settings.pointsMarket = value; break;
                    case "Medal": Settings.pointsMedal = value; break;
                    case "FortParade": Settings.pointsFortParade = value; break;
                    case "FortSteal": Settings.pointsFortSteal = value; break;
                    case "FortCapture": Settings.pointsFortCapture = value; break;
                    case "Mine": Settings.pointsMine = value; break;
                    case "Quarry": Settings.pointsQuarry = value; break;
                    case "Stepherd": Settings.pointsStepherd = value; break;
                    case "Saw": Settings.pointsSaw = value; break;
                    case "Mill": Settings.pointsMill = value; break;
                    case "UpgradeLvl1": Settings.pointsUpgradeLvl1 = value; break;
                    case "UpgradeLvl2": Settings.pointsUpgradeLvl2 = value; break;
                    case "MarketLvl1": Settings.pointsMarketLvl1 = value; break;
                    case "MarketLvl2": Settings.pointsMarketLvl2 = value; break;
                    case "Stone": Settings.pointsStone = value; break;
                    case "Wood": Settings.pointsWood = value; break;
                    case "Meat": Settings.pointsMeat = value; break;
                    case "Corn": Settings.pointsCorn = value; break;
                    case "Ore": Settings.pointsOre = value; break;
                    case "turnLimit": Settings.maxTurn = value; break;
                    case "road" :
                        Settings.goalRoad.Add(value);
                        map.GetRoadByID(value).GoalRoad = true;
                        break;
                    case "town":
                        Settings.goalTown.Add(value);
                        map.GetTownByID(value).GoalTown = true;
                        break;
                    case "roadID":
                        Settings.goalRoadID = value;
                        break;
                    case "townID":
                        Settings.goalTownID = value;
                        break;
                }
            }

            XmlNodeList ban = xDoc.GetElementsByTagName("ban")[0].ChildNodes;
            foreach (XmlNode action in ban)
            {
                bool value = (Convert.ToInt32(action.InnerText) == 0) ? false : true;
                switch (action.Name)
                {
                    case "RandomEvents": Settings.banRandomEvents = value; break;
                    case "ChangeSources": Settings.banChangeSources = value; break;
                    case "Fort": Settings.banFort = value; break;
                    case "Monastery": Settings.banMonastery = value; break;
                    case "Market": Settings.banMarket = value; break;
                    case "FortParade": Settings.banFortParade = value; break;
                    case "FortSteal": Settings.banFortStealSources = value; break;
                    case "FortCapture": Settings.banFortCaptureHexa = value; break;
                    case "UpgradeLvl2": Settings.banSecondUpgrade = value; break;
                    case "MarketLvl2": Settings.banSecondLicence = value; break;
                }
            }
        }

        private void SetDefaultSettings()
        {
            Settings.banFort = false;
            Settings.banFortCaptureHexa = false;
            Settings.banFortParade = false;
            Settings.banFortStealSources = false;
            Settings.banChangeSources = false;
            Settings.banMarket = false;
            Settings.banMonastery = false;
            Settings.banSecondLicence = false;
            Settings.banSecondUpgrade = false;
            Settings.banRandomEvents = false;

            Settings.pointsFort = 0;
            Settings.pointsFortCapture = 0;
            Settings.pointsFortParade = 0;
            Settings.pointsFortSteal = 0;
            Settings.pointsMarket = 0;
            Settings.pointsMarketLvl1 = 0;
            Settings.pointsMarketLvl2 = 0;
            Settings.pointsMedal = 0;
            Settings.pointsMill = 0;
            Settings.pointsMine = 0;
            Settings.pointsMonastery = 0;
            Settings.pointsQuarry = 0;
            Settings.pointsRoad = 0;
            Settings.pointsSaw = 0;
            Settings.pointsStepherd = 0;
            Settings.pointsTown = 5;
            Settings.pointsUpgradeLvl1 = 0;
            Settings.pointsUpgradeLvl2 = 0;
            Settings.pointsWood = 0;
            Settings.pointsStone = 0;
            Settings.pointsOre = 0;
            Settings.pointsMeat = 0;
            Settings.pointsCorn = 0;
            Settings.goalRoad = new List<int>();
            Settings.goalTown = new List<int>();
            Settings.goalRoadID = 0;
            Settings.goalTownID = 0;

            Settings.maxTurn = 50;
        }

        private void PrepareCampaignPlayers(XmlDocument xDoc)
        {
            players.Clear();

            XmlNodeList playersNodes = xDoc.GetElementsByTagName("players")[0].ChildNodes;

            String playerName;
            Color playerColor;
            IComponentAI playerAI;
            int playerOrder;
            int[][] playerPersonality;
            bool playerGen;

            foreach (XmlNode playerNode in playersNodes)
            {
                playerName = "";
                playerColor = Color.Black;
                playerAI = null;
                playerOrder = 0;
                playerPersonality = null;
                playerGen = false;

                foreach (XmlNode info in playerNode.ChildNodes)
                {
                    switch (info.Name)
                    {
                        case "order":
                            playerOrder = Convert.ToInt32(info.InnerText);
                            break;
                        case "name":
                            playerName = info.InnerText;
                            break;
                        case "color":
                            switch (info.InnerText)
                            {
                                case "Red": playerColor = Color.Red; break;
                                case "Blue": playerColor = Color.Blue; break;
                                case "Green": playerColor = Color.Green; break;
                                case "Orange": playerColor = Color.Orange; break;
                                case "Yellow": playerColor = Color.Yellow; break;
                                case "Black": playerColor = Color.Black; break;
                                case "Purple": playerColor = Color.Purple; break;
                            }
                            break;
                        case "AI":
                            if (info.InnerText.ToLower() == "ai gen")
                            {
                                playerGen = true;
                            }
                            if (info.InnerText.ToLower() == "human")
                            {
#if GENETIC
                                foreach (IComponentAI AI in CoreProviderAI.AI)
                                {
                                    if (AI.GetAIName() == "AI Gen")
                                        playerAI = AI.Clone();
                                }
#else
                                playerAI = null;
#endif
                            }
                            else
                            {
                                foreach (IComponentAI AI in CoreProviderAI.AI)
                                {
                                    if (AI.GetAIName() == info.InnerText)
                                        playerAI = AI.Clone();
                                }
                            }
                            break;

                        case "personality" :
                            playerPersonality = Genetic.ParsePersonality(info.InnerText);
                            break;
                    }
                }
                if (playerAI == null)
                    playerName = Settings.playerName;
                Player player = new Player(playerName, playerColor, playerAI, playerOrder, playerPersonality, playerGen);
                players.Add(player);
            }
        }

      

        internal void PrepareCampaignScenario()
        {
            if (mapSource == null)  // neni kampan
                return;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/" + mapSource);

            //state = EGameState.BeforeStart;
            PrepareCampaignPlayers(xDoc);
            PrepareCampaignSetup(xDoc);
        }

        internal void PrepareCampaignMap()
        {
            if (mapSource == null)  // neni kampan
                return;

            InitPlayers();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/" + mapSource);

            XmlNodeList playersNodes = xDoc.GetElementsByTagName("players")[0].ChildNodes;

            for(int loop1 = 0; loop1 < playersNodes.Count; loop1++)
            {
                XmlNode playerNode = playersNodes[loop1];
                activePlayer = players[loop1];

                for (int loop2 = 0; loop2 < playerNode.ChildNodes.Count; loop2++)
                {
                    XmlNode info = playerNode.ChildNodes[loop2];
                    
                    switch (info.Name)
                    {
                        case "towns":
                            foreach (XmlNode townNode in info.ChildNodes)
                            {
                                int ID = Convert.ToInt16(townNode.Attributes[0].Value);
                                map.GetMapController().BuildTown(ID);

                                foreach (XmlNode buildingNode in townNode.ChildNodes)
                                {
                                    int pos = Convert.ToInt16(buildingNode.InnerText);
                                    switch (buildingNode.Name)
                                    {
                                        case "source-building" :
                                            map.GetTownByID(ID).BuildSourceBuilding((byte) pos);
                                            break;
                                        case "fort":
                                            map.GetTownByID(ID).BuildFort((byte)pos);
                                            break;
                                        case "market":
                                            map.GetTownByID(ID).BuildMarket((byte)pos);
                                            break;
                                        case "monastery":
                                            map.GetTownByID(ID).BuildMonastery((byte)pos);
                                            break;
                                    }
                                    
                                }
                            }

                            break;

                        case "roads":
                            foreach (XmlNode roadID in info.ChildNodes)
                            {
                                int ID = Convert.ToInt16(roadID.InnerText);
                                map.GetMapController().BuildRoad(ID);
                            }

                            break;

                        case "source":
                            //activePlayer.AddSources(new SourceAll(0), TransactionState.TransactionStart);
                            foreach (XmlNode kind in info.ChildNodes)
                            {
                                int amount = Convert.ToInt16(kind.InnerText);
                                switch (kind.Name)
                                {
                                    case "wood" :
                                        activePlayer.AddSources(new SourceAll(0, 0, 0, amount, 0), TransactionState.TransactionMiddle);
                                        break;
                                    case "ore":
                                        activePlayer.AddSources(new SourceAll(0, 0, 0, 0, amount), TransactionState.TransactionMiddle);
                                        break;
                                    case "corn":
                                        activePlayer.AddSources(new SourceAll(amount, 0, 0, 0, 0), TransactionState.TransactionMiddle);
                                        break;
                                    case "stone":
                                        activePlayer.AddSources(new SourceAll(0, 0, amount, 0, 0), TransactionState.TransactionMiddle);
                                        break;
                                    case "meat":
                                        activePlayer.AddSources(new SourceAll(0, amount, 0, 0, 0), TransactionState.TransactionMiddle);
                                        break;
                                }
                            }
                            //activePlayer.AddSources(new SourceAll(0), TransactionState.TransactionEnd);
                            break;
                    }
                }
            }
        }

        public void PrepareQuickGame()
        {
            this.ResetGameSettings();
            players.Clear();
            IComponentAI componentAI = null;
            foreach (IComponentAI AI in CoreProviderAI.AI)
            {
                componentAI = AI;
                break;
            }

            int nameCount = Strings.MENU_HOT_SEAT_NAMES.Length;

            int firstPlayerNameID = randomNumber.Next() % nameCount;
            int secondPlayerNameID;

            do
            {
                secondPlayerNameID = randomNumber.Next() % nameCount;
            } while (secondPlayerNameID == firstPlayerNameID);

            if (componentAI != null)
            {
                IComponentAI componentAICopy = componentAI.Clone();
                this.players.Add(new Player(Strings.MENU_HOT_SEAT_NAMES[secondPlayerNameID], Color.Blue, componentAICopy, 0));
            } else
                this.players.Add(new Player(Strings.MENU_HOT_SEAT_NAMES[secondPlayerNameID], Color.Blue, null, 0));

            this.players.Add(new Player(Strings.MENU_HOT_SEAT_NAMES[firstPlayerNameID], Color.Red, null, 1));
        }

        public GameSettings GetGameSettings()
        {
            if (gameSettings != null)
            {
                return gameSettings;
            }
            else
            {
                gameSettings = new GameSettings(Settings.winPoints[0], Strings.GAME_SETTINGS_MAP_TYPE_NORMAL, Strings.GAME_SETTINGS_MAP_SIZE_SMALL, Strings.GAME_SETTINGS_MAP_WEALTH_MEDIUM);
                return gameSettings;
            }
        }

        public void ResetGameSettings()
        {
            gameSettings = null;
            SetDefaultSettings();
        }

        public void DeleteAllPlayers()
        {
            players = new List<Player>();
            playerCount = 0;
        }

        private static void AIThread(IComponentAI ai)
        {
            try
            {
                ai.ResolveAI();
            } 
            catch (Exception exception)
            {
                Player player = GameMaster.Inst().GetActivePlayer();
                player.SetActive(false);
                String log = exception.Message + " : from : " + exception.Source + System.Environment.NewLine +
                             exception.HelpLink + System.Environment.NewLine +
                             exception.StackTrace + System.Environment.NewLine +
                             exception.TargetSite + System.Environment.NewLine +
                             exception.InnerException;
                Logger.Inst().Log(ai.GetAIName() + " - " + player.GetName() + ".txt", log);
            }
        }

        private void SetNewActiveHexa()
        {
            HexaModel hexa;
            TownModel townModel = map.GetTownByID(TownView.GetPickTownID());
                    
            for (byte loop1 = 0; loop1 < 3; loop1++)
            {
                hexa = townModel.GetHexa(loop1);
                if (hexa.GetKind() != HexaKind.Water &&
                    hexa.GetKind() != HexaKind.Nothing &&
                    hexa.GetKind() != HexaKind.Null)
                {
                    HexaView.SetActiveHexaID(hexa.GetID());
                    break;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (state == EGameState.StateGame)
            {
                if (InputManager.Inst().GetGameAction("game", "selecttown").IsPressed())
                {
                    int id = TownView.GetPickTownID();
                    List<ITown> town = activePlayer.GetTown();

                    int loop1;
                    for (loop1 = 0; loop1 < town.Count; loop1++)
                    {
                        if (id == town[loop1].GetTownID())
                        {
                            if (loop1 == town.Count - 1)
                                TownView.SetPickTownID(-1);
                            else
                                TownView.SetPickTownID(town[loop1 + 1].GetTownID());
                            break;
                        }
                    }
                    if (loop1 == town.Count)
                        TownView.SetPickTownID(town[0].GetTownID());
                }

                if (TownView.GetPickTownID() >= 0 &&
                    InputManager.Inst().GetGameAction("game", "selecthexa").IsPressed())
                {
                    TownModel townModel = map.GetTownByID(TownView.GetPickTownID());
                    int hexaID = HexaView.GetActiveHexaID();
                    HexaModel hexa;

                    /// No hexa is selected
                    if (hexaID == -1)
                    {
                        SetNewActiveHexa();
                    }
                    else
                    {
                        for (byte loop1 = 0; loop1 < 3; loop1++)
                        {
                            hexa = townModel.GetHexa(loop1);
                            if (hexaID == hexa.GetID())
                            {
                                for (int loop2 = loop1 + 1; loop2 < 4; loop2++)
                                {
                                    if (loop2 == 3)
                                    {
                                        SetNewActiveHexa();
                                        break;
                                    }

                                    hexa = townModel.GetHexa(loop2);

                                    if (hexa.GetKind() != HexaKind.Water &&
                                       hexa.GetKind() != HexaKind.Nothing &&
                                       hexa.GetKind() != HexaKind.Null)
                                    {
                                        HexaView.SetActiveHexaID(hexa.GetID());
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }

                if (InputManager.Inst().GetGameAction("game", "activatehexa").IsPressed())
                {
                    if (HexaView.GetActiveHexaID() != -1)
                    {
                        HexaView.ActiveHexaEnter();
                    }
                }
            }


            if (activePlayer.GetIsAI())
            {
                if (!hasAIThreadStarted && !Message.Inst().GetIsActive())
                {
                    actualAIThread = new Thread(X => AIThread(activePlayer.GetComponentAI()), 10000);
                    actualAIThread.Start();
                    actualAITime = AI_TIME;
                    hasAIThreadStarted = true;
                }
                if (hasAIThreadStarted)
                {
                    actualAITime -= gameTime.ElapsedGameTime.Milliseconds;
                    if (actualAITime < 0)
                        actualAIThread.Abort();

                    if (!actualAIThread.IsAlive && map.GetMapView().getIsViewQueueClear() && !Message.Inst().GetIsActive())
                    {
                        NextTurn();
                    }
                }
            }
        }

        public bool GetHasBuiltTown() { return hasBuiltTown; }
        public Player GetActivePlayer() { return activePlayer; }
        public Player GetTargetPlayer() { return targetPlayer; }
        public void SetTargetPlayer(Player p) { targetPlayer = p; }
        public int GetPlayerCount() { return players.Count; }
        public Player GetPlayer(int index) { return players[index]; }
        public Player GetPlayer(String playerName)
        {
            foreach (Player p in players)
            {
                if (p.GetName() == playerName)
                    return p;
            }
            return null;
        }
        public List<Player> GetPlayers() { return this.players; }
        public EGameState GetState() { return state; }
        public EFortState GetFortState() { return fortState; }
        public bool IsWinnerNew() 
        {
            if (Message.Inst().GetIsActive())
                return false;
                
            bool temp = winnerNew; 
            winnerNew = false; 

            return temp; 
        }

        public void SetHasBuiltTown(bool hasBuiltTown) { this.hasBuiltTown = hasBuiltTown; }
        public void SetFortState(EFortState state) { fortState = state; }

        public bool StartTurn(bool collectSources)
        {
            if (state == EGameState.StateFirstTown)
            {
                GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionStart);
            }
            if (state == EGameState.StateGame)
            {
                GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
#if TURN_ON_RANDOM_EVENTS
                if(!Settings.banRandomEvents && turnNumber > 2)
                    RandomEvents();
#endif
                if(collectSources)
                    GameState.map.getSources(activePlayer);

                GameState.map.FreeCapturedHexa(activePlayer);
            }

            hasAIThreadStarted = false;

            return true;
        }

        private void RandomEvents()
        {
            if (randomNumber.Next() % 5 == 2)
            {       
                GameState.map.ApplyEvent(RndEvent.getRandomEvent(randomNumber));
            }
        }

        public bool CanNextTurn()
        {
            if (state != EGameState.StateGame)
            {
                Message.Inst().Show(Strings.GAME_ALERT_TITLE_NEXT_TURN_BAD_STATE, Strings.GAME_ALERT_DESCRIPTION_NEXT_TURN_BAD_STATE, GameResources.Inst().GetHudTexture(HUDTexture.IconTown));
                return false;
            }
            
            if(activePlayer.GetIsAI())
            {
                Message.Inst().Show(Strings.GAME_ALERT_TITLE_AI_IS_THINKING, Strings.GAME_ALERT_DESCRIPTION_AI_IS_THINKING, GameResources.Inst().GetHudTexture(HUDTexture.HammersPassive));
                return false;
            }

            if (Message.Inst().GetIsActive())
            {
                return false;
            }

            if (fortState == EFortState.CapturingHexa)
            {
                return false;
            }

            if (fortState == EFortState.DestroyingHexa)
            {
                return false;
            }

            return true;
        }

        public bool NextTurn()
        {
            bool status = true;
            if (state == EGameState.StateGame)
            {
                MarketComponent.Inst().SetIsActive(false);
            }

            if (playerCount == 0)
            {
                winnerNew = true;
                return true;
            }

            if (activePlayerIndex == players.Count - 1)
                CheckWinner();
            if (winnerNew)
            {
                return true;
            }

            if (state == EGameState.StateGame)
                activePlayer.AddSumSourcesStat();

            status &= ChangeActivePlayer();
            targetPlayer = activePlayer;
            hasBuiltTown = false;

            status &= StartTurn(true);

            map.NextTurn();
            TownView.ResetTownView();
            HexaView.SetActiveHexaID(-1);

            return status;
        }

        public void CheckWinner()
        {
            if (!winnerNew)
            {
                int[] points;
                int maxPoints = -1;
                Player bestPlayer = null;

                
                foreach (Player player in players)
                {
                    points = player.GetPoints();
                    if (points[(int) PlayerPoints.Fort] >= Settings.pointsFort &&
                        points[(int) PlayerPoints.FortCaptureHexa] >= Settings.pointsFortCapture &&
                        points[(int) PlayerPoints.FortParade] >= Settings.pointsFortParade &&
                        points[(int) PlayerPoints.FortStealSources] >= Settings.pointsFortSteal &&
                        points[(int) PlayerPoints.LicenceLvl1] >= Settings.pointsMarketLvl1 &&
                        points[(int) PlayerPoints.LicenceLvl2] >= Settings.pointsMarketLvl2 &&
                        points[(int) PlayerPoints.Market] >= Settings.pointsMarket &&
                        points[(int) PlayerPoints.RoadID] >= Settings.goalRoadID &&
                        points[(int) PlayerPoints.TownID] >= Settings.goalTownID &&
                        points[(int) PlayerPoints.Mill] >= Settings.pointsMill &&
                        points[(int) PlayerPoints.Mine] >= Settings.pointsMine &&
                        points[(int) PlayerPoints.Monastery] >= Settings.pointsMonastery &&
                        points[(int) PlayerPoints.Quarry] >= Settings.pointsQuarry &&
                        points[(int) PlayerPoints.Road] >= Settings.pointsRoad &&
                        points[(int) PlayerPoints.Saw] >= Settings.pointsSaw &&
                        points[(int) PlayerPoints.Stepherd] >= Settings.pointsStepherd &&
                        points[(int) PlayerPoints.Town] >= Settings.pointsTown &&
                        points[(int) PlayerPoints.UpgradeLvl1] >= Settings.pointsUpgradeLvl1 &&
                        points[(int) PlayerPoints.UpgradeLvl2] >= Settings.pointsUpgradeLvl2 &&
                        player.GetCollectSourcesNormal().GetCorn() >= Settings.pointsCorn &&
                        player.GetCollectSourcesNormal().GetMeat() >= Settings.pointsMeat &&
                        player.GetCollectSourcesNormal().GetOre() >= Settings.pointsOre &&
                        player.GetCollectSourcesNormal().GetStone() >= Settings.pointsStone &&
                        player.GetCollectSourcesNormal().GetWood() >= Settings.pointsWood)
                    {
                        winnerNew = true;

                        if (player.GetPointSum() > maxPoints)
                        {
                            maxPoints = player.GetPointSum();
                            bestPlayer = player;
                        }
                    }
                }

                if (winnerNew)
                {
                    Message.Inst().Show(Strings.MESSAGE_TITLE_END_GAME, bestPlayer.GetName() + Strings.MESSAGE_DESCRIPTION_END_GAME_WIN, GameResources.Inst().GetHudTexture(HUDTexture.IconFortParade));  
                } else

                if (turnNumber == Settings.maxTurn)
                {
                    winnerNew = true;
                    Message.Inst().Show(Strings.MESSAGE_TITLE_END_GAME, Strings.MESSAGE_DESCRIPTION_END_GAME_LOOSE1 + Settings.maxTurn + Strings.MESSAGE_DESCRIPTION_END_GAME_LOOSE2, GameResources.Inst().GetHudTexture(HUDTexture.HammersPassive));
                }

                // GENETICS
#if GENETIC             
                if (winnerNew)
                {
                    if (playerCount == 1)
                    {
                        fitness += players[0].GetPointSum() * 5 + 30 - turnNumber + players[0].GetCollectSourcesLastTurn().GetAsArray().Sum() / 100.0f;
                    }
                    else
                    {
                        int winnerID = (players[0].GetPointSum() > players[1].GetPointSum()) ? 0 : 1;
                        int looserID = (winnerID == 0) ? 1 : 0;

                        int genID = 0;

                        lastTurnNumber = turnNumber;
                        lastWinner = false;

                        double fitnessX = 0.0;
                        double fitnessY = 0.0;
                        double fitnessZ = 0.0;

                        if (players[winnerID].GetGen())
                        {
                            genID = winnerID;
                            fitnessX = ((double) players[winnerID].GetPointSum()) / 10/* TODO GetGameSettings().GetPoints()*/ * 50.0;

                            if (turnNumber < Settings.maxTurn)
                            {
                                fitnessY += (players[winnerID].GetPointSum() - players[looserID].GetPointSum()) * 2.0 + 5;
                                fitnessZ += (((double) turnNumber) / Settings.maxTurn) * 5.0;
                                lastWinner = true;
                            }
                        }
                        else if (players[looserID].GetGen())
                        {
                            genID = looserID;
                            fitnessX = ((double) players[looserID].GetPointSum()) / 10 /* TODO GetGameSettings().GetPoints() */ * 50.0;
                        }

                        double fitnessA = (double) players[genID].GetSumSpentSources().GetAsArray().Sum() / 300.0 / turnNumber;
                        double fitnessB = (double) players[genID].GetTown().Count * 10.0 / turnNumber;
                        double fitnessC = (double) players[genID].GetRoad().Count * 5.0 / turnNumber;
                        double fitnessE = (double) players[genID].GetFort().Count * 3.0 / turnNumber;
                        double fitnessF = (double) players[genID].GetMonastery().Count * 3.0 / turnNumber;
                        double fitnessG = (double) players[genID].GetMarket().Count * 3.0 / turnNumber;
                        double fitnessD = (double) players[genID].GetCollectSourcesNormal().GetAsArray().Sum() / 20.0 / turnNumber;
                        fitness += fitnessA;
                        fitness += fitnessB;
                        fitness += fitnessC;
                        fitness += fitnessD;
                        fitness += fitnessE;
                        fitness += fitnessF;
                        fitness += fitnessG;
                        fitness += fitnessX;
                        fitness += fitnessY;
                        fitness += fitnessZ;
                        //if (fitness > 75.0)
                        //    Logger.Inst().Log("Great.txt", fitness + ";" + fitnessA + ";" + fitnessB + ";" + fitnessC + ";" + fitnessD + ";" + fitnessE + ";" + fitnessF + ";" + fitnessG + ";" + fitnessX + ";" + fitnessY + ";" + fitnessZ + ";");
                    }

                    if (gameCount % geneticFitnessRound == 0)
                    {
                        if (fitness < 0.0)
                            fitness = 0.0;
                        
                        genetic.SetFitness(fitness);
                        fitness = 0;
                    }
                }
#endif

                // GENETICS
            }
        }

        public void ChangeStateToStateGame()
        {
            state = EGameState.StateGame;

            Message.Inst().Show(Strings.GAME_ALERT_TITLE_GAME_STARTED, Strings.GAME_ALERT_DESCRIPTION_GAME_STARTED, GameResources.Inst().GetHudTexture(HUDTexture.IconRoad));
            foreach (Player player in players)
            {
                player.AddSources(Settings.startResources, TransactionState.TransactionStart);
            }
        }

        public bool IsPausedNew()
        {
            bool temp = pausedNew;
            pausedNew = false;
            return temp;
        }

        public int GetTurnNumber() { return turnNumber; }
        public int GetTurnsLeft()
        {
            return Settings.maxTurn - turnNumber + 1;
        }

        public bool GetPaused()
        {
            return paused;
        }

        public void SetPausedNew(Boolean paused)
        {
            this.pausedNew = paused;
        }

        public void SetPaused(Boolean paused)
        {
            this.paused = paused;
        }

        public void DoMaterialConversion(SourceKind from, SourceKind to, Player p, int fromAmount, int toAmount)
        {
            if (from == SourceKind.Null || to == SourceKind.Null)
                return;

            int rate = p.GetConversionRate(from);

            if (!this.IsMaterialAvailable(from,rate)) { return; }

            //remove material from player
            SourceAll cost = CreateSourceAllCost(from, -fromAmount);
            p.AddSources(cost,TransactionState.TransactionStart);
            

            //add new material
            SourceAll get = CreateSourceAllCost(to, toAmount);
            p.AddSources(get, TransactionState.TransactionEnd);
        }

        public bool ChangeSourcesFor(SourceAll source)
        {
            return map.GetMapController().ChangeSourcesFor(source);
        }

        public int CanChangeSourcesFor(SourceAll source)
        {
            return map.GetMapController().CanChangeSourcesFor(source);
        }

        /// <summary>
        /// Checks whether user has enough resources from the type he wants to change in market
        /// </summary>
        /// <returns></returns>
        protected bool IsMaterialAvailable(SourceKind from, int rate)
        {
            return GetActivePlayer().HaveEnoughMaterialForConversion(from);
        }


        protected SourceAll CreateSourceAllCost(SourceKind kind, int rate)
        {
            switch (kind)
            {
                case SourceKind.Wood: return new SourceAll(0, 0, 0, rate, 0);
                case SourceKind.Stone: return new SourceAll(0, 0, rate, 0, 0);
                case SourceKind.Corn: return new SourceAll(rate, 0, 0, 0, 0);
                case SourceKind.Meat: return new SourceAll(0, rate, 0, 0, 0);
                case SourceKind.Ore: return new SourceAll(0, 0, 0, 0, rate);
                default : return new SourceAll(0);
            }
        }

        public void AddPlayer(Player p)
        {
            players.Add(p);
            ++playerCount;
        }

        public bool ChangeActivePlayer()
        {
            if (state == EGameState.StateFirstTown || state == EGameState.StateGame)
            {
                activePlayerIndex++;
                if (activePlayerIndex == players.Count)
                {
                    switch (state)
                    {
                        case EGameState.StateFirstTown :
                            activePlayerIndex--;
                            state = EGameState.StateSecondTown;
                            break;
                        case EGameState.StateGame :
                            activePlayerIndex = 0;
                            break;
                    }
                }
            } else {
                activePlayerIndex--;
                if (activePlayerIndex < 0)
                {
                    ChangeStateToStateGame();
                    activePlayerIndex = 0;
                }
            }

            activePlayer = players[activePlayerIndex];
            if (!activePlayer.GetIsAI())
                lastHumanPlayer = activePlayer;

            if (state == EGameState.StateGame && activePlayerIndex == 0)
                turnNumber++;

            if (!activePlayer.GetActive())
                return ChangeActivePlayer();

            return true;
        }

        public String GetMedalTitle(Building building)
        {
            switch (building)
            {
                case Building.Town: return Strings.MESSAGE_TITLE_MEDAL_TOWN;
                case Building.Road: return Strings.MESSAGE_TITLE_MEDAL_ROAD;
                case Building.Market: return Strings.MESSAGE_TITLE_MEDAL_MARKET;
                case Building.Monastery: return Strings.MESSAGE_TITLE_MEDAL_MONASTERY;
                case Building.Fort: return Strings.MESSAGE_TITLE_MEDAL_FORT;
                case Building.Saw: return Strings.MESSAGE_TITLE_MEDAL_SAW;
                case Building.Mill: return Strings.MESSAGE_TITLE_MEDAL_MILL;
                case Building.Quarry: return Strings.MESSAGE_TITLE_MEDAL_QUARRY;
                case Building.Stepherd: return Strings.MESSAGE_TITLE_MEDAL_STEPHERD;
                case Building.Mine: return Strings.MESSAGE_TITLE_MEDAL_MINE;
            }
            return null;
        }

        public String GetMedalDescription(Building building)
        {
            switch (building)
            {
                case Building.Town: return Strings.MESSAGE_DESCRIPTION_MEDAL_TOWN;
                case Building.Road: return Strings.MESSAGE_DESCRIPTION_MEDAL_ROAD;
                case Building.Market: return Strings.MESSAGE_DESCRIPTION_MEDAL_MARKET;
                case Building.Monastery: return Strings.MESSAGE_DESCRIPTION_MEDAL_MONASTERY;
                case Building.Fort: return Strings.MESSAGE_DESCRIPTION_MEDAL_FORT;
                case Building.Saw: return Strings.MESSAGE_DESCRIPTION_MEDAL_SAW;
                case Building.Mill: return Strings.MESSAGE_DESCRIPTION_MEDAL_MILL;
                case Building.Quarry: return Strings.MESSAGE_DESCRIPTION_MEDAL_QUARRY;
                case Building.Stepherd: return Strings.MESSAGE_DESCRIPTION_MEDAL_STEPHERD;
                case Building.Mine: return Strings.MESSAGE_DESCRIPTION_MEDAL_MINE;
            }
            return null;
        }

        public Texture2D GetMedaileIcon(Building building)
        {
            Texture2D icon = null;
            switch (building)
            {
                case Building.Road: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalRoad); break;
                case Building.Town: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalTown); break;
                case Building.Mill: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalMill); break;
                case Building.Market: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalMarket); break;
                case Building.Monastery: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalMonastery); break;
                case Building.Fort: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalFort); break;
                case Building.Stepherd: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalStepherd); break;
                case Building.Quarry: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalQuarry); break;
                case Building.Mine: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalMine); break;
                case Building.Saw: icon = GameResources.Inst().GetHudTexture(HUDTexture.IconMedalSaw); break;
            }

            return icon;
        }

        public void Draw2D()
        {
            SpriteBatch spriteBatch = GameState.spriteBatch;
            Vector2 medailPosition = new Vector2(Settings.activeResolution.X - 90.0f, 80.0f);
            spriteBatch.Begin();
            for (int loop1 = 0; loop1 < medailOwner.Length; loop1++)
            {
                if (medailOwner[loop1] == targetPlayer)
                {
                    spriteBatch.Draw(GetMedaileIcon((Building) loop1), medailPosition, Color.White);
                    medailPosition += new Vector2(0.0f, 85.0f);
                }
            }

            spriteBatch.End();

            Tutorial.Inst().Draw(Layer.Layer1);
        }

        public void DrawGeneticInfo()
        {
            SpriteBatch spriteBatch = GameState.spriteBatch;
            spriteBatch.Begin();
#if GENETIC
            
            int x = 15;
            int y = 200;
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), ((lastWinner) ? "Winner" : "Looser") + "  " + lastTurnNumber, new Vector2(x, y - 30), Color.Black);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), genetic.GetGenerationNumber() + "", new Vector2(x, y + 2), Color.Black);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), genetic.GetChromozonID() + "", new Vector2(x, y + 32), Color.Black);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), ((int) (genetic.GetLastFitness()  * 1000)) / 1000.0 + "", new Vector2(x, y + 62), Color.Black);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), TimeSpan.FromMilliseconds(genetic.GetTime()).Hours + ":" + TimeSpan.FromMilliseconds(genetic.GetTime()).Minutes + ":" + TimeSpan.FromMilliseconds(genetic.GetTime()).Seconds, new Vector2(x, y + 92), Color.Black);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((int)(genetic.GetExtinction() * 1000)) / 1000.0 + "", new Vector2(x, y + 122), Color.Black);
            
            x = 13;
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), ((lastWinner) ? "Winner" : "Looser") + "  " + lastTurnNumber, new Vector2(x, y - 30), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), genetic.GetGenerationNumber() + "", new Vector2(x, y), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), genetic.GetChromozonID() + "", new Vector2(x, y + 30), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), ((int)(genetic.GetLastFitness() * 1000)) / 1000.0 + "", new Vector2(x, y + 60), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), TimeSpan.FromMilliseconds(genetic.GetTime()).Hours + ":" + TimeSpan.FromMilliseconds(genetic.GetTime()).Minutes + ":" + TimeSpan.FromMilliseconds(genetic.GetTime()).Seconds, new Vector2(x, y + 90), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), ((int)(genetic.GetExtinction() * 1000)) / 1000.0 + "", new Vector2(x, y + 122), Color.White);
            
            
#endif
            spriteBatch.End();
        }

        public double GetRandomNumber()
        {
            return randomNumber.NextDouble();
        }

        public int GetRandomInt(int bound)
        {
            return randomNumber.Next(bound);
        }

        internal void RestartGame()
        {
#if CHANGE_MAP_EVERY_TURN
            map.Initialize(true);
#else
            map.Initialize(false);
#endif

#if CHANGE_FIRST_2_PLAYERS
            // GENETIC
            Player tempPlayer = players[0];
            players[0] = players[1];
            players[1] = tempPlayer;

            if (players.Count == 4)
            {
                tempPlayer = players[2];
                players[2] = players[3];
                players[3] = tempPlayer;
            }
#endif

            StartGame();
        }

#if GENETIC
        internal void PrintGenetic()
        {
            genetic.PrintAllChromozones();
        }
#endif


        internal void ResetGenetic()
        {
            gameCount = 0;
#if GENETIC
            bool[] bans = new bool[22];
            for (int loop1 = 0; loop1 < 2; loop1++)
            {
                if (Settings.banFort)
                {
                    bans[11 * loop1 + 3] = true;
                    bans[11 * loop1 + 4] = true;
                    bans[11 * loop1 + 9] = true;
                    bans[11 * loop1 + 10] = true;
                }
                if (Settings.banMarket)
                {
                    bans[11 * loop1 + 5] = true;
                    bans[11 * loop1 + 8] = true;
                }
                if (Settings.banMonastery)
                {
                    bans[11 * loop1 + 6] = true;
                    bans[11 * loop1 + 7] = true;
                }
                if (Settings.banFortCaptureHexa)
                {
                    bans[11 * loop1 + 10] = true;
                }
                if (Settings.banFortParade)
                {
                    bans[11 * loop1 + 4] = true;
                }
                if (Settings.banFortStealSources)
                {
                    bans[11 * loop1 + 9] = true;
                }
            }

            lastWinner = false;
            lastTurnNumber = 0;
            fitness = 0.0;
            geneticPopulation = 10;
            geneticFitnessRound = 1;
            bosses = null;
            bosses = new int[1][][];
            bosses[0] = Genetic.ParsePersonality("346;  5; 88;  7;;397; 66;;609; 51; 21; 28;;105; 32; 18; 39; 11;;603; 65;;231; 43; 13; 10; 12; 22;; 67;  7; 46; 28; 15;  4;;338; 87;;535; 46;; 11; 73;;355; 85;;292;  1; 60; 39;;971;  7;;167; 64; 18; 18;;225; 18; 22; 32; 28;;179; 20;;656; 24; 28; 20; 10; 18;;977; 36;  6; 12; 27; 19;;340; 29;;546; 61;;337; 82;; 43; 27;;");
            
            if (bosses != null)
                geneticFitnessRound = bosses.Length;

            genetic = new Genetic(geneticPopulation, 0.95, 0.1, 1, 20000, 20, 1.8, false, bans);
#endif
        }

        internal void SetMap(Map map)
        {
            this.map = map;
        }

        internal Map GetMap()
        {
            return map;
        }

        internal void InitPlayers()
        {
            foreach (Player player in players)
            {
                player.InitNewGame();
            }
        }
    }
}
