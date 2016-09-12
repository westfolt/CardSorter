using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardSorter
{
    static class UserInterface
    {
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
    }
}
