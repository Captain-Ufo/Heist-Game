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
        private int sightDistance;
        private string playerMarker;
        private ConsoleColor playerBaseColor;
        private ConsoleColor playerCurrentColor;

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
        public bool IsStill { get; set; }
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

            IsStill = true;

            playerMarker = marker;
            playerBaseColor = color;
            playerCurrentColor = color;

            timeBetweenMoves = 115;
            timeSinceLastMove = 0;
            sightDistance = 10;
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
        /// <param name="direction">The direction of movement, sing the Directions enum</param>
        /// <param name="level">The level the player is moving in</param>
        /// <param name="game">The current game</param>
        /// <param name="deltaTimeMS">frame timing, to handle movement speed</param>
        public void Move(Directions direction, Level level, Game game, int deltaTimeMS)
        {
            timeSinceLastMove += deltaTimeMS;
            playerCurrentColor = playerBaseColor;

            Clear(level);

            switch (direction)
            {
                case Directions.up:
                    if (level.IsPositionWalkable(X, Y - 1) && (!HasMoved | timeSinceLastMove >= timeBetweenMoves))
                    {
                        Y--;
                    }
                    break;
                case Directions.down:
                    if (level.IsPositionWalkable(X, Y + 1) && (!HasMoved | timeSinceLastMove >= timeBetweenMoves))
                    {
                        Y++;
                    }
                    break;
                case Directions.left:
                    if (level.IsPositionWalkable(X - 1, Y) && (!HasMoved | timeSinceLastMove >= timeBetweenMoves))
                    {
                        X--;
                    }
                    break;
                case Directions.right:
                    if (level.IsPositionWalkable(X + 1, Y) && (!HasMoved | timeSinceLastMove >= timeBetweenMoves))
                    {
                        X++;
                    }
                    break;
            }

            CalculateVisibleArea(level);
            Draw();
            HasMoved = true;
            SetVisibility(X, Y, level);
            timeSinceLastMove -= timeBetweenMoves;
        }

        public void MakeNoise(Level level, Game game)
        {
            playerCurrentColor = ConsoleColor.White;
            Draw();
            game.TunePlayer.PlaySFX(1000, 600);
            level.AlertGuards(new Vector2(X, Y));
        }

        /// <summary>
        /// Draws the player's symbol
        /// </summary>
        public void Draw()
        {
            ConsoleColor previousColor = ForegroundColor;
            ForegroundColor = playerCurrentColor;
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

            if (symbol == SymbolsConfig.Empty.ToString())
            {
                Vector2 tile = new Vector2(X, Y);
                int lightValue = level.GetLightLevelInItile(tile);
                ForegroundColor = ConsoleColor.DarkBlue;
                switch (lightValue)
                {
                    case 0:
                        break;
                    case 1:
                        symbol = SymbolsConfig.Light1.ToString();
                        break;
                    case 2:
                        symbol = SymbolsConfig.Light2.ToString();
                        break;
                    case 3:
                        symbol = SymbolsConfig.Light3.ToString();
                        break;
                }
            }
            else if (symbol == SymbolsConfig.Exit.ToString())
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
            level.Draw();
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
            int visibilityLevel = level.GetLightLevelInItile(new Vector2(xPos, yPos));

            if (visibilityLevel > 0)
            {
                Visibility = 3 * visibilityLevel;
            }
            else
            {
                Visibility = 1;
            }
        }

        public void CalculateVisibleArea(Level level)
        {
            level.UpdateVisibleMap(new Vector2(X, Y));
            level.ClearVisibleMap();

            Vector2[] SightCircumference = Rasterizer.GetCellsAlongEllipse(X, Y, sightDistance * 2, sightDistance);

            foreach (Vector2 point in SightCircumference)
            {
                Vector2[] tiles = Rasterizer.PlotRasterizedLine(X, Y, point.X, point.Y);

                foreach (Vector2 tile in tiles)
                {
                    level.UpdatePlayerHearingArea(tile);

                    if (tile.X  == X && tile.Y == Y)
                    {
                        continue;
                    }
                    if (!level.IsTileTransparent(tile.X, tile.Y, true))
                    {
                        level.UpdateVisibleMap(tile);
                        break;
                    }

                    level.UpdateVisibleMap(tile);
                }
            }
            level.CalculateTilesToDraw();  
        }
    }

    public enum Directions { up, right, down, left }
}
