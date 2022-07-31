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

    //TODO:
    // 1 - in-level messages/notes using numbers for location. Useful to give direction to the player (say, which lever opens which gates)
    //     and for environmental storytelling.
    // 2 - (previous point requires extending MissionConfig: the numbers used for the messages correspond to indexes in a 10 entries array)
    // 3 - move keys to use keysymbol, ¹, ² and ³ for keys, to free up 1,2,3,4 for messages.
    // 4 - lockpick (using the interact key that must be set up for messages). Takes time, changes doors from impassable to passable.
    //     Mapmaking symbols: Vertical door (¦ > open  ! > locked level 1  ¡ > locked level 2  | > locked level 3); In game symbol: |
    //                        Horizontal door (- > open door  ~ > locked level 1   = > locked level 2  ­­± > locked level 3); in game symbol: -
    //                        Chests: ( Ͻ' > No treasure  Ͼ > treasure  Ͽ > random choice ). In game symbols: ◙ (Closed) ◘ (Already opened)

    //Nice to have?:
    // 5? - messages on collection of keys (storytelling, a clue that there's mopre to collect if this unlock the new tier) - optional
    // 6? - (previous point requires extenging MissionConfig)
    // 7? - knock out guards
    // 8? - player can pick up and drop unconscious guards (cannot leave floor if is carrying a guard)
    // 9? - awake guards can spot and revive unconscious guards (same visibility as player)
    // 10? - combat with guards (extend 7 and 8 to deal with corpses as well)

    //
    //OBJECTIONS:
    // 5 - tricky to implement: there's no limit to the number of keys, and that makes it problematic for the MissionConfig. I could use a dictionary
    //     with coordinates for keys and messages for values, but that would make it a lot less user friendly in terms of level creation. (5 depends on 4)
    // 7 - knocking out patrolling guards requires a precision of movement that hits the limits of this game system (or at least what I can do with it
    //     myself). Is it really worth it (7 & 8 depend on 5)
    // 10 - hard to balance. Combat too hard to encourage stealth, it's a bunch of work for a feature no one would use. Combat too easy, it cancels stealthing
    //     which is the point of the game (and the feature the most work has been done on)
}
