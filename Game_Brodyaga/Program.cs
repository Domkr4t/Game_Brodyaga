﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Game_Brodyaga
{
    internal class Program
    {
        private static bool isTimeUp = false;
        public static int timeOfGame = 50;
        private static CancellationTokenSource timerCancellationTokenSource;
        static void Main(string[] args)
        {
            Console.Title = "Бродилка";
            Console.CursorVisible = false;
            Console.WindowHeight = 30;
            Console.WindowWidth = 140;

            int mapNumber = 1, difficult = 1;
            char player = '@';
            
            StartScreen();
            while (true)
            {
                timerCancellationTokenSource = new CancellationTokenSource();
                Task timerTask = StartTimer(timeOfGame, timerCancellationTokenSource.Token);

                Game(player, mapNumber, difficult);
                Console.WriteLine($"\n\n1. Чтобы перезапустить игру нажмите R" +
                    $"\n2. Чтобы изменить карту и сложность нажмите S, сейчас выбрана карта №{mapNumber} и сложность {difficult}" +
                    $"\n3. Чтобы изменить иконку персонажа, нажмите P" +
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
                    Console.Write($"Только что вы играли на {mapNumber} карте\n" +
                        $"Введите номер уровня (от 1 до 2): ");
                    mapNumber = Convert.ToInt32(Console.ReadLine());
                    Console.Clear();
                    Console.Write($"Только что вы играли с уровнем сложности {difficult}\n" +
                        "Введите уровень сложности (от 1 до 3): ");
                    difficult = Convert.ToInt32(Console.ReadLine());
                    switch (difficult)
                    {
                        case 1:
                            timeOfGame = 50;
                            break;
                        case 2:
                            timeOfGame = 30;
                            break;
                        case 3:
                            timeOfGame = 20;
                            break;
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();
                }
                else if (key.Key == ConsoleKey.P)
                {
                    Console.Clear();
                    Console.Write("Введите символ, который будет отображаться персонажем (необходим один символ): ");
                    try
                    {
                        player = Console.ReadLine()[0];
                        if (player == ' ') player = '@';
                    }
                    catch 
                    {
                        Console.Clear();
                        Console.WriteLine("Вы ввели пустое значение!!! ");
                        Console.ReadKey();
                        Console.Clear();
                        continue;
                    }
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
                    map[i, j] = file[j][i];
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

            bar = "";

            for (int i = value; i < maxValue; i++)
            {
                bar += " ";
            }

            Console.Write($"{bar}] {value}/{maxValue}\n");

        }

        public static void StartScreen()
        {
            Console.Write("   ______    ___     __  ___    ______           ____     ____    ____     ____ __  __    ___    ______    ___ \r\n  / ____/   /   |   /  |/  /   / ____/          / __ )   / __ \\  / __ \\   / __ \\\\ \\/ /   /   |  / ____/   /   |\r\n / / __    / /| |  / /|_/ /   / __/            / __  |  / /_/ / / / / /  / / / / \\  /   / /| | / / __    / /| |\r\n/ /_/ /   / ___ | / /  / /   / /___           / /_/ /  / _, _/ / /_/ /  / /_/ /  / /   / ___ |/ /_/ /   / ___ |\r\n\\____/   /_/  |_|/_/  /_/   /_____/          /_____/  /_/ |_|  \\____/  /_____/  /_/   /_/  |_|\\____/   /_/  |_|\r\n                                                                                                               \r\n");
            Console.SetCursorPosition(0, 6);
            Console.WriteLine("Вы участвуете в игре вам необходимо собрать все клады (X) " +
                "пройдя по лабиринту! У вас есть здоровье и мана, если здоровье опуститься ниже 0, " +
                "то вы проиграете, с помощью маны можно ломать стены, кроме внешних, " +
                "при этом ваше здоровье не падает. Удачи!");
            Console.ReadKey();
            Console.Clear();
        }

        public static async Task StartTimer(int seconds, CancellationToken cancellationToken)
        {

            while (seconds > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                Console.SetCursorPosition(0, 3);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"Оставшееся время: {seconds / 60}:{seconds % 60:D2}");
                Console.ForegroundColor = ConsoleColor.White;
                await Task.Delay(1000, cancellationToken);
                seconds--;
            }

            isTimeUp = true;
        }

        public static bool Game(char player, int mapNumber = 1, int difficult = 1)
        {
            char[,] map = ReadMap($@"C:\Projects\Test\Test\bin\Debug\map{mapNumber}.txt");

            Random rnd = new Random();

            int userX = 0, 
                userY = 0,
                startHealth = 3,
                userHealth = 0,
                startMana = 2,
                userMana = 0,
                enemyX = 0,
                enemyY = 0,
                moveEnemy,
                score = 0, 
                countOfX = rnd.Next(1, 11);

            bool isWin;

            switch (difficult)
            {
                case 1:
                    userHealth = 3;
                    userMana = 2;
                    break;
                case 2:
                    userHealth = 2;
                    userMana = 1;
                    break;
                case 3:
                    userHealth = 1;
                    userMana = 0;
                    break;
            }

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

            while (map[enemyX, enemyY] == '#')
            {
                enemyX = rnd.Next(1, map.GetLength(0));
                enemyY = rnd.Next(1, map.GetLength(1));
            }

            while (true)
            {
                Console.SetCursorPosition(0, 1);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Уровень № {mapNumber}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, 2);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Сложность № {difficult}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, 25);
                DrawBar(score, countOfX, ConsoleColor.Yellow, "Количество собранных кладов");
                Console.SetCursorPosition(0, 23);
                DrawBar(userMana, startMana, ConsoleColor.Blue, "Ваша мана");
                Console.SetCursorPosition(0, 21);
                DrawBar(userHealth, startHealth, ConsoleColor.Green, "Ваше здоровье");

                Console.SetCursorPosition(0, 5);
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

                Console.SetCursorPosition(userY, userX + 5);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(player);
                Console.SetCursorPosition(enemyY, enemyX + 5);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write('&');
                Console.ForegroundColor = ConsoleColor.White;
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (map[userX - 1, userY] != '#') userX--;
                        else if (map[userX - 1, userY] == '#')
                        {
                            bool isEdgeWallUp = userX - 1 < 1;

                            if (!isEdgeWallUp && userMana != 0)
                            {

                                map[userX - 1, userY] = ' ';
                                userMana--;

                            }
                            else userHealth--;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (map[userX + 1, userY] != '#') userX++;
                        else if (map[userX + 1, userY] == '#')
                        {
                            bool isEdgeWallDown = userX + 1 == map.GetLength(0) - 1;

                            if (!isEdgeWallDown && userMana != 0)
                            {

                                map[userX + 1, userY] = ' ';
                                userMana--;

                            }
                            else userHealth--;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (map[userX, userY - 1] != '#') userY--;
                        else if (map[userX, userY - 1] == '#')
                        {
                            bool isEdgeWallLeft = userY - 1 < 1;

                            if (!isEdgeWallLeft && userMana != 0)
                            {


                                map[userX, userY - 1] = ' ';
                                userMana--;

                            }
                            else userHealth--;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (map[userX, userY + 1] != '#') userY++;
                        else if (map[userX, userY + 1] == '#')
                        {
                            bool isEdgeWallRight = userY + 1 == map.GetLength(1) - 1;

                            if (!isEdgeWallRight && userMana != 0)
                            {

                                map[userX, userY + 1] = ' ';
                                userMana--;

                            }
                            else userHealth--;
                        }
                        break;
                    
                }

                moveEnemy = rnd.Next(1, 5);

                switch (moveEnemy)
                {
                    case 1: 
                        if (map[enemyX - 1, enemyY] != '#') enemyX--;
                        break;
                    case 2: 
                        if (map[enemyX + 1, enemyY] != '#') enemyX++;
                        break;
                    case 3: 
                        if (map[enemyX, enemyY - 1] != '#') enemyY--;
                        break;
                    case 4:
                        if (map[enemyX, enemyY + 1] != '#') enemyY++;
                        break;
                }

                if (map[userX, userY] == 'X')
                {
                    score++;
                    map[userX, userY] = 'O';
                }

                if (score == (countOfX))
                {
                    isWin = true;
                    timerCancellationTokenSource.Cancel();
                    break;
                    
                }
                else if (userHealth == 0 || isTimeUp)
                {
                    isWin = false;
                    timerCancellationTokenSource.Cancel();
                    break;
                }
                else if(userX == enemyX & userY == enemyY)
                {
                    isWin = false;
                    timerCancellationTokenSource.Cancel();
                    break;
                }


                Console.Clear();
            }
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            if (isWin == true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("__  __   ____    __  __        _       __    ____    _   __\r\n\\ \\/ /  / __ \\  / / / /       | |     / /   /  _/   / | / /\r\n \\  /  / / / / / / / /        | | /| / /    / /    /  |/ / \r\n / /  / /_/ / / /_/ /         | |/ |/ /   _/ /    / /|  /  \r\n/_/   \\____/  \\____/          |__/|__/   /___/   /_/ |_/   \r\n                                                           ");
            }
            else if (isWin == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("__  __   ____    __  __           __    ____    _____    ______\r\n\\ \\/ /  / __ \\  / / / /          / /   / __ \\  / ___/   / ____/\r\n \\  /  / / / / / / / /          / /   / / / /  \\__ \\   / __/   \r\n / /  / /_/ / / /_/ /          / /___/ /_/ /  ___/ /  / /___   \r\n/_/   \\____/  \\____/          /_____/\\____/  /____/  /_____/   \r\n                                                               ");
            }

            return isWin;
        }

    }
}

