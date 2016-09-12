using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardSorter
{
    class UserInterface
    {
        private static bool stopped = true;
        private static bool stopFlag = true;
        public static string wordAction = "";

        public static bool Stopped
        {
            get { return stopped; }
        }

        public static void ProgramStart()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Card Sorter 1.0 started");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void HelpDisplay()
        {
            Console.Clear();
            if (File.Exists("help.txt"))
            {
                using (StreamReader sr = new StreamReader("help.txt"))
                {
                    Console.WriteLine(sr.ReadToEnd());
                }
            }
            else
            {
                Console.WriteLine("Sorry, program help was removed or corrupted");
            }
        }

        public static void ProgressDisplayer()
        {
            string word = wordAction;
            Console.Write(word);
            stopFlag = true;
            stopped = false;
            while (stopFlag)
            {
                for (int i = 0; i <= 5; i++)
                {
                    Console.Write(".");
                    if(!stopFlag)
                        break;
                    Thread.Sleep(500);
                }
                Console.SetCursorPosition(word.Length, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.Black;
                for (int i = 0; i <= 5; i++)
                {
                    Console.Write(Convert.ToChar(219).ToString());
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(word.Length, Console.CursorTop);
            }
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Black;
            for (int i = 0; i <= word.Length+6; i++)
            {
                Console.Write(Convert.ToChar(219).ToString());
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(0,Console.CursorTop);
            stopped = true;
        }

        public static void stopProgressBar()
        {
            stopFlag = false;
        }
    }
}
