﻿/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using static System.Console;

namespace HeistGame
{
    class Tutorial
    {
        public MissionConfig[] TutorialMissions { get; private set; } = new MissionConfig[6];

        private readonly string[] tutorialLevel0 =
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
            "║ @       *         ║         *         ║              *  Ω ║",
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

        private readonly string[] tutorialLevel1 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║                   ║         ║         ║         ║         ║",
            "║         ║         ║         ║    $    ║         ¦         ║",
            "║    +    ║    +  $ ║    +         +         +$   ║         ║",
            "║    §    ║         ║  $      ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ @       *         ║         *         ║              *  Ω ║",
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

        private readonly string[] tutorialLevel2 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║                   ║         ║    Ͼ    ║         ║ Ɔ     Ͽ ║",
            "║         ║         ║         ║         ║         █         ║",
            "║ Ɔ  +    ║    +  $ ║    +    ▎    +         +$   ║         ║",
            "║         ║         ║  $      ║         ║         ║    §    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════÷════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ @       *        0║Ɔ        *        1║              *  Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "╠════~════╦════ ════╬════ ════╦════ ════╬════=════╬═════════╣",
            "║         ║         ║         ║      $  ║         ║         ║",
            "║         ║     $   ║         ║         ║         ║    ¶    ║",
            "║    Ͼ    ║    +         +    ▌    +    ║    +         +    ║",
            "║         ║         ║    $    ║         ║   $     ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        private readonly string[] tutorialLevel3 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║                   ║         ║         ║         ║    Ͽ    ║",
            "║         ║         ║         ║         ║         o         ║",
            "║    +    ║    +    e    +    è    +    à    +    ║    +    ║",
            "║    A    ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║    Ͼ    ║",
            "╠═════════╩════ ════╬════ì════╩════ ════╬════÷════╬═════════╣",
            "║                   ║                   ║         :         ║",
            "║                   ║                   ║        ╔╩╗        ║",
            "║ @       *        0║ E       *         ║        è i   *  Ω ║",
            "║                   ║                   ║        ╚╦╝        ║",
            "║                   ║                   ║         :         ║",
            "╠════a════╦════a════╬════ì════╦════ ════╬════ì════╬════·════╣",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║    I    ║",
            "║    +    ║    +    è    +    ì    +    i    +         +    ║",
            "║    O    ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        private readonly string[] tutorialLevel4 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║    $              ║         ║         ║         ║         ║",
            "║         ║    $    ║    +    ║    +    ║         ▌         ║",
            "║    G    ║         █    b         b         +    ║    Ͽ    ║",
            "║    +    ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║ $   0   $       Ͼ ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ @       *         ║         *         ║              *  Ω ║",
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

        private readonly string[] tutorialLevel5 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║         à         ║         ║    Ɔ    ║         ║    Ͼ    ║",
            "║         ║    +    ║    +    ║         ║         i         ║",
            "║    E    ║    C    e    d         d    a         ║    ¶    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║    Ͽ    ║",
            "╠═════════╩════à════╬════ ════╩════ ════╬════ ════╬══·═══·══╣",
            "║         0         ║                   ║         :         ║",
            "║                   ║                   ║         ║         ║",
            "║ @       *     Ͽ   ║    d    *D   d A  ║    +  B ▌       Ω ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         :         ║",
            "╠═════════╦════à════╬════ ════╦════d════╬════ ════╬══·═══·══╣",
            "║         ║         ║         ║         ║         ║    Ͽ    ║",
            "║         ║         ║         ║         ║         ║         ║",
            "║ I  +    ▎    +    e    d         d    a         i         ║",
            "║         ║         ║         ║    +    ║         ║         ║",
            "║         ║         ║    Ͼ    ║         ║         ║    Ͼ    ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        private readonly string[][] level2ObjectiveMessages =
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

        private readonly string[][] level3ObjectiveMessages =
        {
            new string[]
            {
                "You find a notebook containing the code to unlock the exit. Unfortunately the second half is covered",
                "in a grease stain and is hard to read.",
                "You remember, however, that there's another copy of the code in the closet north of here."
            }
        };

        private readonly string[][] level3Messages =
        {
            new string[]
            {
                "Welcome to the third floor of your training course.",
                "This place is safe, explore and experiment at your will.",
                " ",
                "Remember: if you try to walk through a door and it stops you, it means that it's locked and you have",
                "to lockpick it."
            },
            new string[]
            {
                "As you may have already noticed, not all chests contain valuables.",
                "And those which do, don't necessarily contain the same amount of treasure either.",
                "There's no way to know which is which before you open them"
            }
        };

        private readonly string[][] level4Messages = 
        {
            new string[]
            {
                "Welcome to the fourth floor of the training course.",
                "Not all the levels on this floor are required to access the exit. But you may have to use",
                "some of the others more than once in order to be able to leave.",
                "",
                "Also keep in mind that everything outside your direct area of sight is how you remember it was when",
                "you saw it last time, not necessarily how it is now. You need to see a gate with your eyes before you",
                "can assess if it was affected by a certain lever or not."
            }
        };

        private readonly string[][] level5Messages =
        {
            new string[]
            {
                "This is the fifth floor. Beware, guards are patrolling it, so your movement is not as free as",
                "it used to be on the previous floor.",
                " ",
                "This first room is not as safe as it might seem at first sight, for example."
            }
        };

        private readonly string[][] level6Messages =
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

        private readonly string[] level1Briefing =
        {
            "You are Gareth, the non-copyright infringing Master Thief. This is the training course you built in your hideout.",
            $"Use the ARROW KEYS, W,A,S,D, or NUMPAD 4,8,6,2 to move around the map; your goal in any level is to reach the exit ({SymbolsConfig.Exit}).",
            "You can currently see only your immediate vicinity. You'll need to explore the level to see where the exit is."
        };

        private readonly string[] level2Briefing =
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
            "The status bar at the bottom of the screen also displays informations on the status of your currently known objectives. Also, the exit should",
            "turn green once you have collected all the visible and hidden objectives of the map. Keep in mind this visual effect happens only if you",
            "are currently seeing the exit. The status bar is a more reliable source of informations."
        };

        private readonly string[] level3Briefing =
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

        private readonly string[] level4Briefing =
        {
            $"Levers ({SymbolsConfig.LeverOff}) open and close gates ({SymbolsConfig.Gate}) throughout the level.",
            "They may open optional rooms and passages, or clear the way to your destination.",
            " ",
            "Levers can be linked to multiple gates. They may also close gates that start open when you first enter an area.",
            " ",
            "Levers can be operated both by walking over them, and by selecting them with the interaction cursor."
        };

        private readonly string[] level5Briefing =
        {
            "Beware of Guards! They patrol the grounds, or stand in place and look around.",
            "Check on the status bar how illuminated the spot you're standing in is.Shadows are your friends: the darker the area, the closer a",
            "guard has to be to spot you.",
            "If they see you, they'll get alerted and chase you until they lose sight of you.",
            "They can't see right behind themselves, of course (The arrow indicates in which direction), but be careful; they are still aware of what",
            "happens in their immediate viciniy even in full darkness.",
            "Study their activity well before trying to make your move, and don't get too close no matter where they are looking. If they appear as grey",
            "blocks, it means you are not seeing them, but they are within your hearing range. Use that knowledge to plan your moves in areas you have",
            "not explored yet.",
            "Depending on the difficulty level you chose at the beginning of your adventure, you might be able to bribe them to look the other way.",
            "It will get more expansive the more you do it, so don't be too cocky!",
            "Feel free to experiment here. This is just training: you will be able to retry as many times as you need."
        };

        private readonly string[] level6Briefing =
        {
            "Sometimes guards cannot be sneaked past no matter what. Maybe they stand in the way of your objective, or the room is too lit.",
            "You can peek around corners to asses the situation without exposing yourself. You can toggle peek mode with the R key, or NUMPAD 0",
            "Then choose the direction with the movement keys. Remeber that you have to toggle peek mode off before you can move again.",                        "If you are within their hearing range, You can make noises (by pressing the SPACEBAR or the KEYPAD + KEY to attract them to",
            "a different part of the location and take advantage of the time they'll spend investigating the noise to sneak past them.",
            "Keep in mind that the guards who hear your noise will home in on you. Make sure there is an escape route or a convenient shadow to hide in",
            "before they reach your location."
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
            TutorialMissions[0].Briefing = level1Briefing;
            TutorialMissions[0].Messages = emptyMessages;
            TutorialMissions[0].Outro = emptyBriefing;

            //second mission
            TutorialMissions[1].Name = "Tutorial 2";
            TutorialMissions[1].LevelMap = tutorialLevel1;
            TutorialMissions[1].Briefing = level2Briefing;
            TutorialMissions[1].ObjectivesMessages = level2ObjectiveMessages;
            TutorialMissions[1].Messages = emptyMessages;
            TutorialMissions[1].Outro = emptyBriefing;

            //third mission
            TutorialMissions[2].Name = "Tutorial 3";
            TutorialMissions[2].LevelMap = tutorialLevel2;
            TutorialMissions[2].Briefing = level3Briefing;
            TutorialMissions[2].ObjectivesMessages = level3ObjectiveMessages;
            TutorialMissions[2].Messages = level3Messages;
            TutorialMissions[2].Outro = emptyBriefing;

            //fourth mission
            TutorialMissions[3].Name = "Tutorial 4";
            TutorialMissions[3].LevelMap = tutorialLevel3;
            TutorialMissions[3].Briefing = level4Briefing;
            TutorialMissions[3].Messages = level4Messages;
            TutorialMissions[3].Outro = emptyBriefing;

            //fifth mission
            TutorialMissions[4].Name = "Tutorial 5";
            TutorialMissions[4].LevelMap = tutorialLevel4;
            TutorialMissions[4].Briefing = level5Briefing;
            TutorialMissions[4].Messages = level5Messages;
            TutorialMissions[4].Outro = emptyBriefing;

            //Sixth mission
            TutorialMissions[5].Name = "Tutorial 6";
            TutorialMissions[5].LevelMap = tutorialLevel5;
            TutorialMissions[5].Briefing = level6Briefing;
            TutorialMissions[5].Messages = level6Messages;
            TutorialMissions[5].Outro = emptyBriefing;
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