using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;
using Expanze.Gameplay.Map;
using System.Threading;

namespace Expanze
{
    class GameMaster
    {
        private const int n_player = 2;
        private Player[] players = new Player[n_player];
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

            IComponentAI componentAI = null;
            foreach (IComponentAI AI in CoreProviderAI.AI)
            {
                componentAI = AI;
                //componentAI.InitAIComponent(map);
            }
            IComponentAI componentAI1 = componentAI.Clone();
            componentAI1.InitAIComponent(map);
            IComponentAI componentAI2 = componentAI.Clone();
            componentAI2.InitAIComponent(map);

            players[0] = new Player("Player1", Color.RoyalBlue, (isAI) ? componentAI1 : null);
            players[1] = new Player("Player2", Color.Red, (isAI) ? componentAI2 : null);
            
            activePlayerIndex = 0;
            activePlayer = players[activePlayerIndex];

            pausedNew = false;
            paused = false;

            state = EGameState.StateFirstTown;

            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (activePlayer.getIsAI())
            {
                actualAITime -= gameTime.ElapsedGameTime.Milliseconds;
                if (actualAITime < 0)
                    actualAIThread.Abort();

                if (!actualAIThread.IsAlive && map.getIsViewQueueClear())
                {
                       NextTurn();
                }
            }
        }

        public Player getActivePlayer() { return activePlayer; }
        public EGameState getState() { return state; }
        public bool isWinnerNew() { bool temp = winnerNew; winnerNew = false; return temp; }

        public bool StartTurn()
        {
            if (state == EGameState.StateGame)
            {
                GameState.map.getSources(activePlayer);
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
            if (Settings.costWin.HasPlayerSources(player))
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
                player.addSources(new SourceAll(100), TransactionState.TransactionStart);
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
