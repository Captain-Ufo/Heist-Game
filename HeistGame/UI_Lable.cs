using System.Collections.Generic;
using static System.Console;

namespace HeistGame
{
    internal class UI_Lable
    {
        public HashSet<Vector2> LableTiles { get; private set; }
        public bool IsActive { get; private set; }

        public UI_Lable()
        {
            LableTiles = new HashSet<Vector2>();
        }

        public void DisplayLable(string[] message, bool firstDisplay)
        {
            int messageLength = EvaluateMessageLength(message) + 2;
            int xOffset = messageLength / 2;
            int x1 = WindowWidth / 2;
            int x = x1 - xOffset;

            int yOffset = message.Length / 2;
            int y1 = WindowHeight / 2;
            int y = y1 - yOffset;

            string firstLine = string.Empty;
            string lastLine = string.Empty;

            AddFrame(messageLength, ref firstLine, ref lastLine);

            int lines = message.Length + 2;
            DrawMessage(message, messageLength, x, y, firstLine, lastLine, lines);

            if (firstDisplay) { SetLableTiles(xOffset, messageLength, yOffset, lines); }
            IsActive = true;
        }

        public void Cancel (Game game)
        {
            if (!IsActive) { return; }

            foreach (Vector2 tile in LableTiles)
            {
                Level level = game.ActiveCampaign.Levels[game.CurrentRoom];
                level.DrawTile(tile.X, tile.Y, level.GetElementAt(tile.X, tile.Y, true));
            }
            IsActive = false;
            LableTiles.Clear();
        }

        private int EvaluateMessageLength(string[] message)
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

        private void AddFrame(int messageLength, ref string firstLine, ref string lastLine)
        {
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
        }

        private void DrawMessage(string[] message, int messageLength, int x, int y, string firstLine, string lastLine, int lines)
        {
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
        }

        private void SetLableTiles(int topX, int topY, int bottomX, int bottomY)
        {
            LableTiles.Clear();
            for (int x = topX; x <= bottomX; x++)
            {
                for (int y = topY; y <= bottomY; y++)
                {
                    LableTiles.Add(new Vector2(x, y));
                }
            }
        }
    }
}
