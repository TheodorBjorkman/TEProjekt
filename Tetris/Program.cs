using System;
using System.Threading;
// using System.IO;
using System.Collections.Generic;
using System.Timers;

namespace Tetris
{
    class Program
    {
        static int writeDelay = 20;
        static int test = 0;
        static System.Timers.Timer mainTimer;
        static void Main()
        {
            // Initialization
            StartTimer(2000);
            Console.ReadLine();
            mainTimer.Stop();
            mainTimer.Dispose();
        }
        static void Logic()
        {
            //Apply logic

            //Draw
            Draw();
        }
        static void Draw()
        {
            //Draw objects
            System.Console.WriteLine("Draw");
        }
        static void StartTimer(int time)
        {
            //Create timer
            mainTimer = new System.Timers.Timer(time);
            mainTimer.Elapsed += TimedEvent;
            mainTimer.AutoReset = true;
            mainTimer.Enabled = true;
        }
        static void TimedEvent(Object source, ElapsedEventArgs e)
        {
            Logic();
        }
        static void SlowWrite(string input, bool WriteLine = true)
        {
            foreach(char temp in input)
            {
                Console.Write(temp);
                Thread.Sleep(writeDelay);
            }
            if(WriteLine) Console.WriteLine();
        }
        static void SlowWrite(string[] input, bool WriteLine = true)
        {
            foreach(string temp in input)
            {
                SlowWrite(temp, WriteLine);
            }
        }
        // static void CreateChildThread()
        // {
        //     ThreadStart threadStart = new ThreadStart(Draw);
        //     Thread timerThread = new Thread(threadStart);
        //     timerThread.IsBackground = true;
        // }
    }
    public class MainMenu
    {
        static void Draw()
        {
            Console.WriteLine("MainMenu");
        }
    }
    public class PlayingField
    {
        static void Draw()
        {
            Console.WriteLine("MainMenu");
        }
    }
    public class NextField
    {
        static void Draw()
        {
            Console.WriteLine("MainMenu");
        }
    }
    public class HoldField
    {
        static HeldTetrimino heldTetrimino = new HeldTetrimino();
        static void Draw()
        {
            Console.WriteLine("MainMenu");
            Console.WriteLine();
        }
    }
    public class HeldTetrimino
    {
        static string activeTetrimino = "";
        static List<string> inactiveTetrimino = new List<string>();
    }
    public class Tetrimino 
    {

    }
}
