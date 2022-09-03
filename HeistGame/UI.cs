////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

using System;
using System.Text;
using static System.Console;

namespace HeistGame
{
    internal class UI
    {
        public char[,] Grid { get; private set; }

        public int UITop { get; private set; }

        public UI()
        {
            //TODO: maybe update this to account for different screen sizes/ratios?
            UITop = WindowHeight - 4;

            Grid = new char[4, WindowWidth];

            string firstline = new string('─', WindowWidth);
            string emptyLine = new string(' ', WindowWidth);

            for (int y = 0; y < Grid.GetLength(0); y++)
            {
                for (int x = 0; x < WindowWidth; x++)
                {
                    string line;

                    if (y == 0) { line = firstline; }
                    else { line = emptyLine; }

                    Grid[y, x] = line[x];
                }
            }
        }

        public void DrawUI(Game game)
        {
            int uiPosition = WindowHeight - 2;

            SetCursorPosition(0, uiPosition);
            DisplayGameData(game);
            DrawVisibilityIndicator(game);
            DisplayObjectivesIndicator(game);

            string quitInfo = "Press Escape to quit.";
            SetCursorPosition(WindowWidth - quitInfo.Length - 3, WindowHeight - 2);
            Write(quitInfo);
        }

        private void DisplayGameData(Game game)
        {
            WriteLine();
            Write($"   {game.ActiveCampaign.Levels[game.CurrentLevel].Name}");
            SetCursorPosition(32, CursorTop);
            Write($"Difficulty: {game.DifficultyLevel}");
            SetCursorPosition(54, CursorTop);
            Write($"Loot: ${game.PlayerCharacter.Loot}");
        }

        private void DrawVisibilityIndicator(Game game)
        {
            SetCursorPosition((WindowWidth / 2) - 10, CursorTop);
            Write("Visibility: [ ");
            int visibilityLevel = game.PlayerCharacter.Visibility / 5;
            string visibilityDisplay;
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
                    ForegroundColor = ConsoleColor.DarkYellow;
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

        private void DisplayObjectivesIndicator(Game game)
        {
            SetCursorPosition(CursorLeft + 6, CursorTop);
            Write($"Objectives: {game.ActiveCampaign.Levels[game.CurrentLevel].GetKeyPiecesProgress()}");
        }
    }
}
