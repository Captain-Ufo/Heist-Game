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
        /// <param name="filePath">The path of the file to parse the level from</param>
        /// <param name="difficulty">The difficulty level of the run</param>
        /// <returns></returns>
        public static LevelInfo ParseFileToLevelInfo(string[] levelMap, Difficulty difficulty)
        {
            //first, creating a whole bunch of variables that will hold the informations for the creation of the level

            string firstLine = levelMap[0];

            int rows = levelMap.Length;
            int columns = firstLine.Length;

            string[,] grid = new string[rows, columns];

            int playerStartX = -1;
            int playerStartY = -1;
            int totalGold = 0;

            Vector2 exit = new Vector2(-1, -1);

            List<Vector2> treasures = new List<Vector2>();

            HashSet<Vector2> floorTiles = new HashSet<Vector2>();
            List<Light> strongLights = new List<Light>();
            List<Light> weakLights = new List<Light>();

            LevelLock levLock = new LevelLock();

            Dictionary<Vector2, Lever> leversDictionary = new Dictionary<Vector2, Lever>();

            Lever leverA = new Lever();
            Lever leverE = new Lever();
            Lever leverI = new Lever();
            Lever leverO = new Lever();
            Lever leverU = new Lever();
            Lever leverY = new Lever();

            //these LUT dictionaries serve the sole purpose of making the massive switch block that parses the level
            //more succint and readable
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

            Dictionary<char, Guard> guardsLUT = new Dictionary<char, Guard>
            {
                ['B'] = guard1,
                ['C'] = guard2,
                ['D'] = guard3,
                ['F'] = guard4,
                ['G'] = guard5,
                ['J'] = guard6,
                ['K'] = guard7,
                ['L'] = guard8,
                ['M'] = guard9,
                ['N'] = guard10,
                ['P'] = guard11,
                ['Q'] = guard12,
                ['R'] = guard13,
                ['S'] = guard14,
                ['T'] = guard15
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

            Dictionary<char, List<Vector2>> guardPatrolsLUT = new Dictionary<char, List<Vector2>>
            {
                ['b'] = guard1Patrol,
                ['c'] = guard2Patrol,
                ['d'] = guard3Patrol,
                ['f'] = guard4Patrol,
                ['g'] = guard5Patrol,
                ['j'] = guard6Patrol,
                ['k'] = guard7Patrol,
                ['l'] = guard8Patrol,
                ['m'] = guard9Patrol,
                ['n'] = guard10Patrol,
                ['p'] = guard11Patrol,
                ['q'] = guard12Patrol,
                ['r'] = guard13Patrol,
                ['s'] = guard14Patrol,
                ['t'] = guard15Patrol,
            };

            //Looping through every single character in the grid to find special characters for special gameplay elements 
            //(keys, treasures, levers, guards), and in the end create a bidimensional string array that will be the grid
            //used by the game to display the level.
            //When the switch catches a special characters, it replaces it in the grid with the appropriate representation

            for (int y = 0; y < rows; y++)
            {
                string line = levelMap[y];
                for (int x = 0; x < columns; x++)
                {
                    char currentChar = line[x];

                    Vector2 leverGate;
                    Vector2 patrolPoint;

                    switch (currentChar)
                    {
                        //lights and floors (for lighting purposes)
                        case SymbolsConfig.EmptySpace:
                        case SymbolsConfig.EntranceChar:
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        case SymbolsConfig.StrongLightChar:
                            floorTiles.Add(new Vector2(x, y));
                            strongLights.Add(new Light(x, y, 6));
                            currentChar = SymbolsConfig.EmptySpace;
                            break;
                        case SymbolsConfig.WeakLightChar:
                            floorTiles.Add(new Vector2(x, y));
                            weakLights.Add(new Light(x, y, 4));
                            currentChar = SymbolsConfig.EmptySpace;
                            break;
                        //player spawn point
                        case SymbolsConfig.SpawnChar:
                            playerStartX = x;
                            playerStartY = y;
                            currentChar = SymbolsConfig.EmptySpace;
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        //Exit
                        case SymbolsConfig.ExitChar:
                            exit = new Vector2(x, y);
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        //keys
                        case SymbolsConfig.KeyChar:
                        case '1':
                            currentChar = SymbolsConfig.KeyChar;
                            levLock.AddKey(x, y, 1);
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        case '2':
                            currentChar = SymbolsConfig.EmptySpace;
                            levLock.AddKey(x, y, 2);
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        case '3':
                            currentChar = SymbolsConfig.EmptySpace;
                            levLock.AddKey(x, y, 3);
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        case '4':
                            currentChar = SymbolsConfig.EmptySpace;
                            levLock.AddKey(x, y, 4);
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        //treasures
                        case SymbolsConfig.TreasureChar:
                            totalGold += 100;
                            treasures.Add(new Vector2(x, y));
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        case '£':
                            if (difficulty == Difficulty.VeryEasy || difficulty == Difficulty.Easy || difficulty == Difficulty.Normal || difficulty == Difficulty.Hard)
                            {
                                totalGold += 100;
                                treasures.Add(new Vector2(x, y));
                                currentChar = SymbolsConfig.TreasureChar;
                            }
                            else
                            {
                                currentChar = SymbolsConfig.EmptySpace;
                            }
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        case '€':
                            if (difficulty == Difficulty.VeryEasy || difficulty == Difficulty.Easy)
                            {
                                totalGold += 100;
                                treasures.Add(new Vector2(x, y));
                                currentChar = SymbolsConfig.TreasureChar;
                            }
                            else
                            {
                                currentChar = SymbolsConfig.EmptySpace;
                            }
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        //levers
                        case 'A':
                        case 'E':
                        case 'I':
                        case 'O':
                        case 'U':
                        case 'Y':
                            leversLUT[currentChar].SetLeverCoordinates(x, y);
                            Vector2 leverCoord = new Vector2(x, y);
                            leversDictionary[leverCoord] = leversLUT[currentChar];
                            currentChar = SymbolsConfig.LeverOffChar;
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        //lever gates that are closed when the game begins
                        case 'a':
                        case 'e':
                        case 'i':
                        case 'o':
                        case 'u':
                        case 'y':
                            leverGate = new Vector2(x, y);
                            leverGatesLUT[currentChar].Add(leverGate);
                            currentChar = SymbolsConfig.GateChar;
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        //lever gates that are open when the game begins
                        case 'à':
                        case 'è':
                        case 'ì':
                        case 'ò':
                        case 'ù':
                        case 'ÿ':
                            leverGate = new Vector2(x, y);
                            leverGatesLUT[currentChar].Add(leverGate);
                            currentChar = SymbolsConfig.EmptySpace;
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        //guards
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'F':
                        case 'G':
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
                            guardsLUT[currentChar].AssignOriginPoint(x, y);
                            levelGuards.Add(guardsLUT[currentChar]);
                            currentChar = SymbolsConfig.EmptySpace;
                            floorTiles.Add(new Vector2(x, y));
                            break;
                        //guards patrols
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'f':
                        case 'g':
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
                            patrolPoint = new Vector2(x, y);
                            guardPatrolsLUT[currentChar].Add(patrolPoint);
                            currentChar = SymbolsConfig.EmptySpace;
                            floorTiles.Add(new Vector2(x, y));
                            break;
                    }
                    grid[y, x] = currentChar.ToString();
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

            LevelInfo levelInfo = new LevelInfo(grid, playerStartX, playerStartY, totalGold, exit, treasures.ToArray(), floorTiles,
                                                strongLights.ToArray(), weakLights.ToArray(), levLock, leversDictionary, levelGuards.ToArray());

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
                int currentMinDistance = 1000;
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
        public string[,] Grid { get; }
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

        public LevelInfo(string[,] grid, int playerStartX, int playerStartY, int totalGold, Vector2 exit, Vector2[] treasures, HashSet<Vector2> floorTiles, 
                         Light[]strongLights, Light[] weakLights, LevelLock levelLock, Dictionary<Vector2, Lever> leversDictionary, Guard[] guards)
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
        }
    }
}
