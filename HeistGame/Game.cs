/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

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
        private int totalGold;
        private string levelFilesPath;
        private Menu mainMenu;
        private Menu bribeMenu;
        private Menu campaignsMenu;
        private Menu missionsMenu;
        private SaveSystem saveSystem;
        private Random rng;

        public bool HasDrawnBackground { get; set; }
        public TileSelector Selector { get; private set; }
        public Campaign ActiveCampaign { get; private set; }
        public Player PlayerCharacter { get; private set; }
        public Difficulty DifficultyLevel { get; private set; }
        public int CurrentLevel { get; private set; }
        public int TimesSpotted { get; set; }
        public int TimesCaught { get; private set; }
        public Stopwatch MyStopwatch { get; private set; }
        public ChiptunePlayer TunePlayer { get; private set; }
        public Unlockable ActiveUnlockable { get; set; }

        public Game ()
        {

        }

        /// <summary>
        /// Initializes all the required elements and runs the game
        /// </summary>
        public void Start()
        {
            saveSystem = new SaveSystem();
            TunePlayer = new ChiptunePlayer();
            MyStopwatch = new Stopwatch();
            rng = new Random();
            Selector = new TileSelector(this);

            playerHasBeenCaught = false;
            TimesCaught = 0;
            totalGold = 0;

            levelFilesPath = Directory.GetCurrentDirectory() + "/Levels";

            CreateMenues();
            RunMainMenu();
        }

        public void Restart()
        {
            RunMainMenu();
        }

        #region SetUp
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

                try
                {
                    LevelInfo levelInfo = LevelParser.ParseConfigToLevelInfo(missionConfig, DifficultyLevel);

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

                    Message briefing = new Message(MessageType.BRIEFING, levelFile, missionConfig.Briefing);
                    Message outro = new Message(MessageType.DEBRIFIENG, levelFile, missionConfig.Outro);

                    levels.Add(new Level(levelFile, levelInfo.Grid, levelInfo.PlayerStartX, levelInfo.PlayerStartY, levelInfo.FloorTiles, lightMap, levelInfo.LevLock,
                                         levelInfo.Exit, levelInfo.Treasures, levelInfo.LeversDictionary, levelInfo.Guards, levelInfo.MessagesDictionary, levelInfo.UnlockablesDictionary,
                                         levelInfo.MapsDictionary, levelInfo.Walls, briefing, outro, this));

                    totalGold += levelInfo.TotalGold;
                }
                catch (Exception e)
                {
                    {
                        ErrorWarnings.IncorrectLevelData(levelFile, e);
                        RunMainMenu();
                    }
                }
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

            try
            {
                LevelInfo levelInfo = LevelParser.ParseConfigToLevelInfo(missionConfig, DifficultyLevel);

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
                Message briefing = new Message(MessageType.BRIEFING, levelFile, missionConfig.Briefing);
                Message outro = new Message(MessageType.DEBRIFIENG, levelFile, missionConfig.Outro);

                levels.Add(new Level(levelFile, levelInfo.Grid, levelInfo.PlayerStartX, levelInfo.PlayerStartY, levelInfo.FloorTiles, lightMap, levelInfo.LevLock,
                                     levelInfo.Exit, levelInfo.Treasures, levelInfo.LeversDictionary, levelInfo.Guards, levelInfo.MessagesDictionary, levelInfo.UnlockablesDictionary,
                                     levelInfo.MapsDictionary, levelInfo.Walls, briefing, outro, this));

                totalGold += levelInfo.TotalGold;

                ActiveCampaign = new Campaign(levelFile, levels.ToArray());

                PlayerCharacter = new Player(levels[0]);
                PlayerCharacter.SetLoot(0);
            }
            catch (Exception e)
            {
                {
                    //Safeguard
                    ErrorWarnings.IncorrectLevelData(levelFile, e);
                    RunMainMenu();
                }
            }
        }



        private void InstantiateTutorialEntities(Tutorial tutorial)
        {
            List<Level> levels = new List<Level>();

            for (int i = 0; i < tutorial.TutorialMissions.Length; i++)
            {
                LevelInfo levelInfo = LevelParser.ParseConfigToLevelInfo(tutorial.TutorialMissions[i], DifficultyLevel);

                LightMap lightMap = new LightMap(levelInfo.StrongLights, levelInfo.WeakLights);
                Message briefing = new Message(MessageType.BRIEFING, "Tutorial " + (i + 1), tutorial.TutorialMissions[i].Briefing);
                Message outro = new Message(MessageType.DEBRIFIENG, "Tutorial " + (i + 1), tutorial.TutorialMissions[i].Outro);

                levels.Add(new Level("Tutorial " + (i + 1), levelInfo.Grid, levelInfo.PlayerStartX, levelInfo.PlayerStartY, levelInfo.FloorTiles, lightMap,
                                     levelInfo.LevLock, levelInfo.Exit, levelInfo.Treasures, levelInfo.LeversDictionary, levelInfo.Guards,levelInfo.MessagesDictionary, 
                                     levelInfo.UnlockablesDictionary, levelInfo.MapsDictionary, levelInfo.Walls, briefing, outro, this));
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
            ScreenDisplayer.ClearMessageLog();
            ScreenDisplayer.DisplayLoading();
            InstantiateCampaignEntities(missionDirectory, startBooty, startRoom);
            RunGameLoop(startRoom);
        }



        private void PlayMission(string mission)
        {
            Clear();
            ScreenDisplayer.ClearMessageLog();
            ScreenDisplayer.DisplayLoading();
            InstantiateMissionEntities(mission);
            RunGameLoop(0);
        }



        private void PlayTutorial()
        {
            Tutorial tutorial = new Tutorial();

            Clear();
            ScreenDisplayer.ClearMessageLog();
            ScreenDisplayer.DisplayLoading();
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

            CurrentLevel = startRoom;
            HasDrawnBackground = false;
            bool hasDisplayedBriefing = false;

            while (true)
            {
                if (!hasDisplayedBriefing)
                {
                    MyStopwatch.Stop();
                    ScreenDisplayer.DisplayTextFullScreen(ActiveCampaign.Levels[CurrentLevel].Briefing);
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

                if (!HandleInputs(CurrentLevel, deltaTimeMS))
                {
                    return;
                }

                if (ActiveCampaign.Levels[CurrentLevel].ExploredMap.Count <= 0)
                {
                    PlayerCharacter.CalculateVisibleArea(ActiveCampaign.Levels[CurrentLevel]);
                }

                ActiveCampaign.Levels[CurrentLevel].UpdateGuards(deltaTimeMS, this);

                DrawFrame();

                char elementAtPlayerPosition = ActiveCampaign.Levels[CurrentLevel].GetElementAt(PlayerCharacter.X, PlayerCharacter.Y);

                if (elementAtPlayerPosition == SymbolsConfig.Treasure)
                {
                    TunePlayer.PlaySFX(1000, 100);
                    ActiveCampaign.Levels[CurrentLevel].ChangeElementAt(PlayerCharacter.X, PlayerCharacter.Y, SymbolsConfig.Empty);
                    PlayerCharacter.ChangeLoot(100);
                }
                else if (elementAtPlayerPosition == SymbolsConfig.Key)
                {
                    TunePlayer.PlaySFX(800, 100);
                    ActiveCampaign.Levels[CurrentLevel].CollectKeyPiece(PlayerCharacter.X, PlayerCharacter.Y, this);
                }
                else if ((elementAtPlayerPosition == SymbolsConfig.LeverOff
                    || elementAtPlayerPosition == SymbolsConfig.LeverOn)
                    && PlayerCharacter.HasMoved)
                {
                    TunePlayer.PlaySFX(100, 100);
                    ActiveCampaign.Levels[CurrentLevel].ToggleLever(PlayerCharacter.X, PlayerCharacter.Y);
                }
                else if (elementAtPlayerPosition == SymbolsConfig.Exit && !ActiveCampaign.Levels[CurrentLevel].IsLocked)
                {
                    MyStopwatch.Stop();
                    ScreenDisplayer.DisplayTextFullScreen(ActiveCampaign.Levels[CurrentLevel].Outro);

                    if (ActiveCampaign.Levels.Length > CurrentLevel + 1)
                    {
                        CurrentLevel++;
                        PlayerCharacter.SetStartingPosition(ActiveCampaign.Levels[CurrentLevel].PlayerStartX, ActiveCampaign.Levels[CurrentLevel].PlayerStartY);
                        HasDrawnBackground = false;
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

                if (ActiveUnlockable != null)
                {
                    ActiveUnlockable.Unlock(deltaTimeMS, this);
                    ActiveCampaign.Levels[CurrentLevel].AlertGuards(new Vector2(PlayerCharacter.X, PlayerCharacter.Y), 3);
                }

                //Thread.Sleep(20);
            }

            if (playerHasBeenCaught)
            {
                if (tutorial != null)
                {
                    tutorial.DisplayTutorialFail();
                    HasDrawnBackground = false;
                    playerHasBeenCaught = false;
                    TimesCaught = 0;
                    PlayerCharacter.SetStartingPosition(ActiveCampaign.Levels[CurrentLevel].PlayerStartX, ActiveCampaign.Levels[CurrentLevel].PlayerStartY);
                    ActiveCampaign.Levels[CurrentLevel].Reset();
                    RunGameLoop(CurrentLevel, tutorial);
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
            ControlState state = ControlsManager.HandleInputs(ActiveCampaign.Levels[currentLevel], this, deltaTimeMS);
            if ( state == ControlState.Escape)
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
                    HasDrawnBackground = false;
                    return true;
                }
            }
            return true;
        }



        private void DrawFrame()
        {
            ScreenDisplayer.UpdateUI(this);
            ScreenDisplayer.DrawGameplayScreen(this);
            CursorVisible = false;
        }



        public void CapturePlayer(Guard guard)
        {
            MyStopwatch.Stop();
            if (Selector.IsActive) { Selector.Deactivate(); }

            bool canBeBribed = DifficultyLevel == Difficulty.Easy || DifficultyLevel == Difficulty.VeryEasy || guard.TimesBribed < 1;

            if (!canBeBribed || DifficultyLevel == Difficulty.VeryHard || DifficultyLevel == Difficulty.Ironman || !AttemptBribe(guard.TimesBribed))
            {
                playerHasBeenCaught = true;
                return;
            }

            guard.BribeGuard();

            TimesCaught++;
            Clear();
            HasDrawnBackground = false;

            MyStopwatch.Start();
        }



        public void CancelUnlocking()
        {
            if (ActiveUnlockable != null)
            {
                ActiveUnlockable.CancelUnlocking();
                ActiveUnlockable = null;
            }
            ScreenDisplayer.DeleteLabel();
        }



        private bool AttemptBribe(int amountBribedBefore)
        {
            ActiveUnlockable = null;
            ScreenDisplayer.DeleteLabel();
            Clear();
            ResetColor();
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
                @"                           __.--|~|--.__                                 ,,;/;",
                @"                         /~     | |    ;~\                            ,;;;/;;'",
                @"                        /|      | |    ;~\\                        ,;;;;/;;;' ",
                @"                       |/|      \_/   ;;;|\                      ,;;;;/;;;;'  ",
                @"                       |/ \          ;;;/  )                   ,;;;;/;;;;;'   ",
                @"                   ___ | ______     ;_____ |___....__        ,;;;;/;;;;;'     ",
                @"             ___.-~ \\(| \  (.\ \__/ /.) /:|)~   ~   \     ,;;;;/;;;;;'       ",
                @"         /~~~    ~\    |  ~-.     |   .-~: |//  _.-~~--__,;;;;/;;;;;'         ",
                @"        (.-~___     \.'|    | /-.__.-\|::::| //~       ,;;;;/;;;;;'           ",
                @"        /      ~~--._ \|   /   ______ `\:: |/        ,;;;;/;;;;;'             ",
                @"     .-|             ~~|   |  /''''''\ |:  |       ,;;;;/;;;;;'\              ",
                @"    /                   \  |  ~`'~~''~ |  /      ,;;;;/;;;;;'-__;             ",
                @"    /                   \  |  ~`'~~''~ |  /    ,;;;;/;;;;;'--__;              ",
                @"   (        \             \| `\.____./'|/    ,;;;;/;;;;;'      '\             ",
                @"  / \        \!             \888888888/    ,;;;;/;;;;;'     /    |            ",
                @" |      ___--'|              \8888888/   ,;;;;/;;;;;'      |     |            ",
                @"|`-._---       |               \888/   ,;;;;/;;;;;'              \            ",
                @"|             /                  °   ,;;;;/;;;;;'  \              \__________ ",
                @"(             )                 |  ,;;;;/;;;;;'      |        _.--~           ",
                @" \          \/ \              ,  ;;;;;/;;;;;'       /(     .-~_..--~~~~~~~~~~ ",
                @"  \__        '  `             ,;;;;;/;;;;;'        /  \  / /~                 ",
                @" /          \'  |`._______ ,;;;;;;/;;;;;;'       /:    \/'/'       /|_/|   ``|",
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
                    "Hey, you!",
                    "Stop right there!",
                    "You can't be here!",
                    "What do you think you are doing?!",
                    "You are not allowed here!",
                    "Thief!",
                    "Bandit!",
                    "Criminal!",
                    "You luck's over, criminal!",
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
            ResetColor();

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
                @"             |   | c)%|   \/\   |=  =  ))    [_t%            |              ",
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
                @" ____          />_/8           |   y       |          8\_<\          ____   ",
                @"|o  o|       ,%J__]            |   \       |           [__t %,      |o  o|  ",
                @"| c)%,      ,%> )(8__ ___ ___  |___/___ ___| ______ ___8)(  <%,    _,% (c|  ",
                @"|o__o`%-%-%' __ ]8                                      [ __  '%-%-%`o__o|  ",
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

            if (CurrentLevel == 0 || DifficultyLevel == Difficulty.Easy || DifficultyLevel == Difficulty.Hard || DifficultyLevel == Difficulty.Ironman)
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
            ScreenDisplayer.DisplayAboutScreen(this);
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

            ForegroundColor = ConsoleColor.Yellow;
            WriteLine(SymbolsConfig.OutroArt);
            ResetColor();

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
            ScreenDisplayer.DisplayAboutScreen(this);
        }



        private void ResetGame(bool deleteSave)
        {
            playerHasBeenCaught = false;
            HasDrawnBackground = false;
            TimesCaught = 0;
            TimesSpotted = 0;
            PlayerCharacter.Reset(ActiveCampaign.Levels[0].PlayerStartX,
                                  ActiveCampaign.Levels[0].PlayerStartY,
                                  ActiveCampaign.Levels[0]);
            CurrentLevel = 0;
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
            ResetColor();
            string[] saveFiles = saveSystem.CheckForSavedGames();

            string gameVersionText = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

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
                    ScreenDisplayer.DisplayAboutScreen(this);
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
                    ScreenDisplayer.DisplayAboutScreen(this);
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
                "╔══════════════════════════════════════════════════════════════════════════════════════════╗",
                "║                                                                                          ║",
                "║                        Are you sure you want to delete this save?                        ║",
                "║                                                                                          ║",
                "║                                                                                          ║",
                "║                                                                                          ║",
                "╚══════════════════════════════════════════════════════════════════════════════════════════╝"
            };

            string confirmMenuLine = deletePrompt[4];

            string[] deleteOptions = 
            { 
                "                          No                           ", 
                "                          Yes                          " 
            };

            Menu confirmDeleteFile = new Menu(deletePrompt, deleteOptions);

            do
            {
                selectedIndex = loadSaveMenu.RunWithDeleteEntry(WindowWidth / 2, 8, 2, 0, WindowWidth, 30, out cancelFile);

                if (cancelFile && selectedIndex > 0)
                {
                    // Inserting the savename in the menu for clarity
                    string saveName = availableSaves[selectedIndex - 1];

                    if (saveName.Length >= confirmMenuLine.Length)
                    {
                        //Shortening the savegame name if it's longer than the menu prompt
                        saveName = saveName.Remove(confirmMenuLine.Length - 8, saveName.Length - 1);
                        saveName = saveName + "...";
                    }

                    int insertCount = ((confirmMenuLine.Length - saveName.Length) / 2) - 1;
                    int targetStringLenght = confirmMenuLine.Length;
                    int trimStartIndex = targetStringLenght - insertCount - 1;
                    confirmMenuLine = confirmMenuLine.Insert(insertCount, saveName);
                    int newStringExcess = confirmMenuLine.Length - targetStringLenght;
                    confirmMenuLine = confirmMenuLine.Remove(trimStartIndex, newStringExcess);
                    deletePrompt[4] = confirmMenuLine;
                    confirmDeleteFile.UpdateMenuPrompt(deletePrompt);

                    // Confirming deletion
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



        private bool QuitGame()
        {
            Selector.Deactivate();
            Clear();
            ResetColor();
            string[] quitMenuPrompt =
             {
                "Are you sure you want to quit?",
                " ",
                " "
             };

            if (CurrentLevel > 0)
            {
                quitMenuPrompt[1] = "The game automatically saved the last level you played, but all your progress in the current level will be lost.";
            }

            string[] options = { "Return to game", "Quit to Main Menu", "Quit to desktop" };

            Menu quitMenu = new Menu(quitMenuPrompt, options);
            int selection = quitMenu.Run(WindowWidth / 2, WindowHeight / 3, 2, 0, WindowWidth);
            switch (selection)
            {
                case 0:
                    ControlsManager.State = ControlState.Idle;
                    return false;
                case 1:
                    ControlsManager.State = ControlState.Idle;
                    RunMainMenu();
                    return true;
                case 2:
                    Environment.Exit(0);
                    return true;
                default:
                    ControlsManager.State = ControlState.Idle;
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
