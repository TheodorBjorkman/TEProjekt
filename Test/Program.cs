using System;
using System.Threading;
using System.Collections.Generic;
using System.Timers;

/*
Om man använder en int som enumerator är for loop bäst, annars är oftast det bäst med while loop eller en foreach om du inte manipulerar innehållet av en collection.
*/
namespace Test
{
    class Program
    {
        //Class variables in main class
        static int writeDelay = 200;
        static System.Timers.Timer mainTimer;
        static Level level;
        static Snake snake;
        static Food food;
        static bool first = true;
        static int moveCycle = 0;
        static int syncCheck = 0;
        static int tickTime;
        static int[] bounds = new int[2];
        static void Main()
        {
            //Fix UTF-8
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //Asks for size and tick speed
            Setup();
            //Create new objects
            level = new Level(bounds[0], bounds[1]);
            snake = new Snake(level.xSize, level.ySize);
            food = new Food(level.xSize, level.ySize);
            //Set positions of food and snake on level as well as draw them
            int[] foodPos = food.Reposition();
            CheckFoodPos();
            level.content[snake.xPosList[0], snake.yPosList[0]] = 1;
            level.content[foodPos[0], foodPos[1]] = 2;
            level.Draw();
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    snake.direction = 0;
                    syncCheck = moveCycle;
                    break;
                case ConsoleKey.RightArrow:
                    snake.direction = 1;
                    syncCheck = moveCycle;
                    break;
                case ConsoleKey.DownArrow:
                    snake.direction = 2;
                    syncCheck = moveCycle;
                    break;
                case ConsoleKey.LeftArrow:
                    snake.direction = 3;
                    syncCheck = moveCycle;
                    break;
            }
            StartTimer(tickTime);
            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        if ((snake.direction != 2 && snake.direction != 0) && syncCheck != moveCycle)
                        {
                            snake.direction = 0;
                            syncCheck = moveCycle;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if ((snake.direction != 3 && snake.direction != 1) && syncCheck != moveCycle)
                        {
                            snake.direction = 1;
                            syncCheck = moveCycle;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if ((snake.direction != 0 && snake.direction != 2) && syncCheck != moveCycle)
                        {
                            snake.direction = 2;
                            syncCheck = moveCycle;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if ((snake.direction != 1 && snake.direction != 3) && syncCheck != moveCycle)
                        {
                            snake.direction = 3;
                            syncCheck = moveCycle;
                        }
                        break;
                }
            }
        }
        static void Setup()
        {
            bool legal = false;
            while (!legal)
            {
                Console.WriteLine("Please enter x size of level (min 5):");
                string input = Console.ReadLine();
                int output = ParseInt(input);
                if (output >= 5)
                {
                    legal = true;
                    bounds[0] = output;
                }
            }
            legal = false;
            while (!legal)
            {
                Console.WriteLine("Please enter y size of level (min 6):");
                string input = Console.ReadLine();
                int output = ParseInt(input);
                if (output >= 6)
                {
                    legal = true;
                    bounds[1] = output;
                }
            }
            legal = false;
            while (!legal)
            {
                Console.WriteLine("Please enter speed of clock in milliseconds (min 250):");
                string input = Console.ReadLine();
                int output = ParseInt(input);
                if (output >= 250)
                {
                    legal = true;
                    tickTime = output;
                }
            }
        }
        static int ParseInt(string input)
        {
            bool success;
            int output = -1;
            success = int.TryParse(input, out output);
            return output;
        }
        static void Logic()
        {
            CheckDeath();
            CheckEat();
            snake.Move();
            CheckDeath();
            CheckEat();
            level.Empty();
            for (int i = 0; i < snake.length; i++)
            {
                level.content[snake.xPosList[i], snake.yPosList[i]] = 1;
            }
            level.content[food.x, food.y] = 2;
            level.Draw();
            moveCycle++;
        }
        static void CheckEat()
        {
            if (food.x == snake.xPosList[0] && food.y == snake.yPosList[0]) Eat();
        }
        static void CheckDeath()
        {
            int x = snake.xPosList[0];
            int y = snake.yPosList[0];
            for (int i = 1; i < snake.length; i++)
            {
                if (x == snake.xPosList[i] && y == snake.yPosList[i]) Death();
            }
            if (x < 0 || y < 0 || x > bounds[0] || y > bounds[1]) Death();
        }
        static void Eat()
        {
            bool test = true;
            int[] foodPos = food.Reposition();
            while (test)
            {
                test = CheckFoodPos();
            }
            snake.Increment();
        }
        static bool CheckFoodPos()
        {
            for (int i = 0; i < snake.length; i++)
            {
                if (food.x == snake.xPosList[i] && food.y == snake.yPosList[i])
                {
                    food.Reposition();
                    return true;
                }
            }
            return false;
        }
        static void Death()
        {
            Console.WriteLine($"You lost! Your score was: {snake.length - 1}. Press any key to quit.");
            StopTimer();
            Console.ReadKey();
            Environment.Exit(0);
        }
        static void StartTimer(int time)
        {
            //Create timer but only once
            if (first == true)
            {
                first = false;
                mainTimer = new System.Timers.Timer(time);
                mainTimer.Elapsed += TimedEvent;
                mainTimer.AutoReset = true;
                mainTimer.Enabled = true;
            }
        }
        static void TimedEvent(Object source, ElapsedEventArgs e)
        {
            Logic();
        }
        static void StopTimer()
        {
            mainTimer.Stop();
            mainTimer.Dispose();
        }
        static void SlowWrite(string input, bool WriteLine = true)
        {
            foreach (char temp in input)
            {
                Console.Write(temp);
                Thread.Sleep(writeDelay);
            }
            if (WriteLine) Console.WriteLine();
        }
        static void SlowWrite(string[] input, bool WriteLine = true)
        {
            foreach (string temp in input)
            {
                SlowWrite(temp, WriteLine);
            }
        }
    }
    public class Level
    {
        public Level(int xS = 15, int yS = 11)
        {
            xSize = xS;
            ySize = yS;
            xSizeStat = xS;
            ySizeStat = yS;
            content = new int[xSize, ySize];
            Empty();
        }
        public int xSize;
        public static int xSizeStat;
        public int ySize;
        public static int ySizeStat;
        public int[,] content = new int[xSizeStat, ySizeStat];
        public void Empty()
        {
            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    content[j, i] = 0;
                }
            }
        }
        public int Draw()
        {
            Console.Clear();
            int snakeBits = 0;
            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    if (content[j, i] == 0)
                    {
                        Console.Write('\u2610' + " ");
                    }
                    if (content[j, i] == 1)
                    {
                        snakeBits++;
                        Console.Write('\u2589' + " ");
                        // Console.Write("S ");
                    }
                    if (content[j, i] == 2)
                    {
                        Console.Write('\u2573' + " ");
                    }
                }
                Console.WriteLine();
            }
            return snakeBits;
        }
    }
    public class Snake
    {
        public Snake(decimal xBound, decimal yBound)
        {
            length = 1;
            prevLength = 1;
            newLength = 1;
            xPosList.Add((int)Math.Round((xBound / 2) - 0, 5));
            yPosList.Add((int)Math.Round((yBound / 2) - 0, 5));
            xB = xBound;
            yB = yBound;
        }
        public List<int> xPosList = new List<int>();
        public List<int> yPosList = new List<int>();
        public int direction = 0;
        public int length;
        public static int prevLength;
        public static int newLength;
        public static decimal xB;
        public static decimal yB;
        public void Increment()
        {
            newLength++;
        }
        public void Reset()
        {
            length = 1;
            prevLength = 1;
            newLength = 1;
            xPosList.Add((int)Math.Round((xB / 2) - 0, 5));
            yPosList.Add((int)Math.Round((yB / 2) - 0, 5));
        }
        public void Move()
        {
            List<int> xPosListTemp = new List<int>();
            List<int> yPosListTemp = new List<int>();
            xPosListTemp.AddRange(xPosList);
            yPosListTemp.AddRange(yPosList);
            //Move head
            switch (direction)
            {
                case 0:
                    yPosList[0]--;
                    break;
                case 1:
                    xPosList[0]++;
                    break;
                case 2:
                    yPosList[0]++;
                    break;
                case 3:
                    xPosList[0]--;
                    break;
            }
            //Move body
            for (int i = 1; i < length; i++)
            {
                xPosList[i] = xPosListTemp[i - 1];
                yPosList[i] = yPosListTemp[i - 1];
            }
            //Add new body part
            length = newLength;
            if (prevLength != length)
            {
                prevLength = length;
                xPosList.Add(xPosListTemp[xPosListTemp.Count - 1]);
                yPosList.Add(yPosListTemp[yPosListTemp.Count - 1]);
            }
        }
    }
    public class Food
    {
        Random rnd = new Random();
        public Food(int xB, int yB)
        {
            xBound = xB;
            yBound = yB;
        }
        public static int xBound;
        public static int yBound;
        public int x;
        public int y;
        public int[] Reposition()
        {
            x = rnd.Next(0, xBound);
            y = rnd.Next(0, yBound);
            int[] pos = { x, y };
            return pos;
        }
    }
}
