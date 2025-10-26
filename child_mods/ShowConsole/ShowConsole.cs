using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace ShowConsole
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        static bool _consoleInitialized;
        static bool _isVisible = true;

        void Awake()
        {
            Debug.Log("ShowConsole Mod Loaded!!!");
        }

        void OnEnable()
        {
            if (_consoleInitialized) return;

            try
            {
                InitializeConsole();
                SetupUtf8Encoding();
                _consoleInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to init console: " + ex.Message);
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Debug.Log("Original Author of ShowConsoleMod: InitLoader");
            Console.ForegroundColor = ConsoleColor.White;
        }

        void OnDisable()
        {
            Debug.Log("Please restart the game to hide the console after disabling the mod.");
        }

        static void InitializeConsole()
        {
            Application.logMessageReceivedThreaded += HandleUnityLog;
        }

        static void SetupUtf8Encoding()
        {
            if (GetConsoleWindow() == IntPtr.Zero)
                AllocConsole();

            Console.Title = "Duckov Console";
            var utf8 = new UTF8Encoding(false);
            Console.InputEncoding = utf8;
            Console.OutputEncoding = utf8;
            SetConsoleCP(65001);
            SetConsoleOutputCP(65001);

            Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), utf8) { AutoFlush = true });
            Console.SetError(new StreamWriter(Console.OpenStandardError(), utf8) { AutoFlush = true });

            ShowWindow(GetConsoleWindow(), SW_SHOW);
            _isVisible = true;
        }

        static void HandleUnityLog(string condition, string stackTrace, LogType type)
        {
            try
            {
                Console.ForegroundColor = type switch
                {
                    LogType.Error => ConsoleColor.Red,
                    LogType.Exception => ConsoleColor.Red,
                    LogType.Warning => ConsoleColor.Yellow,
                    _ => ConsoleColor.White
                };

                Console.WriteLine(condition);
                if (!string.IsNullOrEmpty(stackTrace))
                    Console.WriteLine(stackTrace);
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch { }
        }

        public static void Toggle()
        {
            if (_isVisible)
                Hide();
            else
                Show();
        }

        public static void Show()
        {
            ShowWindow(GetConsoleWindow(), SW_SHOW);
            _isVisible = true;
        }

        public static void Hide()
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
            _isVisible = false;
        }

        public static void Clear() => system("CLS");

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("kernel32.dll")] static extern bool AllocConsole();
        [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")] static extern bool SetConsoleOutputCP(uint codePageID);
        [DllImport("kernel32.dll")] static extern bool SetConsoleCP(uint codePageID);
        [DllImport("msvcrt.dll")] public static extern int system(string cmd);
    }
}
