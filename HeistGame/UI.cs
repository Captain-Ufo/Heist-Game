using System;
using System.Collections.Generic;
using static System.Console;

namespace HeistGame
{
    internal class UI
    {
        private Game game;

        private UI_Lable lable;

        public UI (Game game)
        {
            this.game = game;
            lable = new UI_Lable();
        }

        public void DisplayLoading()
        {
            string loadingText = "...Loading...";
            int posY = WindowHeight / 2;
            int halfX = WindowWidth / 2;
            int textOffset = loadingText.Length / 2;
            int posX = halfX - textOffset;

            SetCursorPosition(posX, posY);
            WriteLine(loadingText);
        }

        public void DrawUI()
        {
            int uiPosition = WindowHeight - 4;

            SetCursorPosition(0, uiPosition);
            DrawSeparator();
            DisplayGameData();
            DrawVisibilityIndicator();
            DisplayObjectivesIndicator();

            string quitInfo = "Press Escape to quit.";
            SetCursorPosition(WindowWidth - quitInfo.Length - 3, WindowHeight - 2);
            Write(quitInfo);
        }

        public void DisplayMessageOnLable(string[] message, bool newLable)
        {
            lable.DisplayLable(message, newLable);
        }

        public void DeleteLable()
        {
            lable.Cancel(game);
        }

        public bool IsTileUnderLable(Vector2 tile)
        {
            return lable.LableTiles.Contains(tile);
        }

        //TODO: refactor this so that text can be scrolled one line at a time
        public void DisplayTextFullScreen(string[] text, bool withFraming = true)
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

        public void DisplayAboutScreen()
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
                $"Heist!, a commnd prompt ASCII stealth game by {authorName}",
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
                "Izzy",
                "Charlie & Daisy",
                "Lone",
                "Giorgio",
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

        private void DisplayScreenDecoration()
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

        private void DrawSeparator()
        {
            for (int i = 0; i < WindowWidth; i++)
            {
                Write("_");
            }
            WriteLine("");
        }

        private void DisplayGameData()
        {
            Write($"   {game.ActiveCampaign.Levels[game.CurrentRoom].Name}");
            SetCursorPosition(32, CursorTop);
            Write($"Difficulty: {game.DifficultyLevel}");
            SetCursorPosition(54, CursorTop);
            Write($"Loot: ${game.PlayerCharacter.Loot}");
            SetCursorPosition(70, CursorTop);
        }

        private void DrawVisibilityIndicator()
        {
            Write("Visibility: [ ");
            int visibilityLevel = game.PlayerCharacter.Visibility / 3;
            string visibilityDisplay = "";
            switch (visibilityLevel)
            {
                default:
                case 0:
                    visibilityDisplay = "   ";
                    break;
                case 1:
                    ForegroundColor = ConsoleColor.DarkGray;
                    visibilityDisplay = "░░░";
                    break;
                case 2:
                    ForegroundColor = ConsoleColor.Yellow;
                    visibilityDisplay = "▒▒▒";
                    break;
                case 3:
                    ForegroundColor = ConsoleColor.Yellow;
                    visibilityDisplay = "▓▓▓";
                    break;
            }
            Write(visibilityDisplay);
            ResetColor();
            Write(" ]");
        }

        private void DisplayObjectivesIndicator()
        {
            SetCursorPosition(CursorLeft + 6, CursorTop);
            Write($"Objectives: {game.ActiveCampaign.Levels[game.CurrentRoom].GetKeyPiecesProgress()}");
        }
    }
}
