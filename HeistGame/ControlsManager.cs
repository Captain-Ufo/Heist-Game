using System;
using static System.Console;

namespace HeistGame
{
    internal static class ControlsManager
    {
        public static bool HandleInputs(Level level, Game game, int deltaTimeMS)
        {
            if (KeyAvailable)
            {
                ConsoleKey key;
                do
                {
                    ConsoleKeyInfo keyInfo = ReadKey(true);
                    key = keyInfo.Key;
                }
                while (KeyAvailable);

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.NumPad8:
                        game.PlayerCharacter.Move(Directions.up, level, game, deltaTimeMS);
                        //Stop lockpicking if in progress
                        return true;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.NumPad2:
                        game.PlayerCharacter.Move(Directions.down, level, game, deltaTimeMS);
                        //Stop lockpicking if in progress
                        return true;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                    case ConsoleKey.NumPad4:
                        game.PlayerCharacter.Move(Directions.left, level, game, deltaTimeMS);
                        //Stop lockpicking if in progress
                        return true;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                    case ConsoleKey.NumPad6:
                        game.PlayerCharacter.Move(Directions.right, level, game, deltaTimeMS);
                        //Stop lockpicking if in progress
                        return true;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Add:
                        game.PlayerCharacter.MakeNoise(level, game);
                        //Stop lockpicking if in progress
                        return true;
                    case ConsoleKey.E:
                    case ConsoleKey.Enter:
                        //Interact with items
                        //Stock lockpicking if in progress
                        return true;
                    case ConsoleKey.Escape:
                        return false;
                    default:
                        return true;
                }
            }
            return true;
        }
    }
}
