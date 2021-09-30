using System;
using System.Collections.Generic;

namespace HeistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetConsole("Heist!", 180, 60, false, false, true, true, true);

            Light testLight = new Light(90, 30, 9);
            testLight.TestIlluminatedTiles();

            foreach (KeyValuePair<Vector2, int> tile in testLight.IlluminatedTiles)
            {
                Console.SetCursorPosition(tile.Key.X, tile.Key.Y);
                Console.Write(tile.Value);
            }

            Console.ReadKey(true);

            /*
            Game game = new Game();
            game.Start();*/
        }
    }
}
