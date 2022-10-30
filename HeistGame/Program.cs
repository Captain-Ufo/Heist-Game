/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System.Threading;
using static System.Console;

namespace HeistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetConsole();

            ScreenDisplayer.Initialise();
            
            Clock.Initialize();

            ControlsManager.InitializeControlsTicks();

            Game game = new Game();
            game.Start();
        }
    }

    //TODO / Nice to have?: 
    // 1 - Sneaking, walking and running speed for the player that affect visibility.
    // 2 - New alert level system for the guards and NPCs, with three thresholds.
    // 3 - Custom difficulty settings? Being able to set parameters individually
    //      The above requires not only a refactoring of everything impacted by difficulty, but also a change in the savegame system.
    // 4 - Civilians. Basically similar to guards, except they don't patrol (they only move close to their spawn point) and don't chase
    //      the player. If they spot the player, thy just run away and alert the guards in the process.
    // 5 - Add patrol points where the guard pivots before proceeding on theor path
    // 6 - Combat with guards(extend 7 and 8 to deal with corpses as well)
    // 7 - Manually knock out guards and civilians
    // 8 - Crossbow for lethal and tranquillizer darts. Shots a dart that goes as far as the visible distance. Tranquillizer darts would
    //      knock out guards and civilians, lethal darts would hurt them (one hit kill seems too easy)
    // 9 - Player can pick up and drop unconscious guards (cannot leave floor if is carrying a guard)
    // 10 - Awake guards can spot unconscious and dead guards (same visibility as player), and revive if unconscious.
    //      This would make the guard alerted (temporarily if unconscious, permanently if dead).
    // 11 - Redesign the tutorial to better explain concepts and be more interesting (series of backalleys maps)

    //NOTES:
    // 1 - Sneaking reduces visibility slightly even in light,while walking is normal in light but raises it slightly if in full darkness;
    //      running raises it significantly in all conditions.
    // 2 - Connected topoint 1. Player has visibility level and noise level. When a guard is within seeing range and/or hearing range,
    //      the visibility level and/or the noise level add their values to the guard's alert level. At the first threshold, the guard
    //      stops their patrol and turns in the direction of the player. At the second threshold, the guer goes to investigate the last
    //      recorded position of the disturbance. At the final threshold, the guard is aware of the player and chasing them. Alert levels
    //      naturally diminish over time (deltaTimeMS) unles propped up by further disturbance. Consider natural resting alert levels after
    //      alarms are triggered (or based on difficulty level).
    // 5 - With the new control system in place, hold attack and then push a direction key. The sword will slash in the three tiles
    //      in front of the character in rapid succession in the direction that was indicated. The hit lands when/if the sword tuches
    //      one of the tiles directly orthogonally adjacent to the enemy (so enemyX +/-1 and enemyY +/-1). If the player attacks while
    //      an enemy is attacking, the enemy hit is blocked. Same in reverse. Guards hit are very lethal for balance.


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
