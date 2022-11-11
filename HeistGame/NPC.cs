/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using static System.Console;

namespace HeistGame
{
    internal abstract class NPC: IAttackable
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

        protected int sightDistance;
        protected int hearingDistance;
        protected int alertLevel;
        protected int alertRestingLevel;
        protected int sightTimer;
        protected int sightTick;
        protected int hearingTimer;
        protected int hearingTick;
        protected int alertTimer;
        protected int alertTick;
        protected int previouslyHeardNoiseLevel;

        protected AIBehaviors behavior;
        protected AIState state;

        protected Random rng;

        protected NPC()
        {
        }

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
        public int Health { get; protected set; }
        public bool IsUnconscious { get; protected set; }
        public bool IsDead { get; protected set; }

        /// <summary>
        /// Restores the NPC to their conditions at the beginning of the level. To be used only when retrying levels
        /// </summary>
        public abstract void Reset();

        public void GetHit()
        {
            if (IsDead) { return; }
            if (Health > 0) { Health--; }
            else { IsDead = true; }
        }

        #region Movement
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
        #endregion

        #region AI
        /// <summary>
        /// Updates the NPC's AI behavior
        /// </summary>
        /// <param name="level">The level the guard is in</param>
        /// <param name="game">The current game</param>
        /// <param name="deltaTimeMS">frame timing, to handle movement speed</param>
        public virtual void UpdateBehavior(Level level, Game game, int deltaTimeMS)
        {
            if (alertLevel > 15)
            {
                alertLevel = 15;
            }

            timeSinceLastMove += deltaTimeMS;
        }

        protected bool UpdatePerception(Player player, Level level, int distance)
        {
            Vector2 obstacle = new Vector2(-1, -1);

            bool hasFoundPlayer = SpotPlayer(player, level, distance, ref obstacle);

            hasFoundPlayer = hasFoundPlayer || HearPlayer(player, distance, ref obstacle);

            return hasFoundPlayer;
        }

        //TODO: add timers to pace the speed at which alertLevel increases
        protected bool SpotPlayer(Player player, Level level, int distance, ref Vector2 obstacle)
        {
            bool seen = false;

            switch (Direction)
            {
                case Directions.up:
                    if (player.Y <= Y && distance <= sightDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

                        foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
                        {
                            if (!level.IsTileTransparent(tile.X, tile.Y))
                            {
                                obstacle = tile;
                                break;
                            }
                        }
                        seen = EvaluatePlayerVisibility(player, distance);
                        break;
                    }
                    else
                    {
                        break;
                    }

                case Directions.right:
                    if (player.X >= X && distance <= sightDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

                        foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
                        {
                            if (!level.IsTileTransparent(tile.X, tile.Y))
                            {
                                obstacle = tile;
                                break;
                            }
                        }
                        seen = EvaluatePlayerVisibility(player, distance);
                        break;
                    }
                    else
                    {
                        break;
                    }

                case Directions.down:
                    if (player.Y >= Y && distance <= sightDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

                        foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
                        {
                            if (!level.IsTileTransparent(tile.X, tile.Y))
                            {
                                obstacle = tile;
                                break;
                            }
                        }
                        seen = EvaluatePlayerVisibility(player, distance);
                        break;
                    }
                    else
                    {
                        break;
                    }

                case Directions.left:
                    if (player.X <= X && distance <= sightDistance)
                    {
                        Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

                        foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
                        {
                            if (!level.IsTileTransparent(tile.X, tile.Y))
                            {
                                obstacle = tile;
                                break;
                            }
                        }
                        seen = EvaluatePlayerVisibility(player, distance);
                        break;
                    }
                    else
                    {
                        break;
                    }
            }
            return seen;
        }

        protected bool HearPlayer(Player player, int distance, ref Vector2 obstacle)
        {
            if (distance > hearingDistance) { return false; }

            int noiseModifier = 0;

            if (obstacle.X != -1 && GetDistanceFromTile(obstacle) <= hearingDistance)
            {
                noiseModifier = 2;
            }

            int noise = player.Noise - noiseModifier;

            if (hearingTimer >= hearingTick || noise != previouslyHeardNoiseLevel)
            {
                alertLevel += noise;
                hearingTimer = 0;
            }

            previouslyHeardNoiseLevel = noise;

            if (noise >= 3)
            {
                alertLevel = 15;
                return true;
            }

            if (state == AIState.PreviouslyFoundPlayer && noise > 0)
            {
                return true;
            }

            return false;
        }

        protected bool EvaluatePlayerVisibility(Player player, int distance)
        {
            switch (player.Visibility)
            {
                case >= 3:
                    alertLevel = 15;
                    behavior = AIBehaviors.Chasing;
                    return true;
                case 2:
                    if (distance <= 10)
                    {
                        alertLevel = 15; 
                        behavior = AIBehaviors.Chasing;
                        return true;
                    }
                    else if (sightTimer >= sightTick)
                    {
                        alertLevel += player.Visibility * 2;
                        sightTimer = 0;
                        return false;
                    }
                    return false;
                case 1:
                    if (distance <= 5)
                    {
                        alertLevel = 15;
                        behavior = AIBehaviors.Chasing;
                        return true;
                    }
                    else if (sightTimer >= sightTick)
                    { 
                        alertLevel += player.Visibility * 2;
                        sightTimer = 0;
                        return false;
                    }
                    return false;
            }
            return false;
        }

        //protected bool SpotPlayer(Player player, Level level)
        //{
        //    int aggroDistance = player.Visibility;
        //    if (aggroDistance <= 0) { aggroDistance = 1; }

        //    switch (Direction)
        //    {
        //        case Directions.up:
        //            if (player.Y <= Y && IsTileInRange(player.X, player.Y, aggroDistance))
        //            {
        //                Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

        //                foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
        //                {
        //                    if (!level.IsTileTransparent(tile.X, tile.Y))
        //                    {
        //                        return false;
        //                    }
        //                }
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }

        //        case Directions.right:
        //            if (player.X >= X && IsTileInRange(player.X, player.Y, aggroDistance))
        //            {
        //                Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

        //                foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
        //                {
        //                    if (!level.IsTileTransparent(tile.X, tile.Y))
        //                    {
        //                        return false;
        //                    }
        //                }
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }

        //        case Directions.down:
        //            if (player.Y >= Y && IsTileInRange(player.X, player.Y, aggroDistance))
        //            {
        //                Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

        //                foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
        //                {
        //                    if (!level.IsTileTransparent(tile.X, tile.Y))
        //                    {
        //                        return false;
        //                    }
        //                }
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }

        //        case Directions.left:
        //            if (player.X <= X && IsTileInRange(player.X, player.Y, aggroDistance))
        //            {
        //                Vector2[] tilesBetweenGuardAndPlayer = Rasterizer.GetCellsAlongLine(this.X, this.Y, player.X, player.Y);

        //                foreach (Vector2 tile in tilesBetweenGuardAndPlayer)
        //                {
        //                    if (!level.IsTileTransparent(tile.X, tile.Y))
        //                    {
        //                        return false;
        //                    }
        //                }
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //    }
        //    return false;
        //}
        #endregion

        #region Utilities
        protected bool IsTileInRange(int targetX, int targetY, int range)
        {
            return((X - targetX) * (X - targetX) + (Y - targetY) * (Y - targetY)) < (range * range);
        }

        protected int GetDistanceFromTile(int targetX, int targetY)
        {

            int xDist = X - targetX;
            int yDist = Y - targetY;

            return (int)MathF.Sqrt((xDist * xDist) + (yDist * yDist));
        }

        protected int GetDistanceFromTile(Vector2 target)
        {
            int xDist = X - target.X;
            int yDist = Y - target.Y;

            return (xDist * xDist) + (yDist * yDist);
        }
        #endregion
    }

    public enum AIBehaviors
    {
        Neutral,
        Suspicious,
        Alerted,
        Searching,
        Chasing,
        Fighting,
        Fleeing,
        Returning
    }

    public enum AIState
    {
        NotFoundPlayer,
        FoundPlayer,
        PreviouslyFoundPlayer
    }
}
