namespace HeistGame
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetConsole("Heist!", 180, 60, true, false, false, true, true, true);

            Game game = new Game();
            game.Start();
        }
    }


    //TODO / Nice to have?: 
    // 1 - messages on collection of keys (storytelling, a clue that there's mopre to collect if this unlock the new tier) - optional
    // 2? - (previous point requires extending MissionConfig)
    // 3? - Fog of war? Levels start hidden save for the immediate vicinity of the player. Revealed parts of a level remain visible,
    //      but are not updated. Player has updated visuals only in their immediate vicinity. Can be done by implementing a double
    //      map for levels (one the actual map, used to test for movement and suchm the other the revealed bits only, used for drawing).
    //      Player visibility area can be implemented using the same circle/oval filling algorithms used for the lightmap.
    // 4? - Custom difficulty settings? Being able to set parameters individually
    // 5? - The above requires not only a refactoring of everything impacted by difficulty, but also a change in the savegame system.
    //      Not impossible, but messy.
    // 6? - Add patrol points where the guard pivots before proceeding
    // 7? - knock out guards
    // 8? - player can pick up and drop unconscious guards (cannot leave floor if is carrying a guard)
    // 9? - awake guards can spot and revive unconscious guards (same visibility as player)
    // 10? - combat with guards (extend 7 and 8 to deal with corpses as well)

    //
    //OBJECTIONS:
    // 3 - The doubt is not whether it's doable, it's on how recalculating area of sight and redrawing the map every single frame would impact performance.
    // 4 - Rewriting the difficulty system sounds annoying. Slight concerns on how the menu for the custom difficulty would work.
    // 6 - Requires finding 15 new symbols for the pivoting patrol points, and a rewrite of the guard's AI. Need investigation to find how
    //     estensive the rewrite would be
    // 7 - knocking out patrolling guards requires a precision of movement that hits the limits of this game system (or at least what I can do with it
    //     myself). Is it really worth it (7 & 8 depend on 5)
    // 8 - hard to balance. Combat too hard to encourage stealth, it's a bunch of work for a feature no one would use. Combat too easy, it cancels stealthing
    //     which is the point of the game (and the feature the most work has been done on)
}
