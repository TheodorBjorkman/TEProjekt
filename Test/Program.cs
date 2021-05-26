using System;
using System.Threading;
using System.Collections.Generic;
using System.Timers;

/*
Skriver mest på engelska för är mer van med att tänka/snacka programmering med engelska termer.
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
        static int tickTime = 300;
        static int[] bounds = { 15, 12 };
        //Start logic and direction change logic
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
            //First move logic - any direction allowed
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
            //Start the timer
            StartTimer(tickTime);
            while (true)
            //Loop for moving after first move. Stops 180´s and allows for input of current direction without disallowing change of direction
            //Only allows one change per game tick
            //Uses syncCheck and moveCycle too check wether player has changed direction during this game tick by setting them to be the same on direction change and comparing them
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
        //Setup for level size and game tick speed through user input. Disable for default settings
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
        //Safe check for int input. Outputs -1 if unsuccessfull. Only works because output has to be higher than 0 do to the nature of the variable usage.
        static int ParseInt(string input)
        {
            bool success;
            int output = -1;
            success = int.TryParse(input, out output);
            return output;
        }
        //Main logic for movement, death and eating. Uses helper functions and built in class functions. Called by the timer each game tick.
        static void Logic()
        {
            CheckDeath();
            CheckEat();
            snake.Move();
            CheckDeath();
            CheckEat();
            level.Empty();
            //Set level content on positions of snake to snake.
            for (int i = 0; i < snake.length; i++)
            {
                level.content[snake.xPosList[i], snake.yPosList[i]] = 1;
            }
            //Set level content on position of food to food.
            level.content[food.x, food.y] = 2;
            level.Draw();
            //Increases the move cycle to sync direction change.
            moveCycle++;
        }
        //Checks if snake head is on the same position as food. Calls Eat() if true.
        static void CheckEat()
        {
            if (food.x == snake.xPosList[0] && food.y == snake.yPosList[0]) Eat();
        }
        //Checks if snake head is on the same position as another snake body part or out of bounds. Calls Death() if true.
        static void CheckDeath()
        {
            int x = snake.xPosList[0];
            int y = snake.yPosList[0];
            //Snake on snake check.
            for (int i = 1; i < snake.length; i++)
            {
                if (x == snake.xPosList[i] && y == snake.yPosList[i]) Death();
            }
            //OOB check.
            if (x < 0 || y < 0 || x > bounds[0] || y > bounds[1]) Death();
        }
        //Eat logic. Increments snake length and repositions food making sure it is not on the same place as the snake.
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
        //Logic for checking if food is on snake body. Would get caught in infinite loop if death is not checked before on max snake length.
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
        //Death logic. Shows score, stops timer and awaits input before quitting. Possibly call Main() for full reset implementation.
        static void Death()
        {
            Console.WriteLine($"You lost! Your score was: {snake.length - 1}. Press any key to quit.");
            StopTimer();
            Console.ReadKey();
            Environment.Exit(0);
        }
        //Creates and starts the main timer but only once. For full reset need to set first to true on top of Main();
        static void StartTimer(int time)
        {
            if (first == true)
            {
                first = false;
                mainTimer = new System.Timers.Timer(time);
                //Adds TimedEvent on each completion
                mainTimer.Elapsed += TimedEvent;
                //Makes it continue until stopped in another part of the program
                mainTimer.AutoReset = true;
                mainTimer.Enabled = true;
            }
        }
        //What happens each time timer runs out. Calls Logic().
        static void TimedEvent(Object source, ElapsedEventArgs e)
        {
            Logic();
        }
        //Stops and disposes of timer.
        static void StopTimer()
        {
            mainTimer.Stop();
            mainTimer.Dispose();
        }
        //Slow write function in case wanted. Overloaded for string arrays.
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
    //Level class. Contains class specific functions and variables useful for level state management as well as drawing in console.
    public class Level
    {
        //Constructor. Has default values for size but can be inputed manually.
        public Level(int xS = 15, int yS = 11)
        {
            xSize = xS;
            ySize = yS;
            xSizeStat = xS;
            ySizeStat = yS;
            content = new int[xSize, ySize];
            Empty();
        }
        //Static values cannot be accessed outside class but non-static values cannot be used to initialize content outside of constructor.
        public int xSize;
        public static int xSizeStat;
        public int ySize;
        public static int ySizeStat;
        public int[,] content = new int[xSizeStat, ySizeStat];
        //Sets all content to empty squares (0)
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
        //Draw logic. 0 = empty squares, 1 = filled squares for snake, 2 = X for food. Commented S for snake due to broken UTF values in externalTerminal debugging.
        //Returns snakeBits for some reason idk. Tror jag tänkte använda det för att räkna poäng men kom på att jag bara kan se snake.length anyways.
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
    //Snake class.
    public class Snake
    {
        //Constructor. Needs bounds to set initial positions.
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
        //Increase next length.
        public void Increment()
        {
            newLength++;
        }
        //Potential reset function. Most likely not needed. Probably just create a new snake through Main().
        public void Reset()
        {
            length = 1;
            prevLength = 1;
            newLength = 1;
            xPosList.Add((int)Math.Round((xB / 2) - 0, 5));
            yPosList.Add((int)Math.Round((yB / 2) - 0, 5));
        }
        //Movement logic. Changes position of head depending on direction after saving old positions.
        //Uses old positions to move each part of the snake to the old position of the one before it.
        public void Move()
        {
            //Save old positions
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
            //Add new body part if length has increased. Can change to only two variables but idc enough to do it.
            length = newLength;
            if (prevLength != length)
            {
                prevLength = length;
                xPosList.Add(xPosListTemp[xPosListTemp.Count - 1]);
                yPosList.Add(yPosListTemp[yPosListTemp.Count - 1]);
            }
        }
    }
    //Food class.
    public class Food
    {
        //Constructor. Requires bounds.
        public Food(int xB, int yB)
        {
            xBound = xB;
            yBound = yB;
        }
        Random rnd = new Random();
        public static int xBound;
        public static int yBound;
        public int x;
        public int y;
        //Sets random position within bounds.
        public int[] Reposition()
        {
            x = rnd.Next(0, xBound);
            y = rnd.Next(0, yBound);
            int[] pos = { x, y };
            return pos;
        }
    }
}
