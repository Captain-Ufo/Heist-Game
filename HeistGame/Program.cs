using System;
using System.Collections.Generic;

namespace HeistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetConsole("Heist!", 180, 60, false, false, true, true, true);

            /*Console.CursorVisible = false;

            Light testLight = new Light(90, 30, 7);

            testLight.TestIlluminatedTiles2();

            foreach (KeyValuePair<Vector2, int> tile in testLight.IlluminatedTiles)
            {
                Console.SetCursorPosition(tile.Key.X, tile.Key.Y);
                int lightValue = tile.Value;
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                switch (lightValue)
                {
                    case 3:
                        Console.Write(SymbolsConfig.Light3char);
                        break;
                    case 2:
                        Console.Write(SymbolsConfig.Light2char);
                        break;
                    case 1:
                        Console.Write(SymbolsConfig.Light1char);
                        break;
                }
            }

            Console.ReadKey(true);*/

            Game game = new Game();
            game.Start();
        }
    }
}
