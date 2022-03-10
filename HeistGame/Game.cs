using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text.Json;
using static System.Console;

namespace HeistGame
{
    /// <summary>
    /// The game itself. Contains the logic, the navigation menues, and the visuals
    /// </summary>
    class Game
    {
        //private List<Level> levels;
        private string[] availableMissions;
        private string[] availableCampaigns;
        private bool playerHasBeenCaught;
        private bool hasDrawnBackground;
        private int totalGold;
        private string levelFilesPath;
        private string gameVersion = "1.6.0";
        private Menu mainMenu;
        private Menu bribeMenu;
        private Menu campaignsMenu;
        private Menu missionsMenu;
        private SaveSystem saveSystem;
        private Random rng;

        public Campaign ActiveCampaign { get; private set; }
        public Player PlayerCharacter { get; private set; }
        public Difficulty DifficultyLevel { get; private set; }
        public int CurrentRoom { get; private set; }
        public int TimesSpotted { get; set; }
        public int TimesCaught { get; private set; }
        public Stopwatch MyStopwatch { get; private set; }
        public ChiptunePlayer TunePlayer { get; private set; }

        /// <summary>
        /// Initializes all the required elements and runs the game
        /// </summary>
        public void Start()
        {
            saveSystem = new SaveSystem();
            TunePlayer = new ChiptunePlayer();
            MyStopwatch = new Stopwatch();
            rng = new Random();
            //levels = new List<Level>();

            playerHasBeenCaught = false;
            TimesCaught = 0;
            totalGold = 0;

            levelFilesPath = Directory.GetCurrentDirectory() + "/Levels";

            CreateMenues();
            RunMainMenu();
        }



        #region SetUp
        private void DisplayLoading()
        {
            string loadingText = "...Loading...";
            int posY = WindowHeight / 2;
            int halfX = WindowWidth / 2;
            int textOffset = loadingText.Length / 2;
            int posX = halfX - textOffset;

            SetCursorPosition(posX, posY);
            WriteLine(loadingText);
        }



        private void InstantiateCampaignEntities(string configFileDir, int startBooty, int startLevel)
        {
            string filePath = levelFilesPath + "/" + configFileDir + "/CampaignConfig.txt";

            if (!File.Exists(filePath))
            {
                //Safeguard #1
                ErrorWarnings.InvalidCampaignFile(filePath);
                RunMainMenu();
            }

            string configData = File.ReadAllText(filePath);
            CampaignConfig config = default;

            try
            {
                config = JsonSerializer.Deserialize<CampaignConfig>(configData);
            }
            catch (Exception e)
            {
                //Safeguard #2
                ErrorWarnings.IncorrectCampaignData(filePath, e);
                RunMainMenu();
            }

            List<Level> levels = new List<Level>();

            foreach (string levelFile in config.LevelFiles)
            {
                string levelFilePath = levelFilesPath + "/" + configFileDir + "/" + levelFile + ".txt";

                if (!File.Exists(levelFilePath))
                {
                    //safeguard #3
                    ErrorWarnings.MissingLevelFile(levelFile);
                    RunMainMenu();
                }

                string missionConfigData = File.ReadAllText(levelFilePath);

                MissionConfig missionConfig = default;

                try
                {
                    missionConfig = JsonSerializer.Deserialize<MissionConfig>(missionConfigData);
                }
                catch (Exception e)
                {
                    //Safeguard #4
                    ErrorWarnings.IncorrectLevelData(levelFile, e);
                    RunMainMenu();
                }

                LevelInfo levelInfo = LevelParser.ParseFileToLevelInfo(missionConfig.LevelMap, DifficultyLevel);

                if (levelInfo.PlayerStartX < 0)
                {
                    ErrorWarnings.MissingPlayerStartingPoint(levelFile);
                    RunMainMenu();
                }

                if (levelInfo.Exit.X < 0)
                {
                    ErrorWarnings.MissingExit(levelFile);
                    RunMainMenu();
                }

                LightMap lightMap = new LightMap(levelInfo.StrongLights, levelInfo.WeakLights);

                levels.Add(new Level(levelFile, levelInfo.Grid, levelInfo.PlayerStartX, levelInfo.PlayerStartY, levelInfo.FloorTiles, lightMap, levelInfo.LevLock,
                                     levelInfo.Exit, levelInfo.Treasures, levelInfo.LeversDictionary, levelInfo.Guards, missionConfig.Briefing,
                                     missionConfig.Outro, MyStopwatch));

                totalGold += levelInfo.TotalGold;
            }

            ActiveCampaign = new Campaign(config.Name, levels.ToArray());

            PlayerCharacter = new Player(ActiveCampaign.Levels[startLevel]);
            PlayerCharacter.SetLoot(startBooty);
        }



        private void InstantiateMissionEntities(string levelFile)
        {
            List<Level> levels = new List<Level>();

            string levelFilePath = levelFilesPath + "/" + levelFile + ".txt";

            string missionConfigData = File.ReadAllText(levelFilePath);

            MissionConfig missionConfig = default;

            try
            {
                missionConfig = JsonSerializer.Deserialize<MissionConfig>(missionConfigData);
            }
            catch (Exception e)
            {
                //Safeguard
                ErrorWarnings.IncorrectLevelData(levelFile, e);
                RunMainMenu();
            }

            LevelInfo levelInfo = LevelParser.ParseFileToLevelInfo(missionConfig.LevelMap, DifficultyLevel);

            if (levelInfo.PlayerStartX < 0)
            {
                ErrorWarnings.MissingPlayerStartingPoint(levelFile);
                RunMainMenu();
            }

            if (levelInfo.Exit.X < 0)
            {
                ErrorWarnings.MissingExit(levelFile);
                RunMainMenu();
            }

            LightMap lightMap = new LightMap(levelInfo.StrongLights, levelInfo.WeakLights);

            levels.Add(new Level(levelFile, levelInfo.Grid, levelInfo.PlayerStartX, levelInfo.PlayerStartY, levelInfo.FloorTiles, lightMap, levelInfo.LevLock,
                                 levelInfo.Exit, levelInfo.Treasures, levelInfo.LeversDictionary, levelInfo.Guards, missionConfig.Briefing,
                                 missionConfig.Outro, MyStopwatch));

            totalGold += levelInfo.TotalGold;

            ActiveCampaign = new Campaign(levelFile, levels.ToArray());

            PlayerCharacter = new Player(levels[0]);
            PlayerCharacter.SetLoot(0);
        }



        private void InstantiateTutorialEntities(Tutorial tutorial)
        {
            List<Level> levels = new List<Level>();

            for (int i = 0; i < tutorial.TutorialLevels.Length; i++)
            {
                LevelInfo levelInfo = LevelParser.ParseFileToLevelInfo(tutorial.TutorialLevels[i], DifficultyLevel);

                LightMap lightMap = new LightMap(levelInfo.StrongLights, levelInfo.WeakLights);

                levels.Add(new Level("Tutorial " + (i + 1), levelInfo.Grid, levelInfo.PlayerStartX, levelInfo.PlayerStartY, levelInfo.FloorTiles, lightMap,
                                     levelInfo.LevLock, levelInfo.Exit, levelInfo.Treasures, levelInfo.LeversDictionary, levelInfo.Guards, null,
                                     null, MyStopwatch));
            }

            ActiveCampaign = new Campaign("Tutorial", levels.ToArray());

            PlayerCharacter = new Player(ActiveCampaign.Levels[0]);
            PlayerCharacter.SetLoot(0);
        }
        #endregion



        #region Game
        private void PlayGampaign(string missionDirectory, int startRoom = 0, int startBooty = 0)
        {
            Clear();
            DisplayLoading();
            InstantiateCampaignEntities(missionDirectory, startBooty, startRoom);
            RunGameLoop(startRoom);
        }



        private void PlayMission(string mission)
        {
            Clear();
            DisplayLoading();
            InstantiateMissionEntities(mission);
            RunGameLoop(0);
        }



        private void PlayTutorial()
        {
            Tutorial tutorial = new Tutorial();

            Clear();
            DisplayLoading();
            DifficultyLevel = Difficulty.VeryEasy;
            InstantiateTutorialEntities(tutorial);
            RunGameLoop(0, tutorial);
            tutorial.DisplayEndTutorial();
            ResetGame(false);
            RunMainMenu();
        }



        private void RunGameLoop(int startRoom, Tutorial tutorial = null)
        {
            MyStopwatch.Start();
            long timeAtPreviousFrame = MyStopwatch.ElapsedMilliseconds;

            Clear();

            CurrentRoom = startRoom;
            hasDrawnBackground = false;
            bool hasDisplayedBriefing = false;

            while (true)
            {
                if (!hasDisplayedBriefing)
                {
                    MyStopwatch.Stop();
                    DisplayMissionText(ActiveCampaign.Levels[CurrentRoom].Briefing);
                    hasDisplayedBriefing = true;
                }

                MyStopwatch.Start();

                PlayerCharacter.HasMoved = false;

                if (playerHasBeenCaught)
                {
                    break;
                }

                int deltaTimeMS = (int)(MyStopwatch.ElapsedMilliseconds - timeAtPreviousFrame);
                timeAtPreviousFrame = MyStopwatch.ElapsedMilliseconds;

                if (!HandleInputs(CurrentRoom, deltaTimeMS))
                {
                    return;
                }

                ActiveCampaign.Levels[CurrentRoom].UpdateGuards(deltaTimeMS, this);


                DrawFrame(CurrentRoom);

                if (tutorial != null)
                {
                    tutorial.DisplayTutorialInstructions(CurrentRoom);
                }

                string elementAtPlayerPosition = ActiveCampaign.Levels[CurrentRoom].GetElementAt(PlayerCharacter.X, PlayerCharacter.Y);

                if (elementAtPlayerPosition == SymbolsConfig.TreasureChar.ToString())
                {
                    TunePlayer.PlaySFX(1000, 100);
                    ActiveCampaign.Levels[CurrentRoom].ChangeElementAt(PlayerCharacter.X, PlayerCharacter.Y, SymbolsConfig.EmptySpace.ToString());
                    PlayerCharacter.Draw();
                    PlayerCharacter.ChangeLoot(100);
                }
                else if (elementAtPlayerPosition == SymbolsConfig.KeyChar.ToString())
                {
                    TunePlayer.PlaySFX(800, 100);
                    ActiveCampaign.Levels[CurrentRoom].CollectKeyPiece(PlayerCharacter.X, PlayerCharacter.Y);
                    PlayerCharacter.Draw();
                }
                else if ((elementAtPlayerPosition == SymbolsConfig.LeverOffChar.ToString()
                    || elementAtPlayerPosition == SymbolsConfig.LeverOnChar.ToString())
                    && PlayerCharacter.HasMoved)
                {
                    TunePlayer.PlaySFX(100, 100);
                    ActiveCampaign.Levels[CurrentRoom].ToggleLever(PlayerCharacter.X, PlayerCharacter.Y);
                    PlayerCharacter.Draw();
                }
                else if (elementAtPlayerPosition == SymbolsConfig.ExitChar.ToString() && !ActiveCampaign.Levels[CurrentRoom].IsLocked)
                {
                    MyStopwatch.Stop();
                    DisplayMissionText(ActiveCampaign.Levels[CurrentRoom].Outro);

                    if (ActiveCampaign.Levels.Length > CurrentRoom + 1)
                    {
                        CurrentRoom++;
                        PlayerCharacter.SetStartingPosition(ActiveCampaign.Levels[CurrentRoom].PlayerStartX, ActiveCampaign.Levels[CurrentRoom].PlayerStartY);
                        hasDrawnBackground = false;
                        hasDisplayedBriefing = false;

                        if (tutorial == null)
                        {
                            saveSystem.SaveGame(this);
                        }
                        Clear();
                    }
                    else
                    {
                        if (tutorial == null)
                        {
                            saveSystem.DeleteSaveGame(this);
                        }
                        break;
                    }
                }

                Thread.Sleep(20);
            }

            if (playerHasBeenCaught)
            {
                if (tutorial != null)
                {
                    tutorial.DisplayTutorialFail();
                    hasDrawnBackground = false;
                    playerHasBeenCaught = false;
                    TimesCaught = 0;
                    PlayerCharacter.SetStartingPosition(ActiveCampaign.Levels[CurrentRoom].PlayerStartX, ActiveCampaign.Levels[CurrentRoom].PlayerStartY);
                    ActiveCampaign.Levels[CurrentRoom].Reset();
                    RunGameLoop(CurrentRoom, tutorial);
                }
                else
                {
                    GameOver();
                }
                return;
            }
            else if (tutorial == null)
            {
                WinGame();
            }
        }



        private bool HandleInputs(int currentLevel, int deltaTimeMS)
        {
            if (!PlayerCharacter.HandlePlayerControls(ActiveCampaign.Levels[currentLevel], deltaTimeMS))
            {
                MyStopwatch.Stop();
                if (QuitGame())
                {
                    return false;
                }
                else
                {
                    Clear();
                    MyStopwatch.Start();
                    hasDrawnBackground = false;
                    return true;
                }
            }
            return true;
        }



        private void DrawFrame(int currentRoom)
        {
            if (!hasDrawnBackground)
            {
                ActiveCampaign.Levels[currentRoom].Draw();
                PlayerCharacter.Draw();
                hasDrawnBackground = true;
            }
            ActiveCampaign.Levels[currentRoom].DrawGuards();
            DrawUI(currentRoom);
            CursorVisible = false;
        }



        private void DrawUI(int currentLevel)
        {
            int uiPosition = WindowHeight - 4;

            SetCursorPosition(0, uiPosition);
            for (int i = 0; i < WindowWidth; i++)
            {
                Write("_");
            }
            WriteLine("");
            Write($"   {ActiveCampaign.Levels[currentLevel].Name}");
            SetCursorPosition(32, CursorTop);
            Write($"Difficulty: {DifficultyLevel}");
            SetCursorPosition(54, CursorTop);
            Write($"Loot: ${PlayerCharacter.Loot}");
            SetCursorPosition(70, CursorTop);
            Write("Visibility: [ ");
            int visibilityLevel = PlayerCharacter.Visibility / 3;
            string visibilityDisplay = "";
            switch (visibilityLevel)
            {
                default:
                case 0:
                    visibilityDisplay = "   ";
                    break;
                case 1:
                    ForegroundColor = ConsoleColor.DarkGray;
                    visibilityDisplay = "░░░";
                    break;
                case 2:
                    ForegroundColor = ConsoleColor.Yellow;
                    visibilityDisplay = "▒▒▒";
                    break;
                case 3:
                    ForegroundColor = ConsoleColor.Yellow;
                    visibilityDisplay = "▓▓▓";
                    break;
            }
            Write(visibilityDisplay);
            ResetColor();
            Write(" ]");

            string quitInfo = "Press Escape to quit.";
            SetCursorPosition(WindowWidth - quitInfo.Length - 3, WindowHeight - 2);
            Write(quitInfo);
        }



        public void CapturePlayer(Guard guard)
        {
            MyStopwatch.Stop();

            bool canBeBribed = DifficultyLevel == Difficulty.Easy || DifficultyLevel == Difficulty.VeryEasy || guard.TimesBribed < 1;

            if (!canBeBribed || DifficultyLevel == Difficulty.VeryHard || DifficultyLevel == Difficulty.Ironman || !AttemptBribe(guard.TimesBribed))
            {
                playerHasBeenCaught = true;
                return;
            }

            guard.BribeGuard();

            TimesCaught++;
            Clear();
            hasDrawnBackground = false;

            MyStopwatch.Start();
        }



        private bool AttemptBribe(int amountBribedBefore)
        {
            Clear();
            SetCursorPosition(0, 3);

            int bribeCostIncrease = 50;

            if (DifficultyLevel == Difficulty.Normal || DifficultyLevel == Difficulty.Hard)
            {
                bribeCostIncrease = 100;
            }

            // No setting for Very Hard or Ironman because in the current iteration of the design, at those difficulty levels 
            // being caught means instant game over

            int bribeCost = 100 + (bribeCostIncrease * TimesCaught);

            string[] guardArt =
            {
                @"                           __.--|~|--.__                               ,,;/;  ",
                @"                         /~     | |    ;~\                          ,;;;/;;'  ",
                @"                        /|      | |    ;~\\                      ,;;;;/;;;'   ",
                @"                       |/|      \_/   ;;;|\                    ,;;;;/;;;;'    ",
                @"                       |/ \          ;;;/  )                 ,;;;;/;;;;;'     ",
                @"                   ___ | ______     ;_____ |___....__      ,;;;;/;;;;;'       ",
                @"             ___.-~ \\(| \  \.\ \__/ /./ /:|)~   ~   \   ,;;;;/;;;;;'         ",
                @"         /~~~    ~\    |  ~-.     |   .-~: |//  _.-~~--,;;;;/;;;;;'           ",
                @"        (.-~___     \.'|    | /-.__.-\|::::| //~      ,;;;;/;;;;;'            ",
                @"        /      ~~--._ \|   /   ______ `\:: |/       ,;;;;/;;;;;'              ",
                @"     .-|             ~~|   |  /''''''\ |:  |      ,;;;;/;;;;;' \              ",
                @"    /                   \  |  ~`'~~''~ |  /     ,;;;;/;;;;;'--__;             ",
                @"    /                   \  |  ~`'~~''~ |  /    ,;;;;/;;;;;'--__;              ",
                @"   (        \             \| `\.____./'|/    ,;;;;/;;;;;'      '\             ",
                @"  / \        \!             \888888888/    ,;;;;/;;;;;'     /    |            ",
                @" |      ___--'|              \8888888/   ,;;;;/;;;;;'      |     |            ",
                @"|`-._---       |               \888/   ,;;;;/;;;;;'              \            ",
                @"|             /                  °   ,;;;;/;;;;;'  \              \__________ ",
                @"(             )                 |  ,;;;;/;;;;;'      |        _.--~           ",
                @" \          \/ \              ,  ;;;;;/;;;;;'       /(     .-~_..--~~~~~~~~~~ ",
                @"  \__         '  `       ,     ,;;;;;/;;;;;'    .   /  \   / /~               ",
                @" /          \'  |`._______ ,;;;;;;/;;;;;;'     /   :   \/'/'       /|_/|   ``|",
                @"| _.-~~~~-._ |   \ __   .,;;;;;;/;;;;;;' ~~~~~'  .'    | |       /~ (/\/    ||",
                @"/~ _.-~~~-._\    /~/   ;;;;;;;/;;;;;;;'          |    | |       / ~/_-'|-   /|",
                @"(/~         \| /' |   ;;;;;;/;;;;;;;;            ;   | |       (.-~;  /-   / |",
                @"|            /___ `-,;;;;;/;;;;;;;;'            |   | |      ,/)  /  /-   /  |",
                @" \            \  `-.`---/;;;;;;;;;' |          _'   |T|    /'('  /  /|- _/  //",
                @"   \           /~~/ `-. |;;;;;''    ______.--~~ ~\  |u|  ,~)')  /   | \~-==// ",
                @"     \      /~(   `-\  `-.`-;   /|    ))   __-####\ |a|   (,   /|    |  \     ",
                @"       \  /~.  `-.   `-.( `-.`~~ /##############'~~)| |   '   / |    |   ~\   ",
                @"        \(   \    `-._ /~)_/|  /############'       |X|      /  \     \_\  `\ ",
                @"        ,~`\  `-._  / )#####|/############'   /     |i|  _--~ _/ | .-~~____--'",
                @"       ,'\  `-._  ~)~~ `################'           |o| ((~>/~   \ (((' -_    ",
                @"     ,'   `-.___)~~      `#############             |n|           ~-_     ~\_ ",
                @" _.,'        ,'           `###########              |g|            _-~-__    (",
                @"|  `-.     ,'              `#########       \       | |          ((.-~~~-~_--~",
                @"`\    `-.;'                  `#####' | | '    ((.-~~                          ",
                @"  `-._   )               \     |   |        .       |  \                 '    ",
                @"      `~~_ /                  |    \                |  `--------------------- ",
                @"                                                                              ",
                @"       |/ ~                `.  |    \         .     | O    __.-------------- -",
                @"                                                                              ",
                @"        |                   \ ;      \              | _.- ~                   ",
                @"                                                                              ",
                @"        |                    |        |             |  /  |                   ",
                @"                                                                              ",
                @"         |                   |         |            |/ '  |  RN TX            ",
            };

            foreach (string s in guardArt)
            {
                SetCursorPosition((WindowWidth / 3) - (s.Length / 2), CursorTop);
                WriteLine(s);
            }

            int xPos = (WindowWidth / 4) * 3;

            string guardCry;

            if (amountBribedBefore > 0)
            {
                guardCry = ChooseSecondGuardCry();
            }
            else
            {
                guardCry = ChooseFirstGuardCry();
            }

            string[] prompt =
            {
                $"\"{guardCry}\"",
                " ",
                "A guard caught you! Quick, maybe you can bribe them.",
                $"You have collected ${PlayerCharacter.Loot} so far.",
            };

            string[] options =
            {
                $"Bribe ($ {bribeCost})",
                "Surrender"
            };

            bribeMenu.UpdateMenuItems(prompt, options);

            int selectedIndex = bribeMenu.Run(xPos, 5, 2, (WindowWidth / 3) * 2, WindowWidth);

            switch (selectedIndex)
            {
                case 0:

                    string message;

                    if (PlayerCharacter.Loot >= bribeCost)
                    {
                        message = "The guard pockets your money and grumbles";
                        SetCursorPosition(xPos - message.Length / 2, CursorTop + 4);
                        WriteLine(message);
                        message = "'I don't want to see your face around here again.'";
                        SetCursorPosition(xPos - message.Length / 2, CursorTop);
                        WriteLine(message);
                        message = "'I won't be so kind next time.'";
                        SetCursorPosition(xPos - message.Length / 2, CursorTop);
                        WriteLine(message);
                        PlayerCharacter.ChangeLoot(-bribeCost);
                        ReadKey(true);
                        return true;
                    }

                    if (PlayerCharacter.Loot > 0)
                    {
                        message = "The guard won't be swayed by the paltry sum you can offer.";
                    }
                    else
                    {
                        message = "You pockets are empty. The guard won't be swayed by words alone.";
                    }

                    SetCursorPosition(xPos - message.Length / 2, CursorTop + 4);
                    WriteLine(message);
                    ReadKey(true);
                    return false;

                case 1:
                    return false;

                default:
                    return false;
            }

            string ChooseFirstGuardCry()
            {
                string[] cries = new string[]
                {
                    "HALT!",
                    "Freeze!",
                    "Cought you!",
                    "Hey!",
                    "Stop right there!",
                    "You can't be here!",
                    "You are not allowed here!",
                    "Thief!",
                    "Show's over, thief!",
                    "Who goes there?!"
                };

                int selection = rng.Next(0, cries.Length);

                return cries[selection];
            }

            string ChooseSecondGuardCry()
            {
                string[] cries = new string[]
                {
                    "I told you not to show your face again!",
                    "Hey! Why are you still around?",
                    "This time you won't get off the hook so easily!",
                    "You again?!",
                    "I won't be so nice this time!",
                };

                int selection = rng.Next(0, cries.Length);

                return cries[selection];
            }
        }



        private void GameOver()
        {
            Clear();

            string[] gameOverArt =
            {
                @"                                                                            ",
                @"                                                                            ",
                @"       ____ __|__   ____    _ ________|__ _________ ______ _____            ",
                @"       |           |o  o|      |           |  \__      |                    ",
                @"                   | c)%,      |                 \     |                    ",
                @"                   |o__o'%                 |                       |        ",
                @"    ___|______________ _  %  ,mM  _________|__ ________|__ ____ ___|__      ",
                @"             |            %  |  n    |           |           |              ",
                @"             |      -__/   %  Y /    |           |   ____    |              ",
                @"             |      /       %J_]     |           |  |o  o|   |              ",
                @"   _ _____ __|__ ________|  / /  ____|__ _______    ,%(C | __|__  __ __     ",
                @"       |           |       / /             |        %o__o|         |        ",
                @"                          / /     ,-~~,      Mm,   %               |        ",
                @"       |          ____   / /    ,r/^V\,\    n  |  %    |           |        ",
                @"   ____|_______  |o  o|  \ \    ('_ ~ ( )   \ Y  %  ___|_______ ___|__ _    ",
                @"             |   | c)%|   \/\   ()--()-))    [_t%            |              ",
                @"             |   |o__%|   /  \   \ _(x)88     \ \            |              ",
                @"             |        %   \  |`-. \ _/|8       \ \           |   _/         ",
                @"   _ _____ __|__ ____  %   \ !  ,%J___]>---.____\ \  ________|___\_____     ",
                @"       |  \_       |    %,  \ `,% \  /   /'    (__/    |           |        ",
                @"       |    \      |     `%-%-%/|  \/   /  / =\|88                 |        ",
                @"                   |          | \      /   /888        |                    ",
                @"    ___|________ __|_______   |  '----'   |8  _________|_______ ___|__      ",
                @"             |           |     \          /8     |           |              ",
                @"             |           |     |         |8      |           |              ",
                @"             |           |     |         |8                  |              ",
                @"   _ _____ __|___ _______|____ /          \_ _    ________ __|__  ___ _     ",
                @"       |           |           J\:______/ \            |\__        |        ",
                @"                   |           |           |           | | \       |        ",
                @"       |           |          /            \           |           |        ",
                @"    ___|__ ________|_______  /     \_       \ _____ ___|__ ____ ___|__ _    ",
                @"             |           |  /      /88\      \   |           |              ",
                @"                         | /      /8   \      \  |           |              ",
                @"             |            /      /8  |  \      \                            ",
                @"   _ _____ __|__ ______  /      /8___|__ \      \  _ ________|__ _____ _    ",
                @"       |           |    /     .'8         '.     \     |           |        ",
                @"       | _         |   /     /8|            \     \    |                    ",
                @"       |  \_       |  /__ __/8 |           | \_____\   |           |        ",
                @"   ____|__/________  /   |888__|__ ____  __|__ 8|   \ _|_______ ___|__ _    ",
                @"             |      /   /8           |           \   \       |              ",
                @"                   /  .'8            |            '.  \      |              ",
                @"             |    /__/8  |           |             8\__\     |              ",
                @"     ____ __ |_  |  /8___|__ _____ __|__ ________|_ 8\  l __|__ _____ _     ",
                @"      |         /> /8          |           |         8\ <\         |        ",
                @" ____          />_/8           |   y       |          8\_<\         ____    ",
                @"|o  o|       ,%J__]            |   \       |           [__t %,     | o  o | ",
                @"| c)%,      ,%> )(8__ ___ ___  |___/___ ___| ______ ___8)(  <%,    _,% (c | ",
                @"| o__o`%-%-%' __ ]8                                     [ __  '%-%-%`o__o | ",
            };

            SetCursorPosition(0, 2);

            foreach (string s in gameOverArt)
            {
                SetCursorPosition(WindowWidth - s.Length - 6, CursorTop);
                WriteLine(s);
            }

            string[] gameOverOutro =
            {
                "Oh, no!",
                "You have been caught and shackled! The guards drag you to a dingy cell.",
                "  ",
                $"You've been searched and the guards sequestered $ {PlayerCharacter.Loot} in treasures.",
                "  ",
                "Better luck next time.",
                "Try Again!",
                "  ",
                "  ",
                "Thank you for playing.",
                "  ",
                "  ",
                "÷ GAME OVER ÷",
            };

            SetCursorPosition(0, (WindowHeight / 3) - (gameOverOutro.Length / 2));

            for (int i = 0; i < gameOverOutro.Length; i++)
            {
                SetCursorPosition((WindowWidth / 4) - (gameOverOutro[i].Length / 2), CursorTop);
                if (i == gameOverOutro.Length - 1) { ForegroundColor = ConsoleColor.Red; }
                WriteLine(gameOverOutro[i]);
                ResetColor();
            }

            TunePlayer.PlayGameOverTune();

            if (CurrentRoom == 0 || DifficultyLevel == Difficulty.Easy || DifficultyLevel == Difficulty.Hard || DifficultyLevel == Difficulty.Ironman)
            {
                RestartMenu();
                ResetGame(true);
            }
            else if (DifficultyLevel == Difficulty.VeryEasy || DifficultyLevel == Difficulty.Normal || DifficultyLevel == Difficulty.VeryHard)
            {
                RetryMenu();
                ResetGame(false);
            }

            SetCursorPosition(0, WindowHeight - 2);
            Write("Press any key to continue...");
            ReadKey(true);
            TunePlayer.StopTune();
            DisplayAboutInfo();
        }



        private void WinGame()
        {
            TunePlayer.PlayGameWinTune();

            string[] outro =
            {
                "~·~ CONGRATULATIONS! ~·~",
                "  ",
                "  ",
                "You successfully completed the heist!",
                "  ",
                $"You collected $ {PlayerCharacter.Loot} in treasures, out of a total of $ {totalGold}.",
                $"You have been spotted {TimesSpotted} times, and caught {TimesCaught} times.",
                "  ",
                "  ",
                "  ",
                "  ",
                "Thank you for playing!",
                "  ",
                "Press Enter to continue..."
            };

            if (TimesCaught > 0)
            {
                outro[7] = "Nevertheless, you always persuaded the guards to look the other way.";
            }
            else
            {
                outro[7] = "You sneaked right under the guards's noses!";
                if (PlayerCharacter.Loot == totalGold)
                {
                    outro[8] = "You really are the best of all thieves in the city.";
                }
            }

            Clear();

            WriteLine(SymbolsConfig.OutroArt);

            SetCursorPosition(0, (WindowHeight / 3) - (outro.Length / 2) + 5);

            for (int i = 0; i < outro.Length; i++)
            {
                SetCursorPosition(((WindowWidth / 3) * 2) - (outro[i].Length / 2), CursorTop);
                if (i == 0) { ForegroundColor = ConsoleColor.Green; }
                WriteLine(outro[i]);
                ResetColor();
            }

            while (true)
            {
                ConsoleKeyInfo info = ReadKey(true);
                if (info.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
            ResetGame(true);
            TunePlayer.StopTune();
            DisplayAboutInfo();
        }



        private void ResetGame(bool deleteSave)
        {
            playerHasBeenCaught = false;
            hasDrawnBackground = false;
            TimesCaught = 0;
            TimesSpotted = 0;
            PlayerCharacter.SetLoot(0);
            PlayerCharacter.SetStartingPosition(ActiveCampaign.Levels[0].PlayerStartX, ActiveCampaign.Levels[0].PlayerStartY);
            CurrentRoom = 0;
            totalGold = 0;
            foreach (Level level in ActiveCampaign.Levels)
            {
                level.Reset();
            }

            if (deleteSave)
            {
                saveSystem.DeleteSaveGame(this);
            }
        }
        #endregion



        #region Menus
        private void CreateMenues()
        {
            CreateMainMenu();
            CreateBribeMenu();
            CreateMissionsMenu();
            CreateCampaignsMenu();
        }



        private void CreateMainMenu()
        {
            string[] prompt = {
                "                                                              $$$$$                                  ",
                "                                                              $:::$                                  ",
                "HHHHHHHHH     HHHHHHHHHEEEEEEEEEEEEEEEEEEEEEEIIIIIIIIII   $$$$$:::$$$$$$ TTTTTTTTTTTTTTTTTTTTTTT !!! ",
                "H:::::::H     H:::::::HE::::::::::::::::::::EI::::::::I $$::::::::::::::$T:::::::::::::::::::::T!!:!!",
                "H:::::::H     H:::::::HE::::::::::::::::::::EI::::::::I$:::::$$$$$$$::::$T:::::::::::::::::::::T!:::!",
                "HH::::::H     H::::::HHEE::::::EEEEEEEEE::::EII::::::II$::::$       $$$$$T:::::TT:::::::TT:::::T!:::!",
                "  H:::::H     H:::::H    E:::::E       EEEEEE  I::::I  $::::$            TTTTTT  T:::::T  TTTTTT!:::!",
                "  H:::::H     H:::::H    E:::::E               I::::I  $::::$                    T:::::T        !:::!",
                "  H::::::HHHHH::::::H    E::::::EEEEEEEEEE     I::::I  $:::::$$$$$$$$$           T:::::T        !:::!",
                "  H:::::::::::::::::H    E:::::::::::::::E     I::::I   $$::::::::::::$$         T:::::T        !:::!",
                "  H:::::::::::::::::H    E:::::::::::::::E     I::::I     $$$$$$$$$:::::$        T:::::T        !:::!",
                "  H::::::HHHHH::::::H    E::::::EEEEEEEEEE     I::::I              $::::$        T:::::T        !:::!",
                "  H:::::H     H:::::H    E:::::E               I::::I              $::::$        T:::::T        !!:!!",
                "  H:::::H     H:::::H    E:::::E       EEEEEE  I::::I  $$$$$       $::::$        T:::::T         !!! ",
                "HH::::::H     H::::::HHEE::::::EEEEEEEE:::::EII::::::II$::::$$$$$$$:::::$      TT:::::::TT           ",
                "H:::::::H     H:::::::HE::::::::::::::::::::EI::::::::I$::::::::::::::$$       T:::::::::T       !!! ",
                "H:::::::H     H:::::::HE::::::::::::::::::::EI::::::::I $$$$$$:::$$$$$         T:::::::::T      !!:!!",
                "HHHHHHHHH     HHHHHHHHHEEEEEEEEEEEEEEEEEEEEEEIIIIIIIIII      $:::$             TTTTTTTTTTT       !!! ",
                "                                                             $$$$$                                   "
            };

            string[] options = { "New Campaign", "Single Mission", "Tutorial", "Credits ", "Quit" };

            mainMenu = new Menu(prompt, options);
        }



        private void CreateBribeMenu()
        {
            string[] prompt =
            {
                "A guard caught you! Quick, maybe you can bribe them.",
                "You have collected $0 so far.",
            };

            string[] options =
            {
                "Bribe ($ 0)",
                "Surrender"
            };

            bribeMenu = new Menu(prompt, options);
        }



        private void CreateCampaignsMenu()
        {
            string[] prompt =
            {
                "Choose the campaign you would like to play:",
                " ",
                " ",
                " ",
            };

            availableCampaigns = Directory.GetDirectories(levelFilesPath);
            List<string> options = new List<string>();
            options.Add("Back");
            for (int i = 0; i < availableCampaigns.Length; i++)
            {
                availableCampaigns[i] = availableCampaigns[i].Remove(0, levelFilesPath.Length + 1);
                options.Add(availableCampaigns[i]);
            }

            campaignsMenu = new Menu(prompt, options.ToArray());
        }



        private void CreateMissionsMenu()
        {
            string prompt = "Choose the mission you would like to play:";

            availableMissions = Directory.GetFiles(levelFilesPath).Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
            List<string> options = new List<string>();
            options.Add("Back");
            foreach (string s in availableMissions)
            {
                options.Add(s);
            }

            missionsMenu = new Menu(prompt, options.ToArray());
        }



        private void RunMainMenu()
        {
            Clear();
            string[] saveFiles = saveSystem.CheckForSavedGames();

            string gameVersionText = "Version " + gameVersion;

            SetCursorPosition(WindowWidth - 5 - gameVersionText.Length, WindowHeight - 2);
            WriteLine(gameVersionText);
            SetCursorPosition(0, 0);

            if (saveFiles.Length > 0)
            {
                MainMenuWithContinue(saveFiles);
            }
            else
            {
                DefaultMainMenu();
            }
        }



        private void MainMenuWithContinue(string[] saveFiles)
        {
            string[] options = { "Continue Campaign", "New Campaign", "Single Mission", "Tutorial", "Credits ", "Quit" };

            mainMenu.UpdateMenuOptions(options);

            int selectedIndex = mainMenu.Run(WindowWidth / 2, 10, 5, 0, WindowWidth);

            switch (selectedIndex)
            {
                case 0:
                    LoadSaveMenu(saveFiles);
                    break;
                case 1:
                    SelectCampaign(1);
                    break;
                case 2:
                    SelectMission();
                    break;
                case 3:
                    PlayTutorial();
                    break;
                case 4:
                    DisplayAboutInfo();
                    break;
                case 5:
                    if (!MainMenuQuitGame())
                    {
                        RunMainMenu();
                    }
                    break;
            }
        }



        private void DefaultMainMenu()
        {
            string[] options = { "New Campaign", "Single Mission", "Tutorial", "Credits ", "Quit" };

            mainMenu.UpdateMenuOptions(options);

            int selectedIndex = mainMenu.Run(WindowWidth / 2, 10, 5, 0, WindowWidth);

            switch (selectedIndex)
            {
                case 0:
                    SelectCampaign(2);
                    break;
                case 1:
                    SelectMission();
                    break;
                case 2:
                    PlayTutorial();
                    break;
                case 3:
                    DisplayAboutInfo();
                    break;
                case 4:
                    if (!MainMenuQuitGame())
                    {
                        RunMainMenu();
                    }
                    break;
            }
        }



        private void LoadSaveMenu(string[] availableSaves)
        {
            Clear();

            string[] prompt =
            { 
                "~·~ Which game do you want to load? ~·~",
                "",
                "Press Delete or Backspace if you want to cancel the save file."
            };

            List<string> options = new List<string>();
            options.Add("Back");

            foreach (string s in availableSaves)
            {
                options.Add(s);
            }

            Menu loadSaveMenu = new Menu(prompt, options.ToArray());

            bool cancelFile;
            int selectedIndex;

            string[] deletePrompt =
            {
                "╔═════════════════════════════════════════════╗",
                "║                                             ║",
                "║  Are you sure you want to delete the save?  ║",
                "║                                             ║",
                "║                                             ║",
                "╚═════════════════════════════════════════════╝"
            };

            string[] deleteOptions = 
            { 
                "                      No                       ", 
                "                      Yes                      " 
            };

            Menu confirmDeleteFile = new Menu(deletePrompt, deleteOptions);

            do
            {
                cancelFile = false;

                MenuSelection selection = loadSaveMenu.RunWithDeleteEntry(WindowWidth / 2, 8, 2, 0, WindowWidth, 30);

                cancelFile = selection.cancel;
                selectedIndex = selection.selectedIndex;

                if (cancelFile && selectedIndex > 0)
                {
                    int deleteSelection = confirmDeleteFile.Run(WindowWidth / 2, (WindowHeight / 2) - 5, 2, 0, WindowWidth);

                    if (deleteSelection == 1)
                    {

                        saveSystem.DeleteSaveGame(availableSaves[selectedIndex - 1]);

                        options.Clear();
                        options.Add("Back");
                        availableSaves = saveSystem.CheckForSavedGames();

                        if (availableSaves.Length == 0)
                        {
                            Clear();
                            RunMainMenu();
                            return;
                        }
                        foreach (string s in availableSaves)
                        {
                            options.Add(s);
                        }

                        loadSaveMenu.UpdateMenuOptions(options.ToArray());
                    }
                    Clear();
                }
            }
            while (cancelFile);

            switch (selectedIndex)
            {
                case 0:
                    Clear();
                    RunMainMenu();
                    break;
                default:
                    GameData saveGame = saveSystem.LoadGame(availableSaves[selectedIndex - 1]);
                    TimesSpotted = saveGame.TimesSpotted;
                    TimesCaught = saveGame.TimesCaught;
                    DifficultyLevel = saveGame.DifficultyLevel;
                    if (!Directory.Exists(levelFilesPath + "/" + saveGame.CampaignName))
                    {
                        Clear();
                        ErrorWarnings.MissingCampaignFolder();
                        RunMainMenu();
                        return;
                    }
                    PlayGampaign(saveGame.CampaignName, saveGame.CurrentLevel, saveGame.Booty);
                    break;
            }
        }



        private void SelectCampaign(int saveGameStatus)
        {
            Clear();

            if (saveGameStatus == 1)
            {
                string[] newPrompt =
                {
                    "Choose the campaign you would like to play:",
                    " ",
                    "! WARNING: Savegames detected !",
                    "If you start a a new game with the same campaign and same difficulty level as a previous, uncompleted playthrough, the savegame will be overwritten.",
                };

                campaignsMenu.UpdateMenuPrompt(newPrompt);
            }
            else
            {
                string[] newPrompt =
                {
                    "Choose the campaign you would like to play:",
                    " ",
                    " ",
                    " ",
                };

                campaignsMenu.UpdateMenuPrompt(newPrompt);
            }

            string selectedCampaign;
            int selectedIndex = campaignsMenu.RunWithScrollingOptions(WindowWidth / 2, 5, 2, 0, WindowWidth - 1, 40);

            switch (selectedIndex)
            {
                case 0:
                    Clear();
                    RunMainMenu();
                    return;
                default:
                    selectedCampaign = availableCampaigns[selectedIndex - 1];
                    SelectDifficulty(saveGameStatus);
                    break;
            }

            PlayGampaign(selectedCampaign);
        }



        private void SelectMission()
        {
            Clear();
            int selectedIndex = missionsMenu.RunWithScrollingOptions(WindowWidth / 2, 5, 2, 0, WindowWidth - 1, 40);
            string selectedMission = "";

            switch (selectedIndex)
            {
                case 0:
                    Clear();
                    RunMainMenu();
                    return;
                default:
                    selectedMission = availableMissions[selectedIndex - 1];
                    SelectDifficulty(0);
                    break;
            }

            PlayMission(selectedMission);
        }



        /// <summary>
        /// Menu to select difficulty level.
        /// </summary>
        /// <param name="saveGameStatus">= 0 for single missions, 1 for campaigns with existing savegames, anything else for standard game</param>
        private void SelectDifficulty(int saveGameStatus)
        {
            Clear();

            string[] prompt =
            {
                "~·~ Choose your difficulty level ~·~",
                "  ",
                "The game will autosave your progress every time you complete a level. Only one savegame per difficulty level is possible.",
                "  ",
                "  ",
                "  ",
                "  ",
                "  ",
                "  ",
                "  "
            };

            switch (saveGameStatus)
            {
                case 0:
                    prompt[2] = "Single missions will not be saved";
                    break;
                case 1:
                    prompt[2] = "The game will autosave your progress every time you complete a level. Only one savegame per difficulty level is possible.";
                    prompt[3] = "! Warning: if you start a new game with the same difficulty level as an existing save, the save will be overwritten. !";
                    break;
                default:
                    prompt[2] = "The game will autosave your progress every time you complete a level. Only one savegame per difficulty level is possible.";
                    break;
            }

            string[] options = { "Back", "Very Easy", "Easy", "Normal", "Hard", "Very Hard", "Ironman" };

            string[] vEasyPrompt = new string[prompt.Length];
            Array.Copy(prompt, vEasyPrompt, prompt.Length);
            vEasyPrompt[6] = "VERY EASY: you can bribe guards as many times as you want, if you have collected enough money to do it.";
            if (saveGameStatus == 0) { vEasyPrompt[7] = "Bribe cost increase by $50 each time."; }
            else { vEasyPrompt[7] = "Bribe cost increase by $50 each time. If you game over, you'll be able to reload the last save and retry."; }

            string[] easyPrompt = new string[prompt.Length];
            Array.Copy(prompt, easyPrompt, prompt.Length);
            easyPrompt[6] = "EASY: same conditions as very easy, but if you game over, you'll have to start from the first level.";

            string[] normalPrompt = new string[prompt.Length];
            Array.Copy(prompt, normalPrompt, prompt.Length);
            normalPrompt[6] = "NORMAL: you can bribe each guard only once, after which they'll arrest you if they catch you a second time.";
            if (saveGameStatus == 0) { normalPrompt[7] = "Bribe cost will increase by $100 each time."; }
            else { normalPrompt[7] = "Bribe cost will increase by $100 each time. If you game over, you can reload the last save and retry."; }

            string[] hardPrompt = new string[prompt.Length];
            Array.Copy(prompt, hardPrompt, prompt.Length);
            hardPrompt[6] = "HARD: same conditions as normal, but if you game over, you'll have to start from the first level.";

            string[] vHardPrompt = new string[prompt.Length];
            Array.Copy(prompt, vHardPrompt, prompt.Length);
            vHardPrompt[6] = "VERY HARD: you cannot bribe guards at all. They'll arrest you on sight straight from the first time you'll cross their path.";
            if (saveGameStatus != 0) { vHardPrompt[7] = "You will still be able to load the last save and retry the same level."; }

            string[] ironmanPrompt = new string[prompt.Length];
            Array.Copy(prompt, ironmanPrompt, prompt.Length);
            if (saveGameStatus == 0) { ironmanPrompt[6] = "IRONMAN: You cannot bribe guards at all, it's game over whenever you get caught."; }
            else { ironmanPrompt[6] = "IRONMAN: You cannot bribe guards at all, and if you get caught you'll have to start from the very beginning."; }

            string[] defaultPrompt = prompt;

            string[][] promptsUpdates = new string[][]
            {
                defaultPrompt,
                vEasyPrompt,
                easyPrompt,
                normalPrompt,
                hardPrompt,
                vHardPrompt,
                ironmanPrompt,
            };

            Menu difficultyMenu = new Menu(prompt, options);

            int selectedIndex = difficultyMenu.RunWithUpdatingPrompt(WindowWidth / 2, 8, 0, 0, WindowWidth, promptsUpdates);

            switch (selectedIndex)
            {
                case 0:
                    RunMainMenu();
                    return;
                case 1:
                    DifficultyLevel = Difficulty.VeryEasy;
                    break;
                case 2:
                    DifficultyLevel = Difficulty.Easy;
                    break;
                case 3:
                    DifficultyLevel = Difficulty.Normal;
                    break;
                case 4:
                    DifficultyLevel = Difficulty.Hard;
                    break;
                case 5:
                    DifficultyLevel = Difficulty.VeryHard;
                    break;
                case 6:
                    DifficultyLevel = Difficulty.Ironman;
                    break;
            }
        }



        private void DisplayScreenDecoration()
        {
            SetCursorPosition(1, 0);
            Write("╬");
            for (int i = 2; i < WindowWidth - 1; i++)
            {
                SetCursorPosition(i, 0);
                Write("═");
            }
            SetCursorPosition(WindowWidth - 1, 0);
            Write("╬");
            for (int i = 1; i < WindowHeight - 2; i++)
            {
                SetCursorPosition(1, i);
                Write("║");
                SetCursorPosition(WindowWidth - 1, i);
                Write("║");
            }
            SetCursorPosition(1, WindowHeight - 2);
            Write("╬");
            for (int i = 2; i < WindowWidth - 1; i++)
            {
                SetCursorPosition(i, WindowHeight - 2);
                Write("═");
            }
            SetCursorPosition(WindowWidth - 1, WindowHeight - 2);
            Write("╬");
        }



        private void DisplayMissionText(string[] text)
        {
            if (text == null) { return; }

            if (string.IsNullOrEmpty(text[0])) { return; }

            int firstLineToDisplay = 0;
            int lastLineToDisplay;
            if (text.Length > 48) { lastLineToDisplay = 48; }
            else { lastLineToDisplay = text.Length; }

            List<string> textToDisplay = new List<string>();

            do
            {
                textToDisplay.Clear();
                for (int i = firstLineToDisplay; i < lastLineToDisplay; i++)
                {
                    textToDisplay.Add(text[i]);
                }

                Clear();
                DisplayScreenDecoration();

                SetCursorPosition(0, (WindowHeight / 2) - ((textToDisplay.Count / 2) + 2));

                foreach (string s in textToDisplay)
                {
                    SetCursorPosition((WindowWidth / 2) - (s.Length / 2), CursorTop);
                    WriteLine(s);
                }

                SetCursorPosition((WindowWidth / 2) - 2, CursorTop);
                WriteLine("~··~");

                string t = "Press Enter to continue...";
                SetCursorPosition((WindowWidth / 2) - (t.Length / 2), CursorTop);
                ForegroundColor = ConsoleColor.Green;
                WriteLine(t);
                ResetColor();

                ConsoleKeyInfo info;
                do
                {
                    info = ReadKey(true);
                }
                while (info.Key != ConsoleKey.Enter);

                Clear();

                firstLineToDisplay += 48;
                lastLineToDisplay += 48;
                if (lastLineToDisplay > text.Length - 1)
                {
                    lastLineToDisplay = text.Length;
                }
            }
            while (firstLineToDisplay < text.Length);
        }



        private void DisplayAboutInfo()
        {
            Clear();
            string authorName = "Cristian Baldi";
            string[] credits = new string[]
            {
                " ",
                " ",
                "~·~ CREDITS: ~·~",
                " ",
                " ",
                $"Heist!, a commnd prompt stealth game by {authorName}",
                " ",
                $"Programming: {authorName}",
                "Shoutout to Micheal Hadley's \"Intro To Programming in C#\" course:",
                "https://www.youtube.com/channel/UC_x9TgYAIFHj1ulXjNgZMpQ",
                " ",
                $"Baron's Jail campaign level desing: {authorName}",
                "(I'm not a level designer :P I'm sure you guys can do a lot better!)",
                " ",
                $"Chiptune Music: {authorName}",
                "(I'm not a musician either)",
                " ",
                " ",
                "~·~ ART: ~·~",
                " ",
                "Ascii title from Text To Ascii Art Generator (https://www.patorjk.com/software/taag)",
                " ",
                "Ascii art from Ascii Art Archive (https://www.asciiart.eu/):",
                "Guard art based on 'Orc' by Randall Nortman and Tua Xiong",
                "Win screen art by Henry Segerman",
                "Game over screen art based on art by Jgs",
                " ",
                " ",
                "~·~ TESTING and SPECIAL THANKS: ~·~",
                "Izzy",
                "Charlie & Daisy",
                "Lone",
                "Giorgio"
            };

            foreach (string credit in credits)
            {
                for (int i = 0; i < credits.Length; i++)
                {
                    int cursorXoffset = credits[i].Length / 2;
                    SetCursorPosition((WindowWidth / 2) - cursorXoffset, WindowTop + i + 1);
                    WriteLine(credits[i]);
                }
            }

            SetCursorPosition(0, WindowHeight - 3);
            WriteLine("\n Press any key to return to main menu...");
            ReadKey(true);
            Clear();
            RunMainMenu();
        }



        private bool QuitGame()
        {
            Clear();
            string[] quitMenuPrompt =
             {
                "Are you sure you want to quit?",
                " ",
                " "
             };

            if (CurrentRoom > 0)
            {
                quitMenuPrompt[1] = "The game automatically saved the last level you played, but all your progress in the current level will be lost.";
            }

            string[] options = { "Return to game", "Quit to Main Menu", "Quit to desktop" };

            Menu quitMenu = new Menu(quitMenuPrompt, options);
            int selection = quitMenu.Run(WindowWidth / 2, WindowHeight / 3, 2, 0, WindowWidth);
            switch (selection)
            {
                case 0:
                    return false;
                case 1:
                    RunMainMenu();
                    return true;
                case 2:
                    Environment.Exit(0);
                    return true;
                default:
                    return false;
            }
        }



        private bool MainMenuQuitGame()
        {
            Clear();
            string[] quitMenuPrompt =
            {
                "Are you sure you want to quit?",
            };
            string[] options = { "Yes", "No" };

            Menu quitMenu = new Menu(quitMenuPrompt, options);
            int selection = quitMenu.Run(WindowWidth / 2, WindowHeight / 3, 2, 0, WindowWidth);
            if (selection == 0)
            {
                Environment.Exit(0);
                return true;
            }
            else
            {
                return false;
            }
        }



        private void RestartMenu()
        {
            string prompt = "Would you like to restart the game?";

            string[] options =
            {
                "Yes",
                "No",
            };

            Menu retryMenu = new Menu(prompt, options);

            int selectedIndex = retryMenu.Run(WindowWidth / 4, CursorTop + 2, 1, 0, WindowWidth / 2);

            if (selectedIndex == 0)
            {
                TunePlayer.StopTune();
                ResetGame(true);
                RunGameLoop(0);
            }
        }



        private void RetryMenu()
        {
            string prompt = "Would you like to retry the last level?";

            string[] options =
            {
                "Yes",
                "No",
            };

            Menu retryMenu = new Menu(prompt, options);

            int selectedIndex = retryMenu.Run(WindowWidth / 4, CursorTop + 2, 1, 0, WindowWidth / 2);

            if (selectedIndex == 0)
            {
                TunePlayer.StopTune();
                Retry();
            }
        }



        private void Retry()
        {
            playerHasBeenCaught = false;

            GameData saveGame = saveSystem.LoadGame(this);
            TimesSpotted = saveGame.TimesSpotted;
            TimesCaught = saveGame.TimesCaught;
            DifficultyLevel = saveGame.DifficultyLevel;
            PlayerCharacter.SetLoot(saveGame.Booty);
            PlayerCharacter.SetStartingPosition(ActiveCampaign.Levels[saveGame.CurrentLevel].PlayerStartX, ActiveCampaign.Levels[saveGame.CurrentLevel].PlayerStartY);
            ActiveCampaign.Levels[saveGame.CurrentLevel].Reset();
            RunGameLoop(saveGame.CurrentLevel);
        }
        #endregion;
    }



    public enum Difficulty { VeryEasy, Easy, Normal, Hard, VeryHard, Ironman }
}
