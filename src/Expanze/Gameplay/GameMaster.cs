using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private bool paused = false;

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

        public bool startGame()
        {
            players[0] = new Player(Settings.startScore, "Player1");
            players[1] = new Player(Settings.startScore, "Player2");

            activePlayerIndex = 0;
            activePlayer = players[activePlayerIndex];

            state = State.StateFirstTown;
            return true;
        }

        public Player getActivePlayer() { return activePlayer; }
        public State getState() { return state; }

        public bool nextTurn()
        {
            bool status = true;
            status &= changeActivePlaye();

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

        public bool isPaused()
        {
            if (this.paused)
            {
                this.paused = false;
                return true;
            }
            return false;
        }

        public void setPaused()
        {
            this.paused = true;
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
