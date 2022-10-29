/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace HeistGame
{
    internal abstract class NPC
    {
        protected char[] npcMarkersLUT = new char[] { SymbolsConfig.NPCMarkerUp, 
                                                      SymbolsConfig.NPCMarkerRight,
                                                      SymbolsConfig.NPCMarkerDown,
                                                      SymbolsConfig.NPCMarkerLeft };
        protected int pivotTimer;
        protected int pivotDirection;
        protected int minTimeBetweenPivots;
        protected int durationTimer;
        protected int timeBetweenMoves;
        protected int timeSinceLastMove = 0;

        protected Random rng;

        /// <summary>
        /// The X coordinate of the NPC
        /// </summary>
        public int X { get; protected set; }
        /// <summary>
        /// The Y coordinate of the NPC
        /// </summary>
        public int Y { get; protected set; }
        public Directions Direction { get; protected set; } = Directions.down;
        public char NPCMarker { get; protected set; }
        public ConsoleColor NPCSymbolColor { get; protected set; } = ConsoleColor.White;
        public ConsoleColor NPCTileColor { get; protected set; } = ConsoleColor.Black;

        /// <summary>
        /// Restores the NPC to their conditions at the beginning of the level. To be used only when retrying levels
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Updates the NPC's AI behavior
        /// </summary>
        /// <param name="level">The level the guard is in</param>
        /// <param name="game">The current game</param>
        /// <param name="deltaTimeMS">frame timing, to handle movement speed</param>
        public abstract void UpdateBehavior(Level level, Game game, int deltaTimeMS);

        protected Tile Pathfind(Level level, Tile pathStart, Tile destination)
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

        protected bool MoveTowards(Vector2 destination, Game game)
        {
            Tile guardTile = new Tile(X, Y);
            Tile destinationTile = new Tile(destination.X, destination.Y);
            Tile tileToMoveTo = Pathfind(game.ActiveCampaign.Levels[game.CurrentLevel], destinationTile, guardTile);

            if (tileToMoveTo != null)
            {
                Vector2 movementCoordinates = new Vector2(tileToMoveTo.X, tileToMoveTo.Y);

                Move(game, movementCoordinates);
                return true;
            }
            return false;
        }

        protected void Move(Game game, Vector2 tileToMoveTo)
        {
            if (timeSinceLastMove < timeBetweenMoves)
            {
                return;
            }

            if (X != tileToMoveTo.X)
            {
                if (X - tileToMoveTo.X > 0)
                {
                    if (game.ActiveCampaign.Levels[game.CurrentLevel].IsTileWalkable(X - 1, Y))
                    {
                        X--;
                        Direction = Directions.left;
                    }
                }
                else
                {
                    if (game.ActiveCampaign.Levels[game.CurrentLevel].IsTileWalkable(X + 1, Y))
                    {
                        X++;
                        Direction = Directions.right;
                    }
                }
            }
            else if (Y != tileToMoveTo.Y)
            {
                if (Y - tileToMoveTo.Y > 0)
                {
                    if (game.ActiveCampaign.Levels[game.CurrentLevel].IsTileWalkable(X, Y - 1))
                    {
                        Y--;
                        Direction = Directions.up;
                    }
                }
                else
                {
                    if (game.ActiveCampaign.Levels[game.CurrentLevel].IsTileWalkable(X, Y + 1))
                    {
                        Y++;
                        Direction = Directions.down;
                    }
                }
            }

            NPCMarker = npcMarkersLUT[(int)Direction];

            if (game.ActiveCampaign.Levels[game.CurrentLevel].GetElementAt(X, Y) == SymbolsConfig.LeverOn)
            {
                game.ActiveCampaign.Levels[game.CurrentLevel].ToggleLever(X, Y);
            }

            timeSinceLastMove -= timeBetweenMoves;
        }

        protected bool Pivot(int timer, int frequency, int minTime, int pivotDuration = 0, bool limitedDuration = false)
        {
            if (timeSinceLastMove < timeBetweenMoves)
            {
                return true;
            }

            if (minTimeBetweenPivots > 0)
            {
                timeSinceLastMove -= timeBetweenMoves;
                minTimeBetweenPivots--;
                return true;
            }

            if (timer % frequency == 0)
            {
                if (Direction == Directions.left && pivotDirection == 1)
                {
                    Direction = Directions.up;
                }
                else if (Direction == Directions.up && pivotDirection == -1)
                {
                    Direction = Directions.left;
                }
                else
                {
                    Direction += pivotDirection;
                }

                NPCMarker = npcMarkersLUT[(int)Direction];

                minTimeBetweenPivots = minTime;
            }

            if (limitedDuration)
            {
                durationTimer--;
                if (durationTimer == 0)
                {
                    durationTimer = pivotDuration;
                    timeSinceLastMove -= timeBetweenMoves;
                    return false;
                }
            }

            timeSinceLastMove -= timeBetweenMoves;
            return true;
        }

        protected void ChoosePivotDirection()
        {
            int choice = rng.Next(0, 101);

            if (choice % 2 == 0) { pivotDirection = 1; }
            else { pivotDirection = -1; }
        }

        protected bool SpotPlayer(Player player, Level level)
        {
            int verticalAggroDistance = player.Visibility;
            if (verticalAggroDistance <= 0) { verticalAggroDistance = 1; }
            int horizontalAggroDistance = verticalAggroDistance;

            switch (Direction)
            {
                case Directions.up:
                    if (player.X >= X - horizontalAggroDistance && player.X <= X + horizontalAggroDistance
                        && player.Y >= Y - verticalAggroDistance && player.Y <= Y + 1)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

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
                    if (player.X >= X - 1 && player.X <= X + horizontalAggroDistance
                        && player.Y >= Y - verticalAggroDistance && player.Y <= Y + verticalAggroDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

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
                    if (player.X >= X - horizontalAggroDistance && player.X <= X + horizontalAggroDistance
                        && player.Y >= Y - 1 && player.Y <= Y + verticalAggroDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

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
                    if (player.X >= X - horizontalAggroDistance && player.X <= X + 1
                        && player.Y >= Y - verticalAggroDistance && player.Y <= Y + verticalAggroDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

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

        protected bool HearPlayer(Player player, Level level)
        {
            Vector2 position = new Vector2(X, Y);
            if (level.PlayerHearingArea.Contains(position))
            {
                return true;
            }
            return true;
        }
    }
}
