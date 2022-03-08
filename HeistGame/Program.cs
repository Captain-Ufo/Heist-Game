using System;
using System.Collections.Generic;

namespace HeistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetConsole("Heist!", 180, 60, false, false, true, true, true);

            Game game = new Game();
            game.Start();

            //TODO:
            // - 'Delete savegame' menu or functionality
            // - knock out guards
            // - player can pick up and drop unconscious guards (cannot leave floor if is carrying a guard)
            // - awake guards can spot and revive unconscious guards (same visibility as player)
            // - in-level messages/notes using numbers for location
            // - (previous point requires extending MissionConfig file: numbers indicate location, then dictionary pairs location with message)
            // - move keys to use keysymbol, ¹, ² and ³ for keys, to free up 1,2,3,4 for messages.
        }
    }
}
