using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CorePlugin;
using Expanze.Gameplay.Map;

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

        IComponentAI componentAI;

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

            players[0] = new Player(Settings.startScore, "Player1", Color.RoyalBlue, false);
            players[1] = new Player(Settings.startScore, "Player2", Color.Red, isAI);

            foreach (IComponentAI AI in CoreProviderAI.AI)
            {
                componentAI = AI;
                componentAI.InitAIComponent(map);
            }

            activePlayerIndex = 0;
            activePlayer = players[activePlayerIndex];

            pausedNew = false;
            paused = false;

            state = EGameState.StateFirstTown;
            return true;
        }

        public void Update()
        {
            if (activePlayer.getIsAI())
            {
                componentAI.ResolveAI();
                if (state == EGameState.StateGame)
                    nextTurn();
            }
        }

        public Player getActivePlayer() { return activePlayer; }
        public EGameState getState() { return state; }
        public bool isWinnerNew() { bool temp = winnerNew; winnerNew = false; return temp; }

        public bool nextTurn()
        {
            bool status = true;
            if (state == EGameState.StateGame)
            {
                //checkWinner(activePlayer);
            }
            status &= changeActivePlaye();

            if (state == EGameState.StateGame)
            {
                GameState.map.getSources(activePlayer);

            }
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
