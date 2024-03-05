using System;
using System.IO;

namespace Game_Brodyaga
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int mapNumber = 1;
            while (true)
            {
                Game(mapNumber);
                Console.WriteLine("\n\n1. Чтобы перезапустить игру нажмите R" +
                    "\n2. Чтобы выбрать другой уровень нажмите S" +
                    "\n3. Чтобы выйти из игры нажмите Esc!");
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.R)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
                else if (key.Key == ConsoleKey.Escape) break;
                else if (key.Key == ConsoleKey.S)
                {
                    Console.Clear();
                    Console.Write("Введите номер уровня (от 1 до 2): ");
                    mapNumber = Convert.ToInt32(Console.ReadLine());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();
                }
            }

        }

        private static char[,] ReadMap(string path)
        {
            string[] file = File.ReadAllLines(path);

            char[,] map = new char[GetMaxValueOfLines(file), file.Length];

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = file[i][j];
                }
            }

            return map;
        }

        private static int GetMaxValueOfLines(string[] lines)
        {
            int maxLength = lines[0].Length;

            foreach (var line in lines)
            {
                if (line.Length > maxLength)
                    maxLength = line.Length;
            }

            return maxLength;
        }

        private static void DrawBar(int value, int maxValue, ConsoleColor color, string whatIsIt)
        {
            string bar = null;

            for (int i = 0; i < value; i++)
            {
                bar += " ";
            }
            Console.WriteLine(whatIsIt);
            Console.Write('[');
            Console.BackgroundColor = color;
            Console.Write(bar);
            Console.BackgroundColor = ConsoleColor.Black;

            bar = " ";

            for (int i = value; i < maxValue; i++)
            {
                bar += " ";
            }

            Console.Write($"{bar}] {value}/{maxValue}\n");

        }

        public static void Game(int mapNumber = 1)
        {
            Console.Title = "Бродилка";
            bool isOpen = true;
            Console.CursorVisible = false;

            char[,] map = ReadMap($@"C:\Projects\Game_Brodyaga\Game_Brodyaga\bin\Debug\map{mapNumber}.txt");

            Random rnd = new Random();

            int userX = 0, userY = 0, score = 0, countOfX = rnd.Next(1, 11);

            for (int i = 0; i < countOfX; i++)
            {
                int row = rnd.Next(1, map.GetLength(0));
                int col = rnd.Next(1, map.GetLength(1));


                if (map[row, col] == ' ')
                {
                    map[row, col] = 'X';
                }
                else
                {
                    i--;
                }
            }

            while (map[userX, userY] == '#')
            {
                userX = rnd.Next(1, map.GetLength(0));
                userY = rnd.Next(1, map.GetLength(1));
            }


            while (isOpen)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Вы участвуете в игре вам необходимо собрать все клады (X) " +
                "пройдя по лабиринту!");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Уровень № {mapNumber}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, 22);
                DrawBar(score, countOfX, ConsoleColor.Yellow, "Количество собранных кладов");

                Console.SetCursorPosition(0, 3);
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        if (map[i, j] == 'X' || map[i, j] == 'O')
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(map[i, j]);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                            Console.Write(map[i, j]);
                    }
                    Console.WriteLine();
                }

                Console.SetCursorPosition(userY, userX + 3);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write('@');
                Console.ForegroundColor = ConsoleColor.White;
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (map[userX - 1, userY] != '#') userX--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (map[userX + 1, userY] != '#') userX++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (map[userX, userY - 1] != '#') userY--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (map[userX, userY + 1] != '#') userY++;
                        break;

                }

                if (map[userX, userY] == 'X')
                {
                    score++;
                    map[userX, userY] = 'O';

                }

                if (score == (countOfX))
                {
                    isOpen = false;
                }

                Console.Clear();
            }

            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("__  __   ____    __  __        _       __    ____    _   __\r\n\\ \\/ /  / __ \\  / / / /       | |     / /   /  _/   / | / /\r\n \\  /  / / / / / / / /        | | /| / /    / /    /  |/ / \r\n / /  / /_/ / / /_/ /         | |/ |/ /   _/ /    / /|  /  \r\n/_/   \\____/  \\____/          |__/|__/   /___/   /_/ |_/   \r\n                                                           ");
        }
    }
}

