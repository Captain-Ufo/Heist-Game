////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

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
    // 1 - Scrolling levels and decouple level size from screen size
    // 2? - Custom difficulty settings? Being able to set parameters individually
    // 3? - The above requires not only a refactoring of everything impacted by difficulty, but also a change in the savegame system.
    // 4? - Civilians. Basically similar to guards, except they don't patrol (they only move close to their spawn point) and don't chase
    //      the player. If they spot the player, thy just run away and alert the guards in the process.
    // 5? - Add patrol points where the guard pivots before proceeding
    // 6? - Combat with guards(extend 7 and 8 to deal with corpses as well)
    // 7? - Manually knock out guards and civilians
    // 8? - Crossbow for lethal and tranquillizer darts. Shots a dart that goes as far as the visible distance. Tranquillizer darts would
    //      knock out guards and civilians, lethal darts would hurt them (one hit kill seems too easy)
    // 9? - Player can pick up and drop unconscious guards (cannot leave floor if is carrying a guard)
    // 10? - Awake guards can spot and revive unconscious guards (same visibility as player)
    // 11? - Maps in the level. Different types: all walls/layout, exit location, objectives and exit. Maybe two versions each: one in chest,
    //       one "in the wild" to be collected straight away.
    // 12 - Redesign the tutorial to better explain concepts and be more interesting (series of backalleys maps)
    // 13 - List of messages read so far (only for the level? Including briefiengs and outros too? Color coded for type?)


    //OBJECTIONS:
    // 2 - Rewriting the difficulty system sounds annoying. Slight concerns on how the menu for the custom difficulty would work.
    // 3 - Concerns about how to refactor everything connected to Difficulty.
    // 5 - Requires finding 15 new symbols for the pivoting patrol points, and a rewrite of the guard's AI. Need investigation to find how
    //     estensive the rewrite would be.
    // 6 - Hard to balance. Combat too hard to encourage stealth, it's a bunch of work for a feature no one would use. Combat too easy, it cancels stealthing
    //     which is the point of the game (and the feature the most work has been done on) (7 & 8 depend on 5 and/or 6 being implemented).
    // 7 - Manually knocking out moving NPCs requires a precision of movement that hits the limits of this game system (or at least what I can do with it
    //     myself). Is it really worth it? On top of that, it becomes mandatory that NPCs cannot see in the tile right behind them.
    // 11 - The usual problem: find enough symbols for the map parsing.
}
