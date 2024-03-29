﻿using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace HeistGame
{
    /// <summary>
    /// A gameplay element that patrols the level or pivots in place, and spots, chases and catches the player if within range.
    /// </summary>
    class Guard
    {
        private Directions direction = Directions.down;
        private Vector2[] patrolPath;
        private int nextPatrolPoint;
        private int verticalAggroDistance = 5;
        private int horizontalAggroDistance = 10;
        private int bribeTimer;
        private int bribeTimerDuration;
        private bool isBribed;
        private int alertTimer;
        private bool isAlerted;
        private bool isReturning;
        private int pivotTimer;
        private string[] guardMarkersTable = new string[] { "^", ">", "V", "<" };
        private string guardMarker;
        private ConsoleColor guardSymbolColor = ConsoleColor.Black;
        private ConsoleColor guardTileColor = ConsoleColor.DarkRed;
        private int walkingSpeed = 150;
        private int runningSpeed = 120;
        private int timeBetweenMoves;
        private int timeSinceLastMove = 0;
        private Vector2 originPoint;
        private Vector2 lastPatrolPoint;
        private Vector2 lastKnownPlayerPosition;
        private Random rng;

        /// <summary>
        /// To be set depending on difficulty level. If true, it will prevent being bribed a second time
        /// </summary>
        public int TimesBribed { get; private set; }
        /// <summary>
        /// The X coordinate of the Guard
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The Y coordinate of the guard
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Instantiates a Guard Object and sets its parameters
        /// </summary>
        public Guard()
        {
            rng = new Random();
            nextPatrolPoint = 0;
            isBribed = false;
            isAlerted = false;
            isReturning = false;
            TimesBribed = 0;
            bribeTimer = 0;
            bribeTimerDuration = 50;
            alertTimer = 0;
            pivotTimer = rng.Next(201);
            timeBetweenMoves = walkingSpeed;
            direction = Directions.up;
            guardMarker = guardMarkersTable[(int)direction];
        }

        /// <summary>
        /// Assigns the Guard's initial position when the level is first loaded. To be used only when parsing the level file.
        /// </summary>
        /// <param name="x">The X position</param>
        /// <param name="y">The Y position</param>
        public void AssignOriginPoint(int x, int y)
        {
            X = x;
            Y = y;

            originPoint = new Vector2(X, Y);
            lastPatrolPoint = originPoint;
        }

        /// <summary>
        /// Assigns the offsets to account for map centering. To be used only at the beginning of the game
        /// </summary>
        /// <param name="xOffset">The X offset (from 0)</param>
        /// <param name="yOffset">The Y offset (from 0)</param>
        public void AssignOffset(int xOffset, int yOffset)
        {
            X += xOffset;
            Y += yOffset;

            originPoint.X = X;
            originPoint.Y = Y;

            for (int i = 0; i < patrolPath.Length; i++)
            {
                patrolPath[i].X += xOffset;
                patrolPath[i].Y += yOffset;
            }
        }

        /// <summary>
        /// Assigns the patrol path
        /// </summary>
        /// <param name="path">An array of patrol path points in the form of a Coordinates objects</param>
        public void AssignPatrol(Vector2[] path)
        {
            patrolPath = path;
        }

        /// <summary>
        /// Updates the guard's AI behavior
        /// </summary>
        /// <param name="level">The level the guard is in</param>
        /// <param name="game">The current game</param>
        /// <param name="deltaTimeMS">frame timing, to handle movement speed</param>
        public void UpdateBehavior(Level level, Game game, int deltaTimeMS)
        {
            timeSinceLastMove += deltaTimeMS;

            if (timeSinceLastMove < timeBetweenMoves)
            {
                return;
            }

            UpdateBribe();

            if (SpotPlayer(game, level))
            {
                if (!isAlerted)
                {
                    game.TunePlayer.PlaySFX(1200, 600);
                    game.TimesSpotted++;
                }

                guardTileColor = ConsoleColor.Red;
                lastKnownPlayerPosition = new Vector2(game.PlayerCharacter.X, game.PlayerCharacter.Y);
                isAlerted = true;
                isReturning = false;
                timeBetweenMoves = runningSpeed;
                MoveTowards(lastKnownPlayerPosition, level);
            }
            else if (isAlerted)
            {
                guardTileColor = ConsoleColor.Magenta;
                AlertedBehavior(level);
            }
            else if (isReturning)
            {
                guardTileColor = ConsoleColor.DarkRed;
                ReturnToPatrol(level);
            }
            else
            {
                guardTileColor = ConsoleColor.DarkRed;
                timeBetweenMoves = walkingSpeed;
                if (patrolPath.Length > 0)
                {
                    Move(level, Patrol());
                }
                else
                {
                    Pivot(pivotTimer, 20);

                    if (pivotTimer == int.MaxValue) { pivotTimer = 0; }
                    else { pivotTimer += rng.Next(1, 5); }
                }
                lastPatrolPoint.X = X;
                lastPatrolPoint.Y = Y;
            }

            CatchPlayer(game);

            timeSinceLastMove -= timeBetweenMoves;
        }

        /// <summary>
        /// Prevents a Game Over
        /// </summary>
        /// <param name="IsGameEasy">Sets the flag that is used to determine how many times a guard can be bribed</param>
        public void BribeGuard()
        {
            isBribed = true;
            if (isAlerted)
            {
                isAlerted = false;
                isReturning = true;
            }
        }

        /// <summary>
        /// Restores the guard to their conditions at the beginning of the level. To be used only when retrying levels
        /// </summary>
        public void Reset()
        {
            nextPatrolPoint = 0;
            bribeTimer = 0;
            isBribed = false;
            alertTimer = 0;
            isAlerted = false;
            isReturning = false;
            pivotTimer = rng.Next(201);
            X = originPoint.X;
            Y = originPoint.Y;
            timeSinceLastMove = 0;
            TimesBribed = 0;
        }

        /// <summary>
        /// Draws the guard symbol
        /// </summary>
        public void Draw()
        {
            ConsoleColor previousFColor = ForegroundColor;
            ConsoleColor previusBGColor = BackgroundColor;
            ForegroundColor = guardSymbolColor;
            BackgroundColor = guardTileColor;
            SetCursorPosition(X, Y);
            Write(guardMarker);
            ForegroundColor = previousFColor;
            BackgroundColor = previusBGColor;
        }

        /// <summary>
        /// Replaces the guard symbol with whatever static tile is in the map grid in the previous position of the guard
        /// </summary>
        /// <param name="level">The level from which to gather the information required (which symbol to use, the state of the exit, etc)</param>
        public void Clear(Level level)
        {
            string symbol = level.GetElementAt(X, Y);

            SetCursorPosition(X, Y);

            if (symbol == "$")
            {
                ForegroundColor = ConsoleColor.Yellow;
            }

            if (symbol == SymbolsConfig.ExitChar.ToString())
            {
                if (level.IsLocked)
                {
                    ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    ForegroundColor = ConsoleColor.Green;
                }
            }
            Write(symbol);
            ResetColor();
        }

        private void UpdateBribe()
        {
            if (isBribed)
            {
                bribeTimer++;
            }

            if (bribeTimer > bribeTimerDuration)
            {
                isBribed = false;
                TimesBribed++;
                bribeTimer = 0;
            }
        }

        private bool SpotPlayer(Game game, Level level)
        {
            if (isBribed)
            {
                return false;
            }

            switch (direction)
            {
                case Directions.up:
                    if (game.PlayerCharacter.X >= X - horizontalAggroDistance && game.PlayerCharacter.X <= X + horizontalAggroDistance
                        && game.PlayerCharacter.Y >= Y - verticalAggroDistance && game.PlayerCharacter.Y <= Y + 1)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = GetTilesBetweenGuardAndPlayer(this.X, this.Y, game.PlayerCharacter.X, game.PlayerCharacter.Y);

                        foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
                        {
                            if (!level.IsTileTransparent(tile.X, tile.Y))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Directions.right:
                    if (game.PlayerCharacter.X >= X - 1 && game.PlayerCharacter.X <= X + horizontalAggroDistance
                        && game.PlayerCharacter.Y >= Y - verticalAggroDistance && game.PlayerCharacter.Y <= Y + verticalAggroDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = GetTilesBetweenGuardAndPlayer(this.X, this.Y, game.PlayerCharacter.X, game.PlayerCharacter.Y);

                        foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
                        {
                            if (!level.IsTileTransparent(tile.X, tile.Y))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Directions.down:
                    if (game.PlayerCharacter.X >= X - horizontalAggroDistance && game.PlayerCharacter.X <= X + horizontalAggroDistance
                        && game.PlayerCharacter.Y >= Y - 1 && game.PlayerCharacter.Y <= Y + verticalAggroDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = GetTilesBetweenGuardAndPlayer(this.X, this.Y, game.PlayerCharacter.X, game.PlayerCharacter.Y);

                        foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
                        {
                            if (!level.IsTileTransparent(tile.X, tile.Y))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Directions.left:
                    if (game.PlayerCharacter.X >= X - horizontalAggroDistance && game.PlayerCharacter.X <= X + 1
                        && game.PlayerCharacter.Y >= Y - verticalAggroDistance && game.PlayerCharacter.Y <= Y + verticalAggroDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = GetTilesBetweenGuardAndPlayer(this.X, this.Y, game.PlayerCharacter.X, game.PlayerCharacter.Y);

                        foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
                        {
                            if (!level.IsTileTransparent(tile.X, tile.Y))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
            return false;
        }

        private void AlertedBehavior(Level level)
        {
            if (X != lastKnownPlayerPosition.X || Y != lastKnownPlayerPosition.Y)
            {
                if (MoveTowards(lastKnownPlayerPosition, level))
                {
                    alertTimer = 0;
                    return;
                }
            }

            alertTimer++;

            Pivot(alertTimer, 10);

            if (alertTimer > 50)
            {
                alertTimer = 0;
                isAlerted = false;
                guardTileColor = ConsoleColor.DarkRed;
                timeBetweenMoves = walkingSpeed;
                isReturning = true;
            }
        }

        private void ReturnToPatrol(Level level)
        {
            if (X != lastPatrolPoint.X || Y != lastPatrolPoint.Y)
            {
                MoveTowards(new Vector2(lastPatrolPoint.X, lastPatrolPoint.Y), level);
                return;
            }

            isReturning = false;
        }

        private Tile Pathfind(Level level, Tile pathStart, Tile destination)
        {
            pathStart.SetDistance(destination.X, destination.Y);
            List<Tile> activeTiles = new List<Tile>();
            activeTiles.Add(pathStart);
            List<Tile> visitedTiles = new List<Tile>();

            while (activeTiles.Any())
            {
                Tile tileToCheck = activeTiles.OrderBy(tile => tile.CostDistance).First();

                if (tileToCheck.X == destination.X && tileToCheck.Y == destination.Y)
                {
                    return tileToCheck.Parent;
                }

                visitedTiles.Add(tileToCheck);
                activeTiles.Remove(tileToCheck);

                List<Tile> walkableNeighbors = level.GetWalkableNeighborsOfTile(tileToCheck, destination);

                foreach (Tile neighbor in walkableNeighbors)
                {
                    if (visitedTiles.Any(tile => tile.X == neighbor.X && tile.Y == neighbor.Y))
                    {
                        //The tile was already evaluated by the pathfinding, so we skip it
                        continue;
                    }

                    if (activeTiles.Any(tile => tile.X == neighbor.X && tile.Y == neighbor.Y))
                    {
                        //re-check a tile evaluated on a previous cycle to see if now it has a better value
                        Tile existingTile = activeTiles.First(tile => tile.X == neighbor.X && tile.Y == neighbor.Y);

                        if (existingTile.CostDistance > neighbor.CostDistance)
                        {
                            activeTiles.Remove(existingTile);
                            activeTiles.Add(neighbor);
                        }
                    }
                    else
                    {
                        activeTiles.Add(neighbor);
                    }
                }
            }
            return null;
        }

        private bool MoveTowards(Vector2 destination, Level level)
        {
            Tile guardTile = new Tile(X, Y);
            Tile destinationTile = new Tile(destination.X, destination.Y);
            Tile tileToMoveTo = Pathfind(level, destinationTile, guardTile);

            if (tileToMoveTo != null)
            {
                Vector2 movementCoordinates = new Vector2(tileToMoveTo.X, tileToMoveTo.Y);

                Move(level, movementCoordinates);
                return true;
            }
            return false;
        }

        private void CatchPlayer(Game game)
        {
            if (isBribed)
            {
                return;
            }

            if (game.PlayerCharacter.X >= X - 1 && game.PlayerCharacter.X <= X + 1
                && game.PlayerCharacter.Y >= Y - 1 && game.PlayerCharacter.Y <= Y + 1)
            {
                game.CapturePlayer(this);
            }
        }

        private Vector2[] GetTilesBetweenGuardAndPlayer(int guardX, int guardY, int playerX, int playerY)
        {
            bool isLineSteep = Math.Abs(playerY - guardY) > Math.Abs(playerX - guardX);

            if (isLineSteep)
            {
                int temp = guardX;
                guardX = guardY;
                guardY = temp;
                temp = playerX;
                playerX = playerY;
                playerY = temp;
            }

            if (guardX > playerX)
            {
                int temp = guardX;
                guardX = playerX;
                playerX = temp;
                temp = guardY;
                guardY = playerY;
                playerY = temp;
            }

            int deltaX = playerX - guardX;
            int deltaY = Math.Abs(playerY - guardY);
            int error = deltaX / 2;
            int yStep = (guardY < playerY) ? 1 : -1;
            int y = guardY;

            List<Vector2> tilesBetweenGuardAndPlayer = new List<Vector2>();

            for (int x = guardX; x <= playerX; x++)
            {
                tilesBetweenGuardAndPlayer.Add(new Vector2((isLineSteep ? y : x), (isLineSteep ? x : y)));
                error = error - deltaY;
                if (error < 0)
                {
                    y += yStep;
                    error += deltaX;
                }
            }

            return tilesBetweenGuardAndPlayer.ToArray();
        }

        private Vector2 Patrol()
        {
            if (X != patrolPath[nextPatrolPoint].X || Y != patrolPath[nextPatrolPoint].Y)
            {
                return new Vector2(patrolPath[nextPatrolPoint].X, patrolPath[nextPatrolPoint].Y);
            }
            else
            {
                if (nextPatrolPoint < patrolPath.Length - 1)
                {
                    nextPatrolPoint++;
                }
                else
                {
                    nextPatrolPoint = 0;
                }
            }

            return new Vector2(X, Y);
        }

        private void Pivot(int timer, int frequency)
        {
            if (timer % frequency == 0)
            {
                if (direction == Directions.left)
                {
                    direction = Directions.up;
                }
                else
                {
                    direction += 1;
                }

                guardMarker = guardMarkersTable[(int)direction];
            }
        }

        private void Move(Level level, Vector2 tileToMoveTo)
        {
            this.Clear(level);

            if (X != tileToMoveTo.X)
            {
                if (X - tileToMoveTo.X > 0)
                {
                    if (level.IsPositionWalkable(X - 1, Y))
                    {
                        X--;
                        direction = Directions.left;
                    }
                }
                else
                {
                    if (level.IsPositionWalkable(X + 1, Y))
                    {
                        X++;
                        direction = Directions.right;
                    }
                }
            }
            else if (Y != tileToMoveTo.Y)
            {
                if (Y - tileToMoveTo.Y > 0)
                {
                    if (level.IsPositionWalkable(X, Y - 1))
                    {
                        Y--;
                        direction = Directions.up;
                    }
                }
                else
                {
                    if (level.IsPositionWalkable(X, Y + 1))
                    {
                        Y++;
                        direction = Directions.down;
                    }
                }
            }

            guardMarker = guardMarkersTable[(int)direction];

            if (level.GetElementAt(X, Y) == SymbolsConfig.LeverOnChar.ToString())
            {
                level.ToggleLever(X, Y);
            }
        }
    }
}
