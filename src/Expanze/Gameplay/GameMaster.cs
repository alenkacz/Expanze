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
        }

        public void AddToPlayerCount(int i) { playerCount += i; }

        public bool StartGame(Map map)
        {
            this.map = map;

            playerCount = players.Count;

            medailOwner = new Player[(int) Building.Count];
            for (int loop1 = 0; loop1 < players.Count; loop1++)
            {
                medailOwner[loop1] = null;
            }

            IComponentAI AI;
            foreach (Player player in players)
            {
                AI = player.GetComponentAI();
                if (AI != null) // its computer then
                {
                    AI.InitAIComponent(map.GetMapController());
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
            int pointsForMedal = 5;
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

        public void SetGameSettings(int points, string mapType, string mapSize, string mapWealth)
        {
            gameSettings = new GameSettings(points,mapType,mapSize,mapWealth);
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
                this.players.Add(new Player(Strings.MENU_HOT_SEAT_NAMES[secondPlayerNameID], Color.Blue, componentAICopy));
            } else
                this.players.Add(new Player(Strings.MENU_HOT_SEAT_NAMES[secondPlayerNameID], Color.Blue, null));

            this.players.Add(new Player(Strings.MENU_HOT_SEAT_NAMES[firstPlayerNameID], Color.Red, null));
        }

        public GameSettings GetGameSettings()
        {
            if (gameSettings != null)
            {
                return gameSettings;
            }
            else
            {
                return new GameSettings(Settings.winPoints[0], Strings.GAME_SETTINGS_MAP_TYPE_NORMAL, Strings.GAME_SETTINGS_MAP_SIZE_SMALL, Strings.GAME_SETTINGS_MAP_WEALTH_MEDIUM);
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
                    actualAIThread = new Thread(X => AIThread(activePlayer.GetComponentAI()));
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
                RandomEvents();
                GameState.map.getSources(activePlayer);
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

            if (playerCount == 1)
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

                if (winnerNew)
                {
                    Message.Inst().Show("Konec hry", bestPlayer.GetName() + " nejrychleji expandoval a ostatní ho uznali za nejvhodnějšího vládce ostrova.", GameResources.Inst().GetHudTexture(HUDTexture.IconFortParade));  
                }

                if (turnNumber == 100)
                {
                    winnerNew = true;
                    Message.Inst().Show("Konec hry", "Nikdo nevyhrál, všichni prohráli. Nikomu nestačilo 100 kol k zisku dostatečného počtu bodů.", GameResources.Inst().GetHudTexture(HUDTexture.HammersPassive));
                }
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
    }
}
