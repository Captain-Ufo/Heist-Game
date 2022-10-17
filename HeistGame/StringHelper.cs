/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace HeistGame
{
    public static class StringHelper
    {
        public static string[] SplitStringAtLength(string line, int desiredLength, char splitter = ' ')
        {
            if (line.Length < desiredLength)
            {
                return new string[1] { line };
            }
            int actualLength = 0;

            List<string> splitString = new List<string>();

            List<char> lineToSplit = line.ToCharArray().ToList();

            bool foundSpace = false;

            while (lineToSplit.Count > desiredLength)
            {
                for (int i = desiredLength - 1; i >= 0; i--)
                {
                    if (lineToSplit[i] == splitter)
                    {
                        actualLength = i;
                        foundSpace = true;
                        break;
                    }

                    foundSpace = false;
                }

                if (actualLength == 0) 
                { 
                    actualLength = desiredLength;
                    foundSpace = false;
                }

                char[] newLine =  new char[actualLength];
                Array.Copy(lineToSplit.ToArray(), newLine, actualLength);

                lineToSplit.RemoveRange(0, actualLength);
                if (foundSpace) { lineToSplit.RemoveAt(0); }

                splitString.Add(new string(newLine));

                if (lineToSplit.Count <= desiredLength)
                {
                    break;
                }
            }

            if (lineToSplit.Count > 0)
            {
                splitString.Add(new string(lineToSplit.ToArray()));
            }

            return splitString.ToArray();
        }

        public static string[] SplitStringAtLength(string[] lines, int length, char splitter = ' ')
        {
            List<string> splitString = new List<string>();

            foreach (string line in lines)
            {
                string[] splitLines = SplitStringAtLength(line, length, splitter);
                splitString.AddRange(splitLines);
            }

            return splitString.ToArray();
        }

        public static string[] SplitStringAtLength(List<string> lines, int length, char splitter = ' ')
        {
            return SplitStringAtLength(lines.ToArray(), length, splitter);
        }
    }
}
