using System;
using System.Collections.Generic;
using static System.Console;

namespace HeistGame
{
    /// <summary>
    /// Creates a console based keyboard controlled menu and handles user interactions with it
    /// </summary>
    class Menu
    {
        private int selectedIndex;
        private string[] options;
        private string[] prompt;
        private ChiptunePlayer ctp;

        /// <summary>
        /// Instantiates a Menu object
        /// </summary>
        /// <param name="prompt">The single string that prompts the player to chose</param>
        /// <param name="options">The array of options the menu displays</param>
        public Menu(string prompt, string[] options)
        {
            this.prompt = new string[] { prompt };
            this.options = options;
            selectedIndex = 0;
            ctp = new ChiptunePlayer();
        }

        /// <summary>
        /// Instantiates a Menu object
        /// </summary>
        /// <param name="prompt">The prompt in string array form, to be used for multiple lines prompts, or ascii/text based art</param>
        /// <param name="options">The list of options the menu displays</param>
        public Menu(string[] prompt, string[] options)
        {
            this.prompt = prompt;
            this.options = options;
            selectedIndex = 0;
            ctp = new ChiptunePlayer();
        }


        /// <summary>
        /// Displays the menu and handles user inputs for options selection
        /// </summary>
        /// <param name="xPos">Horizontal position of the menu (prompt and options) on the screen. Input 0 for the left side of the screen, 
        /// any other number to center the menu around that position</param>
        /// <param name="yPos">Vertical position of the prompt on the screen. Input 0 for the very top of the screen</param>
        /// <param name="optionsOffset">The vertical distance between the menu prompt and the option list</param>
        /// <param name="lineStart">The X coordinate that indicates where each line in the menu starts (so that part of the screen can remain
        /// uneffected by the updates of the various parts). Use 0 for the left edge of the screen</param>
        /// <param name="lineEnd">The X coordinate that indicates where each line in the menu ends (so that part of the screen can remain
        /// uneffected by the updates of the various parts). Use WindowWidth for the right edge of the screen</param>
        /// <returns>The index of the chosen option, after the user selects one and hits enter</returns>
        public int Run(int xPos, int yPos, int optionsOffset, int lineStart, int lineEnd)
        {
            ConsoleKey keyPressed;
            selectedIndex = 0;

            do
            {
                SetCursorPosition(0, 0);
                DisplayPrompt(xPos, yPos, lineStart, lineEnd);
                DisplayOptions(xPos, optionsOffset);

                ConsoleKeyInfo info = ReadKey(true);
                keyPressed = info.Key;

                switch (keyPressed)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.NumPad8:
                    case ConsoleKey.W:

                        selectedIndex--;
                        if (selectedIndex < 0)
                        {
                            selectedIndex = options.Length - 1;
                        }
                        ctp.PlaySFX(1000, 100); ;
                        while (KeyAvailable) { ReadKey(true); }
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.S:

                        selectedIndex++;
                        if (selectedIndex == options.Length)
                        {
                            selectedIndex = 0;
                        }
                        ctp.PlaySFX(1000, 100);
                        while (KeyAvailable) { ReadKey(true); }
                        break;
                }
            }
            while (keyPressed != ConsoleKey.Enter);

            return selectedIndex;
        }

        /// <summary>
        /// Displays the menu, allowing for a dynamic prompt that updates depending on the selected option
        /// </summary>
        /// <param name="xPos">Horizontal position of the menu (prompt and options) on the screen. Input 0 for the left side of the screen, 
        /// any other number to center the menu around that position</param>
        /// <param name="yPos">Vertical position of the prompt on the screen. Input 0 for the very top of the screen</param>
        /// <param name="optionsOffset">The verticfal distance between the menu prompt and the option list</param>
        /// <param name="lineStart">The X coordinate that indicates where each line in the menu starts (so that part of the screen can remain
        /// uneffected by the updates of the various parts). Use 0 for the left edge of the screen</param>
        /// <param name="lineEnd">The X coordinate that indicates where each line in the menu ends (so that part of the screen can remain
        /// uneffected by the updates of the various parts). Use WindowWidth for the right edge of the screen</param>
        /// <param name="updatedPrompts">An array containing the different prompts per each option. This array MUST have as many entries as the number of options.</param>
        /// <returns>The index of the chosen option, after the user selects one and hits enter</returns>
        public int RunWithUpdatingPrompt(int xPos, int yPos, int optionsOffset, int lineStart, int lineEnd, string[][] updatedPrompts)
        {
            ConsoleKey keyPressed;

            do
            {
                SetCursorPosition(0, 0);
                DisplayPrompt(xPos, yPos, lineStart, lineEnd);
                DisplayOptions(xPos, optionsOffset);

                ConsoleKeyInfo info = ReadKey(true);
                keyPressed = info.Key;

                switch (keyPressed)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.NumPad8:
                    case ConsoleKey.W:

                        selectedIndex--;
                        if (selectedIndex < 0)
                        {
                            selectedIndex = options.Length - 1;
                        }
                        prompt = updatedPrompts[selectedIndex];
                        ctp.PlaySFX(1000, 100);
                        while (KeyAvailable) { ReadKey(true); }
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.S:

                        selectedIndex++;
                        if (selectedIndex == options.Length)
                        {
                            selectedIndex = 0;
                        }
                        prompt = updatedPrompts[selectedIndex];
                        ctp.PlaySFX(1000, 100);
                        while (KeyAvailable) { ReadKey(true); }
                        break;
                }
            }
            while (keyPressed != ConsoleKey.Enter);

            return selectedIndex;
        }

        /// <summary>
        /// Displays only a subset of all the menu options, scrolling to a new subset (if available) when needed
        /// </summary>
        /// <param name="xPos">Horizontal position of the menu (prompt and options) on the screen. Input 0 for the left side of the screen, 
        /// any other number to center the menu around that position</param>
        /// <param name="yPos">Vertical position of the prompt on the screen. Input 0 for the very top of the screen</param>
        /// <param name="optionsOffset">The verticfal distance between the menu prompt and the option list</param>
        /// <param name="lineStart">The X coordinate that indicates where each line in the menu starts (so that part of the screen can remain
        /// uneffected by the updates of the various parts). Use 0 for the left edge of the screen</param>
        /// <param name="lineEnd">The X coordinate that indicates where each line in the menu ends (so that part of the screen can remain
        /// uneffected by the updates of the various parts). Use WindowWidth for the right edge of the screen</param>
        /// <param name="numberOfDisplayedOptions">The number of ptions to be displayed per screen; counting from 1
        /// (so 3, for example translates to an options range of 0-2)</param>
        /// <returns>The index of the chosen option, after the user selects one and hits enter</returns>
        public int RunWithScrollingOptions(int xPos, int yPos, int optionsOffset, int lineStart, int lineEnd, int numberOfDisplayedOptions)
        {
            ConsoleKey keyPressed;

            int firstShownOption = 0;
            int lastShownOption;

            if (options.Length < numberOfDisplayedOptions)
            {
                lastShownOption = options.Length - 1;
            }
            else
            {
                lastShownOption = numberOfDisplayedOptions - 1;
            }

            do
            {
                SetCursorPosition(0, 0);
                DisplayPrompt(xPos, yPos, lineStart, lineEnd);
                DisplaySelectionOfOptions(xPos, optionsOffset, lineStart, lineEnd, firstShownOption, lastShownOption);

                ConsoleKeyInfo info = ReadKey(true);
                keyPressed = info.Key;

                switch (keyPressed)
                {

                    case ConsoleKey.UpArrow:
                    case ConsoleKey.NumPad8:
                    case ConsoleKey.W:

                        selectedIndex--;
                        if (selectedIndex < 0)
                        {
                            if (numberOfDisplayedOptions >= options.Length)
                            {
                                selectedIndex = options.Length - 1;
                            }
                            else
                            {
                                selectedIndex = lastShownOption;
                            }
                        }
                        else if (selectedIndex < firstShownOption)
                        {
                            firstShownOption -= numberOfDisplayedOptions;
                            lastShownOption = firstShownOption + numberOfDisplayedOptions - 1;

                            if (firstShownOption < 0)
                            {
                                firstShownOption = 0;
                                lastShownOption = numberOfDisplayedOptions - 1;
                            }
                        }

                        ctp.PlaySFX(1000, 100);
                        while (KeyAvailable) { ReadKey(true); }
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.S:

                        selectedIndex++;
                        if (selectedIndex > lastShownOption)
                        {
                            if (selectedIndex < options.Length)
                            {
                                firstShownOption += numberOfDisplayedOptions;
                                lastShownOption += numberOfDisplayedOptions;
                                if (lastShownOption >= options.Length)
                                {
                                    Clear();
                                    lastShownOption = options.Length - 1;
                                }
                            }
                            else if (selectedIndex == options.Length)
                            {
                                selectedIndex = firstShownOption;
                            }
                        }
                        ctp.PlaySFX(1000, 100);
                        while (KeyAvailable) { ReadKey(true); }
                        break;
                }
            }
            while (keyPressed != ConsoleKey.Enter);

            return selectedIndex;
        }

        /// <summary>
        /// Displays only a subset of all the menu options, scrolling to a new subset (if available) when needed. Also returns whether delete or cancel are pressed on an
        /// entry, so tha the rest of the program can deal with deleting the entry.
        /// </summary>
        /// <param name="xPos">Horizontal position of the menu (prompt and options) on the screen. Input 0 for the left side of the screen, 
        /// any other number to center the menu around that position</param>
        /// <param name="yPos">Vertical position of the prompt on the screen. Input 0 for the very top of the screen</param>
        /// <param name="optionsOffset">The verticfal distance between the menu prompt and the option list</param>
        /// <param name="lineStart">The X coordinate that indicates where each line in the menu starts (so that part of the screen can remain
        /// uneffected by the updates of the various parts). Use 0 for the left edge of the screen</param>
        /// <param name="lineEnd">The X coordinate that indicates where each line in the menu ends (so that part of the screen can remain
        /// uneffected by the updates of the various parts). Use WindowWidth for the right edge of the screen</param>
        /// <param name="numberOfDisplayedOptions">The number of ptions to be displayed per screen; counting from 1
        /// (so 3, for example translates to an options range of 0-2)</param>
        /// <returns>The index of the chosen option, after the user selects one and hits enter</returns>
        public MenuSelection RunWithDeleteEntry(int xPos, int yPos, int optionsOffset, int lineStart, int lineEnd, int numberOfDisplayedOptions)
        {
            bool cancel = false;
            MenuSelection selection;

            ConsoleKey keyPressed;

            int firstShownOption = 0;
            int lastShownOption;

            if (options.Length < numberOfDisplayedOptions)
            {
                lastShownOption = options.Length - 1;
            }
            else
            {
                lastShownOption = numberOfDisplayedOptions - 1;
            }

            do
            {
                SetCursorPosition(0, 0);
                DisplayPrompt(xPos, yPos, lineStart, lineEnd);
                DisplaySelectionOfOptions(xPos, optionsOffset, lineStart, lineEnd, firstShownOption, lastShownOption);

                ConsoleKeyInfo info = ReadKey(true);
                keyPressed = info.Key;

                switch (keyPressed)
                {
                    case ConsoleKey.Backspace:
                    case ConsoleKey.Delete:

                        cancel = true;
                        ctp.PlaySFX(1000, 200);
                        while (KeyAvailable) { ReadKey(true); }
                        break;

                    case ConsoleKey.UpArrow:
                    case ConsoleKey.NumPad8:
                    case ConsoleKey.W:

                        selectedIndex--;
                        if (selectedIndex < 0)
                        {
                            if (numberOfDisplayedOptions >= options.Length)
                            {
                                selectedIndex = options.Length - 1;
                            }
                            else
                            {
                                selectedIndex = lastShownOption;
                            }
                        }
                        else if (selectedIndex < firstShownOption)
                        {
                            firstShownOption -= numberOfDisplayedOptions;
                            lastShownOption = firstShownOption + numberOfDisplayedOptions - 1;

                            if (firstShownOption < 0)
                            {
                                firstShownOption = 0;
                                lastShownOption = numberOfDisplayedOptions - 1;
                            }
                        }

                        ctp.PlaySFX(1000, 100);
                        while (KeyAvailable) { ReadKey(true); }
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.S:

                        selectedIndex++;
                        if (selectedIndex > lastShownOption)
                        {
                            if (selectedIndex < options.Length)
                            {
                                firstShownOption += numberOfDisplayedOptions;
                                lastShownOption += numberOfDisplayedOptions;
                                if (lastShownOption >= options.Length)
                                {
                                    Clear();
                                    lastShownOption = options.Length - 1;
                                }
                            }
                            else if (selectedIndex == options.Length)
                            {
                                selectedIndex = firstShownOption;
                            }
                        }
                        ctp.PlaySFX(1000, 100);
                        while (KeyAvailable) { ReadKey(true); }
                        break;
                }
            }
            while (keyPressed != ConsoleKey.Enter && keyPressed != ConsoleKey.Backspace && keyPressed != ConsoleKey.Delete);

            selection.cancel = cancel;
            selection.selectedIndex = selectedIndex;

            return selection;
        }

        /// <summary>
        /// Updates prompt and options in an already instantiated Menu object; use for multi-line prompts
        /// </summary>
        /// <param name="prompt">The new prompt, as a string array</param>
        /// <param name="options">The new array of options</param>
        public void UpdateMenuItems(string[] prompt, string[] options)
        {
            this.prompt = prompt;
            this.options = options;
        }

        /// <summary>
        /// Updates prompt and options in an already instantiated Menu object; use for single string prompts
        /// </summary>
        /// <param name="prompt">The new prompt, as a single string</param>
        /// <param name="options">The new array of options</param>
        public void UpdateMenuItems(string prompt, string[] options)
        {
            this.prompt = new string[] { prompt };
            this.options = options;
        }

        /// <summary>
        /// Updates the prompt alone in an already instantiated Menu object; use for a multi-line prompt
        /// </summary>
        /// <param name="prompt">The new prompt, as a string array</param>
        public void UpdateMenuPrompt(string[] prompt)
        {
            this.prompt = prompt;
        }

        /// <summary>
        /// Updates the options alone in an already instantiated Menu object; use for a single string prompt
        /// </summary>
        /// <param name="prompt">The new prompt, as a single string</param>
        public void UpdateMenuPrompt(string prompt)
        {
            this.prompt = new string[] { prompt };
        }

        /// <summary>
        /// Updates the list of options in an already instantiated Menu object
        /// </summary>
        /// <param name="options">The new array of options</param>
        public void UpdateMenuOptions(string[] options)
        {
            this.options = options;
        }

        private void DisplayPrompt(int xPosition, int yPosition, int lineStart, int lineEnd)
        {
            SetCursorPosition(0, yPosition);

            foreach (string s in prompt)
            {
                int posX = xPosition - (s.Length / 2);
                for (int i = lineStart; i < posX; i++)
                {
                    SetCursorPosition(i, CursorTop);
                    Write(" ");
                }
                SetCursorPosition(posX, CursorTop);
                Write(s);
                for (int i = CursorLeft; i < lineEnd - 1; i++)
                {
                    SetCursorPosition(i, CursorTop);
                    Write(" ");
                }
                SetCursorPosition(lineEnd - 1, CursorTop);
                WriteLine();
            }
        }

        private void DisplayOptions(int xPosition, int optionsOffset)
        {
            SetCursorPosition(xPosition, CursorTop + optionsOffset);
            for (int i = 0; i < options.Length; i++)
            {
                string option = options[i];
                int posX = xPosition - (option.Length / 2) - 4;
                string prefix = " ";
                string suffix = " ";

                if (i == selectedIndex)
                {
                    prefix = ">";
                    suffix = "<";
                    ForegroundColor = ConsoleColor.Black;
                    BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    ForegroundColor = ConsoleColor.White;
                    BackgroundColor = ConsoleColor.Black;
                }

                SetCursorPosition(posX, CursorTop);
                WriteLine($"{prefix} [ {option} ] {suffix}");
            }
            CursorVisible = false;
            ResetColor();
        }

        private void DisplaySelectionOfOptions(int xPos, int optionsOffset, int lineStart, int lineEnd, int firstShownOption, int lastShownOption)
        {
            ConsoleColor textBackground;
            ConsoleColor textColor;

            SetCursorPosition(xPos, CursorTop + optionsOffset);
            if (firstShownOption > 0)
            {
                WriteLine("^");
            }
            else
            {
                WriteLine(" ");
            }

            for (int i = firstShownOption; i <= lastShownOption; i++)
            {
                string option = options[i];
                int posX = xPos - (option.Length / 2) - 4;
                string prefix = " ";
                string suffix = " ";

                if (i == selectedIndex)
                {
                    prefix = ">";
                    suffix = "<";
                    textColor = ConsoleColor.Black;
                    textBackground = ConsoleColor.White;
                }
                else
                {
                    textColor = ConsoleColor.White;
                    textBackground = ConsoleColor.Black;
                }

                for (int j = lineStart; j < posX; j++)
                {
                    SetCursorPosition(j, CursorTop);
                    Write(" ");
                }
                ForegroundColor = textColor;
                BackgroundColor = textBackground;
                Write($"{prefix} [ {option} ] {suffix}");
                ResetColor();
                for (int j = CursorLeft; j < lineEnd - 1; j++)
                {
                    SetCursorPosition(j, CursorTop);
                    Write(" ");
                }
                SetCursorPosition(lineEnd - 1, CursorTop);
                WriteLine();
            }
            SetCursorPosition(WindowWidth / 2, CursorTop);
            if (lastShownOption < options.Length - 1)
            {
                WriteLine("V");
            }
            else
            {
                WriteLine(" ");
            }
            CursorVisible = false;
        }
    }

    public struct MenuSelection
    {
        public bool cancel;
        public int selectedIndex;
    }
}
