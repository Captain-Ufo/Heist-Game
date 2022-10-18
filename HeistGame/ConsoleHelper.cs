/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.Json;
using static System.Console;

namespace HeistGame
{
    class ConsoleHelper
    {
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_RESTORE = 0xF120;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(HandleRef hWnd, out Rect lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public Coord dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
            public string FaceName;
        }

        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(StdHandle index);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern Int32 SetCurrentConsoleFontEx(
          IntPtr ConsoleOutput,
          bool MaximumWindow,
          ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);

        private enum StdHandle
        {
            OutputHandle = -11
        }

        /// <summary>
        /// Automatically sets the console reading the config.ini file
        /// </summary>
        public static void SetConsole()
        {
            SettingsData settings = GetSettingsFromConfig();

            ConfigureConsole(settings.Name, settings.Font, settings.FontHeight, settings.FontWidth, settings.MaximizeOnStart,
                             settings.ConsoleWidth, settings.ConsoleHeight, settings.CenterWindow, false, false, true, true,
                             true);
        }

        /// <summary>
        /// Sets the console to the defined parameters.
        /// </summary>
        /// <param name="title">The title that appears in the console window bar.</param>
        /// <param name="width">The console window's width.</param>
        /// <param name="height">The console window's height.</param>
        /// <param name="center">Set to True to center the window on screen</param>
        /// <param name="blockClosing">Set to true to disable the close button.</param>
        /// <param name="blockMinimize">Set to true to disable the minimize button.</param>
        /// <param name="blockMaximize">Set to true to disable the maximize button. Note that it won't prevent automatic mazimization
        /// if the window is dragged to the top of the screen.</param>
        /// <param name="blockResize">Set to true to prevent manual resizing of the window via dragging the edges.</param>
        /// <param name="blockScrolling">Set to true to prevent manual scrolling. Note that the window will automatically scroll anyway if the displayed text
        /// is larger than the window size.</param>
        public static void ConfigureConsole(string title, string fontName, short fontHeight, short fontWidth, bool maximize, int width, int height, bool center, bool blockClosing, bool blockMinimize, bool blockMaximize, bool blockResize, bool blockScrolling)
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            OutputEncoding = System.Text.Encoding.Unicode;

            if (handle == IntPtr.Zero)
            {
                return;
            }

            Title = title;

            if (!maximize && (width < 70 || height < 60))
            {
                SetBufferSize(WindowWidth, WindowHeight);
                ErrorWarnings.InvalidWindowSize();
                width = 70;
                height = 60;
            }

            try
            {
                SetConsoleFont(fontName, fontWidth, fontHeight);
            }
            catch (Exception e)
            {
                ErrorWarnings.FontSettingError(e);
            }

            if (maximize)
            {
                SetWindowSize(LargestWindowWidth, LargestWindowHeight);
                AnchorWindow();
                
            }
            else
            {
                try
                {
                    SetWindowSize(width, height);
                }
                catch (ArgumentOutOfRangeException)
                {
                    ErrorWarnings.ConsoleSizeError();
                }
            }

            if (blockScrolling) 
            {
                if (maximize)
                {
                    SetBufferSize(LargestWindowWidth, LargestWindowHeight);
                }
                else
                {
                    SetBufferSize(width, height);
                }
            }
            if (center && !maximize) { CenterWindow(); }

            if (blockClosing) { DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND); }
            if (blockMinimize) { DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND); }
            if (blockResize) { DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND); }
            if (blockMaximize) 
            { 
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_RESTORE, MF_BYCOMMAND);
            }
        }

        public static void SetConsoleFont(string fontName, short w, short h)
        {
            if ((fontName == string.Empty) || (fontName == " "))
            {
                ErrorWarnings.InvalidFontName();
                return;
            }

            if (h < 6  || w < 6)
            {
                ErrorWarnings.InvalidFontSize();
                h = 14;
                w = 13;
            }

            CONSOLE_FONT_INFO_EX ConsoleFontInfo = new CONSOLE_FONT_INFO_EX()
            {
                FaceName = fontName,
                dwFontSize = new Coord(w, h)
            };

            ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);
            _ = SetCurrentConsoleFontEx(GetStdHandle(StdHandle.OutputHandle), false, ref ConsoleFontInfo);
        }

        private static Size GetScreenSize() => new Size(GetSystemMetrics(0), GetSystemMetrics(1));

        private static Size GetWindowSize(IntPtr window)
        {
            if (!GetWindowRect(new HandleRef(null, window), out Rect rect))
                throw new Exception("Unable to get window rect!");

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            return new Size(width, height);
        }

        private static void CenterWindow()
        {
            IntPtr window = GetConsoleWindow();

            if (window == IntPtr.Zero)
            {
                throw new Exception("Couldn't find a window to center!");
            }

            Size screenSize = GetScreenSize();
            Size windowSize = GetWindowSize(window);

            int xPos = (screenSize.Width - windowSize.Width) / 2;
            int yPos = (screenSize.Height - windowSize.Height) / 2;
            SetWindowPos(window, IntPtr.Zero, xPos, yPos, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
        }

        private static void AnchorWindow()
        {
            IntPtr window = GetConsoleWindow();

            if (window == IntPtr.Zero)
            {
                throw new Exception("Couldn't find a window to center!");
            }

            Size screenSize = GetScreenSize();
            Size windowSize = GetWindowSize(window);

            int xPos = (screenSize.Width - windowSize.Width) / 2;
            int yPos = 0;
            SetWindowPos(window, IntPtr.Zero, xPos, yPos, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
        }

        static SettingsData GetSettingsFromConfig()
        {
            string configFilePath = Directory.GetCurrentDirectory() + "/Config";
            string configFileName = configFilePath + "/Config.ini";

            if (!Directory.Exists(configFilePath))
            {
                return CreateDefaultConfig(configFilePath);
            }
            else if (!File.Exists(configFileName))
            {
                return CreateDefaultConfig(configFilePath);
            }
            else
            {
                string settingsFile = File.ReadAllText(configFileName);
                SettingsData settings = JsonSerializer.Deserialize<SettingsData>(settingsFile);
                return settings;
            }
        }

        static SettingsData CreateDefaultConfig(string path)
        {
            ErrorWarnings.MissingConfig(path);

            SettingsData data = new SettingsData();
            data.Name = "Heist!";
            data.Font = "Consolas";
            data.FontHeight = 16;
            data.FontWidth = 15;
            data.MaximizeOnStart = true;
            data.ConsoleWidth = 126;
            data.ConsoleHeight = 60;
            data.CenterWindow = false;

            string configData = JsonSerializer.Serialize(data);
            string dataFileName = "/Config.ini";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = path + dataFileName;
            File.WriteAllText(filePath, configData);

            return data;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        private struct Size
        {
            public int Width { get; set; }
            public int Height { get; set; }

            public Size(int width, int height)
            {
                Width = width;
                Height = height;
            }
        }

        private class SettingsData
        {
            public string Name { get; set; }
            public string Font { get; set; }
            public short FontHeight { get; set; }
            public short FontWidth { get; set; }
            public bool MaximizeOnStart { get; set; }
            public int ConsoleWidth { get; set; }
            public int ConsoleHeight { get; set; }
            public bool CenterWindow { get; set; }

            public SettingsData() { }
        }
    }
}
