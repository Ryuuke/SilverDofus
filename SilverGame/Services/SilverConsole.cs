using System;
using System.Timers;

namespace SilverGame.Services
{
    static class SilverConsole
    {
        private static readonly Timer Timer = new Timer(45);
        private static AnimationState _state;

        public static void LoadTimer()
        {
            Timer.Elapsed += TimerOnElapsed;
        }

        public static void Welcome()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("  ________     __     __    ___         ___   __________    _________  ");
            Console.WriteLine(" |   _____|   |  |   |  |   \\  \\       /  /  |   _______|  |  |      |");
            Console.WriteLine(" |  |         |  |   |  |    \\  \\     /  /   |  |          |  |      | ");
            Console.WriteLine(" |  |_____    |  |   |  |     \\  \\   /  /    |  |______    |  |______|");
            Console.WriteLine(" |_____   |   |  |   |  |      \\  \\ /  /     |   ______|   |  |______  ");
            Console.WriteLine("       |  |   |  |   |  |       \\     /      |  |          |  |   |  | Dofus ");
            Console.WriteLine("  _____|  |   |  |   |  |_____   \\   /       |  |_______   |  |   |  |  By");
            Console.WriteLine(" |________|   |__|   |________|   \\_/        |__________|  |__|   |__| Ryuuke");
            Console.WriteLine("    ________________________________________________________________  ");

            Console.WriteLine(Environment.NewLine);
            
            Console.ResetColor();
        }

        public static void WriteLine(string text, ConsoleColor color = ConsoleColor.Gray, bool line = true)
        {
            Console.ForegroundColor = color;

            if (line)
                Console.WriteLine(text);
            else
                Console.Write(text);

            Console.ResetColor();
        }

        private static int _startX;
        private static int _startY;

        public static void StartLoad(string message)
        {
            Timer.Enabled = true;

            WriteLine(string.Format("{0} [-]", message), ConsoleColor.DarkGreen, false);

            _startX = Console.CursorLeft;
            _startY = Console.CursorTop;
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var consoleX = Console.CursorLeft - 2;
            var consoleY = Console.CursorTop;

            Console.SetCursorPosition(consoleX, consoleY);

            switch (_state)
            {
                case AnimationState.State1:
                    _state = AnimationState.State2;
                    WriteLine("|", ConsoleColor.DarkGreen, false);
                    break;
                case AnimationState.State2:
                    _state = AnimationState.State3;
                    WriteLine("/", ConsoleColor.DarkGreen, false);
                    break;
                case AnimationState.State3:
                    _state = AnimationState.State4;
                    WriteLine("-", ConsoleColor.DarkGreen, false);
                    break;
                case AnimationState.State4:
                    _state = AnimationState.State1;
                    WriteLine(@"\", ConsoleColor.DarkGreen, false);
                    break;
            }

            Console.SetCursorPosition(_startX, _startY);
        }

        public static void StopLoad()
        {
            Timer.Enabled = false;

            var consoleX = Console.CursorLeft - 3;
            var consoleY = Console.CursorTop;

            Console.SetCursorPosition(consoleX, consoleY);

            WriteLine("Completed !", ConsoleColor.DarkGreen);
        }

        private enum AnimationState
        {
            State1,
            State2,
            State3,
            State4
        }
    }
}
