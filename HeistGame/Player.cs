/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.Collections.Generic;
using static System.Console;

namespace HeistGame
{
    /// <summary>
    /// The player's avatar in the game
    /// </summary>
    class Player: IAttackable
    {
        private int timeBetweenMoves;
        private int walkSpeed;
        private int runSpeed;
        private int sneakSpeed;
        private int timeSinceLastMove;
        private int sightDistance;
        private int hearingDistance;
        private int colorTick;
        private ConsoleColor moveColor;
        private ConsoleColor sneakColor;
        private ConsoleColor runColor;
        private Directions peekDirection;
        private Vector2 peekOffset;

        /// <summary>
        /// The symbols that indicates the player on screen
        /// </summary>
        public char PlayerMarker { get; private set; }
        public ConsoleColor CurrentColor { get; private set; }
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
        /// The player's health
        /// </summary>
        public int Health { get; private set; }
        /// <summary>
        /// Whether the player has moved in the current frame or not
        /// </summary>
        public bool IsStill { get; set; }
        public bool HasMoved { get; set; }
        /// <summary>
        /// Describes how far the player can be seen (depends on the local tile's light level)
        /// </summary>
        public int Visibility { get; private set; }
        public int Noise { get; private set; }

        /// <summary>
        /// Instantiates a Player object
        /// </summary>
        /// <param name="startingX">The initial X position</param>
        /// <param name="startingY">The initial Y position</param>
        /// <param name="marker">(Optional) The symbol that represents the player on the map</param>
        /// <param name="moveCol">(Optional) The color of the player's symbol</param>
        public Player(Level level, char marker = SymbolsConfig.PlayerSymbol,
                      ConsoleColor moveCol = ConsoleColor.Cyan,
                      ConsoleColor sneakCol = ConsoleColor.DarkCyan,
                      ConsoleColor runCol = ConsoleColor.White)
        { 
            X = level.PlayerStartX;
            Y = level.PlayerStartY;

            Health = 10;

            SetVisibility(level.PlayerStartX, level.PlayerStartY, level);

            IsStill = true;

            PlayerMarker = marker;
            moveColor = moveCol;
            sneakColor = sneakCol;
            runColor = runCol;
            CurrentColor = sneakCol;
            colorTick = -1;

            walkSpeed = 130;
            sneakSpeed = 220;
            runSpeed = 50;

            timeBetweenMoves = walkSpeed;
            timeSinceLastMove = 0;
            sightDistance = 20;
            hearingDistance = 12;

            peekDirection = Directions.idle;
            peekOffset = new Vector2(0, 0);
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

        public void UpdateTick(int deltaTimeMS)
        {
            timeSinceLastMove += deltaTimeMS;
            if (colorTick >= 0)
            {
                colorTick--;
                if (colorTick == 0)
                {
                    CurrentColor = sneakColor;
                }
            }
        }

        /// <summary>
        /// Updates the player's coordinates, moving them by one tile at a time
        /// </summary>
        /// <param name="direction">The direction of movement, sing the Directions enum</param>
        /// <param name="level">The level the player is moving in</param>
        /// <param name="game">The current game</param>
        /// <param name="deltaTimeMS">frame timing, to handle movement speed</param>
        public void Move(Directions direction, MovementMode mode, Level level, Game game)
        {
            if (mode == MovementMode.sneaking) 
            { 
                CurrentColor = sneakColor;
                colorTick = 2;
                timeBetweenMoves = sneakSpeed;
            }
            else if (mode == MovementMode.walking)
            { 
                CurrentColor = moveColor;
                colorTick = 2;
                timeBetweenMoves = walkSpeed;
            }
            else
            { 
                CurrentColor = runColor;
                colorTick = 2;
                timeBetweenMoves = runSpeed;
            }

            if (timeSinceLastMove < timeBetweenMoves)
            {
                return;
            }

            switch (direction)
            {
                case Directions.up:
                    if (level.IsTileWalkable(X, Y - 1))
                    {
                        Y--;
                    }
                    break;
                case Directions.down:
                    if (level.IsTileWalkable(X, Y + 1))
                    {
                        Y++;
                    }
                    break;
                case Directions.left:
                    if (level.IsTileWalkable(X - 1, Y))
                    {
                        X--;
                    }
                    break;
                case Directions.right:
                    if (level.IsTileWalkable(X + 1, Y))
                    {
                        X++;
                    }
                    break;
            }

            CalculateVisibleArea(level);
            CalculateHearingArea(level);
            HasMoved = true;
            SetVisibility(X, Y, level);
            timeSinceLastMove = 0;
        }

        public void StartPeek()
        {
            CurrentColor = ConsoleColor.DarkCyan;
        }

        public void Peek(Directions direction, Level level)
        {
            if (peekDirection == direction) { return; }

            peekDirection = direction;

            switch(peekDirection)
            {
                case Directions.up:
                    if (! level.IsTileWalkable(X, Y - 1)) { return; }
                    peekOffset.X = 0;
                    peekOffset.Y = -1;
                    break;
                case Directions.down:
                    if (!level.IsTileWalkable(X, Y + 1)) { return; }
                    peekOffset.X = 0;
                    peekOffset.Y = 1;
                    break;
                case Directions.left:
                    if (!level.IsTileWalkable(X - 1, Y)) { return; }
                    peekOffset.X = -1;
                    peekOffset.Y = 0;
                    break;
                case Directions.right:
                    if (!level.IsTileWalkable(X + 1, Y)) { return; }
                    peekOffset.X = 1;
                    peekOffset.Y = 0;
                    break;
            }

            CalculateVisibleArea(level);

            peekOffset.X = 0;
            peekOffset.Y = 0;
        }

        public void ResetPeek(Level level)
        {
            if (peekDirection == Directions.idle) { return; }

            peekDirection = Directions.idle;
            peekOffset.X = 0;
            peekOffset.Y = 0;
            CalculateVisibleArea(level);
            
        }

        public void MakeNoise(Level level, Game game)
        {
            CurrentColor = runColor;
            colorTick = 100;
            game.TunePlayer.PlaySFX(1000, 600);
            level.AlertGuards(new Vector2(X, Y));
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

        public void CalculateHearingArea(Level level)
        {
            level.SetPlayerHearingArea(Rasterizer.RasterizedFilledCircle(X, Y, hearingDistance));
        }

        public void CalculateVisibleArea(Level level)
        {
            HashSet<char> wallCorners = new HashSet<char>()
            {
                '╔', '╗', '╝', '╚', '╠', '╣', '╦', '╩', '╬'
            };

            level.ClearPlayerVisibleMap();

            Vector2[] SightCircumference = Rasterizer.GetCellsAlongEllipse(X + peekOffset.X, Y + peekOffset.Y, sightDistance, sightDistance);

            foreach (Vector2 point in SightCircumference)
            {
                bool hasFoundObstacle = false;

                Vector2[] tiles = Rasterizer.PlotRasterizedLine(X + peekOffset.X, Y + peekOffset.Y, point.X, point.Y);

                foreach (Vector2 tile in tiles)
                {
                    //level.UpdatePlayerHearingArea(tile);

                    if (hasFoundObstacle) { continue; }

                    if (tile.X == X && tile.Y == Y)
                    {
                        continue;
                    }

                    if (!level.IsTileTransparent(tile.X, tile.Y))
                    {
                        if (level.IsTileInsideBounds(tile))
                        {
                            level.UpdateVisibleMap(tile);
                        }

                        if (level.IsTileInsideBounds(tile.X + 1, tile.Y))
                        {
                            char tileXplus1 = level.GetElementAt(tile.X + 1, tile.Y);
                            if (wallCorners.Contains(tileXplus1))
                            {
                                Vector2 cornerTile = new Vector2(tile.X + 1, tile.Y);

                                if (level.IsTileInsideBounds(cornerTile))
                                {
                                    level.UpdateVisibleMap(cornerTile);
                                }
                            }
                        }
                        if (level.IsTileInsideBounds(tile.X - 1, tile.Y))
                        {
                            char tileXminus1 = level.GetElementAt(tile.X - 1, tile.Y);
                            if (wallCorners.Contains(tileXminus1))
                            {
                                Vector2 cornerTile = new Vector2(tile.X - 1, tile.Y);

                                if (level.IsTileInsideBounds(cornerTile))
                                {
                                    level.UpdateVisibleMap(cornerTile);
                                }
                            }
                        }
                        if (level.IsTileInsideBounds(tile.X, tile.Y + 1))
                        {
                            char tileYplus1 = level.GetElementAt(tile.X, tile.Y + 1);
                            if (wallCorners.Contains(tileYplus1))
                            {
                                Vector2 cornerTile = new Vector2(tile.X, tile.Y + 1);

                                if (level.IsTileInsideBounds(cornerTile))
                                {
                                    level.UpdateVisibleMap(cornerTile);
                                }
                            }
                        }
                        if (level.IsTileInsideBounds(tile.Y, tile.Y - 1))
                        {
                            char tileYminus1 = level.GetElementAt(tile.Y, tile.Y - 1);
                            if (wallCorners.Contains(tileYminus1))
                            {
                                Vector2 cornerTile = new Vector2(tile.X, tile.Y - 1);

                                if (level.IsTileInsideBounds(cornerTile))
                                {
                                    level.UpdateVisibleMap(cornerTile);
                                }
                            }
                        }

                        hasFoundObstacle = true;
                    }

                    level.UpdateVisibleMap(tile);
                }
            }  
        }

        public void GetHit()
        {
            if (Health <= 0)
            {
                return;
            }

            Health--;
        }

        public void Reset(int xPos, int yPos, Level level)
        {
            X = xPos;
            Y = yPos;
            Loot = 0;
            SetVisibility(X, Y, level);
        }

        private void SetVisibility(int xPos, int yPos, Level level)
        {
            int visibilityLevel = level.GetLightLevelInItile(new Vector2(xPos, yPos));

            if (visibilityLevel > 0)
            {
                Visibility = 5 * visibilityLevel;
            }
            else
            {
                Visibility = 1;
            }
        }
    }

    public enum Directions { up, right, down, left, idle }

    public enum MovementMode { sneaking, walking, running }
}
