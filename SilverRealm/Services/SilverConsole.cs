using System;

namespace SilverRealm.Services
{
    class SilverConsole
    {
        public static void Welcome()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("  ________     __     __    ___         ___   __________    _________  ");
            Console.WriteLine(" |   _____|   |  |   |  |   \\  \\       /  /  |   _______|  |  |      |");
            Console.WriteLine(" |  |         |  |   |  |    \\  \\     /  /   |  |          |  |      | ");
            Console.WriteLine(" |  |_____    |  |   |  |     \\  \\   /  /    |  |______    |  |______|");
            Console.WriteLine(" |_____   |   |  |   |  |      \\  \\ /  /     |   ______|   |  |______  ");
            Console.WriteLine("       |  |   |  |   |  |       \\     /      |  |          |  |   |  | Dofus ");
            Console.WriteLine("  _____|  |   |  |   |  |____    \\   /       |  |_______   |  |   |  | By");
            Console.WriteLine(" |________|   |__|   |________|   \\_/        |__________|  |__|   |__|  Ryuuke");
            Console.WriteLine("    ________________________________________________________________  ");

            Console.WriteLine(Environment.NewLine);

            Console.ResetColor();
        }

        public static void WriteLine<T>(T text, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;

            Console.WriteLine(text);

            Console.ResetColor();
        }
    }
}
