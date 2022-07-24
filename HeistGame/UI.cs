using System;
using System.Collections.Generic;
using static System.Console;

namespace HeistGame
{
    internal class UI
    {
        private Game game;

        public UI (Game game)
        {
            this.game = game;
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

        public void DrawUI(int currentLevel)
        {
            int uiPosition = WindowHeight - 4;

            SetCursorPosition(0, uiPosition);
            for (int i = 0; i < WindowWidth; i++)
            {
                Write("_");
            }
            WriteLine("");
            Write($"   {game.ActiveCampaign.Levels[currentLevel].Name}");
            SetCursorPosition(32, CursorTop);
            Write($"Difficulty: {game.DifficultyLevel}");
            SetCursorPosition(54, CursorTop);
            Write($"Loot: ${game.PlayerCharacter.Loot}");
            SetCursorPosition(70, CursorTop);
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

            string quitInfo = "Press Escape to quit.";
            SetCursorPosition(WindowWidth - quitInfo.Length - 3, WindowHeight - 2);
            Write(quitInfo);
        }

        public void DisplayMessageOnLable(string[] message)
        {
            int messageLength = EvaluateMessageLength() + 2;
            int xOffset = messageLength / 2;
            int x1 = WindowWidth / 2;
            int x = x1 - xOffset;

            int yOffset = message.Length / 2;
            int y1 = WindowHeight / 2;
            int y = y1 - yOffset;

            string firstLine = string.Empty;
            string lastLine = string.Empty;

            for (int i = 0; i <= messageLength + 1; i++)
            {
                char symbol1;
                char symbol2;

                if (i == 0)
                {
                    symbol1 = '┌';
                    symbol2 = '└';
                }
                else if (i == messageLength + 1)
                {
                    symbol1 = '┐';
                    symbol2 = '┘';
                }
                else
                {
                    symbol1 = '─';
                    symbol2 = '─';
                }
                firstLine += symbol1;
                lastLine += symbol2;
            }

            int lines = message.Length + 2;

            for (int i = 0; i < lines; i++)
            {
                SetCursorPosition(x, y + i);
                if (i == 0)
                {
                    Write(firstLine);
                }
                else if (i == lines - 1)
                {
                    Write(lastLine);
                }
                else
                {
                    Write("|");
                    int lengthDifference = messageLength - message[i - 1].Length;
                    int half = lengthDifference / 2;
                    if (lengthDifference > 0)
                    {
                        for (int j = 1; j <= half; j++)
                        {
                            Write(" ");
                        }
                    }
                    Write(message[i - 1]);
                    if (lengthDifference > 0)
                    {
                        lengthDifference -= half;
                        for (int j = 1; j <= lengthDifference; j++)
                        {
                            Write(" ");
                        }
                    }
                    Write("|");
                }
            }

            ReadKey();
            Clear();
            game.HasDrawnBackground = false;

            int EvaluateMessageLength()
            {
                int messageLength = 0;
                for (int i = 0; i < message.Length; i++)
                {
                    int length = message[i].Length;
                    if (length > messageLength)
                    {
                        messageLength = length;
                    }
                }
                return messageLength;
            }
        }

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
    }
}
