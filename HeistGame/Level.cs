using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Console;

namespace HeistGame
{
    /// <summary>
    /// Holds informations about the current level
    /// </summary>
    class Level
    {
        private string[,] grid;
        private int rows;
        private int columns;
        private int xOffset;
        private int yOffset;
        private Vector2 exit;
        private LevelLock levelLock;
        private Dictionary<Vector2, Lever> leversDictionary;
        private Vector2[] treasures;
        private Guard[] levelGuards;
        private Stopwatch stopwatch;

        /// <summary>
        /// The name of the floor, extracted from the level file name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The briefing tetx to be displayed before a level starts
        /// </summary>
        public string[] Briefing { get; private set; }

        /// <summary>
        /// The outro text to be displayed once the level has been completed
        /// </summary>
        public string[] Outro { get; private set; }

        /// <summary>
        /// Whether the exit is open (either because there's no key in the level or because the player has collected the key) or not
        /// </summary>
        public bool IsLocked { get; private set; }

        /// <summary>
        /// The X coordinate of the player's starting position
        /// </summary>
        public int PlayerStartX { get; private set; }
        /// <summary>
        /// The Y coordinate of the player's starting position
        /// </summary>
        public int PlayerStartY { get; private set; }
        /// <summary>
        /// The set of all walkable tiles on the floor, including those under gates, levers, treasures and keys
        /// </summary>
        public HashSet<Vector2> FloorTiles { get; private set; }
        /// <summary>
        /// The lightmap of the level
        /// </summary>
        public LightMap Lights { get; private set; }

        /// <summary>
        /// Instantiates a World object
        /// </summary>
        /// <param name="name">The level's title</param>
        /// <param name="grid">The grid of sumbols that represents the level, in the form of a bi-dimensional string array</param>
        /// <param name="startX">The player's starting X coordinate, int format</param>
        /// <param name="startY">The player's starting Y coordinate, int format</param>
        /// <param name="floorTiles">The collection of all walkable tiles in the level</param>
        /// <param name="levelLock">The System that handles keys in the level, as a LevelLock object</param>
        /// <param name="exit">The coordinates of the exit point</param>
        /// <param name="treasures">The array containing the coordinates of all the treasures in the level</param>
        /// <param name="levers">The collection of levers in the level</param>
        /// <param name="guards">The collection of guards in the level</param>
        /// <param name="briefing">The intro text to the level; an array of strings, one per each line.</param>
        /// <param name="outro">The text to be displayed once the level is complete; an array of strings, one per each line.</param>
        /// <param name="stopwatch">The game's Stopwatch field</param>
        public Level(string name, string[,] grid, int startX, int startY, HashSet<Vector2> floorTiles, LightMap lightmap, LevelLock levelLock, Vector2 exit,
                     Vector2[] treasures, Dictionary<Vector2, Lever> levers, Guard[] guards, string[] briefing, string[] outro, Stopwatch stopwatch)
        {
            Name = name;
            Briefing = briefing;
            Outro = outro;

            this.grid = grid;

            rows = this.grid.GetLength(0);
            columns = this.grid.GetLength(1);

            xOffset = (WindowWidth / 2) - (columns / 2);
            yOffset = ((WindowHeight - 5) / 2) - (rows / 2);
            //yOffset = 0;

            this.stopwatch = stopwatch;

            this.treasures = treasures;

            this.exit = exit;

            leversDictionary = new Dictionary<Vector2, Lever>();

            foreach (KeyValuePair<Vector2, Lever> leverInfo in levers)
            {
                Vector2 coordinatesWithOffset = new Vector2(leverInfo.Key.X + xOffset, leverInfo.Key.Y + yOffset);
                Lever lever = leverInfo.Value;

                leversDictionary[coordinatesWithOffset] = lever;
            }

            levelGuards = guards;

            foreach (Guard guard in guards)
            {
                guard.AssignOffset(xOffset, yOffset);
            }

            this.levelLock = levelLock;

            IsLocked = levelLock.IsLocked();

            PlayerStartX = startX + xOffset;
            PlayerStartY = startY + yOffset;

            FloorTiles = floorTiles;
            Lights = lightmap;

            Lights.CalculateLightMap(this);
        }

        /// <summary>
        /// Draws the map on screen, automatically centered
        /// </summary>
        public void Draw()
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    string element = grid[y, x];
                    SetCursorPosition(x + xOffset, y + yOffset);

                    if (element == SymbolsConfig.EnclosedSpaceChar.ToString())
                    {
                        element = SymbolsConfig.EmptySpace.ToString();
                    }
                    else if (element == SymbolsConfig.EmptySpace.ToString())
                    {
                        Vector2 tile = new Vector2(x, y);
                        int lightValue = Lights.FloorTilesValues[tile];
                        ForegroundColor = ConsoleColor.DarkBlue;
                        switch (lightValue)
                        {
                            case 0:
                                break;
                            case 1:
                                element = SymbolsConfig.Light1char.ToString();
                                break;
                            case 2:
                                element = SymbolsConfig.Light2char.ToString();
                                break;
                            case 3:
                                element = SymbolsConfig.Light3char.ToString();
                                break;
                        }
                    }
                    else if (element == SymbolsConfig.ExitChar.ToString())
                    {
                        if (IsLocked)
                        {
                            ForegroundColor = ConsoleColor.Red;
                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Green;
                        }
                    }
                    else if (element == SymbolsConfig.KeyChar.ToString())
                    {
                        ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else if (element == SymbolsConfig.TreasureChar.ToString())
                    {
                        ForegroundColor = ConsoleColor.Yellow;
                    }
                    else if (element == "☺")
                    {
                        ForegroundColor = ConsoleColor.DarkMagenta;
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Gray;
                    }
                    Write(element);
                }
            }

            ResetColor();
        }


        private void DrawTile(int x, int y, string element)
        {
            SetCursorPosition(x, y);
            if (element == SymbolsConfig.EmptySpace.ToString())
            {
                Vector2 tile = new Vector2(x, y);
                int lightValue = GetLightLevelInItle(tile);
                ForegroundColor = ConsoleColor.DarkBlue;
                switch (lightValue)
                {
                    case 0:
                        break;
                    case 1:
                        element = SymbolsConfig.Light1char.ToString();
                        break;
                    case 2:
                        element = SymbolsConfig.Light2char.ToString();
                        break;
                    case 3:
                        element = SymbolsConfig.Light3char.ToString();
                        break;
                }
            }
            else if (element == SymbolsConfig.ExitChar.ToString())
            {
                if (IsLocked)
                {
                    ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    ForegroundColor = ConsoleColor.Green;
                }
            }
            else if (element == SymbolsConfig.KeyChar.ToString())
            {
                ForegroundColor = ConsoleColor.DarkYellow;
            }
            else if (element == SymbolsConfig.TreasureChar.ToString())
            {
                ForegroundColor = ConsoleColor.Yellow;
            }
            else if (element == "☺")
            {
                ForegroundColor = ConsoleColor.DarkMagenta;
            }
            else
            {
                ForegroundColor = ConsoleColor.Gray;
            }
            Write(element);
            ResetColor();
        }

        /// <summary>
        /// Returns the light level at the tile's coordinates
        /// </summary>
        /// <param name="tile">The coordinates of the tile to be checked, as a Vector2</param>
        /// <param name="withOffset">Set to true if the coordinates provided are with the centering offsets</param>
        /// <returns></returns>
        public int GetLightLevelInItle(Vector2 tile, bool withOffset = true)
        {
            if (withOffset)
            {
                tile.X -= xOffset;
                tile.Y -= yOffset;
            }

            return Lights.FloorTilesValues[tile];
        }

        /// <summary>
        /// Checks if a certain position on the grid contains a symbol that can be traversed by the player or the guards
        /// </summary>
        /// <param name="x">The X coordinate of the position to check</param>
        /// <param name="y">The Y coordinate of the position to check</param>
        /// <returns>Returns true if walkable, false if not</returns>
        public bool IsPositionWalkable(int x, int y)
        {
            x -= xOffset;
            y -= yOffset;

            if (x < 0 || y < 0 || x >= columns || y >= rows)
            {
                return false;
            }

            return grid[y, x] == SymbolsConfig.EmptySpace.ToString() ||
                   grid[y, x] == SymbolsConfig.Light1char.ToString() ||
                   grid[y, x] == SymbolsConfig.Light2char.ToString() ||
                   grid[y, x] == SymbolsConfig.Light3char.ToString() ||
                   grid[y, x] == "-" ||
                   grid[y, x] == "|" ||
                   grid[y, x] == SymbolsConfig.EntranceChar.ToString() ||
                   grid[y, x] == SymbolsConfig.ExitChar.ToString() ||
                   grid[y, x] == SymbolsConfig.KeyChar.ToString() ||
                   grid[y, x] == SymbolsConfig.TreasureChar.ToString() ||
                   grid[y, x] == SymbolsConfig.LeverOffChar.ToString() ||
                   grid[y, x] == SymbolsConfig.LeverOnChar.ToString();
        }

        /// <summary>
        /// Checks if the given position in the grid contains a symbols that allows sight and light to pass through.
        /// </summary>
        /// <param name="x">The X coordinate of the position to check</param>
        /// <param name="y">The Y coordinate of the position to check</param>
        /// <param name="removeOffset">Indicates whether the check must be performed with or without the offsets used to center the map on the screen</param>
        /// <returns></returns>
        public bool IsTileTransparent(int x, int y, bool removeOffset = true)
        {
            if (removeOffset)
            {
                x -= xOffset;
                y -= yOffset;
            }

            if (x < 0 || y < 0 || x >= columns || y >= rows)
            {
                return false;
            }

            return grid[y, x] == SymbolsConfig.EmptySpace.ToString() ||
                   grid[y, x] == SymbolsConfig.Light1char.ToString() ||
                   grid[y, x] == SymbolsConfig.Light2char.ToString() ||
                   grid[y, x] == SymbolsConfig.Light3char.ToString() ||
                   grid[y, x] == SymbolsConfig.ExitChar.ToString() ||
                   grid[y, x] == SymbolsConfig.EntranceChar.ToString() ||
                   grid[y, x] == SymbolsConfig.KeyChar.ToString() ||
                   grid[y, x] == SymbolsConfig.TreasureChar.ToString() ||
                   grid[y, x] == SymbolsConfig.LeverOffChar.ToString() ||
                   grid[y, x] == SymbolsConfig.LeverOnChar.ToString();
        }

        /// <summary>
        /// Gets which symbol is present in a given location
        /// </summary>
        /// <param name="x">The X coordinate of the position to check</param>
        /// <param name="y">The Y coordinate of the position to check</param>
        /// <returns>Returns the symbol found at these coordinates on the grid</returns>
        public string GetElementAt(int x, int y)
        {
            return grid[y - yOffset, x - xOffset];
        }

        public string GetElementAt(int x, int y, bool withOffset = true)
        {
            if (withOffset)
            {
                y -= yOffset;

                x -= xOffset;
            }

            return grid[y, x];
        }

        /// <summary>
        /// Replaces a symbol in a given location
        /// </summary>
        /// <param name="x">The X coordinate of the symbol to replace</param>
        /// <param name="y">The X coordinate of the symbol to replace</param>
        /// <param name="newElement">The new symbol</param>
        /// <param name="withOffset">(optional) default true, set to false if the indicated coordinates are without the offset applied</param>
        public void ChangeElementAt(int x, int y, string newElement, bool withOffset = true, bool redraw = true)
        {
            int destX = x;
            int destY = y;

            if (withOffset)
            {
                destX -= xOffset;
                destY -= yOffset;
            }

            grid[destY, destX] = newElement;

            if (redraw)
            {
                if (!withOffset)
                {
                    x += xOffset;
                    y += yOffset;
                }

                DrawTile(x, y, newElement);
            }
        }

        /// <summary>
        /// Resets the floor elements (levers and gates, treasures, keys, guards) to their original state
        /// </summary>
        public void Reset()
        {
            ResetLevers();
            ResetGuards();
            ResetKeys();
            ResetTreasures();
            Lights.CalculateLightMap(this);
        }

        /// <summary>
        /// Pathfinding helper function. Returns the immediately adjecent walkable tiles (north, west, south and east) to the one provided
        /// </summary>
        /// <param name="currentTile">The Tile to find neighbors of</param>
        /// <param name="targetTile">The destination of the pathfinding</param>
        /// <returns></returns>
        public List<Tile> GetWalkableNeighborsOfTile(Tile currentTile, Tile targetTile)
        {
            List<Tile> neighborsList = new List<Tile>();

            if (currentTile.Y - 1 >= 0 && IsPositionWalkable(currentTile.X, currentTile.Y - 1))
            {
                neighborsList.Add(CreateNewTile(currentTile.X, currentTile.Y - 1));
            }

            if (currentTile.Y + 1 < rows + yOffset && IsPositionWalkable(currentTile.X, currentTile.Y + 1))
            {
                neighborsList.Add(CreateNewTile(currentTile.X, currentTile.Y + 1));
            }

            if (currentTile.X - 1 >= 0 && IsPositionWalkable(currentTile.X - 1, currentTile.Y))
            {
                neighborsList.Add(CreateNewTile(currentTile.X - 1, currentTile.Y));
            }

            if (currentTile.X + 1 < columns + xOffset && IsPositionWalkable(currentTile.X + 1, currentTile.Y))
            {
                neighborsList.Add(CreateNewTile(currentTile.X + 1, currentTile.Y));
            }

            return neighborsList;

            Tile CreateNewTile(int x, int y)
            {
                Tile tile = new Tile(x, y);
                tile.Parent = currentTile;
                tile.Cost = currentTile.Cost + 1;
                tile.SetDistance(targetTile.X, targetTile.Y);

                return tile;
            }
        }

        /// <summary>
        /// Toggles the lever in a given position on the grid
        /// </summary>
        /// <param name="x">The X coordinate on the grid of the level to toggle</param>
        /// <param name="y">The Y coordinate on the grid of the level to toggle</param>
        public void ToggleLever(int x, int y)
        {
            stopwatch.Stop();

            Vector2 leverCoord = new Vector2(x, y);

            if (leversDictionary.ContainsKey(leverCoord))
            {
                Lever lever = leversDictionary[leverCoord];
                lever.Toggle(this, xOffset, yOffset);
            }

            Lights.CalculateLightMap(this);
            RedrawFloors();
            stopwatch.Start();
        }

        /// <summary>
        /// Collects the key piece and cheks the locked status
        /// </summary>
        /// <param name="x">The X coordinate of the collected key</param>
        /// <param name="y">The Y coordinate of the collected key</param>
        public void CollectKeyPiece(int x, int y)
        {
            IsLocked = levelLock.CollectKeyPiece(this, x, y);

            if (!IsLocked)
            {
                DrawTile(exit.X + xOffset, exit.Y + yOffset, SymbolsConfig.ExitChar.ToString());
            }
        }

        /// <summary>
        /// Updates all the guards in the level. moving them along their patrols
        /// </summary>
        /// <param name="deltaDimeMS">The time passed since last check, to set the guard's speed</param>
        /// <param name="game">The current game</param>
        public void UpdateGuards(int deltaDimeMS, Game game)
        {
            if (levelGuards.Length > 0)
            {
                foreach (Guard guard in levelGuards)
                {
                    guard.UpdateBehavior(this, game, deltaDimeMS);
                }
            }
        }

        /// <summary>
        /// Draws all guards
        /// </summary>
        public void DrawGuards()
        {
            if (levelGuards.Length > 0)
            {
                foreach (Guard guard in levelGuards)
                {
                    guard.Draw();
                }
            }
        }

        private void RedrawFloors()
        {
            HashSet<Vector2> guardsPositions = new HashSet<Vector2>();
            for (int i = 0; i < levelGuards.Length; i++)
            {
                Vector2 guardPos = new Vector2(levelGuards[i].X, levelGuards[i].Y);
                guardsPositions.Add(guardPos);
            }

            foreach (Vector2 tile in FloorTiles)
            {
                if (GetElementAt(tile.X, tile.Y, false) != SymbolsConfig.EmptySpace.ToString())
                {
                    continue;
                }

                Vector2 tileWithOffset = new Vector2(tile.X + xOffset, tile.Y + yOffset);

                if (guardsPositions.Contains(tileWithOffset))
                {
                    //skips tiles with guards to prevent flickering. The guard's update will take care of redrawing the tile once they move.
                    continue;
                }

                SetCursorPosition(tile.X + xOffset, tile.Y + yOffset);

                char symbol = SymbolsConfig.EmptySpace;

                int lightValue = GetLightLevelInItle(tile, false);

                ForegroundColor = ConsoleColor.DarkBlue;
                switch (lightValue)
                {
                    case 0:
                        break;
                    case 1:
                        symbol = SymbolsConfig.Light1char;
                        break;
                    case 2:
                        symbol = SymbolsConfig.Light2char;
                        break;
                    case 3:
                        symbol = SymbolsConfig.Light3char;
                        break;
                }

                Write(symbol);
            }
        }

        private void ResetTreasures()
        {
            foreach (Vector2 treasure in treasures)
            {
                ChangeElementAt(treasure.X, treasure.Y, SymbolsConfig.TreasureChar.ToString(), false, false);
            }
        }

        private void ResetLevers()
        {
            foreach (Lever lever in leversDictionary.Values)
            {
                if (lever.IsOn)
                {
                    lever.Toggle(this, xOffset, yOffset, false);
                }
            }
        }

        private void ResetKeys()
        {
            levelLock.ResetKeys(this);
        }

        private void ResetGuards()
        {
            foreach (Guard guard in levelGuards)
            {
                guard.Reset();
            }
        }
    }
}
