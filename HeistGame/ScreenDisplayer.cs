////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

using static System.Console;

namespace HeistGame
{
    internal class ScreenDisplayer
    {
        private static UI ui;
        private static UI_Lable lable;
        private static SafeFileHandle safeFileHandle;
        private static int leftOffset;
        private static int topOffset;


        //All these elements come from stackoverflow.com/questions/2754518/how-can-i-write-fast-colored-output-to-console.
        //Yeah, I'm code monkeying this and I have no regrets for now :P
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutputW(
            SafeFileHandle hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short _X, short _Y)
            {
                X = _X;
                Y = _Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)] public ushort UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [STAThread]
        public static void Initialise()
        {
            ui = new UI();
            lable = new UI_Lable();
            leftOffset = WindowWidth / 2;
            topOffset = WindowHeight / 2;

            //This bit is required for the fast screen writing from StackOverflow.
            safeFileHandle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        }

        /// <summary>
        /// Collects and compare data from the various game elements and creates a the matrix that will be displayed.
        /// After that, it raws (very quickly) the whole screen from said data.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        [STAThread]
        public static void DrawGameplayScreen(Game game)
        {
            if (safeFileHandle.IsInvalid) { return; }

            short windowWidth = (short)WindowWidth;
            short windowHeight = (short)WindowHeight;

            CharInfo[] buffer = new CharInfo[windowWidth * windowHeight];
            SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = windowWidth, Bottom = windowHeight };
            char c = ' ';

            Random random = new Random();

            for (int y = 0; y < windowHeight; y++)
            {
                for (int x = 0; x < windowWidth; x++)
                {
                    //translate screen tile coordinates to map tile coordinates
                    int offsetX = x - leftOffset + game.PlayerCharacter.X;
                    int offsetY = y - topOffset + game.PlayerCharacter.Y;

                    Level level = game.ActiveCampaign.Levels[game.CurrentLevel];

                    Vector2 tile = new Vector2(offsetX, offsetY);

                    //Check if it's the center of the screen (that is, where the player would be in the scrolling map system)
                    if (x == leftOffset && y == topOffset)
                    {
                        c = game.PlayerCharacter.PlayerMarker;
                    }

                    //Add UI
                    else if (y >= ui.UITop)
                    {
                        c = ui.Grid[y - ui.UITop, x];
                    }

                    //Check if the tile is under a lable
                    else if (IsTileUnderLable(new Vector2(x, y)))
                    {
                        c = lable.LableTiles[new Vector2(x, y)];
                    }

                    else
                    {
                        //Check guard tiles
                        if (level.VisibleGuards.ContainsKey(tile))
                        {
                            c = level.VisibleGuards[tile].NPCMarker;
                            //TODO: check offsets and such in the cleanup/refactor round
                        }

                        //Check if tile has not been explored, in which case it has to be empty
                        else if (!level.ExploredMap.ContainsKey(tile))
                        {
                            c = SymbolsConfig.Empty;
                        }

                        //Check if tile has not been explored, in which case it has to be empty
                        else if (level.Grid[tile.Y, tile.X] == SymbolsConfig.Empty && level.VisibleMap.Contains(tile))
                        {
                            int lightLevel = level.GetLightLevelInItile(tile);

                            switch (lightLevel)
                            {
                                case 0:
                                    c = SymbolsConfig.Light0;
                                    break;
                                case 1:
                                    c = SymbolsConfig.Light1;
                                    break;
                                case 2:
                                    c = SymbolsConfig.Light2;
                                    break;
                                case 3:
                                    c = SymbolsConfig.Light3;
                                    break;
                            }
                        }

                        else
                        {
                            //Finally, if all special conditions fail, just add the tile from the map
                            c = level.ExploredMap[tile];
                        }
                    }

                    buffer[y * windowWidth + x].Char.UnicodeChar = c;

                    if (c == SymbolsConfig.PlayerSymbol)
                    {
                        buffer[y * windowWidth + x].Attributes = (short)game.PlayerCharacter.CurrentColor;
                    }

                    if (game.Selector.IsActive && game.Selector.X == tile.X && game.Selector.Y == tile.Y)
                    {
                        //if the tile is selected, set colors to balck symbol + white background
                        buffer[y * windowWidth + x].Attributes = (0 | (15 << 4));
                    }
                    else if (level.VisibleMap.Contains(tile))
                    {
                        //set colors
                        //black = 0
                        //dark blue = 1
                        //dark green = 2
                        //dark cyan = 3
                        //dark red = 4
                        //dark magenta = 5
                        //dark yellow = 6
                        //grey = 7
                        //dark grey = 8
                        //blue = 9
                        //green = 10
                        //cyan = 11
                        //red = 12
                        //magenta = 13
                        //yellow = 14
                        //white = 15
                        short color;

                        switch (c)
                        {
                            case SymbolsConfig.Light0:
                            case SymbolsConfig.Light1:
                            case SymbolsConfig.Light2:
                            case SymbolsConfig.Light3:
                                //blue
                                if (level.GetElementAt(tile.X, tile.Y) == SymbolsConfig.Empty)
                                {
                                    //blue
                                    buffer[y * windowWidth + x].Attributes = 1;
                                }
                                else
                                {
                                    //light grey (because the horizontal window uses the same character as light0)
                                    buffer[y * windowWidth + x].Attributes = 7;
                                }
                                break;
                            case SymbolsConfig.Treasure:
                                //jellow
                                buffer[y * windowWidth + x].Attributes = 14;
                                break;
                            case SymbolsConfig.Exit:
                                if (level.IsLocked)
                                {
                                    //red
                                    buffer[y * windowWidth + x].Attributes = 12;
                                }
                                else
                                {
                                    //green
                                    buffer[y * windowWidth + x].Attributes = 10;
                                }
                                break;
                            case SymbolsConfig.Key:
                                //dark yellow
                                buffer[y * windowWidth + x].Attributes = 6;
                                break;
                            case SymbolsConfig.NPCMarkerUp:
                            case SymbolsConfig.NPCMarkerRight:
                            case SymbolsConfig.NPCMarkerDown:
                            case SymbolsConfig.NPCMarkerLeft:
                                color = (short)level.VisibleGuards[tile].NPCSymbolColor;
                                short bgColor = (short)level.VisibleGuards[tile].NPCTileColor;
                                buffer[y * windowWidth + x].Attributes = (short)(color | (short)(bgColor << 4));
                                break;
                            case SymbolsConfig.ChestClosed:
                            case SymbolsConfig.ChestOpened:
                            case SymbolsConfig.Signpost:
                                //white
                                buffer[y * windowWidth + x].Attributes = 15;
                                break;
                            default:
                                //grey
                                buffer[y * windowWidth + x].Attributes = 7;
                                break;
                        }
                    }
                    else
                    {
                        if (c == SymbolsConfig.NPCMarkerDown || c == SymbolsConfig.NPCMarkerUp ||
                            c == SymbolsConfig.NPCMarkerLeft || c == SymbolsConfig.NPCMarkerRight)
                        {
                            //dark grey
                            buffer[y * windowWidth + x].Attributes = (8 | (8 << 4));
                        }
                        else
                        {
                            //dark grey
                            buffer[y * windowWidth + x].Attributes = 8;
                        }
                    }
                }
            }

            WriteConsoleOutputW(safeFileHandle, buffer, new Coord() { X = windowWidth, Y = windowHeight }, new Coord() { X = 0, Y = 0 }, ref rect);
        }

        /// <summary>
        /// Draws (very quickly) the whole screen from the matrix that is passed in it
        /// </summary>
        /// <param name="screen">The matrix to draw</param>
        public static void DrawScreen(char[,] screen)
        {
            if (safeFileHandle.IsInvalid) { return; }

            short windowWidth = (short)WindowWidth;
            short windowHeight = (short)WindowHeight;

            CharInfo[] buffer = new CharInfo[WindowWidth * WindowHeight];
            SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = windowWidth, Bottom = windowHeight };

            for (int y = 0; y < WindowHeight; y++)
            {
                for (int x = 0; x < WindowWidth; x++)
                {
                    char c = screen[y, x];

                    buffer[y * WindowWidth + x].Char.AsciiChar = Convert.ToByte(c);

                    //TODO: Set colors
                }
            }

            WriteConsoleOutputW(safeFileHandle, buffer, new Coord() { X = windowWidth, Y = windowHeight }, new Coord() { X = 0, Y = 0 }, ref rect);
        }

        public static void DisplayUI(Game game)
        {
            ui.DrawUI(game);
        }

        public static void DisplayMessageOnLable(string[] message)
        {
            lable.ActivateLable(message);
        }

        public static void DeleteLable(Game game)
        {
            lable.Cancel(game.ActiveCampaign.Levels[game.CurrentLevel]);
        }

        public static bool IsTileUnderLable(Vector2 tile)
        {
            return lable.LableTiles.ContainsKey(tile);
        }

        public static void DisplayAboutScreen(Game game)
        {
            Clear();
            string authorName = "Cristian";
            string[] credits = new string[]
            {
                " ",
                " ",
                "~·~ CREDITS: ~·~",
                " ",
                " ",
                $"Heist!, a commnd prompt text based stealth game by {authorName}",
                " ",
                $"Programming: {authorName}",
                "Shoutout to Micheal Hadley's \"Intro To Programming in C#\" course:",
                "https://www.youtube.com/channel/UC_x9TgYAIFHj1ulXjNgZMpQ",
                " ",
                $"Baron's Jail campaign level desing: {authorName}",
                "(I'm not a level designer :P I'm sure you guys can do a lot better!)",
                " ",
                $"Chiptune Music: {authorName}",
                "(I'm not a musician either)",
                " ",
                " ",
                "~·~ ART: ~·~",
                " ",
                "Ascii title from Text To Ascii Art Generator (https://www.patorjk.com/software/taag)",
                " ",
                "Ascii art from Ascii Art Archive (https://www.asciiart.eu/):",
                "Guard art based on 'Orc' by Randall Nortman and Tua Xiong",
                "Win screen art by Henry Segerman",
                "Game over screen art based on art by Jgs",
                " ",
                " ",
                "~·~ TESTING and SPECIAL THANKS: ~·~",
                "Charlie & Daisy",
                "Lone",
                " ",
                " ",
                "Thank you for playing!"
            };

            foreach (string credit in credits)
            {
                for (int i = 0; i < credits.Length; i++)
                {
                    int cursorXoffset = credits[i].Length / 2;
                    SetCursorPosition((WindowWidth / 2) - cursorXoffset, WindowTop + i + 1);
                    WriteLine(credits[i]);
                }
            }

            SetCursorPosition(0, WindowHeight - 3);
            WriteLine("\n Press any key to return to main menu...");
            ReadKey(true);
            Clear();
            game.Restart();
        }

        public static void DisplayLoading()
        {
            string loadingText = "...Loading...";
            int posY = WindowHeight / 2;
            int halfX = WindowWidth / 2;
            int textOffset = loadingText.Length / 2;
            int posX = halfX - textOffset;

            SetCursorPosition(posX, posY);
            WriteLine(loadingText);
        }

        //TODO: refactor this so that text can be scrolled one line at a time
        public static void DisplayTextFullScreen(string[] text, bool withFraming = true)
        {
            if (text == null) { return; }

            if (string.IsNullOrEmpty(text[0])) { return; }

            int firstLineToDisplay = 0;
            int lastLineToDisplay;
            if (text.Length > 48) { lastLineToDisplay = 48; }
            else { lastLineToDisplay = text.Length; }

            List<string> textToDisplay = new List<string>();

            do
            {
                textToDisplay.Clear();
                for (int i = firstLineToDisplay; i < lastLineToDisplay; i++)
                {
                    textToDisplay.Add(text[i]);
                }

                Clear();
                if (withFraming) { DisplayScreenDecoration(); }

                SetCursorPosition(0, (WindowHeight / 2) - ((textToDisplay.Count / 2) + 2));

                foreach (string s in textToDisplay)
                {
                    SetCursorPosition((WindowWidth / 2) - (s.Length / 2), CursorTop);
                    WriteLine(s);
                }

                SetCursorPosition((WindowWidth / 2) - 2, CursorTop);
                WriteLine("~··~");

                string t = "Press Enter to continue...";
                SetCursorPosition((WindowWidth / 2) - (t.Length / 2), CursorTop);
                ForegroundColor = ConsoleColor.Green;
                WriteLine(t);
                ResetColor();

                ConsoleKeyInfo info;
                do
                {
                    info = ReadKey(true);
                }
                while (info.Key != ConsoleKey.Enter);

                Clear();

                firstLineToDisplay += 48;
                lastLineToDisplay += 48;
                if (lastLineToDisplay > text.Length - 1)
                {
                    lastLineToDisplay = text.Length;
                }
            }
            while (firstLineToDisplay < text.Length);
        }

        private static void DisplayScreenDecoration()
        {
            SetCursorPosition(1, 0);
            Write("╬");
            for (int i = 2; i < WindowWidth - 1; i++)
            {
                SetCursorPosition(i, 0);
                Write("═");
            }
            SetCursorPosition(WindowWidth - 1, 0);
            Write("╬");
            for (int i = 1; i < WindowHeight - 2; i++)
            {
                SetCursorPosition(1, i);
                Write("║");
                SetCursorPosition(WindowWidth - 1, i);
                Write("║");
            }
            SetCursorPosition(1, WindowHeight - 2);
            Write("╬");
            for (int i = 2; i < WindowWidth - 1; i++)
            {
                SetCursorPosition(i, WindowHeight - 2);
                Write("═");
            }
            SetCursorPosition(WindowWidth - 1, WindowHeight - 2);
            Write("╬");
        }
    }
}
