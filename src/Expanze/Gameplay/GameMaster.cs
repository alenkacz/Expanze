using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;
using Expanze.Gameplay.Map;
using System.Threading;
using Expanze.Gameplay;

namespace Expanze
{
    class GameMaster
    {
        private int n_player = 0;
        private List<Player> players = new List<Player>();
        private Player activePlayer;
        private int activePlayerIndex;
        private EGameState state;
        // when is game paused and player see paused menu, he cant build towers etc
        private bool paused = false;
        // used for open paused menu
        private bool pausedNew = false;

        private bool winnerNew = false;

        private Map map;

        private ThreadStart actualAIStart;
        private Thread actualAIThread;
        private const int AI_TIME = 5000;   // this is time which have each plugin each turn to resolve AI
        private int actualAITime;           // how much time has AI before it will be aborted


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

        public bool startGame(bool isAI, Map map)
        {
            this.map = map;

            n_player = players.Count;

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

            state = EGameState.StateFirstTown;

            return true;
        }

        public void setGameSettings(int points, string mapType, string mapSize, string mapWealth)
        {
            gameSettings = new GameSettings(points,mapType,mapSize,mapWealth);
        }

        public void prepareQuickGame()
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
                actualAITime -= gameTime.ElapsedGameTime.Milliseconds;
                if (actualAITime < 0)
                    actualAIThread.Abort();

                if (!actualAIThread.IsAlive && map.GetMapView().getIsViewQueueClear())
                {
                       NextTurn();
                }
            }
        }

        public Player getActivePlayer() { return activePlayer; }
        public int getPlayerCount() { return players.Count; }
        public Player getPlayer(int index) { return players[index]; }

        public EGameState getState() { return state; }
        public bool isWinnerNew() { bool temp = winnerNew; winnerNew = false; return temp; }

        public bool StartTurn()
        {
            RandomEvents();
            if (state == EGameState.StateGame)
            {
                GameState.map.getSources(activePlayer);
                RandomEvents();
            }

            if (activePlayer.getIsAI())
            {
                actualAIStart = new ThreadStart(activePlayer.getComponentAI().ResolveAI);
                actualAIThread = new Thread(actualAIStart);
                actualAIThread.Start();
                actualAITime = AI_TIME;
            }

            return true;
        }

        private void RandomEvents()
        {
            Random random = new System.Random();
            if (random.Next() % 2 == 0)
            {
                RndEvent.getRandomEvent();
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

        public void checkWinner(Player player)
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

        public void changeStateToStateGame()
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
                    changeStateToStateGame();
                    activePlayerIndex = 0;
                }
            }

            activePlayer = players[activePlayerIndex];

            return true;
        }
    }
}
