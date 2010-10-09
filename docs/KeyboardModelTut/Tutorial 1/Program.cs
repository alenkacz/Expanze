using System;

namespace tutorial
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameClass game = new GameClass())
            {
                game.Run();
            }
        }
    }
}

