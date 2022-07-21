using System;
using static System.Console;

namespace HeistGame
{
    internal static class ControlsManager
    {
        public static bool HandleInputs()
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
                        //Tell player  to move up
                        //Stop lockpicking if in progress
                        return true;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.NumPad2:
                        //Tell Player to move down
                        //Stop lockpicking if in progress
                        return true;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                    case ConsoleKey.NumPad4:
                        //Tell Player to move left
                        //Stop lockpicking if in progress
                        return true;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                    case ConsoleKey.NumPad6:
                        //Tell Player to move right
                        //Stop lockpicking if in progress
                        return true;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Add:
                        //Tell Player to make noise
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
