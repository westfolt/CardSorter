using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardSorter
{
    static class UserInterface
    {
        private static bool stopFlag = true;
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

        public static async Task AsyncProgressDisplayer(string word)
        {
            Console.Write(word);
            stopFlag = true;
            while (stopFlag)
            {
                for (int i = 0; i <= 5; i++)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
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
        }

        public static void stopProgressBar(string stopWord)
        {
            if (stopWord == "stop")
                stopFlag = false;
        }
    }
}
