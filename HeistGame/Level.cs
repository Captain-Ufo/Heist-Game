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
        private readonly string[,] grid;
        private readonly int rows;
        private readonly int columns;
        private readonly int xOffset;
        private readonly int yOffset;
        private readonly Vector2 exit;
        private readonly LevelLock levelLock;
        private readonly Dictionary<Vector2, Lever> leversDictionary;
        private readonly Vector2[] treasures;
        private readonly Guard[] levelGuards;
        private readonly Stopwatch stopwatch;
        private readonly Dictionary<Vector2, string[]> messagesDictionary;
        private readonly Dictionary<Vector2, Unlockable> unlockables;

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
        /// <param name="lightmap">The lightmap of the level</param>
        /// <param name="levelLock">The System that handles keys in the level, as a LevelLock object</param>
        /// <param name="exit">The coordinates of the exit point</param>
        /// <param name="treasures">The array containing the coordinates of all the treasures in the level</param>
        /// <param name="levers">The collection of levers in the level</param>
        /// <param name="guards">The collection of guards in the level</param>
        /// <param name="messages">The commection of messages in the level</param>
        /// <param name="briefing">The intro text to the level; an array of strings, one per each line.</param>
        /// <param name="outro">The text to be displayed once the level is complete; an array of strings, one per each line.</param>
        /// <param name="stopwatch">The game's Stopwatch field</param>
        public Level(string name, string[,] grid, int startX, int startY, HashSet<Vector2> floorTiles, LightMap lightmap, LevelLock levelLock, Vector2 exit,
                     Vector2[] treasures, Dictionary<Vector2, Lever> levers, Guard[] guards, Dictionary<Vector2, string[]> messages, Dictionary<Vector2, Unlockable> unlockables,
                     string[] briefing, string[] outro, Stopwatch stopwatch)
        {
            Name = name;
            Briefing = briefing;
            Outro = outro;

            this.grid = grid;

            rows = this.grid.GetLength(0);
            columns = this.grid.GetLength(1);

            xOffset = (WindowWidth / 2) - (columns / 2);
            yOffset = ((WindowHeight - 5) / 2) - (rows / 2);

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

            messagesDictionary = new Dictionary<Vector2, string[]>();

            foreach (KeyValuePair<Vector2, string[]> messageInfo in messages)
            {
                Vector2 coordinatesWithOffset = new Vector2(messageInfo.Key.X + xOffset, messageInfo.Key.Y + yOffset);

                messagesDictionary[coordinatesWithOffset] = messageInfo.Value;
            }

            levelGuards = guards;

            this.unlockables = unlockables;

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

                    if (element == SymbolsConfig.EnclosedSpace.ToString())
                    {
                        element = SymbolsConfig.Empty.ToString();
                    }
                    else if (element == SymbolsConfig.Empty.ToString())
                    {
                        Vector2 tile = new Vector2(x, y);
                        int lightValue = Lights.FloorTilesValues[tile];
                        ForegroundColor = ConsoleColor.DarkBlue;
                        switch (lightValue)
                        {
                            case 0:
                                break;
                            case 1:
                                element = SymbolsConfig.Light1.ToString();
                                break;
                            case 2:
                                element = SymbolsConfig.Light2.ToString();
                                break;
                            case 3:
                                element = SymbolsConfig.Light3.ToString();
                                break;
                        }
                    }
                    else if (element == SymbolsConfig.Exit.ToString())
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
                    else if (element == SymbolsConfig.Key.ToString())
                    {
                        ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else if (element == SymbolsConfig.Treasure.ToString())
                    {
                        ForegroundColor = ConsoleColor.Yellow;
                    }
                    else if (element == "☺")
                    {
                        ForegroundColor = ConsoleColor.DarkMagenta;
                    }
                    else if (element == SymbolsConfig.ChestClosed.ToString() || element == SymbolsConfig.ChestClosed.ToString())
                    {
                        ForegroundColor = ConsoleColor.White;
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


        public void DrawTile(int x, int y, string element, bool highlighted = false)
        {
            if (highlighted) { BackgroundColor = ConsoleColor.White; }

            SetCursorPosition(x, y);
            if (element == SymbolsConfig.Empty.ToString())
            {
                Vector2 tile = new Vector2(x, y);
                int lightValue = GetLightLevelInItle(tile);
                if (highlighted) { ForegroundColor = ConsoleColor.Black; }
                else { ForegroundColor = ConsoleColor.DarkBlue; }
                switch (lightValue)
                {
                    case 0:
                        break;
                    case 1:
                        element = SymbolsConfig.Light1.ToString();
                        break;
                    case 2:
                        element = SymbolsConfig.Light2.ToString();
                        break;
                    case 3:
                        element = SymbolsConfig.Light3.ToString();
                        break;
                }
            }
            else if (element == SymbolsConfig.Exit.ToString())
            {
                if (IsLocked)
                {
                    if (highlighted) { ForegroundColor = ConsoleColor.Black; }
                    else { ForegroundColor = ConsoleColor.Red; }
                }
                else
                {
                    if (highlighted) { ForegroundColor = ConsoleColor.Black; }
                    else { ForegroundColor = ConsoleColor.Green; }
                }
            }
            else if (element == SymbolsConfig.Key.ToString())
            {
                if (highlighted) { ForegroundColor = ConsoleColor.Black; }
                else { ForegroundColor = ConsoleColor.DarkYellow; }
            }
            else if (element == SymbolsConfig.Treasure.ToString())
            {
                if (highlighted) { ForegroundColor = ConsoleColor.Black; }
                else { ForegroundColor = ConsoleColor.Yellow; }
            }
            else if (element == "☺")
            {
                if (highlighted) { ForegroundColor = ConsoleColor.Black; }
                else { ForegroundColor = ConsoleColor.DarkMagenta; }
            }
            else if (element == SymbolsConfig.ChestClosed.ToString() || element == SymbolsConfig.ChestClosed.ToString())
            {
                if (highlighted) { ForegroundColor = ConsoleColor.Black; }
                ForegroundColor = ConsoleColor.White;
            }
            else
            {
                if (highlighted) { ForegroundColor = ConsoleColor.Black; }
                else { ForegroundColor = ConsoleColor.Gray; }
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

            if (grid[tile.Y, tile.X] == "-" || grid[tile.Y, tile.X] == "|")
            {
                return 0;
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

            if (grid[y, x] == SymbolsConfig.VerticalDoorVisual.ToString() || grid[y, x] == SymbolsConfig.HorizontalDoorVisual.ToString())
            {
                Vector2 tile = new Vector2(x, y);
                return !unlockables[tile].IsLocked();
            }

            return grid[y, x] == SymbolsConfig.Empty.ToString() ||
                   grid[y, x] == SymbolsConfig.Light1.ToString() ||
                   grid[y, x] == SymbolsConfig.Light2.ToString() ||
                   grid[y, x] == SymbolsConfig.Light3.ToString() ||
                   grid[y, x] == SymbolsConfig.Entrance.ToString() ||
                   grid[y, x] == SymbolsConfig.Exit.ToString() ||
                   grid[y, x] == SymbolsConfig.Key.ToString() ||
                   grid[y, x] == SymbolsConfig.Treasure.ToString() ||
                   grid[y, x] == SymbolsConfig.LeverOff.ToString() ||
                   grid[y, x] == SymbolsConfig.LeverOn.ToString();
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

            return grid[y, x] == SymbolsConfig.Empty.ToString() ||
                   grid[y, x] == SymbolsConfig.Light1.ToString() ||
                   grid[y, x] == SymbolsConfig.Light2.ToString() ||
                   grid[y, x] == SymbolsConfig.Light3.ToString() ||
                   grid[y, x] == SymbolsConfig.Exit.ToString() ||
                   grid[y, x] == SymbolsConfig.Entrance.ToString() ||
                   grid[y, x] == SymbolsConfig.Key.ToString() ||
                   grid[y, x] == SymbolsConfig.Treasure.ToString() ||
                   grid[y, x] == SymbolsConfig.LeverOff.ToString() ||
                   grid[y, x] == SymbolsConfig.LeverOn.ToString()||
                   grid[y, x] == SymbolsConfig.Signpost.ToString()||
                   grid[y, x] == SymbolsConfig.ChestClosed.ToString()||
                   grid[y, x] == SymbolsConfig.ChestOpened.ToString();
        }

        /// <summary>
        /// Gets which symbol is present in a given location
        /// </summary>
        /// <param name="x">The X coordinate of the position to check</param>
        /// <param name="y">The Y coordinate of the position to check</param>
        /// <returns>Returns the symbol found at these coordinates on the grid</returns>
        public string GetElementAt(int x, int y)
        {
            int _x = x - xOffset;
            int _y = y - yOffset;

            if (_x < 0 || _y < 0 || _x >= columns || _y >= rows) { return null; }

            return grid[_y, _x];
        }

        public string GetElementAt(int x, int y, bool withOffset = true)
        {
            if (withOffset)
            {
                y -= yOffset;

                x -= xOffset;
            }

            if (x < 0 || y < 0 || x >= columns || y >= rows) { return null; }

            return grid[y, x];
        }

        public bool InteractWithElementAt(int x, int y, Game game)
        {
            string element = GetElementAt (x, y);

            if (element == null) { return false; }

            if (element == SymbolsConfig.Empty.ToString()) { return false; }
            if (element == "═" || element == "╔" || element == "╗" || element == "║" || element == "╚" || element == "╝" ||
                element == "╠" || element == "╣" || element == "╩" || element == "╦" || element == "╬" || 
                element == SymbolsConfig.Gate.ToString())
            {
                return false;
            }

            if (element == SymbolsConfig.Treasure.ToString()) 
            {
                game.TunePlayer.PlaySFX(1000, 100);
                game.PlayerCharacter.ChangeLoot(100);
                ChangeElementAt(x, y, SymbolsConfig.Empty.ToString());
                return true;
            }
            if (element == SymbolsConfig.Key.ToString())
            {
                game.TunePlayer.PlaySFX(800, 100);
                CollectKeyPiece(x, y, game);
                return true;
            }
            if (element == SymbolsConfig.LeverOff.ToString() || element == SymbolsConfig.LeverOn.ToString())
            {
                game.TunePlayer.PlaySFX(100, 100);
                ToggleLever(x, y);
                game.PlayerCharacter.Draw();
                return true;
            }
            if (element == SymbolsConfig.Signpost.ToString())
            {
                ReadMessage(x, y, game);
                return true;
            }
            if (element == SymbolsConfig.ChestClosed.ToString() || element == SymbolsConfig.ChestOpened.ToString())
            {
                ControlsManager.ResetControlState(game);
                Lockpick(x, y, game);
                return true;
            }
            if (element == SymbolsConfig.HorizontalDoorVisual.ToString() || element == SymbolsConfig.VerticalDoorVisual.ToString())
            {
                ControlsManager.ResetControlState(game);
                Lockpick(x, y, game);
                return true;
            }

            return false;
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
            ResetUnlockables();
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
        public void ToggleLever(int x, int y, bool withOffset = true)
        {
            stopwatch.Stop();

            Vector2 leverCoord = new Vector2(x, y);

            if (leversDictionary.ContainsKey(leverCoord))
            {
                Lever lever = leversDictionary[leverCoord];
                int xOffsetLocal = 0;
                int yOffsetLocal = 0;
                if (withOffset)
                {
                    xOffsetLocal = xOffset;
                    yOffsetLocal = yOffset;
                }
                lever.Toggle(this, xOffsetLocal, yOffsetLocal);
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
        public void CollectKeyPiece(int x, int y, Game game)
        {
            IsLocked = levelLock.CollectKeyPiece(game, x, y);

            if (!IsLocked)
            {
                DrawTile(exit.X + xOffset, exit.Y + yOffset, SymbolsConfig.Exit.ToString());
            }
        }

        /// <summary>
        /// Returns a string that shows how many keys have been collected over the total known so far
        /// </summary>
        public string GetKeyPiecesProgress()
        {
            return $"{levelLock.TotalCollectedKeys} / {levelLock.TotalKnownKeys}";
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
        /// Alerts guards of comething out of their line of sight
        /// </summary>
        /// <param name="targetPosition">The position the guards will investigate</param>
        public void AlertGuards(Vector2 targetPosition, int range = 0)
        {
            if (levelGuards.Length > 0)
            {
                foreach (Guard guard in levelGuards)
                {
                    guard.AlertGuard(targetPosition, range);
                }
            }
        }

        /// <summary>
        /// Draws all guards
        /// </summary>
        public void DrawGuards(Game game)
        {
            if (levelGuards.Length > 0)
            {
                foreach (Guard guard in levelGuards)
                {
                    guard.Draw(game);
                }
            }
        }

        private void ReadMessage(int x, int y, Game game)
        {
            Vector2 messageCoords = new Vector2(x, y);
            game.MyStopwatch.Stop();
            game.UserInterface.DisplayTextFullScreen(messagesDictionary[messageCoords]);
            ControlsManager.ResetControlState(game);
            game.HasDrawnBackground = false;
            game.MyStopwatch.Start();
        }

        private void Lockpick(int x, int y, Game game)
        {
            x -= xOffset;
            y -= yOffset;

            Vector2 tile = new Vector2(x, y);

            if (!unlockables.ContainsKey(tile))
            {
                throw new Exception($"Error! failed to find unlockable in disctionary at {x}, {y}");
            }

            unlockables[tile].Unlock(game);
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
                if (GetElementAt(tile.X, tile.Y, false) != SymbolsConfig.Empty.ToString())
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

                char symbol = SymbolsConfig.Empty;

                int lightValue = GetLightLevelInItle(tile, false);

                ForegroundColor = ConsoleColor.DarkBlue;
                switch (lightValue)
                {
                    case 0:
                        break;
                    case 1:
                        symbol = SymbolsConfig.Light1;
                        break;
                    case 2:
                        symbol = SymbolsConfig.Light2;
                        break;
                    case 3:
                        symbol = SymbolsConfig.Light3;
                        break;
                }

                Write(symbol);
                ResetColor();
            }
        }

        private void ResetTreasures()
        {
            foreach (Vector2 treasure in treasures)
            {
                ChangeElementAt(treasure.X, treasure.Y, SymbolsConfig.Treasure.ToString(), false, false);
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

        private void ResetUnlockables()
        {
            foreach (KeyValuePair<Vector2, Unlockable> unlockable in unlockables)
            {
                unlockable.Value.Reset();
            }
        }
    }
}
