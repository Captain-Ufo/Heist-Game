/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.Text;
using static System.Console;

namespace HeistGame
{
    internal class UI
    {
        public char[,] Grid { get; private set; }

        public int UITop { get; private set; }
        private int width;
        private StringBuilder sb;

        public UI()
        {
            //TODO: maybe update this to account for different screen sizes/ratios?
            UITop = WindowHeight - 4;
            width = WindowWidth;
            sb = new StringBuilder();

            Grid = new char[4, WindowWidth];

            string firstline = new string('─', WindowWidth);
            string emptyLine = new string(' ', WindowWidth);

            for (int y = 0; y < Grid.GetLength(0); y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string line;

                    if (y == 0) { line = firstline; }
                    else { line = emptyLine; }

                    Grid[y, x] = line[x];
                }
            }
        }

        public void UpdateUI(Game game)
        {
            char c = ' ';
            int offset = 0;
            int visibilityDisplayPosition = (width / 2) - 10;
            int lastElementPosition = width - 24;
            string uiElement;

            for (int x = 0; x < width; x++)
            {
                if (x < 4)
                {
                    c = ' ';
                }

                else if (x <= 32)
                {
                    if (x == 4)
                    {
                        offset = x;
                    }

                    int x1 = x - offset;

                    if (x1 < game.ActiveCampaign.Levels[game.CurrentLevel].Name.Length)
                    {
                        c = game.ActiveCampaign.Levels[game.CurrentLevel].Name[x1]; 
                    }
                    else
                    {
                        c = ' ';
                    }
                }

                else if (x <= 54)
                {
                    if (x == 33)
                    {
                        offset = x;
                    }

                    int x1 = x - offset;
                    uiElement = $"Difficulty: {game.DifficultyLevel}";
                    if (x1 < uiElement.Length)
                    {
                        c = uiElement[x1];
                    }
                    else
                    {
                        c = ' ';
                    }
                }

                else if (x <= visibilityDisplayPosition)
                {
                    if (x == 55)
                    {
                        offset = x;
                    }

                    int x1 = x - offset;
                    uiElement = $"Loot: {game.PlayerCharacter.Loot}";
                    if (x1 < uiElement.Length)
                    {
                        c = uiElement[x1];
                    }
                    else
                    {
                        c = ' ';
                    }

                }
                
                else if (x <= 95)
                {
                    if (x == visibilityDisplayPosition + 1)
                    {
                        offset = x;
                    }

                    int x1 = x - offset;
                    uiElement = SetVisibilityIndicator(game);
                    if (x1 < uiElement.Length)
                    {
                        c = uiElement[x1];
                    }
                    else
                    {
                        c = ' ';
                    }
                }

                else if (x <= lastElementPosition)
                {
                    if (x == 96)
                    {
                        offset = x;
                    }

                    int x1 = x - offset;
                    uiElement = $"Objectives: {game.ActiveCampaign.Levels[game.CurrentLevel].GetKeyPiecesProgress()}";
                    if (x1 < uiElement.Length)
                    {
                        c = uiElement[x1];
                    }
                    else
                    {
                        c = ' ';
                    }
                }

                else
                {
                    if (x == lastElementPosition + 1)
                    {
                        offset = x;
                    }

                    int x1 = x - offset;
                    uiElement = "Press Esc to quit.";
                    if (x1 < uiElement.Length)
                    {
                        c = uiElement[x1];
                    }
                    else
                    {
                        c = ' ';
                    }
                }

                Grid[2, x] = c;
            }
        }

        private string SetVisibilityIndicator(Game game)
        {
            sb.Clear();
            sb.Append("Visibility: [ ");
            int visibilityLevel = game.PlayerCharacter.Visibility / 5;
            switch (visibilityLevel)
            {
                default:
                case 0:
                    sb.Append("   ");
                    break;
                case 1:
                    ForegroundColor = ConsoleColor.DarkGray;
                    sb.Append("░░░");
                    break;
                case 2:
                    ForegroundColor = ConsoleColor.DarkYellow;
                    sb.Append("▒▒▒");
                    break;
                case 3:
                    ForegroundColor = ConsoleColor.Yellow;
                    sb.Append("▓▓▓");
                    break;
            }
            sb.Append(" ]");
            return sb.ToString();
        }

        /*
        public void DrawUI(Game game)
        {
            int uiPosition = WindowHeight - 3;

            SetCursorPosition(0, uiPosition);
            DisplayGameData(game);
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

        private void DisplayObjectivesIndicator(Game game)
        {
            SetCursorPosition(CursorLeft + 6, CursorTop);
            Write($"Objectives: {game.ActiveCampaign.Levels[game.CurrentLevel].GetKeyPiecesProgress()}");
        }
        */
    }
}
