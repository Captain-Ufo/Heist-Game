////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

using System;
using System.Collections.Generic;

namespace HeistGame
{
    /// <summary>
    /// Contains the public static method that parses the level from the text file provided
    /// </summary>
    class LevelParser
    {
        /// <summary>
        /// Reads a text file and interprets all the informations required to create a level. It's a static method so there's no need to instantiate
        /// a LevelParser to use it.
        /// </summary>
        /// <param name="mission">The config file for the mission</param>
        /// <param name="difficulty">The difficulty level of the run</param>
        /// <returns></returns>
        public static LevelInfo ParseConfigToLevelInfo(MissionConfig mission, Difficulty difficulty)
        {
            //first, creating a whole bunch of variables that will hold the informations for the creation of the level
            string firstLine = mission.LevelMap[0];

            int rows = mission.LevelMap.Length;
            int columns = firstLine.Length;

            char[,] grid = new char[rows, columns];

            int playerStartX = -1;
            int playerStartY = -1;
            int totalGold = 0;

            Vector2 exit = new Vector2(-1, -1);

            List<Vector2> treasures = new List<Vector2>();

            HashSet<Vector2> floorTiles = new HashSet<Vector2>();
            List<Light> strongLights = new List<Light>();
            List<Light> weakLights = new List<Light>();

            LevelLock levLock = new LevelLock();
            levLock.AddMessages(mission.ObjectivesMessages);

            Dictionary<Vector2, Lever> leversDictionary = new Dictionary<Vector2, Lever>();

            Lever leverA = new Lever();
            Lever leverE = new Lever();
            Lever leverI = new Lever();
            Lever leverO = new Lever();
            Lever leverU = new Lever();
            Lever leverY = new Lever();

            Dictionary<Vector2, string[]> messagesDictionary = new Dictionary<Vector2, string[]>();

            Dictionary<Vector2, Unlockable> unlockablesDictionary = new Dictionary<Vector2, Unlockable>();

            //these LUT dictionaries serve the sole purpose of making the massive switch block that parses the level
            //more succint and readable, since this method is already pretty massive
            Dictionary<char, Lever> leversLUT = new Dictionary<char, Lever>
            {
                ['A'] = leverA,
                ['E'] = leverE,
                ['I'] = leverI,
                ['O'] = leverO,
                ['U'] = leverU,
                ['Y'] = leverY
            };

            List<Vector2> leverAGates = new List<Vector2>();
            List<Vector2> leverEGates = new List<Vector2>();
            List<Vector2> leverIGates = new List<Vector2>();
            List<Vector2> leverOGates = new List<Vector2>();
            List<Vector2> leverUGates = new List<Vector2>();
            List<Vector2> leverYGates = new List<Vector2>();

            Dictionary<char, List<Vector2>> leverGatesLUT = new Dictionary<char, List<Vector2>>
            {
                ['a'] = leverAGates,
                ['à'] = leverAGates,
                ['e'] = leverEGates,
                ['è'] = leverEGates,
                ['i'] = leverIGates,
                ['ì'] = leverIGates,
                ['o'] = leverOGates,
                ['ò'] = leverOGates,
                ['u'] = leverUGates,
                ['ù'] = leverUGates,
                ['y'] = leverYGates,
                ['ÿ'] = leverYGates
            };

            List<Guard> levelGuards = new List<Guard>();

            Guard guard1 = new Guard();
            Guard guard2 = new Guard();
            Guard guard3 = new Guard();
            Guard guard4 = new Guard();
            Guard guard5 = new Guard();
            Guard guard6 = new Guard();
            Guard guard7 = new Guard();
            Guard guard8 = new Guard();
            Guard guard9 = new Guard();
            Guard guard10 = new Guard();
            Guard guard11 = new Guard();
            Guard guard12 = new Guard();
            Guard guard13 = new Guard();
            Guard guard14 = new Guard();
            Guard guard15 = new Guard();
            Guard guard16 = new Guard();
            Guard guard17 = new Guard();
            Guard guard18 = new Guard();
            Guard guard19 = new Guard();
            Guard guard20 = new Guard();

            Dictionary<char, Guard> guardsLUT = new Dictionary<char, Guard>
            {
                ['B'] = guard1,
                ['C'] = guard2,
                ['D'] = guard3,
                ['F'] = guard4,
                ['G'] = guard5,
                ['H'] = guard6,
                ['J'] = guard7,
                ['K'] = guard8,
                ['L'] = guard9,
                ['M'] = guard10,
                ['N'] = guard11,
                ['P'] = guard12,
                ['Q'] = guard13,
                ['R'] = guard14,
                ['S'] = guard15,
                ['T'] = guard16,
                ['V'] = guard17,
                ['W'] = guard18,
                ['X'] = guard19,
                ['Z'] = guard20,
            };

            List<Vector2> guard1Patrol = new List<Vector2>();
            List<Vector2> guard2Patrol = new List<Vector2>();
            List<Vector2> guard3Patrol = new List<Vector2>();
            List<Vector2> guard4Patrol = new List<Vector2>();
            List<Vector2> guard5Patrol = new List<Vector2>();
            List<Vector2> guard6Patrol = new List<Vector2>();
            List<Vector2> guard7Patrol = new List<Vector2>();
            List<Vector2> guard8Patrol = new List<Vector2>();
            List<Vector2> guard9Patrol = new List<Vector2>();
            List<Vector2> guard10Patrol = new List<Vector2>();
            List<Vector2> guard11Patrol = new List<Vector2>();
            List<Vector2> guard12Patrol = new List<Vector2>();
            List<Vector2> guard13Patrol = new List<Vector2>();
            List<Vector2> guard14Patrol = new List<Vector2>();
            List<Vector2> guard15Patrol = new List<Vector2>();
            List<Vector2> guard16Patrol = new List<Vector2>();
            List<Vector2> guard17Patrol = new List<Vector2>();
            List<Vector2> guard18Patrol = new List<Vector2>();
            List<Vector2> guard19Patrol = new List<Vector2>();
            List<Vector2> guard20Patrol = new List<Vector2>();

            Dictionary<char, List<Vector2>> guardPatrolsLUT = new Dictionary<char, List<Vector2>>
            {
                ['b'] = guard1Patrol,
                ['c'] = guard2Patrol,
                ['d'] = guard3Patrol,
                ['f'] = guard4Patrol,
                ['g'] = guard5Patrol,
                ['h'] = guard6Patrol,
                ['j'] = guard7Patrol,
                ['k'] = guard8Patrol,
                ['l'] = guard9Patrol,
                ['m'] = guard10Patrol,
                ['n'] = guard11Patrol,
                ['p'] = guard12Patrol,
                ['q'] = guard13Patrol,
                ['r'] = guard14Patrol,
                ['s'] = guard15Patrol,
                ['t'] = guard16Patrol,
                ['v'] = guard17Patrol,
                ['w'] = guard18Patrol,
                ['x'] = guard19Patrol,
                ['z'] = guard20Patrol
            };

            Dictionary<char, int> doorsLocksLUT = new Dictionary<char, int>
            {
                [SymbolsConfig.HorizontalDoorOpen] = 0,
                [SymbolsConfig.VerticalDoorOpen] = 0,
                [SymbolsConfig.HorizontalDoorLock1] = 1,
                [SymbolsConfig.VerticalDoorLock1] = 1,
                [SymbolsConfig.HorizontalDoorLock2] = 2,
                [SymbolsConfig.VerticalDoorLock2] = 2,
                [SymbolsConfig.HorizontalDoorLock3] = 3,
                [SymbolsConfig.VerticalDoorLock3] = 3,
            };

            Dictionary<char, char> doorsVisualsLUT = new Dictionary<char, char>
            {
                [SymbolsConfig.HorizontalDoorOpen] = SymbolsConfig.HorizontalDoorVisual,
                [SymbolsConfig.HorizontalDoorLock1] = SymbolsConfig.HorizontalDoorVisual,
                [SymbolsConfig.HorizontalDoorLock2] = SymbolsConfig.HorizontalDoorVisual,
                [SymbolsConfig.HorizontalDoorLock3] = SymbolsConfig.HorizontalDoorVisual,
                [SymbolsConfig.VerticalDoorOpen] = SymbolsConfig.VerticalDoorVisual,
                [SymbolsConfig.VerticalDoorLock1] = SymbolsConfig.VerticalDoorVisual,
                [SymbolsConfig.VerticalDoorLock2] = SymbolsConfig.VerticalDoorVisual,
                [SymbolsConfig.VerticalDoorLock3] = SymbolsConfig.VerticalDoorVisual,
            };

            Dictionary<Vector2, IMap> mapLUT = new Dictionary<Vector2, IMap>();

            List<Vector2> wallTiles = new List<Vector2>();

            //Looping through every single character in the grid to find special characters for special gameplay elements 
            //(keys, treasures, levers, guards), and in the end create a bidimensional string array that will be the grid
            //used by the game to display the level.
            //When the switch catches a special characters, it replaces it in the grid with the appropriate representation
            //and assigns its coordinates to the correct variable

            for (int y = 0; y < rows; y++)
            {
                string line = mission.LevelMap[y];
                for (int x = 0; x < columns; x++)
                {
                    char currentChar = line[x];

                    Vector2 coordinates = new Vector2(x, y);

                    switch (currentChar)
                    {
                        //lights and floors (for lighting purposes)
                        case SymbolsConfig.Empty:
                        case SymbolsConfig.Entrance:
                            floorTiles.Add(coordinates);
                            break;
                        case SymbolsConfig.StrongLight:
                            floorTiles.Add(coordinates);
                            strongLights.Add(new Light(x, y, 6));
                            currentChar = SymbolsConfig.Empty;
                            break;
                        case SymbolsConfig.WeakLight:
                            floorTiles.Add(new Vector2(x, y));
                            weakLights.Add(new Light(x, y, 4));
                            currentChar = SymbolsConfig.Empty;
                            break;
                        case SymbolsConfig.TransparentWallHorizontal:
                        case SymbolsConfig.TransparentWallVertical:
                            wallTiles.Add(coordinates);
                            floorTiles.Add(coordinates);
                            break;
                        // Walls
                        case '╔':
                        case '╗':
                        case '╝':
                        case '╚':
                        case '╠':
                        case '╣':
                        case '╦':
                        case '╩':
                        case '╬':
                        case '═':
                        case '║':
                        case '#':
                            wallTiles.Add(coordinates);
                            break;
                        //player spawn point
                        case SymbolsConfig.Spawn:
                            playerStartX = x;
                            playerStartY = y;
                            currentChar = SymbolsConfig.Empty;
                            floorTiles.Add(coordinates);
                            break;
                        //Exit
                        case SymbolsConfig.ExitConfigSymbol:
                            currentChar = SymbolsConfig.Exit;
                            exit = coordinates;
                            floorTiles.Add(coordinates);
                            break;
                        //keys
                        case SymbolsConfig.Key:
                            currentChar = SymbolsConfig.Key;
                            levLock.AddKey(x, y, 1);
                            floorTiles.Add(coordinates);
                            break;
                        case '§':
                            currentChar = SymbolsConfig.Empty;
                            levLock.AddKey(x, y, 2);
                            floorTiles.Add(coordinates);
                            break;
                        case 'ç':
                            currentChar = SymbolsConfig.Empty;
                            levLock.AddKey(x, y, 3);
                            floorTiles.Add(coordinates);
                            break;
                        case '&':
                            currentChar = SymbolsConfig.Empty;
                            levLock.AddKey(x, y, 4);
                            floorTiles.Add(coordinates);
                            break;
                        //treasures
                        case SymbolsConfig.Treasure:
                            totalGold += 100;
                            treasures.Add(coordinates);
                            floorTiles.Add(coordinates);
                            break;
                        case '£':
                            if (difficulty == Difficulty.VeryEasy || difficulty == Difficulty.Easy || difficulty == Difficulty.Normal || difficulty == Difficulty.Hard)
                            {
                                totalGold += 100;
                                treasures.Add(coordinates);
                                currentChar = SymbolsConfig.Treasure;
                            }
                            else
                            {
                                currentChar = SymbolsConfig.Empty;
                            }
                            floorTiles.Add(coordinates);
                            break;
                        case '€':
                            if (difficulty == Difficulty.VeryEasy || difficulty == Difficulty.Easy)
                            {
                                totalGold += 100;
                                treasures.Add(coordinates);
                                currentChar = SymbolsConfig.Treasure;
                            }
                            else
                            {
                                currentChar = SymbolsConfig.Empty;
                            }
                            floorTiles.Add(coordinates);
                            break;
                        //levers
                        case 'A':
                        case 'E':
                        case 'I':
                        case 'O':
                        case 'U':
                        case 'Y':
                            leversLUT[currentChar].SetLeverCoordinates(x, y);
                            leversDictionary[coordinates] = leversLUT[currentChar];
                            currentChar = SymbolsConfig.LeverOff;
                            floorTiles.Add(coordinates);
                            break;
                        //lever gates that are closed when the game begins
                        case 'a':
                        case 'e':
                        case 'i':
                        case 'o':
                        case 'u':
                        case 'y':
                            leverGatesLUT[currentChar].Add(coordinates);
                            currentChar = SymbolsConfig.Gate;
                            wallTiles.Add(coordinates);
                            floorTiles.Add(coordinates);
                            break;
                        //lever gates that are open when the game begins
                        case 'à':
                        case 'è':
                        case 'ì':
                        case 'ò':
                        case 'ù':
                        case 'ÿ':
                            leverGatesLUT[currentChar].Add(coordinates);
                            currentChar = SymbolsConfig.Empty;
                            floorTiles.Add(coordinates);
                            break;
                        //guards
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'F':
                        case 'G':
                        case 'H':
                        case 'J':
                        case 'K':
                        case 'L':
                        case 'M':
                        case 'N':
                        case 'P':
                        case 'Q':
                        case 'R':
                        case 'S':
                        case 'T':
                        case 'V':
                        case 'W':
                        case 'X':
                        case 'Z':
                            guardsLUT[currentChar].AssignOriginPoint(x, y);
                            levelGuards.Add(guardsLUT[currentChar]);
                            currentChar = SymbolsConfig.Empty;
                            floorTiles.Add(coordinates);
                            break;
                        //guards patrols
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'f':
                        case 'g':
                        case 'h':
                        case 'j':
                        case 'k':
                        case 'l':
                        case 'm':
                        case 'n':
                        case 'p':
                        case 'q':
                        case 'r':
                        case 's':
                        case 't':
                        case 'v':
                        case 'w':
                        case 'x':
                        case 'z':
                            guardPatrolsLUT[currentChar].Add(coordinates);
                            currentChar = SymbolsConfig.Empty;
                            floorTiles.Add(coordinates);
                            break;
                        //Messages/signs
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '₀':
                        case '₁':
                        case '₂':
                        case '₃':
                        case '₄':
                        case '₅':
                        case '₆':
                        case '₇':
                        case '₈':
                        case '₉':
                            int messageIndex = 0;
                            string[] messageText;
                            if (currentChar == '0' || currentChar == '1' || currentChar == '2' || currentChar == '3' ||
                                currentChar == '4' || currentChar == '5' || currentChar == '6' || currentChar == '7' ||
                                currentChar == '8' || currentChar == '9')
                            {
                                messageIndex = Int32.Parse(currentChar.ToString());
                            }
                            else if (currentChar == '₀')
                            {
                                messageIndex = 10;
                            }
                            else if (currentChar == '₁')
                            {
                                messageIndex = 11;
                            }
                            else if (currentChar == '₂')
                            {
                                messageIndex = 12;
                            }
                            else if (currentChar == '₃')
                            {
                                messageIndex = 13;
                            }
                            else if (currentChar == '₄')
                            {
                                messageIndex = 14;
                            }
                            else if (currentChar == '₅')
                            {
                                messageIndex = 15;
                            }
                            else if (currentChar == '₆')
                            {
                                messageIndex = 16;
                            }
                            else if (currentChar == '₇')
                            {
                                messageIndex = 17;
                            }
                            else if (currentChar == '₈')
                            {
                                messageIndex = 18;
                            }
                            else if (currentChar == '₉')
                            {
                                messageIndex = 19;
                            }
                            messageText = mission.Messages[messageIndex];
                            messagesDictionary.Add(coordinates, messageText);
                            currentChar = SymbolsConfig.Signpost;
                            floorTiles.Add(coordinates); // Necessary for lighmaps
                            break;
                        //Chests
                        case SymbolsConfig.ChestEmpty:
                            unlockablesDictionary.Add(coordinates, new Chest(2, 0, x, y));
                            currentChar = SymbolsConfig.ChestClosed;
                            floorTiles.Add(coordinates); // Necessary for lighmaps
                            break;
                        case SymbolsConfig.ChestWithTreasure:
                            unlockablesDictionary.Add(coordinates, new Chest(2, 200, x, y));
                            totalGold += 200;
                            currentChar = SymbolsConfig.ChestClosed;
                            floorTiles.Add(coordinates); // Necessary for lighmaps
                            break;
                        case SymbolsConfig.ChestWithRandomTresture:
                            Random rand = new Random();
                            int treasureValue = rand.Next(0, 201);
                            unlockablesDictionary.Add(coordinates, new Chest(2, treasureValue, x, y));
                            totalGold += treasureValue;
                            currentChar = SymbolsConfig.ChestClosed;
                            floorTiles.Add(coordinates); // Necessary for lighmaps
                            break;
                        //Doors
                        case SymbolsConfig.HorizontalDoorOpen:
                        case SymbolsConfig.HorizontalDoorLock1:
                        case SymbolsConfig.HorizontalDoorLock2:
                        case SymbolsConfig.HorizontalDoorLock3:
                        case SymbolsConfig.VerticalDoorOpen:
                        case SymbolsConfig.VerticalDoorLock1:
                        case SymbolsConfig.VerticalDoorLock2:
                        case SymbolsConfig.VerticalDoorLock3:
                            unlockablesDictionary.Add(coordinates, new Door(doorsLocksLUT[currentChar]));
                            wallTiles.Add(coordinates);
                            currentChar = doorsVisualsLUT[currentChar];
                            break;
                        //Maps
                        case SymbolsConfig.ExitMap:
                            mapLUT[coordinates] = new ExitMap();
                            currentChar = SymbolsConfig.Signpost;
                            floorTiles.Add(coordinates);
                            break;
                        case SymbolsConfig.WallsMap:
                            mapLUT[coordinates] = new WallsMap();
                            currentChar = SymbolsConfig.Signpost;
                            floorTiles.Add(coordinates);
                            break;
                        case SymbolsConfig.ObjectivesMap:
                            mapLUT[coordinates] = new ObjectvesMap();
                            currentChar = SymbolsConfig.Signpost;
                            floorTiles.Add(coordinates);
                            break;
                        case SymbolsConfig.FullMap:
                            mapLUT[coordinates] = new FullMap();
                            currentChar = SymbolsConfig.Signpost;
                            floorTiles.Add(coordinates);
                            break;
                    }
                    grid[y, x] = currentChar;
                }
            }

            leverA.AssignGates(leverAGates.ToArray());
            leverE.AssignGates(leverEGates.ToArray());
            leverI.AssignGates(leverIGates.ToArray());
            leverO.AssignGates(leverOGates.ToArray());
            leverU.AssignGates(leverUGates.ToArray());
            leverY.AssignGates(leverYGates.ToArray());

            guard1.AssignPatrol(ArrangePatrolPoints(guard1, guard1Patrol).ToArray());
            guard2.AssignPatrol(ArrangePatrolPoints(guard2, guard2Patrol).ToArray());
            guard3.AssignPatrol(ArrangePatrolPoints(guard3, guard3Patrol).ToArray());
            guard4.AssignPatrol(ArrangePatrolPoints(guard4, guard4Patrol).ToArray());
            guard5.AssignPatrol(ArrangePatrolPoints(guard5, guard5Patrol).ToArray());
            guard6.AssignPatrol(ArrangePatrolPoints(guard6, guard6Patrol).ToArray());
            guard7.AssignPatrol(ArrangePatrolPoints(guard7, guard7Patrol).ToArray());
            guard8.AssignPatrol(ArrangePatrolPoints(guard8, guard8Patrol).ToArray());
            guard9.AssignPatrol(ArrangePatrolPoints(guard9, guard9Patrol).ToArray());
            guard10.AssignPatrol(ArrangePatrolPoints(guard10, guard10Patrol).ToArray());
            guard11.AssignPatrol(ArrangePatrolPoints(guard11, guard11Patrol).ToArray());
            guard12.AssignPatrol(ArrangePatrolPoints(guard12, guard12Patrol).ToArray());
            guard13.AssignPatrol(ArrangePatrolPoints(guard13, guard13Patrol).ToArray());
            guard14.AssignPatrol(ArrangePatrolPoints(guard14, guard14Patrol).ToArray());
            guard15.AssignPatrol(ArrangePatrolPoints(guard15, guard15Patrol).ToArray());
            guard16.AssignPatrol(ArrangePatrolPoints(guard16, guard16Patrol).ToArray());
            guard17.AssignPatrol(ArrangePatrolPoints(guard17, guard17Patrol).ToArray());
            guard18.AssignPatrol(ArrangePatrolPoints(guard18, guard18Patrol).ToArray());
            guard19.AssignPatrol(ArrangePatrolPoints(guard19, guard19Patrol).ToArray());
            guard20.AssignPatrol(ArrangePatrolPoints(guard20, guard20Patrol).ToArray());

            //Create the LevelInfo with all the parameters collected by parsing the map
            LevelInfo levelInfo = new LevelInfo(grid, playerStartX, playerStartY, totalGold, exit, treasures.ToArray(), floorTiles, strongLights.ToArray(),
                                                weakLights.ToArray(), levLock, leversDictionary, levelGuards.ToArray(), messagesDictionary, unlockablesDictionary,
                                                mapLUT, wallTiles.ToArray());

            return levelInfo;
        }

        /// <summary>
        /// This rearranges the patrol points provided so that they are in the correct sequence from the closest to the farthest
        /// </summary>
        /// <param name="guard">The guard that follows the patrol</param>
        /// <param name="guardPatrol">The list of patrol points</param>
        /// <returns></returns>
        public static List<Vector2> ArrangePatrolPoints(Guard guard, List<Vector2> guardPatrol)
        {
            //I'm pretty sure the rearrange happens in the least efficient way possible (but this is what I could come up with)
            //by finding the closest one in an orthogonal direction compared to a starting point (which at the beginning is the starting
            //position of the guard  - not included in the patrol points, so if the designer wants, the guard can start in a different position
            //than the path they will follow for the rest of the level, as long as it's orthogonal to at least one of the patrol points) and in
            //each iteration it becomes the previously found patrol point.
            //The loop continues until the newly created list includes as many entries as the original one.
            //Considering that this is done only at the beginning, as the game loads the levels (and that it doesn't seem to slow down loading in any
            //significant way), I reckon it's an acceptable solution.

            Vector2 startPoint = new Vector2(guard.X, guard.Y);

            List<Vector2> arrangedPatrolPoints = new List<Vector2>();

            while (arrangedPatrolPoints.Count < guardPatrol.Count)
            {
                int currentMinDistance = 1000; //Arbitrary huge number
                int closestPatrolX = 0;
                int closestPatrolY = 0;

                for (int i = 0; i < guardPatrol.Count; i++)
                {
                    bool alreadyAdded = false;

                    foreach (Vector2 c in arrangedPatrolPoints)
                    {
                        if (c.X == guardPatrol[i].X && c.Y == guardPatrol[i].Y)
                        {
                            alreadyAdded = true;
                        }
                    }

                    if (alreadyAdded) { continue; }

                    int xDifference = Math.Abs(guardPatrol[i].X - startPoint.X);
                    int yDifference = Math.Abs(guardPatrol[i].Y - startPoint.Y);

                    if (xDifference > 0 && yDifference > 0) { continue; }

                    if (xDifference == 0 && yDifference == 0) { continue; }

                    if (xDifference == 0)
                    {
                        if (yDifference <= currentMinDistance)
                        {
                            currentMinDistance = yDifference;

                            closestPatrolX = guardPatrol[i].X;
                            closestPatrolY = guardPatrol[i].Y;
                        }
                    }

                    if (yDifference == 0)
                    {
                        if (xDifference <= currentMinDistance)
                        {
                            currentMinDistance = xDifference;

                            closestPatrolX = guardPatrol[i].X;
                            closestPatrolY = guardPatrol[i].Y;
                        }
                    }
                }

                Vector2 closestPatrolPoint = new Vector2(closestPatrolX, closestPatrolY);
                arrangedPatrolPoints.Add(closestPatrolPoint);
                startPoint = closestPatrolPoint;
            }

            return arrangedPatrolPoints;
        }
    }

    /// <summary>
    /// Just a helper class that holds all the information required to create a level
    /// </summary>
    class LevelInfo
    {
        public char[,] Grid { get; }
        public LevelLock LevLock { get; }
        public int PlayerStartX { get; }
        public int PlayerStartY { get; }
        public int TotalGold { get; }
        public Vector2 Exit { get; }
        public Vector2[] Treasures { get; }
        public HashSet<Vector2> FloorTiles { get; }
        public Light[] StrongLights { get; }
        public Light[] WeakLights { get; }
        public Dictionary<Vector2, Lever> LeversDictionary { get; }
        public Guard[] Guards { get; }
        public Dictionary<Vector2, string[]> MessagesDictionary { get; }
        public Dictionary<Vector2, Unlockable> UnlockablesDictionary { get; }
        public Dictionary<Vector2, IMap> MapsDictionary { get; }
        public Vector2[] Walls { get; }

        public LevelInfo(char[,] grid, int playerStartX, int playerStartY, int totalGold, Vector2 exit, Vector2[] treasures, HashSet<Vector2> floorTiles, 
                         Light[] strongLights, Light[] weakLights, LevelLock levelLock, Dictionary<Vector2, Lever> leversDictionary, Guard[] guards,
                         Dictionary<Vector2, string[]> messagesDictionary, Dictionary<Vector2, Unlockable> unlockablesDictionary,
                         Dictionary<Vector2, IMap> mapsDictionary, Vector2[] walls)
        {
            Grid = grid;
            LevLock = levelLock;
            PlayerStartX = playerStartX;
            PlayerStartY = playerStartY;
            TotalGold = totalGold;
            Exit = exit;
            Treasures = treasures;
            FloorTiles = floorTiles;
            StrongLights = strongLights;
            WeakLights = weakLights;
            LeversDictionary = leversDictionary;
            Guards = guards;
            MessagesDictionary = messagesDictionary;
            UnlockablesDictionary = unlockablesDictionary;
            MapsDictionary = mapsDictionary;
            Walls = walls;
        }
    }
}
