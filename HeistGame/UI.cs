////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

using System;
using System.Collections.Generic;
using static System.Console;

namespace HeistGame
{
    internal class UI
    {
        public char[,] Grid { get; private set; }

        public UI()
        {
            Grid = new char[4, WindowWidth];
        }

        public void DrawUI(Game game)
        {
            int uiPosition = WindowHeight - 4;

            SetCursorPosition(0, uiPosition);
            DrawSeparator();
            DisplayGameData(game);
            DrawVisibilityIndicator(game);
            DisplayObjectivesIndicator(game);

            string quitInfo = "Press Escape to quit.";
            SetCursorPosition(WindowWidth - quitInfo.Length - 3, WindowHeight - 2);
            Write(quitInfo);
        }

        private void DrawSeparator()
        {
            for (int i = 0; i < WindowWidth; i++)
            {
                Write("_");
            }
            WriteLine("");
        }

        private void DisplayGameData(Game game)
        {
            WriteLine();
            Write($"   {game.ActiveCampaign.Levels[game.CurrentRoom].Name}");
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
            Write($"Objectives: {game.ActiveCampaign.Levels[game.CurrentRoom].GetKeyPiecesProgress()}");
        }
    }
}
