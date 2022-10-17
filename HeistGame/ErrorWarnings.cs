/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace HeistGame
{
    /// <summary>
    /// A collection of custom errors, specific for the game, for ease of use. None of these throw exceptions
    /// </summary>
    class ErrorWarnings
    {

        public static void InvalidFontName()
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: invalid font name *!!";
            DisplayWarning(warning, -3);
            ResetColor();
            warning = "Unspecified name for the font in Config.ini.";
            DisplayWarning(warning, -2);
            ForegroundColor = ConsoleColor.Red;
            warning = "This is a critical error. The program will be terminated.";
            DisplayWarning(warning, -1);
            ResetColor();
            warning = "Please define a valid font name in Config.ini and restart the program.";
            DisplayWarning(warning);
            SetCursorPosition(0, WindowHeight - 1);
            WriteLine("\n\nPress any key to quit...");
            ReadKey(true);

            Environment.Exit(0);
        }

        public static void FontSettingError(Exception e)
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: could not set the specified Font *!!";
            DisplayWarning(warning, -4);
            ResetColor();
            ForegroundColor = ConsoleColor.Red;
            warning = "This is a critical error. The program will be terminated. The error was:";
            DisplayWarning(warning, -3);
            DisplayWarning(e.Message, -2);
            ResetColor();
            warning = "Please make sure that the font specified in Config.in is installed, or define a different, valid, TTF font";
            DisplayWarning(warning, -1);
            warning = "and restart the program.";
            DisplayWarning(warning);
            SetCursorPosition(0, WindowHeight - 1);
            WriteLine("\n\nPress any key to quit...");
            ReadKey(true);

            Environment.Exit(0);
        }

        public static void ConsoleSizeError()
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: could not set the specified Console size *!!";
            DisplayWarning(warning, -4);
            ResetColor();
            ForegroundColor = ConsoleColor.Red;
            warning = "This is a critical error. The program will be terminated.";
            DisplayWarning(warning, -3);
            ResetColor();
            warning = "To fix this error, please try changing the size of the Console or the character size in Config.ini,";
            DisplayWarning(warning, -1);
            warning = "and restart the program.";
            DisplayWarning(warning);
            SetCursorPosition(0, WindowHeight - 1);
            WriteLine("\n\nPress any key to quit...");
            ReadKey(true);

            Environment.Exit(0);
        }

        public static void InvalidFontSize()
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: invalid font size *!!";
            DisplayWarning(warning, -3);
            ResetColor();
            warning = "The console font cannot be size 6 or less.";
            DisplayWarning(warning, -2);
            ForegroundColor = ConsoleColor.Red;
            warning = "The program will attempt to apply the default font size. If you prefer a different size,";
            DisplayWarning(warning, -1);
            ResetColor();
            warning = "please define a valid font size in Config.ini and restart the program.";
            DisplayWarning(warning);
            SetCursorPosition(0, WindowHeight - 1);
            WriteLine("\n\nPress any key to quit...");
            ReadKey(true);
        }

        public static void InvalidWindowSize()
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: invalid Window size *!!";
            DisplayWarning(warning, -3);
            ResetColor();
            warning = "The window cannot be smaller than 70 columns and 60 rows.";
            DisplayWarning(warning, -2);
            ForegroundColor = ConsoleColor.Red;
            warning = "The program will continue and attempt to apply the minimum window size. If you prefer a different size,";
            DisplayWarning(warning, -1);
            ResetColor();
            warning = "please define a valid size in Config.ini and restart the program.";
            DisplayWarning(warning);
            SetCursorPosition(0, WindowHeight - 1);
            WriteLine("\n\nPress any key to quit...");
            ReadKey(true);
        }

        public static void MissingConfig(string path)
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: could not find Config.ini *!!";
            DisplayWarning(warning, -1);
            ResetColor();
            warning = $"A new Config.ini with default values will be created in {path}.";
            DisplayWarning(warning, 0);
            SetCursorPosition(0, WindowHeight - 1);
            WriteLine("\n\nPress any key to continue...");
            ReadKey(true);
        }

        public static void InvalidCampaignFile(string campaignPath)
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: invalid, misnamed or non existent campaign config file *!!";
            DisplayWarning(warning, -3);
            warning = $"{campaignPath}";
            DisplayWarning(warning, -2);
            ResetColor();
            warning = "If this is a custom campaign, check the manual for instructions on how to create it correctly.";
            DisplayWarning(warning);
            SetCursorPosition(0, WindowHeight - 1);
            Write("Press any key to return to main menu...");
            ReadKey(true);
        }

        public static void IncorrectCampaignData(string campaignPath, Exception e)
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: cannot extract campaign data from the campaign config file *!!";
            DisplayWarning(warning, -4);
            warning = $"{campaignPath}";
            DisplayWarning(warning, -3);
            ResetColor();
            warning = "The campaign file contains missing or incorrectly formatted data.";
            DisplayWarning(warning, -1);
            warning = "If this is a custom campaign or you edited an existing campaign, please check the manual for instructions on how to create the config file correctly.";
            DisplayWarning(warning);
            DisplayWarning(e.Message);
            SetCursorPosition(0, WindowHeight - 1);
            Write("Press any key to return to main menu...");
            ReadKey(true);
        }

        public static void MissingLevelFile(string levelFile)
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = $"!!* ERROR: invalid, misnamed or non existent level file: {levelFile} *!!";
            DisplayWarning(warning, -2);
            ResetColor();
            warning = "If this is a custom campaign, check that all the level names in the config file match the level files in the folder.";
            DisplayWarning(warning);
            SetCursorPosition(0, WindowHeight - 1);
            Write("Press any key to return to main menu...");
            ReadKey(true);
        }

        public static void MissingCampaignFolder()
        {
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: cannot find the correct campaign folder *!!";
            DisplayWarning(warning, -5);
            ResetColor();
            warning = "The campaign might have been deleted, or its folder renamed.";
            DisplayWarning(warning, -4);
            warning = "Please check that the campaign folder is in the correct place, or delete the save file";
            DisplayWarning(warning, -3);
            SetCursorPosition(0, WindowHeight - 1);
            Write("Press any key to return to main menu...");
            ReadKey(true);
        }

        public static void IncorrectLevelData(string levelfile, Exception e)
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = "!!* ERROR: cannot extract Level data from the level config file *!!";
            DisplayWarning(warning, -6);
            warning = $"{levelfile}";
            DisplayWarning(warning, -5);
            ResetColor();
            warning = "The campaign file contains missing or incorrectly formatted data.";
            DisplayWarning(warning, -3);
            warning = "If this is a custom mission or you edited an existing mission, please check the manual for instructions on how to create the config file correctly.";
            DisplayWarning(warning, -2);
            DisplayWarning(e.Message);
            SetCursorPosition(0, WindowHeight - 1);
            Write("Press any key to return to main menu...");
            ReadKey(true);
        }

        public static void MissingPlayerStartingPoint(string levelFile)
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = $"!!* ERROR: level {levelFile} lacks a player start point *!!";
            DisplayWarning(warning, -3);
            ResetColor();
            warning = "If this is a custom or edited level, please check that the map contains the correct marker.";
            DisplayWarning(warning, -1);
            warning = "Please refer to the manual for more informations.";
            DisplayWarning(warning);
            SetCursorPosition(0, WindowHeight - 1);
            Write("Press any key to return to main menu...");
            ReadKey(true);
        }

        public static void MissingExit(string levelFile)
        {
            Clear();
            ForegroundColor = ConsoleColor.Red;
            string warning = $"!!* ERROR: level {levelFile} lacks an exit *!!";
            DisplayWarning(warning, -3);
            ResetColor();
            warning = "If this is a custom or edited level, please check that the map contains the correct marker.";
            DisplayWarning(warning, -1);
            warning = "Please refer to the manual for more informations.";
            DisplayWarning(warning);
            SetCursorPosition(0, WindowHeight - 1);
            Write("Press any key to return to main menu...");
            ReadKey(true);
        }

        private static void DisplayWarning(string warning, int initialYPosition = 0)
        {
            SetCursorPosition((WindowWidth / 2) - (warning.Length / 2), WindowHeight / 2 + initialYPosition);
            WriteLine(warning);
        }
    }
}
