//#define CHANGE_FIRST_2_PLAYERS
//#define CHANGE_MAP_EVERY_TURN
//#define TURN_ON_RANDOM_EVENTS

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

        private int activePlayerIndex;
        private EGameState state;
        private EFortState fortState;
        private bool hasBuiltTown;          /// In first 2 rounds each player can build only one town

        // when is game paused and player see paused menu, he cant build towers etc
        private bool paused;
        // used for open paused menu
        private bool pausedNew;

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

        public bool StartGame(Map map)
        {
            this.map = map;
            gameCount++;

            playerCount = players.Count;

            medailOwner = new Player[(int) Building.Count];
            for (int loop1 = 0; loop1 < players.Count; loop1++)
            {
                medailOwner[loop1] = null;
            }

            IComponentAI AI;
            int[][] setArray;
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
                    }

                    if (!players[loop1].GetGen())
                    {
                        setArray = genetic.GetBestOnePersonality();
                    }
                    // GENETICS 
#endif

                    AI.InitAIComponent(map.GetMapController(), setArray);
                }
            }
            
            activePlayerIndex = 0;
            activePlayer = players.ElementAt(activePlayerIndex);
            targetPlayer = activePlayer;

            pausedNew = false;
            paused = false;
            winnerNew = false;

            hasAIThreadStarted = false;
            state = EGameState.StateFirstTown;
            fortState = EFortState.Normal;

            randomNumber = new System.Random();

            hasBuiltTown = false;

            turnNumber = 0;

            PromptWindow.Inst().Deactive();
            Message.Inst().ClearMessages();
            MarketComponent.Inst().SetIsActive(false);
            PathNode.SetIsValid(false);

            return true;
        }

        public void PlayerWantMedail(Player player, Building medal)
        {
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
                    medailOwner[(int)medal].AddPoints(-pointsForMedal);
                    medailOwner[(int)medal].GetStatistic().AddStat(Statistic.Kind.Medals, -1, turnNumber);
                }
                medailOwner[(int)medal] = player;
                medailOwner[(int)medal].GetStatistic().AddStat(Statistic.Kind.Medals, 1, turnNumber);
                player.AddPoints(pointsForMedal);
                Message.Inst().Show(GetMedalTitle(medal),GetMedalDescription(medal),GetMedaileIcon(medal));
            }
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
            XmlNodeList points = xDoc.GetElementsByTagName("points")[0].ChildNodes;
            foreach (XmlNode goal in points)
            {
                int value = Convert.ToInt32(goal.InnerText);
                switch (goal.Name)
                {
                    case "WinPoints": GameMaster.Inst().GetGameSettings().SetPoints(value); break;
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
                }
            }

            XmlNodeList ban = xDoc.GetElementsByTagName("ban")[0].ChildNodes;
            foreach (XmlNode action in ban)
            {
                bool value = (Convert.ToInt32(action.InnerText) == 0) ? false : true;
                switch (action.Name)
                {
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

            foreach (XmlNode player in playersNodes)
            {
                playerName = "";
                playerColor = Color.Black;
                playerAI = null;
                playerOrder = 0;
                playerPersonality = null;
                playerGen = false;

                foreach (XmlNode info in player.ChildNodes)
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
                            playerPersonality = new int[11][];
                            playerPersonality[0] = new int[4];
                            playerPersonality[1] = new int[2];
                            playerPersonality[2] = new int[4];
                            playerPersonality[3] = new int[5];
                            playerPersonality[4] = new int[2];
                            playerPersonality[5] = new int[6];
                            playerPersonality[6] = new int[6];
                            playerPersonality[7] = new int[2];
                            playerPersonality[8] = new int[2];
                            playerPersonality[9] = new int[2];
                            playerPersonality[10] = new int[2];
                            
                            String [] koef = info.InnerText.Split(";".ToCharArray());
                            int koefID = 0;
                            for (int loop1 = 0; loop1 < playerPersonality.Length; loop1++)
                            {
                                for (int loop2 = 0; loop2 < playerPersonality[loop1].Length; loop2++)
                                {
                                    while (koefID < koef.Length - 1 && koef[koefID] == "")
                                        koefID++;

                                    playerPersonality[loop1][loop2] = Convert.ToInt32(koef[koefID++]);
                                }
                            }

                            break;
                    }
                }
                players.Add(new Player(playerName, playerColor, playerAI, playerOrder, playerPersonality, playerGen));
            }
        }

        internal void PrepareCampaignMap(string mapName)
        {
            mapSource = mapName;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Content/Maps/" + mapSource);

            PrepareCampaignPlayers(xDoc);
            PrepareCampaignSetup(xDoc);
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

        public bool StartTurn()
        {
            if (state == EGameState.StateFirstTown)
            {
                GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionStart);
            }
            if (state == EGameState.StateGame)
            {
                GetActivePlayer().AddSources(new SourceAll(0), TransactionState.TransactionEnd);
#if TURN_ON_RANDOM_EVENTS
                RandomEvents();
#endif
                GameState.map.getSources(activePlayer);
                GameState.map.FreeCapturedHexa(activePlayer);
            }

            hasAIThreadStarted = false;

            return true;
        }

        private void RandomEvents()
        {
            if (randomNumber.Next() % 10 == 2)
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

            status &= StartTurn();

            map.NextTurn();
            TownView.ResetTownView();
            HexaView.SetActiveHexaID(-1);

            return status;
        }

        public void CheckWinner()
        {
            if (!winnerNew)
            {
                int points;
                int maxPoints = -1;
                Player bestPlayer = null;

                
                foreach (Player player in players)
                {
                    points = player.GetPoints();
                    if (points >= GetGameSettings().GetPoints())
                    {
                        winnerNew = true;

                        if (points > maxPoints)
                        {
                            maxPoints = points;
                            bestPlayer = player;
                        }
                    }
                }

                if (winnerNew && turnNumber < Settings.maxTurn)
                {
                    Message.Inst().Show(Strings.MESSAGE_TITLE_END_GAME, bestPlayer.GetName() + Strings.MESSAGE_DESCRIPTION_END_GAME_WIN, GameResources.Inst().GetHudTexture(HUDTexture.IconFortParade));  
                }

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
                        fitness += players[0].GetPoints() * 5 + 30 - turnNumber + players[0].GetCollectSourcesLastTurn().GetAsArray().Sum() / 100.0f;
                    }
                    else
                    {
                        int winnerID = (players[0].GetPoints() > players[1].GetPoints()) ? 0 : 1;
                        int looserID = (winnerID == 0) ? 1 : 0;

                        double add;
                        int genID = 0;
                        if (players[winnerID].GetGen())
                        {
                            genID = winnerID;
                            add = (2 * players[winnerID].GetPoints() - players[looserID].GetPoints()) * 10;

                            if (turnNumber < Settings.maxTurn)
                                fitness += 30.0 + add;
                        }
                        else if (players[looserID].GetGen())
                        {
                            genID = looserID;
                            add = (2 * players[looserID].GetPoints() -  players[winnerID].GetPoints()) * 10;
                            if(add > 0.0)
                                fitness += add;
                        }
                        fitness += (double) players[genID].GetTown().Count * 5.0 / turnNumber;
                        fitness += (double) players[genID].GetRoad().Count * 1.0 / turnNumber;
                        fitness += (double) players[genID].GetCollectSourcesNormal().GetAsArray().Sum() / 40.0 / turnNumber;
                    }

                    if (gameCount % geneticFitnessRound == 0)
                    {
                        //fitness -= 8;
                        if (fitness < 0.0)
                            fitness = 0.0;
                        fitness /= 10.0f;
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
        }

        public void DrawGeneticInfo()
        {
#if GENETIC
            SpriteBatch spriteBatch = GameState.spriteBatch;
            spriteBatch.Begin();
            int x = 15;
            int y = 200;
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), genetic.GetGenerationNumber() + "", new Vector2(x, y + 2), Color.Black);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), genetic.GetChromozonID() + "", new Vector2(x, y + 32), Color.Black);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), ((int) (genetic.GetLastFitness()  * 1000)) / 1000.0 + "", new Vector2(x, y + 62), Color.Black);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), TimeSpan.FromMilliseconds(genetic.GetTime()).Minutes + ":" + TimeSpan.FromMilliseconds(genetic.GetTime()).Seconds, new Vector2(x, y + 92), Color.Black);
            x = 13;
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), genetic.GetGenerationNumber() + "", new Vector2(x, y), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), genetic.GetChromozonID() + "", new Vector2(x, y + 30), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalBig), ((int)(genetic.GetLastFitness() * 1000)) / 1000.0 + "", new Vector2(x, y + 60), Color.White);
            spriteBatch.DrawString(GameResources.Inst().GetFont(EFont.MedievalSmall), TimeSpan.FromMilliseconds(genetic.GetTime()).Minutes + ":" + TimeSpan.FromMilliseconds(genetic.GetTime()).Seconds, new Vector2(x, y + 90), Color.White);
            
            spriteBatch.End();
#endif
        }

        public double GetRandomNumber()
        {
            return randomNumber.NextDouble();
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

            StartGame(map);
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
            fitness = 0.0;
            geneticPopulation = 100;
            geneticFitnessRound = 1;
            genetic = new Genetic(geneticPopulation, 0.30, 0.05, 1, 2000, 10, 2.0);
#endif
        }
    }
}
