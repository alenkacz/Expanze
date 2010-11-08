using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Expanze.AI;

namespace Expanze
{
    class GameMaster
    {
        public enum State {StateFirstTown, StateSecondTown, StateGame};

        private const int n_player = 2;
        private Player[] players = new Player[n_player];
        private Player activePlayer;
        private int activePlayerIndex;
        private State state;
        // when is game paused and player see paused menu, he cant build towers etc
        private bool paused = false;
        // used for open paused menu
        private bool pausedNew = false;

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
        private GameMaster() { }

        public bool startGame(bool isAI)
        {
            players[0] = new Player(Settings.startScore, "Player1", Color.RoyalBlue, false);
            players[1] = new Player(Settings.startScore, "Player2", Color.Red, isAI);

            componentAI = new NoobAI();

            activePlayerIndex = 0;
            activePlayer = players[activePlayerIndex];

            pausedNew = false;
            paused = false;

            state = State.StateFirstTown;
            return true;
        }

        public void Update()
        {
            if (activePlayer.getIsAI())
            {
                componentAI.ResolveAI(GameState.map);
            }
        }

        public Player getActivePlayer() { return activePlayer; }
        public State getState() { return state; }

        public bool nextTurn()
        {
            bool status = true;
            status &= changeActivePlaye();

            if(state == GameMaster.State.StateGame)
            GameState.map.getSources(activePlayer);
            return status;
        }

        public void changeStateToStateGame()
        {
            state = State.StateGame;
            foreach (Player player in players)
            {
                player.addSources(100, 100, 100, 100, 100);
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
            if (state == State.StateFirstTown || state == State.StateGame)
            {
                activePlayerIndex++;
                if (activePlayerIndex == n_player)
                {
                    switch (state)
                    {
                        case State.StateFirstTown :
                            activePlayerIndex--;
                            state = State.StateSecondTown;
                            break;
                        case State.StateGame :
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
