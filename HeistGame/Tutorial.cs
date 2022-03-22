using System;
using static System.Console;

namespace HeistGame
{
    class Tutorial
    {
        public string[][] TutorialLevels { get; private set; } = new string[4][];

        string[] tutorialLevel1 =
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
            "║ X       *         ║         *         ║              *  Ð ║",
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

        string[] tutorialLevel2 =
        {
            "╔═════════╦═════════╦═════════╦═════════╦═════════╦═════════╗",
            "║                   ║         ║         ║         ║         ║",
            "║         ║         ║         ║    $    ║         |         ║",
            "║    +    ║    +  $ ║    +         +         +$   ║         ║",
            "║    2    ║         ║  $      ║         ║         ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╠═════════╩════ ════╬════ ════╩════ ════╬════ ════╬═════════╣",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "║ X       *         ║         *         ║              *  Ð ║",
            "║                   ║                   ║         ║         ║",
            "║                   ║                   ║         ║         ║",
            "╠════-════╦════ ════╬════ ════╦════ ════╬════ ════╬═════════╣",
            "║         ║         ║         ║      $  ║         ║         ║",
            "║         ║     $   ║         ║         ║         ║    1    ║",
            "║         ║    +         +         +    ║    +         +    ║",
            "║         ║         ║    $    ║         ║   $     ║         ║",
            "║         ║         ║         ║         ║         ║         ║",
            "╚═════════╩═════════╩═════════╩═════════╩═════════╩═════════╝"
        };

        string[] tutorialLevel3 =
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
            "║ X       *         ║ E       *         ║         i    *  Ð ║",
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

        string[] tutorialLevel4 =
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
            "║ X       *         ║         *         ║              *  Ð ║",
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

        public Tutorial()
        {
            TutorialLevels[0] = tutorialLevel1;
            TutorialLevels[1] = tutorialLevel2;
            TutorialLevels[2] = tutorialLevel3;
            TutorialLevels[3] = tutorialLevel4;
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
                        $"Use arrow keys, WASD or numpad keys to move around the map; reach the exit ({SymbolsConfig.ExitChar}) to complete the level."
                    };
                    DisplaytextCentered(instructions);
                    break;

                case 1:
                    instructions = new string[]
                    {
                        $"Loot ({SymbolsConfig.TreasureChar}) are optional collectibles in each floor.",
                        " ",
                        "If the exit is green, it means it's open and you are free to leave the location (assuming no other obstacles are in the way.)",
                        $"If the exit is red, you will need to find one or more Keys ({SymbolsConfig.KeyChar}) to unlock it.",
                        " ",
                        "When you are on a heist, you may have limited knowledge of the locations layouts: you will know where to look for some of the keys,",
                        "but as you collect the known ones, you might find that there are others hidden about the floor.",
                        " ",
                        "If the exit doesn't turn green when you collect the last key, check the wwhole map in case you discovered the location of another one."
                    };
                    DisplaytextCentered(instructions);
                    break;

                case 2:
                    instructions = new string[]
                    {
                         $"Levers (\\) open and close gates ({SymbolsConfig.GateChar}) throughout the level.",
                         "They may open optional rooms and passages, or clear the way to your destination.",
                         "Levers can be linked to multiple gates. They may also close gates that start open when you first enter an area",
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