using System;
using System.Runtime.InteropServices;
using static System.Console;

namespace HeistGame
{
    class ConsoleHelper
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Sets the console to the defined parameters.
        /// </summary>
        /// <param name="title">The title that appears in the console window bar.</param>
        /// <param name="width">The console window's width.</param>
        /// <param name="height">The console window's height.</param>
        /// <param name="blockClosing">Set to true to disable the close button.</param>
        /// <param name="blockMinimize">Set to true to disable the minimize button.</param>
        /// <param name="blockMaximize">Set to true to disable the maximize button. Note that it won't prevent automatic mazimization
        /// if the window is dragged to the top of the screen.</param>
        /// <param name="blockResize">Set to true to prevent manual resizing of the window via dragging the edges.</param>
        /// <param name="blockScrolling">Set to true to prevent manual scrolling. Note that the window will automatically scroll anyway if the displayed text
        /// is larger than the window size.</param>
        public static void SetConsole(string title, int width, int height, bool blockClosing, bool blockMinimize, bool blockMaximize, bool blockResize, bool blockScrolling)
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                if (blockClosing) { DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND); }
                if (blockMinimize) { DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND); }
                if (blockMaximize) { DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND); }
                if (blockResize) { DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND); }
            }

            Title = title;

            try
            {
                SetWindowSize(width, height);
                if (blockScrolling) { SetBufferSize(width, height); }
            }
            catch (ArgumentOutOfRangeException)
            {
                DisplayConsoleSizeWarning();
            }
        }

        private static void DisplayConsoleSizeWarning()
        {
            WriteLine("Error setting the preferred console size.");
            WriteLine("You can continue using the program, but glitches may occour, and it will probably not be displayed correctly.");
            WriteLine("To fix this error, please try changing the character size of the Console.");

            WriteLine("\n\nPress any key to continue...");
            ReadKey(true);
        }
    }
}
