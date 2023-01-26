/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace HeistGame
{
    internal class UI_Label
    {
        public Dictionary<Vector2, char> LabelTiles { get; private set; }
        public bool IsActive { get; private set; }

        public UI_Label()
        {
            LabelTiles = new Dictionary<Vector2, char>();
        }

        public void ActivateLabel(string[] message)
        {
            int maxMessageLength = EvaluateMessageLength(message);
            int xOffset = (maxMessageLength + 2) / 2;
            int x = WindowWidth / 2;
            int labelLeft = x - xOffset;
            int labelRight = labelLeft + maxMessageLength + 1;

            int lableHeight = message.Length + 2;
            int yOffset = lableHeight;
            int y = WindowHeight / 2 - 2;
            int labelTop = y - yOffset;
            int labelBottom = labelTop + lableHeight - 1;

            ComposeLable(labelLeft, labelRight, labelTop, labelBottom, maxMessageLength, message);

            IsActive = true;
        }

        private void ComposeLable(int labelLeft, int labelright, int labelTop, int labelBottom, int maxMessageLength, string[] message)
        {
            string[] lableMessage = new string[message.Length];
            StringBuilder sb = new StringBuilder(maxMessageLength);

            for (int i = 0; i < message.Length; i++)
            {
                if (message[i].Length == maxMessageLength) 
                { 
                    lableMessage[i] = message[i];
                    continue; 
                }

                sb.Clear();
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

            LabelTiles.Clear();
            int yIndex = -2;
            for (int y = labelTop; y <= labelBottom; y++)
            {
                yIndex++;
                int xIndex = -2;
                for (int x = labelLeft; x <= labelright; x++)
                {
                    xIndex++;

                    if (y == labelTop)
                    {
                        if (x == labelLeft)
                        {
                            LabelTiles.Add(new Vector2(x, y), '┌');
                            continue;
                        }

                        if (x == labelright)
                        {
                            LabelTiles.Add(new Vector2(x, y), '┐');
                            continue;
                        }

                        LabelTiles.Add(new Vector2(x, y), '─');
                        continue;
                    }

                    if (y == labelBottom)
                    {
                        if (x == labelLeft)
                        {
                            LabelTiles.Add(new Vector2(x, y), '└');
                            continue;
                        }

                        if (x == labelright)
                        {
                            LabelTiles.Add(new Vector2(x, y), '┘');
                            continue;
                        }

                        LabelTiles.Add(new Vector2(x, y), '─');
                        continue;
                    }

                    if (x == labelLeft || x == labelright)
                    {
                        LabelTiles.Add(new Vector2(x, y), '|');
                        continue;
                    }


                    string line = lableMessage[yIndex];
                    char c = line[xIndex];

                    LabelTiles.Add(new Vector2(x, y), c);

                }
            }
        }


        public void Cancel ()
        {
            if (!IsActive) { return; }
            IsActive = false;
            LabelTiles.Clear();
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
