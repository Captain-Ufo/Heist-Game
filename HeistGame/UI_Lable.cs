////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace HeistGame
{
    internal class UI_Lable
    {
        public Dictionary<Vector2, char> LableTiles { get; private set; }
        public bool IsActive { get; private set; }

        public UI_Lable()
        {
            LableTiles = new Dictionary<Vector2, char>();
        }

        public void ActivateLable(string[] message)
        {
            int maxMessageLength = EvaluateMessageLength(message);
            int xOffset = (maxMessageLength + 2) / 2;
            int x = WindowWidth / 2;
            int lableLeft = x - xOffset;
            int lableRight = lableLeft + maxMessageLength;

            int lableHeight = message.Length + 2;
            int yOffset =lableHeight;
            int y = WindowHeight / 2 - 2;
            int lableTop = y - yOffset;
            int lableBottom = lableTop + lableHeight;

            ComposeLable(lableLeft, lableRight, lableTop, lableBottom, maxMessageLength, message);

            IsActive = true;
        }

        private void ComposeLable(int lableLeft, int lableright, int lableTop, int lableBottom, int maxMessageLength, string[] message)
        {
            string[] lableMessage = new string[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                if (message[i].Length == maxMessageLength) { continue; }

                StringBuilder sb = new StringBuilder(maxMessageLength);

                int lengthDifference = maxMessageLength - message[i].Length;
                int firstHalf = lengthDifference / 2;
                int secondHalf = lengthDifference - firstHalf;
                string startingSpace = new string(' ', firstHalf);
                string tailSpace = new string(' ', secondHalf);
                sb.Append(startingSpace);
                sb.Append(message[i]);
                sb.Append(tailSpace);
                lableMessage[i] = sb.ToString();
            }

            LableTiles.Clear();
            for (int y = lableTop; y <= lableBottom; y++)
            {
                for (int x = lableLeft; x <= lableright; x++)
                {

                    if (y == lableTop)
                    {
                        if (x == lableLeft)
                        {
                            LableTiles.Add(new Vector2(x, y), '┌');
                            continue;
                        }

                        if (x == lableright)
                        {
                            LableTiles.Add(new Vector2(x, y), '┐');
                            continue;
                        }

                        LableTiles.Add(new Vector2(x, y), '─');
                        continue;
                    }

                    if (y == lableBottom)
                    {
                        if (x == lableLeft)
                        {
                            LableTiles.Add(new Vector2(x, y), '└');
                            continue;
                        }

                        if (x == lableright)
                        {
                            LableTiles.Add(new Vector2(x, y), '┘');
                            continue;
                        }

                        LableTiles.Add(new Vector2(x, y), '─');
                        continue;
                    }

                    if (x == lableLeft || x == lableright)
                    {
                        LableTiles.Add(new Vector2(x, y), '|');
                        continue;
                    }

                    string line = lableMessage[y];
                    char c = line[x];

                    LableTiles.Add(new Vector2(x, y), c);

                }
            }
        }


        public void Cancel (Level level)
        {
            if (!IsActive) { return; }
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
    }
}
