using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expanze
{
    class GameMaster
    {
        private Player[] players = new Player[2];
        private Player activePlayer;

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

            activePlayer = players[0];

            return true;
        }

        public Player getActivePlayer()
        {
            return activePlayer;
        }

        public bool changeActivePlayer()
        {
            if (activePlayer == players[0])
            {
                activePlayer = players[1];
            }
            else
            {
                activePlayer = players[0];
            }

            return true;
        }
    }
}
