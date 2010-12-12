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

namespace Expanze
{
    class GameMaster
    {
        private int n_player = 0;
        private List<Player> players = new List<Player>();

        private Player[] medailOwner;

        private Player activePlayer;
        private int activePlayerIndex;
        private EGameState state;
        // when is game paused and player see paused menu, he cant build towers etc
        private bool paused;
        // used for open paused menu
        private bool pausedNew;

        private bool winnerNew;

        private Map map;

        private ThreadStart actualAIStart;
        private Thread actualAIThread;
        private const int AI_TIME = 5000;   // this is time which have each plugin each turn to resolve AI
        private int actualAITime;           // how much time has AI before it will be aborted
        private bool hasAIThreadStarted;

        private Random randomNumber;

        private static GameMaster instance = null;

        private GameSettings gameSettings;

        public static GameMaster getInstance()
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
        private GameMaster() {}

        public bool StartGame(bool isAI, Map map)
        {
            this.map = map;

            n_player = players.Count;

            medailOwner = new Player[(int) Building.Count];
            for (int loop1 = 0; loop1 < players.Count; loop1++)
            {
                medailOwner[loop1] = null;
            }

            IComponentAI AI;
            foreach (Player player in players)
            {
                AI = player.getComponentAI();
                if (AI != null) // its computer then
                {
                    AI.InitAIComponent(map.GetMapController());
                }
            }
            
            activePlayerIndex = 0;
            activePlayer = players.ElementAt(activePlayerIndex);

            pausedNew = false;
            paused = false;
            winnerNew = false;

            hasAIThreadStarted = false;
            state = EGameState.StateFirstTown;

            randomNumber = new System.Random();

            return true;
        }

        public void PlayerWantMedail(Player player, Building medal)
        {
            int minCount = 10;
            switch(medal)
            {
                case Building.Town :
                    minCount = 5;
                    break;
                case Building.Road :
                    minCount = 10;
                    break;
                default :
                    minCount = 3;
                    break;
            }

            int pointsForMedail = 15;

            if((medailOwner[(int) medal] == null && player.getBuildingCount(medal) >= minCount) ||
               (medailOwner[(int) medal] != null && player.getBuildingCount(medal) > medailOwner[(int) medal].getBuildingCount(medal)))
            {
                if (medailOwner[(int)medal] != null)
                {
                    medailOwner[(int)medal].addPoints(-pointsForMedail);
                }
                medailOwner[(int)medal] = player;
                player.addPoints(pointsForMedail);
                GameState.message.showAlert(getMedalTitle(medal),getMedalDescription(medal),getMedaileIcon(medal));
            }
        }

        public void setGameSettings(int points, string mapType, string mapSize, string mapWealth)
        {
            gameSettings = new GameSettings(points,mapType,mapSize,mapWealth);
        }

        public void PrepareQuickGame()
        {
            this.resetGameSettings();
            players.Clear();
            this.players.Add(new Player("Monte Carlos", Color.Red,null));
            this.players.Add(new Player("Calamity Jain", Color.Blue, null));
        }

        public GameSettings getGameSettings()
        {
            if (gameSettings != null)
            {
                return gameSettings;
            }
            else
            {
                return new GameSettings(50,"normální","malá","střední");
            }
        }

        public void resetGameSettings()
        {
            gameSettings = null;
        }

        public void deleteAllPlayers()
        {
            players = new List<Player>();
            n_player = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (activePlayer.getIsAI())
            {
                if (!hasAIThreadStarted && !GameState.message.getIsActive())
                {
                    actualAIStart = new ThreadStart(activePlayer.getComponentAI().ResolveAI);
                    actualAIThread = new Thread(actualAIStart);
                    actualAIThread.Start();
                    actualAITime = AI_TIME;
                    hasAIThreadStarted = true;
                }
                if (hasAIThreadStarted)
                {
                    actualAITime -= gameTime.ElapsedGameTime.Milliseconds;
                    if (actualAITime < 0)
                        actualAIThread.Abort();

                    if (!actualAIThread.IsAlive && map.GetMapView().getIsViewQueueClear() && !GameState.message.getIsActive())
                    {
                        NextTurn();
                    }
                }
            }
        }

        public Player getActivePlayer() { return activePlayer; }
        public int getPlayerCount() { return players.Count; }
        public Player getPlayer(int index) { return players[index]; }

        public List<Player> getPlayers()
        {
            return this.players;
        }

        public EGameState getState() { return state; }
        public bool isWinnerNew() { bool temp = winnerNew; winnerNew = false; return temp; }

        public bool StartTurn()
        {
            if (state == EGameState.StateGame)
            {
                GameState.map.getSources(activePlayer);
                RandomEvents();
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

        public bool NextTurn()
        {
            bool status = true;
            if (state == EGameState.StateGame)
            {
                //checkWinner(activePlayer);
            }
            status &= changeActivePlaye();

            status &= StartTurn();

            map.NextTurn();

            return status;
        }

        public void CheckWinner(Player player)
        {
            bool isWinner = false;
            if (player.getPoints() >= getGameSettings().getPoints())
            {
                isWinner = true;
            }

            if (isWinner)
            {
                winnerNew = true;
            }
        }

        public void ChangeStateToStateGame()
        {
            state = EGameState.StateGame;
            foreach (Player player in players)
            {
                player.addSources(Settings.startResources, TransactionState.TransactionStart);
            }
        }

        public bool isPausedNew()
        {
            if (this.pausedNew)
            {
                this.pausedNew = false;
                return true;
            }
            return false;
        }

        public bool getPaused()
        {
            return paused;
        }

        public void setPausedNew(Boolean paused)
        {
            this.pausedNew = paused;
        }

        public void setPaused(Boolean paused)
        {
            this.paused = paused;
        }

        public void doMaterialConversion(HexaKind from, HexaKind to, Player p, int fromAmount, int toAmount)
        {
            int rate = p.getConversionRate(from);

            if (!this.isMaterialAvailable(from,rate)) { return; }

            //remove material from player
            SourceAll cost = createSourceAllCost(from, -fromAmount);
            p.addSources(cost,TransactionState.TransactionStart);
            

            //add new material
            SourceAll get = createSourceAllCost(to, toAmount);
            p.addSources(get, TransactionState.TransactionEnd);
        }

        /// <summary>
        /// Checks whether user has enough resources from the type he wants to change in market
        /// </summary>
        /// <returns></returns>
        protected bool isMaterialAvailable(HexaKind from, int rate)
        {
            if (from == HexaKind.Cornfield)
            {
                return (getActivePlayer().getCorn() >= rate) ? true : false;
            }
            else if ( from == HexaKind.Pasture )
            {
                return (getActivePlayer().getMeat() >= rate) ? true : false;
            } 
            else if( from == HexaKind.Mountains ) 
            {
                return (getActivePlayer().getOre() >= rate) ? true : false;
            }
            else if( from == HexaKind.Stone ) 
            {
                return (getActivePlayer().getStone() >= rate) ? true : false;
            }
            else if ( from == HexaKind.Forest )
            {
                return (getActivePlayer().getWood() >= rate) ? true : false;
            }
            else
            {
                return false;
            }
        }


        protected SourceAll createSourceAllCost(HexaKind kind, int rate)
        {
            if (kind == HexaKind.Forest)
            {
                return new SourceAll(rate, 0, 0, 0, 0);
            }
            else if (kind == HexaKind.Stone)
            {
                return new SourceAll(0, rate, 0, 0, 0);
            }
            else if (kind == HexaKind.Cornfield)
            {
                return new SourceAll(0, 0, rate, 0, 0);
            }
            else if (kind == HexaKind.Pasture)
            {
                return new SourceAll(0, 0, 0, rate, 0);
            }
            else if (kind == HexaKind.Mountains)
            {
                return new SourceAll(0, 0, 0, 0, rate);
            }
            else
            {
                return new SourceAll(0, 0, 0, 0, 0);
            }
        }

        public void addPlayer(Player p)
        {
            players.Add(p);
            ++n_player;
        }

        public bool changeActivePlaye()
        {
            if (state == EGameState.StateFirstTown || state == EGameState.StateGame)
            {
                activePlayerIndex++;
                if (activePlayerIndex == n_player)
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

            return true;
        }

        public String getMedalTitle(Building building)
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

        public String getMedalDescription(Building building)
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

        public Texture2D getMedaileIcon(Building building)
        {
            Texture2D icon = null;
            switch (building)
            {
                case Building.Road: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalRoad); break;
                case Building.Town: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalTown); break;
                case Building.Mill: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalMill); break;
                case Building.Market: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalMarket); break;
                case Building.Monastery: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalMonastery); break;
                case Building.Fort: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalFort); break;
                case Building.Stepherd: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalStepherd); break;
                case Building.Quarry: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalQuarry); break;
                case Building.Mine: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalMine); break;
                case Building.Saw: icon = GameResources.Inst().getHudTexture(HUDTexture.IconMedalSaw); break;
            }

            return icon;
        }

        public void Draw2D()
        {
            SpriteBatch spriteBatch = GameState.spriteBatch;
            Vector2 medailPosition = new Vector2(Settings.activeResolution.X - 80.0f, 100.0f);
            spriteBatch.Begin();
            for (int loop1 = 0; loop1 < medailOwner.Length; loop1++)
            {
                if (medailOwner[loop1] == activePlayer)
                {
                    spriteBatch.Draw(getMedaileIcon((Building) loop1), medailPosition, Color.White);
                    medailPosition += new Vector2(0.0f, 85.0f);
                }
            }
            spriteBatch.End();
        }
    }
}
