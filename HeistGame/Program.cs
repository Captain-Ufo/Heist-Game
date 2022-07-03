using System;
using static System.Console;

namespace HeistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetConsole("Heist!", 180, 60, true, false, false, true, true, true);

            Game game = new Game();
            game.Start();

            //TODO?:
            // 1 - knock out guards
            // 2 - player can pick up and drop unconscious guards (cannot leave floor if is carrying a guard)
            // 3 - awake guards can spot and revive unconscious guards (same visibility as player)
            // 4 - in-level messages/notes using numbers for location
            // 5 - (previous point requires extending MissionConfig file: numbers indicate location, then dictionary pairs location with message)
            // 6 - move keys to use keysymbol, ¹, ² and ³ for keys, to free up 1,2,3,4 for messages.
            //
            //OBJECTIONS:
            // 1 - knocking out patrolling guards requires a precision of movement that hits the limits of this game system (or at least what I can do with it
            //     myself). Is it really worth it (2 & 3 depend on 1)
            // 4 - Messages could be an interesting environmental storytelling tool. They can't do anything else, though (I.E. they can't "unhide" any secret that
            //     is not visible at a glance of the map. Are they really worth it? All in all, they seem a bit more interesting, gameplay-wise, than 1.
            //     (5 & 6 depend on 4)
        }
    }
}
