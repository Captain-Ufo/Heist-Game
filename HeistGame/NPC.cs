using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace HeistGame
{
    internal abstract class NPC
    {
        protected ConsoleColor npcSymbolColor = ConsoleColor.White;
        protected ConsoleColor npcTileColor = ConsoleColor.Black;
        protected string[] npcMarkersLUT = new string[] { "^", ">", "V", "<" };
        protected string npcMarker;

        protected Directions direction = Directions.down;

        protected int pivotTimer;
        protected int pivotDirection;
        protected int minTimeBetweenPivots;
        protected int durationTimer;

        protected Random rng;
        protected ScreenDisplayer screenDisplayer;

        public NPC(ScreenDisplayer sc)
        {
            screenDisplayer = sc;
        }

        /// <summary>
        /// The X coordinate of the NPC
        /// </summary>
        public int X { get; protected set; }
        /// <summary>
        /// The Y coordinate of the NPC
        /// </summary>
        public int Y { get; protected set; }

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

        /// <summary>
        /// Draws the guard symbol
        /// </summary>
        public void Draw(Game game)
        {
            Vector2 tile = new Vector2(X, Y);

            if (screenDisplayer.IsTileUnderLable(tile)) { return; }

            if (!game.ActiveCampaign.Levels[game.CurrentRoom].CanPlayerHearTile(tile)) { return; }

            ConsoleColor previousFColor = ForegroundColor;
            ConsoleColor previusBGColor = BackgroundColor;

            if (game.ActiveCampaign.Levels[game.CurrentRoom].CanPlayerSeeTile(tile))
            {
                ForegroundColor = npcSymbolColor;
                BackgroundColor = npcTileColor;
            }
            else
            {
                ForegroundColor = ConsoleColor.DarkGray;
                BackgroundColor = ConsoleColor.DarkGray;
            }

            SetCursorPosition(X, Y);
            Write(npcMarker);
            ForegroundColor = previousFColor;
            BackgroundColor = previusBGColor;
        }

        /// <summary>
        /// Replaces the guard symbol with whatever static tile is in the map grid in the previous position of the guard
        /// </summary>
        /// <param name="level">The level from which to gather the information required (which symbol to use, the state of the exit, etc)</param>
        public void Clear(Level level)
        {
            Vector2 tile = new Vector2(X, Y);
            string symbol = level.GetElementAt(X, Y);

            SetCursorPosition(X, Y);

            if (symbol == SymbolsConfig.Empty.ToString())
            {
                if (level.CanPlayerSeeTile(tile))
                {
                    int lightValue = level.GetLightLevelInItile(tile);
                    ForegroundColor = ConsoleColor.DarkBlue;
                    switch (lightValue)
                    {
                        case 0:
                            symbol = SymbolsConfig.Light0.ToString();
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
            }
            else if (symbol == SymbolsConfig.Treasure.ToString())
            {
                if (level.CanPlayerSeeTile(tile))
                    { ForegroundColor = ConsoleColor.Yellow; }
                else if (level.HasPlayerExploredTile(tile))
                        { ForegroundColor = ConsoleColor.DarkGray; }
                else { symbol = SymbolsConfig.Empty.ToString(); }
            }
            else if (symbol == SymbolsConfig.Key.ToString())
            {
                if (level.CanPlayerSeeTile(tile))
                    { ForegroundColor = ConsoleColor.Yellow; }
                else if (level.HasPlayerExploredTile(tile))
                        { ForegroundColor = ConsoleColor.DarkGray; }
                else { symbol = SymbolsConfig.Empty.ToString(); }
            }
            else if (symbol == SymbolsConfig.Exit.ToString())
            {
                if (level.CanPlayerSeeTile(tile))
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
                else if(level.HasPlayerExploredTile(tile)) 
                        { ForegroundColor = ConsoleColor.DarkGray; }
                else { symbol = SymbolsConfig.Empty.ToString(); }
            }
            Write(symbol);
            ResetColor();
        }

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
            Tile tileToMoveTo = Pathfind(game.ActiveCampaign.Levels[game.CurrentRoom], destinationTile, guardTile);

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
            if (!screenDisplayer.IsTileUnderLable(new Vector2(X, Y)))
            {
                this.Clear(game.ActiveCampaign.Levels[game.CurrentRoom]);
            }

            if (X != tileToMoveTo.X)
            {
                if (X - tileToMoveTo.X > 0)
                {
                    if (game.ActiveCampaign.Levels[game.CurrentRoom].IsTileWalkable(X - 1, Y))
                    {
                        X--;
                        direction = Directions.left;
                    }
                }
                else
                {
                    if (game.ActiveCampaign.Levels[game.CurrentRoom].IsTileWalkable(X + 1, Y))
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
                    if (game.ActiveCampaign.Levels[game.CurrentRoom].IsTileWalkable(X, Y - 1))
                    {
                        Y--;
                        direction = Directions.up;
                    }
                }
                else
                {
                    if (game.ActiveCampaign.Levels[game.CurrentRoom].IsTileWalkable(X, Y + 1))
                    {
                        Y++;
                        direction = Directions.down;
                    }
                }
            }

            npcMarker = npcMarkersLUT[(int)direction];

            if (game.ActiveCampaign.Levels[game.CurrentRoom].GetElementAt(X, Y) == SymbolsConfig.LeverOn.ToString())
            {
                game.ActiveCampaign.Levels[game.CurrentRoom].ToggleLever(X, Y);
            }
        }

        protected bool Pivot(int timer, int frequency, int minTime, int pivotDuration = 0, bool limitedDuration = false)
        {

            if (minTimeBetweenPivots > 0)
            {
                minTimeBetweenPivots--;
                return true;
            }

            if (timer % frequency == 0)
            {
                if (direction == Directions.left && pivotDirection == 1)
                {
                    direction = Directions.up;
                }
                else if (direction == Directions.up && pivotDirection == -1)
                {
                    direction = Directions.left;
                }
                else
                {
                    direction += pivotDirection;
                }

                npcMarker = npcMarkersLUT[(int)direction];

                minTimeBetweenPivots = minTime;
            }

            if (limitedDuration)
            {
                durationTimer--;
                if (durationTimer == 0)
                {
                    durationTimer = pivotDuration;
                    return false;
                }
            }

            return true;
        }

        protected void ChoosePivotDirection()
        {
            int choice = rng.Next(0, 101);

            if (choice % 2 == 0) { pivotDirection = 1; }
            else { pivotDirection = -1; }
        }
    }
}
