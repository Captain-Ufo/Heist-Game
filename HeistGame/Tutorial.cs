using System;
using static System.Console;

namespace HeistGame
{
    class Tutorial
    {
        public MissionConfig[] TutorialMissions { get; private set; } = new MissionConfig[6];

        string[] tutorialLevel0 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║                   ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ¦         ║",
            "║    +    ║    +    ║    +         +         +    ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *         ║         *         ║              *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "╠════~════╦════ ════╬════ ════╦════ ════╬════ ════╬═════════╣",
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
            "║         ║         ║         ║    $    ║         ¦         ║",
            "║    +    ║    +  $ ║    +         +         +$   ║         ║",
            "║    ¹    ║         ║  $      ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *         ║         *         ║              *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "╠════~════╦════ ════╬════ ════╦════ ════╬════ ════╬═════════╣",
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
            "║                   ║         ║    Ͼ    ║         ║ Ɔ     Ͽ ║",
            "║         ║         ║         ║         ║         |         ║",
            "║ Ɔ  +    ║    +  $ ║    +    !    +         +$   ║         ║",
            "║         ║         ║  $      ║         ║         ║    ¹    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════÷════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *        0║Ɔ        *        1║              *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "╠════~════╦════ ════╬════ ════╦════ ════╬════=════╬═════════╣",
            "║         ║         ║         ║      $  ║         ║         ║",
            "║         ║     $   ║         ║         ║         ║    ¶    ║",
            "║    Ͼ    ║    +         +    ¡    +    ║    +         +    ║",
            "║         ║         ║    $    ║         ║   $     ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        string[] tutorialLevel3 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║                   ║         ║         ║         ║    Ͽ    ║",
            "║         ║         ║         ║         ║         o         ║",
            "║    +    ║    +    e    +    è    +    à    +    ║    +    ║",
            "║    A    ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║    Ͼ    ║",
            "╠═════════╩════ ════╬════ì════╩════ ════╬════÷════╬═════════╣",
            "║                   ║                   ║         ┆         ║",
            "║                   ║                   ║        ╔╩╗        ║",
            "║ X       *        0║ E       *         ║        è i   *  Ω ║",
            "║                   ║                   ║        ╚╦╝        ║",
            "║                   ║                   ║         ┆         ║",
            "╠════a════╦════a════╬════ì════╦════ ════╬════ì════╬════╍════╣",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║    I    ║",
            "║    +    ║    +    è    +    ì    +    i    +         +    ║",
            "║    O    ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        string[] tutorialLevel4 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║    $              ║         ║         ║         ║         ║",
            "║         ║    $    ║    +    ║    +    ║         |         ║",
            "║    G    ║         |    b         b         +    ║    Ͽ    ║",
            "║    +    ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║ $   0   $       Ͼ ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *         ║         *         ║              *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║ $       $       Ͼ ║                   ║         ║         ║",
            "╠═════════╦════ ════╬════ ════╦════ ════╬════ ════╬═════════╣",
            "║         ║         ║         ║         ║         ║    Ͽ    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║ c  + C       +         b  B      b  c ║    D              ║",
            "║         ║         ║    +    ║    +    ║    +    ║         ║",
            "║         ║         ║         ║         ║         ║    Ͽ    ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        string[] tutorialLevel5 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║         à         ║         ║    Ɔ    ║         ║    Ͼ    ║",
            "║         ║    +    ║    +    ║         ║         i         ║",
            "║    E    ║    C    e    d         d    a         ║    ¶    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║    Ͽ    ║",
            "╠═════════╩════à════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║         0         ║                   ║         ┆         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *     Ͽ   ║    d    *D   d A  ║    +  B ¡       Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ┆         ║",
            "╠═════════╦════à════╬════ ════╦════d════╬════ ════╬═════════╣",
            "║         ║         ║         ║         ║         ║    Ͽ    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║ I  +    !    +    e    d         d    a         i         ║",
            "║         ║         ║         ║    +    ║         ║         ║",
            "║         ║         ║    Ͼ    ║         ║         ║    Ͼ    ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        string[][] level2ObjectiveMessages =
        {
            new string[]
            {
                "Touching what you thought was the key of the exit, you discover it's only a fake, carved in wood.",
                "There's a little sheet of paper under it with some scribbles.",
                "You read:",
                " ",
                "'The key is actually in the room of the northwest corner.'"
            }
        };

        string[][] level3ObjectiveMessages =
        {
            new string[]
            {
                "You find a notebook containing the code to unlock the exit. Unfortunately the second half is covered",
                "in a grease stain and is hard to read.",
                "You remember, however, that there's another copy of the code in the closet north of here."
            }
        };

        string[][] level3Messages =
        {
            new string[]
            {
                "Welcome to the third floor of your training course.",
                "This place is safe, explore and experiment at your will.",
                " ",
                "Remember: if you try to walk through a door and it stops you, it means that it's locked and you have",
                "To lockpick it."
            },
            new string[]
            {
                "As you may have already noticed, not all chests contain valuables.",
                "And those which do, don't necessarily contain the same amount of treasure either.",
                "There's no way to know which is which before you open them"
            }
        };

        string[][] level4Messages = 
        {
            new string[]
            {
                "Welcome to the fourth floor of the training course.",
                "Not all the levels on this floor are required to access the exit. But you may have to use",
                "some of the others more than once in order to be able to leave."
            }
        };

        string[][] level5Messages =
        {
            new string[]
            {
                "This is the fifth floor. Beware, guards are patrolling it, so your movement is not as free as",
                "it used to be on the previous floor.",
                " ",
                "This first room is not as safe as it might seem at first sight, for example."
            }
        };

        string[][] level6Messages =
        {
            new string[]
            {
                "This is the final test of your training course.",
                "As such, it's the most difficult task to complete smoothly. Good luck!",
                " ",
                "Just a suggestion: Guards are not the brightest, but they are not too stupid either. Beware of how",
                "you disrupt the patrols. Also, you cannot control which guard will hear the noise, so keep an eye",
                "on all the other guards nearby before you distract someone.",
                "You wouldn't want to trap yourself in a corner!"
            }
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
            TutorialMissions[5] = new MissionConfig();

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
            TutorialMissions[1].ObjectivesMessages = level2ObjectiveMessages;
            TutorialMissions[1].Messages = emptyMessages;
            TutorialMissions[1].Outro = emptyBriefing;

            //third mission
            TutorialMissions[2].Name = "Tutorial 3";
            TutorialMissions[2].LevelMap = tutorialLevel2;
            TutorialMissions[2].Briefing = emptyBriefing;
            TutorialMissions[2].ObjectivesMessages = level3ObjectiveMessages;
            TutorialMissions[2].Messages = level3Messages;
            TutorialMissions[2].Outro = emptyBriefing;

            //fourth mission
            TutorialMissions[3].Name = "Tutorial 4";
            TutorialMissions[3].LevelMap = tutorialLevel3;
            TutorialMissions[3].Briefing = emptyBriefing;
            TutorialMissions[3].Messages = level4Messages;
            TutorialMissions[3].Outro = emptyBriefing;

            //fifth mission
            TutorialMissions[4].Name = "Tutorial 5";
            TutorialMissions[4].LevelMap = tutorialLevel4;
            TutorialMissions[4].Briefing = emptyBriefing;
            TutorialMissions[4].Messages = level5Messages;
            TutorialMissions[4].Outro = emptyBriefing;

            //Sixth mission
            TutorialMissions[5].Name = "Tutorial 6";
            TutorialMissions[5].LevelMap = tutorialLevel5;
            TutorialMissions[5].Briefing = emptyBriefing;
            TutorialMissions[5].Messages = level6Messages;
            TutorialMissions[5].Outro = emptyBriefing;
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
                        $"If the exit is red, you will need to find one or more Objectives ({SymbolsConfig.Key}) before you can leave. Objectives can be all sorts",
                        "of things, depending on the heist. Artifacts, keys, special treasures, etc.",
                        "You can collect both simple loot items and objectives by just walking over them.",
                        " ",
                        "When you are on a heist, you may have limited knowledge of the locations layouts: you will know where to look for some of the Objectives,",
                        "but as you collect the known ones, you might find that there are others hidden about the floor. You may also find hints on how to proceed.",
                        " ",
                        "If the exit doesn't turn green when you collect the last key, check the wwhole map in case you discovered the location of another one.",
                        "The status bar at the bottom of the screen also displays informations on the status of your currently known objectives."
                    };
                    DisplaytextCentered(instructions);
                    break;

                case 2:
                    instructions = new string[]
                    {
                        "Some elements in the locations can be interacted with. Use the interaction button (ENTER or E) to toggle the cursor,",
                        "then use the arrow keys to navigate the tiles around you. Press Interact or SPACE again on a tile to interact with it.",
                        $"Interactables are readable material ({SymbolsConfig.Signpost}) which may contain information on the level, or just some",
                        $"narrative, chests ({SymbolsConfig.ChestClosed}), which may contain loot, and doors ({SymbolsConfig.HorizontalDoorVisual} and {SymbolsConfig.VerticalDoorVisual}).",
                        " ",
                        "Chests and some doors may have to be lockpicked before you can use them. Once the doors are unlocked, you can just walk",
                        " though them s if they were open space. Lockpicking takes some time and makes noise, so beware of your surroundings when",
                        "you do it.",
                        "You can stop lockpicking at any time by pressing any button, or simply stepping away from the door or the chest.",
                        "Locks have different complexity, reperesented by lock levels. When you interrupt a lockpicking, you lose all the progress",
                        "on the current level. Previously completed levels remain completed.",
                        "The interaction cursor button can also be used to collect regular treasures and objectives, if you wish."
                    };
                    DisplaytextCentered(instructions);
                    break;

                case 3:
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

                case 4:
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

                case 5:
                    instructions = new string[]
                    {
                        "Sometimes guards cannot be sneaked past no matter what. Maybe they stand in the way of your objective, or the room is too lit.",
                        "If you are within their hearing range, You can make noises (by pressing the SPACEBAR or the KEYPAD + KEY to attract them to",
                        "a different part of the location and take advantage of the time they'll spend investigating the noise to sneak past them.",
                        "Keep in mind that the guards who hear your noise will home in on you. Make sure there is an escape route or a convenient shadow to hide in",
                        "before they reach your location."
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