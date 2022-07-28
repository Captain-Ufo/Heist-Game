using System;
using static System.Console;

namespace HeistGame
{
    class Tutorial
    {
        public MissionConfig[] TutorialMissions { get; private set; } = new MissionConfig[5];

        string[] tutorialLevel0 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║                   ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         |         ║",
            "║    +    ║    +    ║    +         +         +    ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *         ║         *         ║              *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "╠════-════╦════ ════╬════ ════╦════ ════╬════ ════╬═════════╣",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║    +         +         +    ║    +         +    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        string[] tutorialLevel1 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║                   ║         ║         ║         ║         ║",
            "║         ║         ║         ║    $    ║         |         ║",
            "║    +    ║    +  $ ║    +         +         +$   ║         ║",
            "║    ¹    ║         ║  $      ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *         ║         *         ║              *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "╠════-════╦════ ════╬════ ════╦════ ════╬════ ════╬═════════╣",
            "║         ║         ║         ║      $  ║         ║         ║",
            "║         ║     $   ║         ║         ║         ║    ¶    ║",
            "║         ║    +         +         +    ║    +         +    ║",
            "║         ║         ║    $    ║         ║   $     ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        string[] tutorialLevel2 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║                   ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         o    $    ║",
            "║    +    ║    +    e    +    è    +    à    +    ║    +    ║",
            "║    A    ║         ║         ║         ║         ║    $    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ì════╩════ì════╬════ì════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *         ║ E       *         ║         i    *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "╠════a════╦════a════╬════ì════╦════ì════╬════ ════╬═════════╣",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║    I    ║",
            "║    +    ║    +    è    +         +    ║    +              ║",
            "║    O    ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        string[] tutorialLevel3 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║    $              ║         ║         ║         ║         ║",
            "║         ║    $    ║    +    ║    +    ║         |         ║",
            "║    G    ║         |    b         b         +    ║         ║",
            "║    +    ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║ $       $       $ ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *         ║         *         ║              *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║ $       $       $ ║                   ║         ║         ║",
            "╠═════════╦════ ════╬════ ════╦════ ════╬════ ════╬═════════╣",
            "║         ║         ║         ║         ║         ║    $    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║ c  + C       +         b  B      b  c ║    D              ║",
            "║         ║         ║    +    ║    +    ║    +    ║         ║",
            "║         ║         ║         ║         ║         ║    $    ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        string[] tutorialLevel4 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║         à         ║         ║         ║         ║    $    ║",
            "║         ║    +    ║    +    ║         ║         i         ║",
            "║    E    ║    C    e    d         d    a         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║    $    ║",
            "╠═════════╩════à════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *         ║    d    *D   d A  ║    +  B      *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "╠═════════╦════à════╬════ ════╦════d════╬════ ════╬═════════╣",
            "║         ║         ║         ║         ║         ║    $    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║ I  +         +    e    d         d    a         i         ║",
            "║         ║         ║         ║    +    ║         ║         ║",
            "║         ║         ║         ║         ║         ║    $    ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        public Tutorial()
        {
            string[] emptyBriefing = new string[] { "" };
            string[][] emptyMessages = new string[1][];
            emptyMessages[0] = emptyBriefing;

            TutorialMissions[0] = new MissionConfig();
            TutorialMissions[1] = new MissionConfig();
            TutorialMissions[2] = new MissionConfig();
            TutorialMissions[3] = new MissionConfig();
            TutorialMissions[4] = new MissionConfig();

            //first mission
            TutorialMissions[0].Name = "Tutorial 1";
            TutorialMissions[0].LevelMap = tutorialLevel0;
            TutorialMissions[0].Briefing = emptyBriefing;
            TutorialMissions[0].Messages = emptyMessages;
            TutorialMissions[0].Outro = emptyBriefing;

            //second mission
            TutorialMissions[1].Name = "Tutorial 2";
            TutorialMissions[1].LevelMap = tutorialLevel1;
            TutorialMissions[1].Briefing = emptyBriefing;
            TutorialMissions[1].Messages = emptyMessages;
            TutorialMissions[1].Outro = emptyBriefing;

            //third mission
            TutorialMissions[2].Name = "Tutorial 3";
            TutorialMissions[2].LevelMap = tutorialLevel2;
            TutorialMissions[2].Briefing = emptyBriefing;
            TutorialMissions[2].Messages = emptyMessages;
            TutorialMissions[2].Outro = emptyBriefing;

            //fourth mission
            TutorialMissions[3].Name = "Tutorial 4";
            TutorialMissions[3].LevelMap = tutorialLevel3;
            TutorialMissions[3].Briefing = emptyBriefing;
            TutorialMissions[3].Messages = emptyMessages;
            TutorialMissions[3].Outro = emptyBriefing;

            //first mission
            TutorialMissions[4].Name = "Tutorial 5";
            TutorialMissions[4].LevelMap = tutorialLevel4;
            TutorialMissions[4].Briefing = emptyBriefing;
            TutorialMissions[4].Messages = emptyMessages;
            TutorialMissions[4].Outro = emptyBriefing;
        }

        public void DisplayTutorialInstructions(int index)
        {
            string[] instructions;

            switch (index)
            {
                case 0:
                    instructions = new string[]
                    {
                        "You are Gareth, the non-copyright infringing Master Thief. This is the training course you built in your hideout.",
                        $"Use the ARROW KEYS, W,A,S,D, or NUMPAD 4,8,6,2 to move around the map; your goal in any level is to reach the exit ({SymbolsConfig.Exit})."
                    };
                    DisplaytextCentered(instructions);
                    break;

                case 1:
                    instructions = new string[]
                    {
                        $"Loot ({SymbolsConfig.Treasure}) are optional collectibles in each floor.",
                        " ",
                        "If the exit is green, it means it's open and you are free to leave the location (assuming no other obstacles are in the way.)",
                        $"If the exit is red, you will need to find one or more Objectives ({SymbolsConfig.Key}) to unlock it. Objectives can be all sorts",
                        "of things, depending on the mission type. Artifacts, keys, special treasures, etc.",
                        "You can collect both simple loot items and objectives by just walking over them.",
                        " ",
                        "When you are on a heist, you may have limited knowledge of the locations layouts: you will know where to look for some of the Objectives,",
                        "but as you collect the known ones, you might find that there are others hidden about the floor. You may also find hints on how to proceed.",
                        " ",
                        "If the exit doesn't turn green when you collect the last key, check the wwhole map in case you discovered the location of another one."
                    };
                    DisplaytextCentered(instructions);
                    break;

                case 2:
                    instructions = new string[]
                    {
                         $"Levers ({SymbolsConfig.LeverOff}) open and close gates ({SymbolsConfig.Gate}) throughout the level.",
                         "They may open optional rooms and passages, or clear the way to your destination.",
                         " ",
                         "Levers can be linked to multiple gates. They may also close gates that start open when you first enter an area.",
                         " ",
                         "Levers can be operated both by walking over them, and by selecting them with the interaction cursor."
                    };
                    DisplaytextCentered(instructions);
                    break;

                case 3:
                    instructions = new string[]
                    {
                         "Beware of Guards! They patrol the grounds, or stand in place and look around.",
                         "Be careful of how illuminated the spot you're standing in is.",
                         "Shadows are your friends: the darker the area, the closer a guard has to be to spot you.",
                         "If they see you, they'll get alerted and chase you until they loose sight of you. You may be able to safely hide in some closets.",
                         "They can't see right behind them, of course, but be careful; they are still aware of what happens in their immediate viciniy,",
                         "even in full darkness.",
                         "Study their activity well before trying to make your move, and don't get too close no matter where they are looking.",
                         "Depending on the difficulty level you chose at the beginning of your adventure, you might be able to bribe them to look the other way.",
                         "It will get more expansive the more you do it, so don't be too cocky!",
                         "Feel free to experiment here. This is just training: you will be able to retry as many times as you need."
                    };
                    DisplaytextCentered(instructions);
                    break;

                case 4:
                    instructions = new string[]
                    {
                        "Sometimes guards cannot be sneaked past no matter what. Maybe they stand in the way of your objective, or the room is too lit.",
                        "If you are within their hearing range, You can make noises (by pressing the SPACEBAR or the KEYPAD + KEY to attract them to",
                        "a different part of the location and take advantage of the time they'll spend investigating the noise to sneak past them.",
                        "Keep in mind that the guards who hear your noise will home in on you. Make sure there is an escape route or a convenient shadow to hide in",
                        "before they reach your location.",
                        "You cannot control which guard will hear the noise, so keep an eye on all the other guards nearby before you distract someone.",
                        "You wouldn't want to trap yourself!"
                    };
                    DisplaytextCentered(instructions);
                    break;
            }
        }

        private void DisplaytextCentered(string[] instructions)
        {
            for (int i = 0; i < instructions.Length; i++)
            {
                int cursorXoffset = instructions[i].Length / 2;
                SetCursorPosition((WindowWidth / 2) - cursorXoffset, WindowTop + i + 1);
                WriteLine(instructions[i]);
            }
        }

        public void DisplayTutorialFail()
        {
            Clear();
            string[] failMessage = new string[]
            {
                "The guards caught you!",
                " ",
                "Don't worry, this is just for training. You can retry as many times as you need.",
                " ",
                " ",
                " ",
                "Press any key when you are ready to retry."
            };
            DisplaytextCentered(failMessage);
            ReadKey(true);
            return;
        }

        public void DisplayEndTutorial()
        {
            //TODO: update this with an assessment of performance? If the player has not collected all gold and/or been caught, suggest new training session?

            Clear();
            string[] endMessage = new string[]
            {
                " ",
                " ",
                "Congratulations, you completed the training course!",
                "With your master thief skills refreshed, you are ready to start new adventures.",
                " ",
                "Press Enter to return to the main menu..."
            };

            DisplaytextCentered(endMessage);

            ConsoleKeyInfo info;
            do
            {
                info = ReadKey(true);
            }
            while (info.Key != ConsoleKey.Enter);
        }
    }
}