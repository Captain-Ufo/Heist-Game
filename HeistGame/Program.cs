using System;

namespace HeistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetConsole("Heist!", 180, 60, false, false, true, true, true);

            Game game = new Game();
            game.Start();
        }
    }
}
