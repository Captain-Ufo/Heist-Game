using System;
using static System.Console;

namespace HeistGame
{
    /// <summary>
    /// The player's avatar in the game
    /// </summary>
    class Player
    {
        private int timeBetweenMoves;
        private int timeSinceLastMove;
        private string playerMarker;
        private ConsoleColor playerColor;

        /// <summary>
        /// The player's current X position
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// The player's current Y position
        /// </summary>
        public int Y { get; private set; }
        /// <summary>
        /// The current amount of collected treasure
        /// </summary>
        public int Loot { get; private set; }
        /// <summary>
        /// Whether the player has moved in the current frame or not
        /// </summary>
        public bool HasMoved { get; set; }
        /// <summary>
        /// Describes how far the player can be seen (depends on the local tile's light level)
        /// </summary>
        public int Visibility { get; private set; }

        /// <summary>
        /// Instantiates a Player object
        /// </summary>
        /// <param name="startingX">The initial X position</param>
        /// <param name="startingY">The initial Y position</param>
        /// <param name="marker">(Optional) The symbol that represents the player on the map</param>
        /// <param name="color">(Optional) The color of the player's symbol</param>
        public Player(Level level, string marker = "☺", ConsoleColor color = ConsoleColor.Cyan)
        { 
            X = level.PlayerStartX;
            Y = level.PlayerStartY;

            SetVisibility(level.PlayerStartX, level.PlayerStartY, level);

            playerMarker = marker;
            playerColor = color;

            timeBetweenMoves = 115;
            timeSinceLastMove = 0;
        }

        /// <summary>
        /// Sets the player's starting position at the beginning of a level
        /// </summary>
        /// <param name="x">The X coordinate of the position</param>
        /// <param name="y">The Y coordinate of the position</param>
        public void SetStartingPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Updates the player's coordinates, moving them by one tile at a time
        /// </summary>
        /// <param name="level">The level the player is moving in</param>
        /// <param name="direction">The direction of the movement</param>
        /// <param name="deltaTimeMS">frame timing, to handle movement speed</param>
        public bool HandlePlayerControls(Level level, int deltaTimeMS)
        {
            timeSinceLastMove += deltaTimeMS;

            if (KeyAvailable)
            {
                ConsoleKey key;
                do
                {
                    ConsoleKeyInfo keyInfo = ReadKey(true);
                    key = keyInfo.Key;
                }
                while (KeyAvailable);

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.NumPad8:
                        if (level.IsPositionWalkable(X, Y - 1) && timeSinceLastMove >= timeBetweenMoves)
                        {
                            Clear(level);
                            Y--;
                            Draw();
                            HasMoved = true;
                            SetVisibility(X, Y, level);
                            timeSinceLastMove -= timeBetweenMoves;
                        }
                        return true;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                    case ConsoleKey.NumPad2:
                        if (level.IsPositionWalkable(X, Y + 1) && timeSinceLastMove >= timeBetweenMoves)
                        {
                            Clear(level);
                            Y++;
                            Draw();
                            HasMoved = true;
                            SetVisibility(X, Y, level);
                            timeSinceLastMove -= timeBetweenMoves;
                        }
                        return true;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                    case ConsoleKey.NumPad4:
                        if (level.IsPositionWalkable(X - 1, Y) && timeSinceLastMove >= timeBetweenMoves)
                        {
                            Clear(level);
                            X--;
                            Draw();
                            HasMoved = true;
                            SetVisibility(X, Y, level);
                            timeSinceLastMove -= timeBetweenMoves;
                        }
                        return true;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                    case ConsoleKey.NumPad6:
                        if (level.IsPositionWalkable(X + 1, Y) && timeSinceLastMove >= timeBetweenMoves)
                        {
                            Clear(level);
                            X++;
                            Draw();
                            HasMoved = true;
                            SetVisibility(X, Y, level);
                            timeSinceLastMove -= timeBetweenMoves;
                        }
                        return true;
                    case ConsoleKey.Escape:
                        return false;
                    default:
                        return true;
                }
            }
            return true;
        }

        /// <summary>
        /// Draws the player's symbol
        /// </summary>
        public void Draw()
        {
            ConsoleColor previousColor = ForegroundColor;
            ForegroundColor = playerColor;
            SetCursorPosition(X, Y);
            Write(playerMarker);
            ForegroundColor = previousColor;
        }

        /// <summary>
        /// Replaces the player's symbol with whatever map symbol should be present in that position
        /// </summary>
        /// <param name="level">The level from which to gather the information required (which symbol to use, the state of the exit, etc)</param>
        public void Clear(Level level)
        {
            string symbol = level.GetElementAt(X, Y);

            SetCursorPosition(X, Y);

            if (symbol == SymbolsConfig.EmptySpace.ToString())
            {
                Vector2 tile = new Vector2(X, Y);
                int lightValue = level.GetLightLevelInItle(tile);
                ForegroundColor = ConsoleColor.DarkBlue;
                switch (lightValue)
                {
                    case 0:
                        break;
                    case 1:
                        symbol = SymbolsConfig.Light1char.ToString();
                        break;
                    case 2:
                        symbol = SymbolsConfig.Light2char.ToString();
                        break;
                    case 3:
                        symbol = SymbolsConfig.Light3char.ToString();
                        break;
                }
            }
            else if (symbol == SymbolsConfig.ExitChar.ToString())
            {
                if (level.IsLocked)
                {
                    ForegroundColor = ConsoleColor.Red;
                }
            }
            else
            {
                ResetColor();
            }

            Write(symbol);
            ResetColor();
        }

        /// <summary>
        /// Sets the current player loot to the indicated amount (use ONLY to set the loot to a specified amount, or to reset it to 0)
        /// </summary>
        /// <param name="amount"></param>
        public void SetLoot(int amount)
        {
            Loot = amount;
        }

        /// <summary>
        /// Changes the value of the lood by the indicated amount. Input negative numbers to subtract from it.
        /// </summary>
        /// <param name="amount"></param>
        public void ChangeLoot(int amount)
        {
            Loot += amount;
        }

        private void SetVisibility(int xPos, int yPos, Level level)
        {
            int visibilityLevel = level.GetLightLevelInItle(new Vector2(xPos, yPos));

            if (visibilityLevel > 0)
            {
                Visibility = 3 * visibilityLevel;
            }
            else
            {
                Visibility = 1;
            }
        }
    }

    public enum Directions { up, right, down, left }
}
