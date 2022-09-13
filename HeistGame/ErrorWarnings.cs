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
