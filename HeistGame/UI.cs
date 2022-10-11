/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.Text;
using System.Collections.Generic;
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
            UITop = WindowHeight - 5;
            width = WindowWidth;
            sb = new StringBuilder();

            Grid = new char[5, WindowWidth];

            sb.Append("┌");
            sb.Append('─', width - 2);
            sb.Append("┐");
            string firstline = sb.ToString();
            
            sb.Clear();
            sb.Append("└");
            sb.Append('─', width - 2);
            sb.Append("┘");
            string lastline = sb.ToString();

            sb.Clear();
            sb.Append("│");
            sb.Append(' ', width - 2);
            sb.Append("│");
            string emptyLine = sb.ToString();

            for (int y = 0; y < Grid.GetLength(0); y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string line;

                    if (y == 0) { line = firstline; }
                    else if (y == Grid.GetLength(0) - 1) { line = lastline; }
                    else { line = emptyLine; }

                    Grid[y, x] = line[x];
                }
            }
            sb.Clear();
        }

        public void UpdateUI(Game game)
        {
            //"$1000000" - 8 chars
            //"¶: 10/40" - 8 characters
            //"[ ░░░░ ]" - 8 characters
            //"♥♥♥♥♥♥♥♥♥♥" - 10 characters
            //"Press ESC to pause" - 18 characters

            string loot = SetLootIndicator(game.PlayerCharacter);
            string objectives = SetObjectivesIndicator(game.ActiveCampaign.Levels[game.CurrentLevel]);
            string visibility = SetVisibilityIndicator(game.PlayerCharacter);
            string health = SetHealthIndicator(game.PlayerCharacter);
            string menuInfo = "Press ESC to pause";
            int screenQuarters = (width - 4) / 4;

            int total = 0;

            sb.Clear();
            sb.Append("│ ");
            total += 2;

            int spacing = screenQuarters - health.Length - (objectives.Length / 2);
            sb.Append(health);
            total += health.Length;

            sb.Append(' ', spacing);
            total += spacing;

            sb.Append(objectives);
            total += objectives.Length;

            spacing = ((width - 2) / 2) - total - (visibility.Length / 2);
            sb.Append(' ', spacing);
            total += spacing;

            sb.Append(visibility);
            total += visibility.Length;

            spacing = (screenQuarters * 3) - total - (loot.Length / 2);
            sb.Append(' ', spacing);
            total += spacing;

            sb.Append(loot);
            total += loot.Length;

            spacing = (width - 2) - menuInfo.Length - total;
            sb.Append(' ', spacing);
            sb.Append(menuInfo);
            sb.Append(" │");

            string line = sb.ToString();
            sb.Clear();

            for (int x = 0; x < width; x++)
            {

                Grid[2, x] = line[x];
            }
        }

        private string SetObjectivesIndicator(Level level)
        {
            sb.Clear();
            string objectives = level.GetKeyPiecesProgress();
            sb.Append($"{SymbolsConfig.Key}: ");
            sb.Append(objectives);
            if (objectives.Length % 2 > 0) { sb.Append(' '); }
            return sb.ToString();
        }

        private string SetLootIndicator(Player player)
        {
            sb.Clear();
            string loot = player.Loot.ToString();
            int emptySpace = 8 - loot.Length - 1;
            sb.Append('$');
            sb.Append(loot);
            if (emptySpace > 0) { sb.Append(' ', emptySpace); }
            return sb.ToString();
        }

        private string SetHealthIndicator(Player player)
        {
            sb.Clear();
            int health = player.Health;
            int emptySpace = 10 - health;
            sb.Append('♥', health);
            if (emptySpace > 0) {  sb.Append(' ', emptySpace); }
            return sb.ToString();
        }

        private string SetVisibilityIndicator(Player player)
        {
            sb.Clear();
            sb.Append("[ ");
            int visibilityLevel = player.Visibility / 5;
            switch (visibilityLevel)
            {
                default:
                case 0:
                    sb.Append("   ");
                    break;
                case 1:
                    ForegroundColor = ConsoleColor.DarkGray;
                    sb.Append("░░░░");
                    break;
                case 2:
                    ForegroundColor = ConsoleColor.DarkYellow;
                    sb.Append("▒▒▒▒");
                    break;
                case 3:
                    ForegroundColor = ConsoleColor.Yellow;
                    sb.Append("▓▓▓▓");
                    break;
            }
            sb.Append(" ]");
            return sb.ToString();
        }
    }
}
