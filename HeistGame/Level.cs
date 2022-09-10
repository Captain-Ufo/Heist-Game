////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

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
        private readonly int rows;
        private readonly int columns;
        private readonly Vector2 exit;
        private readonly LevelLock levelLock;
        private readonly Dictionary<Vector2, Lever> leversDictionary;
        private readonly Vector2[] treasures;
        private readonly Stopwatch stopwatch;
        private readonly Dictionary<Vector2, string[]> messagesDictionary;
        private readonly Dictionary<Vector2, Unlockable> unlockables;
        private readonly Game game;

        private Vector2[] wallTiles;

        public Guard[] LevelGuards { get; private set; }

        public char[,] Grid { get; private set; }

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
        /// The set of all tiles seen by the player since the beginning of the level (not necessarily those they see now).
        /// The tile symbol is keyed with the tile's coordinates as Vector2
        /// </summary>
        public Dictionary<Vector2, char> ExploredMap { get; private set; }
        /// <summary>
        /// The set of all map tiles that have been seen by the player at any point while moving through the level.
        /// </summary>
        public HashSet<Vector2> ExploredMapSet { get; private set; }

        /// <summary>
        /// The set of all map tiles currently visible to the player
        /// </summary>
        public HashSet<Vector2> VisibleMap { get; private set; }
        /// <summary>
        /// The set of all map tiles the player can hear movement from
        /// </summary>
        public HashSet<Vector2> PlayerHearingArea { get; private set; }
        /// <summary>
        /// The set of all guards currently in the player's hearing range, keyed with their current tile in this frame
        /// </summary>
        public Dictionary<Vector2, Guard> VisibleGuards { get; private set; }
        /// <summary>
        /// The set of all maps on the level
        /// </summary>
        public Dictionary<Vector2, IMap> Maps { get; private set; }
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
        public Level(string name, char[,] grid, int startX, int startY, HashSet<Vector2> floorTiles, LightMap lightmap, LevelLock levelLock, Vector2 exit,
                     Vector2[] treasures, Dictionary<Vector2, Lever> levers, Guard[] guards, Dictionary<Vector2, string[]> messages, Dictionary<Vector2, 
                     Unlockable> unlockables, Dictionary<Vector2, IMap> maps, Vector2[] walls, string[] briefing, string[] outro, Game game)
        {
            VisibleMap = new HashSet<Vector2>();
            ExploredMap = new Dictionary<Vector2, char>();
            ExploredMapSet = new HashSet<Vector2>();
            PlayerHearingArea = new HashSet<Vector2>();
            VisibleGuards = new Dictionary<Vector2, Guard>();
            
            Name = name;
            Briefing = briefing;
            Outro = outro;

            Grid = grid;

            rows = Grid.GetLength(0);
            columns = Grid.GetLength(1);

            this.game = game;
            this.stopwatch = game.MyStopwatch;

            this.treasures = treasures;

            this.exit = exit;

            leversDictionary = new Dictionary<Vector2, Lever>();

            foreach (KeyValuePair<Vector2, Lever> leverInfo in levers)
            {
                Vector2 LeverCoordinates = new Vector2(leverInfo.Key.X, leverInfo.Key.Y);
                Lever lever = leverInfo.Value;

                leversDictionary[LeverCoordinates] = lever;
            }

            messagesDictionary = new Dictionary<Vector2, string[]>();

            foreach (KeyValuePair<Vector2, string[]> messageInfo in messages)
            {
                Vector2 SignpostCoordinates = new Vector2(messageInfo.Key.X , messageInfo.Key.Y);

                messagesDictionary[SignpostCoordinates] = messageInfo.Value;
            }

            LevelGuards = guards;

            this.unlockables = unlockables;

            this.levelLock = levelLock;

            IsLocked = levelLock.IsLocked();

            PlayerStartX = startX;
            PlayerStartY = startY;

            FloorTiles = floorTiles;
            Lights = lightmap;
            Maps = maps;
            wallTiles = walls;

            Lights.CalculateLightMap(this);
        }

        public void UpdatePlayerHearingArea(Vector2 tile)
        {
            PlayerHearingArea.Add(tile);
            if (!PlayerHearingArea.Contains(tile))
            {
                PlayerHearingArea.Add(tile);
            }
        }

        public void UpdateVisibleMap(Vector2 tile)
        {
            if (!VisibleMap.Contains(tile))
            {
                VisibleMap.Add(tile);
            }

            if (!ExploredMap.ContainsKey(tile))
            {
                ExploredMap.Add(tile, Grid[tile.Y, tile.X]);
            }
            else
            {
                ExploredMap[tile] = Grid[tile.Y, tile.X];
            }
        }

        public void ClearPlayerPercetionMaps()
        {
            PlayerHearingArea.Clear();

            if (VisibleMap.Count > 0)
            {
                VisibleMap = new HashSet<Vector2>();
            }
        }

        /// <summary>
        /// Returns the light level at the tile's coordinates
        /// </summary>
        /// <param name="tile">The coordinates of the tile to be checked, as a Vector2</param>
        /// <param name="withOffset">Set to true if the coordinates provided are with the centering offsets</param>
        /// <returns></returns>
        public int GetLightLevelInItile(Vector2 tile)
        {
            if (Grid[tile.Y, tile.X] == '-' || Grid[tile.Y, tile.X] == '|')
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
        public bool IsTileWalkable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= columns || y >= rows)
            {
                return false;
            }

            if (Grid[y, x] == SymbolsConfig.VerticalDoorVisual || Grid[y, x] == SymbolsConfig.HorizontalDoorVisual)
            {
                Vector2 tile = new Vector2(x, y);
                return !unlockables[tile].IsLocked();
            }

            return Grid[y, x] == SymbolsConfig.Empty ||
                   Grid[y, x] == SymbolsConfig.Light1 ||
                   Grid[y, x] == SymbolsConfig.Light2 ||
                   Grid[y, x] == SymbolsConfig.Light3 ||
                   Grid[y, x] == SymbolsConfig.Entrance ||
                   Grid[y, x] == SymbolsConfig.Exit ||
                   Grid[y, x] == SymbolsConfig.Key ||
                   Grid[y, x] == SymbolsConfig.Treasure ||
                   Grid[y, x] == SymbolsConfig.LeverOff ||
                   Grid[y, x] == SymbolsConfig.LeverOn;
        }

        public bool CanPlayerHearTile (Vector2 tile)
        {
            return PlayerHearingArea.Contains(tile);
        }

        public bool CanPlayerSeeTile(Vector2 tile)
        {
            return VisibleMap.Contains(tile);
        }

        public bool HasPlayerExploredTile(Vector2 tile)
        {
            return ExploredMap.ContainsKey(tile);
        }

        public bool IsTileInsideBounds(Vector2 tile)
        {
            if (tile.X < 0 || tile.Y < 0 || tile.X >= columns || tile.Y >= rows)
            {
                return false;
            }

            return true;
        }

        public bool IsTileInsideBounds(int x, int y)
        {
            if (x < 0 || y < 0 || x >= columns || y >= rows)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the given position in the grid contains a symbols that allows sight and light to pass through.
        /// </summary>
        /// <param name="x">The X coordinate of the position to check</param>
        /// <param name="y">The Y coordinate of the position to check</param>
        /// <returns></returns>
        public bool IsTileTransparent(int x, int y)
        {
            if (x < 0 || y < 0 || x >= columns || y >= rows)
            {
                return false;
            }

            return Grid[y, x] == SymbolsConfig.Empty ||
                   Grid[y, x] == SymbolsConfig.Light1 ||
                   Grid[y, x] == SymbolsConfig.Light2 ||
                   Grid[y, x] == SymbolsConfig.Light3 ||
                   Grid[y, x] == SymbolsConfig.Exit ||
                   Grid[y, x] == SymbolsConfig.Entrance ||
                   Grid[y, x] == SymbolsConfig.Key ||
                   Grid[y, x] == SymbolsConfig.Treasure ||
                   Grid[y, x] == SymbolsConfig.LeverOff ||
                   Grid[y, x] == SymbolsConfig.LeverOn||
                   Grid[y, x] == SymbolsConfig.Signpost||
                   Grid[y, x] == SymbolsConfig.ChestClosed||
                   Grid[y, x] == SymbolsConfig.ChestOpened||
                   Grid[y, x] == SymbolsConfig.TransparentWallHorizontal||
                   Grid[y, x] == SymbolsConfig.TransparentWallVertical;
        }

        /// <summary>
        /// Gets which symbol is present in a given location
        /// </summary>
        /// <param name="x">The X coordinate of the position to check</param>
        /// <param name="y">The Y coordinate of the position to check</param>
        /// <returns>Returns the symbol found at these coordinates on the grid</returns>
        public char GetElementAt(int x, int y)
        {
            return Grid[y, x];
        }

        public bool InteractWithElementAt(int x, int y, Game game)
        {
            if (!IsTileInsideBounds(x, y)) { return false; }

            char element = GetElementAt(x, y);

            switch (element)
            {
                case SymbolsConfig.Empty:
                case '═':
                case '╔':
                case '╗':
                case '║':
                case '╚':
                case '╝':
                case '╠':
                case '╣':
                case '╩':
                case '╦':
                case '╬':
                case SymbolsConfig.Gate:
                    return false;

                case SymbolsConfig.Treasure:
                    game.TunePlayer.PlaySFX(1000, 100);
                    game.PlayerCharacter.ChangeLoot(100);
                    ChangeElementAt(x, y, SymbolsConfig.Empty);
                    return true;

                case SymbolsConfig.Key:
                    game.TunePlayer.PlaySFX(800, 100);
                    CollectKeyPiece(x, y, game);
                    return true;

                case SymbolsConfig.LeverOff:
                case SymbolsConfig.LeverOn:
                    game.TunePlayer.PlaySFX(100, 100);
                    ToggleLever(x, y);
                    return true;

                case SymbolsConfig.Signpost:
                    Vector2 tile = new Vector2(x, y);
                    if (Maps.ContainsKey(tile))
                    {
                        Maps[tile].RevealMap(this);
                    }
                    else 
                    {
                        ReadMessage(x, y, game); 
                    }
                    return true;

                case SymbolsConfig.ChestClosed:
                case SymbolsConfig.ChestOpened:
                    ControlsManager.ResetControlState(game);
                    Lockpick(x, y, game);
                    return true;

                case SymbolsConfig.HorizontalDoorVisual:
                case SymbolsConfig.VerticalDoorVisual:
                    ControlsManager.ResetControlState(game);
                    Lockpick(x, y, game);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Replaces a symbol in a given location
        /// </summary>
        /// <param name="x">The X coordinate of the symbol to replace</param>
        /// <param name="y">The X coordinate of the symbol to replace</param>
        /// <param name="newElement">The new symbol</param>
        public void ChangeElementAt(int x, int y, char newElement)
        {
            Grid[y, x] = newElement;

            Vector2 tile = new Vector2(x, y);
            if (VisibleMap.Contains(tile))
            {
                ExploredMap[tile] = newElement;
            }
        }

        public bool RevealExitTile()
        {
            if (ExploredMap.ContainsKey(exit))
            {
                ScreenDisplayer.DisplayMessageOnLable(
                new string[]
                {
                    "You can't get any other useful information."
                });

                return false;
            }

            ScreenDisplayer.DisplayMessageOnLable(
                new string[]
                {
                    "You find directions that give you an idea",
                    "about where to go to leave this place"
                });
            ExploredMap.Add(exit, SymbolsConfig.Exit);
            return true;
        }

        public bool RevealAllAWalls()
        {
            int revealedTiles = 0;
            foreach (Vector2 tile in wallTiles)
            {
                if (ExploredMap.ContainsKey(tile))
                {
                    continue;
                }

                ExploredMap.Add(tile, Grid[tile.Y, tile.X]);
                revealedTiles++;
            }

            if (revealedTiles > 0)
            {
                ScreenDisplayer.DisplayMessageOnLable(
                new string[]
                {
                    "You find details about the layout of the sorrounding area."
                });
                return true;
            }
            else
            {
                ScreenDisplayer.DisplayMessageOnLable(
                new string[]
                {
                    "You can't get any other useful information."
                });
                return false;
            }
        }

        public bool RevealObjectives()
        {
            List<Vector2> objectives = levelLock.GetTierKeys();
            int revealedTiles = 0;

            foreach (Vector2 tile in objectives)
            {
                if (ExploredMap.ContainsKey(tile))
                {
                    continue;
                }

                ExploredMap.Add(tile, Grid[tile.Y, tile.X]);
                revealedTiles++;
            }

            if (revealedTiles > 0)
            {
                ScreenDisplayer.DisplayMessageOnLable(
                new string[]
                {
                    "You find useful informations about your objectives."
                });
                return true;
            }
            else
            {
                ScreenDisplayer.DisplayMessageOnLable(
                new string[]
                {
                    "This document contains informations about your objectives,",
                    "but you can't find anything you don't already know."
                });
                return false;
            }
        }

        public void RevealAllMap()
        {
            int revealedElements = 0;

            if (RevealAllAWalls()) { revealedElements++; }
            if (RevealExitTile()) { revealedElements++; }
            if (RevealObjectives()) { revealedElements++; }

            if (revealedElements == 0)
            {
                ScreenDisplayer.DisplayMessageOnLable(
                    new string[]
                    {
                        "You can't get any other useful information."
                    });
            }
            else if (revealedElements == 3)
            {
                ScreenDisplayer.DisplayMessageOnLable(
                new string[]
                {
                    "You find detailed directions about this location,",
                    "covering every aspects of the place."
                });
            }
            else
            {
                ScreenDisplayer.DisplayMessageOnLable(
                new string[]
                {
                    "Even though you already knew some of this info,",
                    "you were able to get some useful elements out of this map."
                });
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
            VisibleMap.Clear();
            ExploredMap.Clear();
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

            if (currentTile.Y - 1 >= 0 && IsTileWalkable(currentTile.X, currentTile.Y - 1))
            {
                neighborsList.Add(CreateNewTile(currentTile.X, currentTile.Y - 1));
            }

            if (currentTile.Y + 1 < rows && IsTileWalkable(currentTile.X, currentTile.Y + 1))
            {
                neighborsList.Add(CreateNewTile(currentTile.X, currentTile.Y + 1));
            }

            if (currentTile.X - 1 >= 0 && IsTileWalkable(currentTile.X - 1, currentTile.Y))
            {
                neighborsList.Add(CreateNewTile(currentTile.X - 1, currentTile.Y));
            }

            if (currentTile.X + 1 < columns && IsTileWalkable(currentTile.X + 1, currentTile.Y))
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
                lever.Toggle(this, game);
            }

            Lights.CalculateLightMap(this);
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
            if (LevelGuards.Length > 0)
            {
                VisibleGuards.Clear();
                foreach (Guard guard in LevelGuards)
                {
                    guard.UpdateBehavior(this, game, deltaDimeMS);

                    Vector2 guardPosition = new Vector2(guard.X, guard.Y);
                    if (PlayerHearingArea.Contains(guardPosition))
                    {
                        VisibleGuards.Add(guardPosition, guard);
                    }
                }
            }
        }

        /// <summary>
        /// Alerts guards of comething out of their line of sight
        /// </summary>
        /// <param name="targetPosition">The position the guards will investigate</param>
        public void AlertGuards(Vector2 targetPosition, int range = 0)
        {
            if (LevelGuards.Length > 0)
            {
                foreach (Guard guard in LevelGuards)
                {
                    guard.AlertGuard(targetPosition, range);
                }
            }
        }

        private void ReadMessage(int x, int y, Game game)
        {
            Vector2 messageCoords = new Vector2(x, y);
            game.MyStopwatch.Stop();
            ScreenDisplayer.DisplayTextFullScreen(messagesDictionary[messageCoords]);
            ControlsManager.ResetControlState(game);
            game.HasDrawnBackground = false;
            game.MyStopwatch.Start();
        }

        private void Lockpick(int x, int y, Game game)
        {
            Vector2 tile = new Vector2(x, y);

            if (!unlockables.ContainsKey(tile))
            {
                throw new Exception($"Error! failed to find unlockable in disctionary at {x}, {y}");
            }

            unlockables[tile].Unlock(game);
        }

        private void ResetTreasures()
        {
            foreach (Vector2 treasure in treasures)
            {
                ChangeElementAt(treasure.X, treasure.Y, SymbolsConfig.Treasure);
            }
        }

        private void ResetLevers()
        {
            foreach (Lever lever in leversDictionary.Values)
            {
                if (lever.IsOn)
                {
                    lever.Toggle(this, game);
                }
            }
        }

        private void ResetKeys()
        {
            levelLock.ResetKeys(this);
        }

        private void ResetGuards()
        {
            foreach (Guard guard in LevelGuards)
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
