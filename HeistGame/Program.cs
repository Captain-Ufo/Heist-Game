/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.IO;
using System.Text.Json;

namespace HeistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetConsole();

            ScreenDisplayer.Initialise();

            Game game = new Game();
            game.Start();
        }
    }

    //TODO / Nice to have?: 
    // 1 - Switch control system to direct tapping of control information for smoothness
    // 2? - Custom difficulty settings? Being able to set parameters individually
    //      The above requires not only a refactoring of everything impacted by difficulty, but also a change in the savegame system.
    // 3? - Civilians. Basically similar to guards, except they don't patrol (they only move close to their spawn point) and don't chase
    //      the player. If they spot the player, thy just run away and alert the guards in the process.
    // 4? - Add patrol points where the guard pivots before proceeding
    // 5? - Combat with guards(extend 7 and 8 to deal with corpses as well)
    // 6? - Manually knock out guards and civilians
    // 7? - Crossbow for lethal and tranquillizer darts. Shots a dart that goes as far as the visible distance. Tranquillizer darts would
    //      knock out guards and civilians, lethal darts would hurt them (one hit kill seems too easy)
    // 8? - Player can pick up and drop unconscious guards (cannot leave floor if is carrying a guard)
    // 9? - Awake guards can spot and revive unconscious guards (same visibility as player)
    // 10 - Redesign the tutorial to better explain concepts and be more interesting (series of backalleys maps)
    // 11 - Adapt UI elements to different screen sizes (whithin reason)
    // 12 - hearing range sparated from visual range (smaller)


    //OBJECTIONS:
    // 2 - Rewriting the difficulty system sounds annoying. Slight concerns on how the menu for the custom difficulty would work.
    // 3 - Concerns about how to refactor everything connected to Difficulty.
    // 4 - Requires finding 15 new symbols for the pivoting patrol points, and a rewrite of the guard's AI. Need investigation to find how
    //     estensive the rewrite would be.
    // 5 - Hard to balance. Combat too hard to encourage stealth, it's a bunch of work for a feature no one would use. Combat too easy, it cancels stealthing
    //     which is the point of the game (and the feature the most work has been done on) (7 & 8 depend on 5 and/or 6 being implemented).
    // 6 - Manually knocking out moving NPCs requires a precision of movement that hits the limits of this game system (or at least what I can do with it
    //     myself). Is it really worth it? On top of that, it becomes mandatory that NPCs cannot see in the tile right behind them.
}
