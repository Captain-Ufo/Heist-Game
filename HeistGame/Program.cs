/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

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
    // 1 - Switch control system to direct tapping of keyboard information for smoothness and additional functionality
    // 2 - Sneaking, walking and running speed for the player that affect visibility (sneaking reduces it slightly even in light,
    //     walking is normal in light but raises it slightly if in full darkness - to simulate noise -, running raises it significantly
    //     in all conditions).
    // 3 - Custom difficulty settings? Being able to set parameters individually
    //      The above requires not only a refactoring of everything impacted by difficulty, but also a change in the savegame system.
    // 4 - Civilians. Basically similar to guards, except they don't patrol (they only move close to their spawn point) and don't chase
    //      the player. If they spot the player, thy just run away and alert the guards in the process.
    // 5 - Add patrol points where the guard pivots before proceeding
    // 6 - Combat with guards(extend 7 and 8 to deal with corpses as well)
    // 7 - Manually knock out guards and civilians
    // 8 - Crossbow for lethal and tranquillizer darts. Shots a dart that goes as far as the visible distance. Tranquillizer darts would
    //      knock out guards and civilians, lethal darts would hurt them (one hit kill seems too easy)
    // 9 - Player can pick up and drop unconscious guards (cannot leave floor if is carrying a guard)
    // 10 - Awake guards can spot unconscious and dead guards (same visibility as player), and revive if unconscious.
    //      This would make the guard alerted (temporarily if unconscious, permanently if dead).
    // 11 - Redesign the tutorial to better explain concepts and be more interesting (series of backalleys maps)
    // 12 - Adapt UI elements to different screen sizes (whithin reason)
    // 13 - hearing range sparated from visual range (smaller).
    // 14 - Gates update visuals if inside hearing range.

    //NOTES:
    // 6 - With the new control system in place, hold attack and then push a direction key. The sword will slash in the three tiles
    //     in front of the character in rapid succession in the direction that was indicated. The hit lands when/if the sword tuches
    //     one of the tiles directly orthogonally adjacent to the enemy (so enemyX +/-1 and enemyY +/-1). If the player attacks while
    //     an enemy is attacking, the enemy hit is blocked. Same in reverse. Guards hit are very lethal for balance.


    //OBJECTIONS:
    // 3 - Rewriting the difficulty system sounds annoying. Slight concerns on how the menu for the custom difficulty would work.
    // 4 - Concerns about how to refactor everything connected to Difficulty.
    // 5 - Requires finding 15 new symbols for the pivoting patrol points, and a rewrite of the guard's AI. Need investigation to find how
    //     estensive the rewrite would be.
    // 6 - Hard to balance. Combat too hard to encourage stealth, it's a bunch of work for a feature no one would use. Combat too easy, it cancels stealthing
    //     which is the point of the game (and the feature the most work has been done on) (7 & 8 depend on 5 and/or 6 being implemented).
    // 7 - Manually knocking out moving NPCs requires a precision of movement that hits the limits of this game system (or at least what I can do with it
    //     myself). Is it really worth it? On top of that, it becomes mandatory that NPCs cannot see in the tile right behind them.
}
