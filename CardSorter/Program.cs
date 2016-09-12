using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Ionic.BZip2;
using Ionic.Zip;
using Ionic.Zlib;

namespace CardSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();//clear command prompt on program start
            ProgramStart();
            Console.ReadKey();
            string pathFrom = @"D:\prog\C#\Visual Studio\real\CardSorter\CardSorter\bin\Debug\1";
            string pathTo = pathFrom;//temporary, replace with another folder!!!
            if (!Directory.Exists(pathFrom))
            {
                throw new DirectoryNotFoundException("There is no such directory");
            }
            List<LogItem> LogsCollection = new List<LogItem>();

            string[] filesInFolder = Directory.GetFiles(pathFrom);
            foreach (string file in filesInFolder)
            {
                LogsCollection.Add(new LogItem(file));
            }
            FileSystemTasks.AsyncMover(LogsCollection, pathTo).GetAwaiter().GetResult();//invoke of mover to move files into different folders
            FileSystemTasks.AsyncMassiveArchiver(pathFrom).GetAwaiter().GetResult();

            Console.ReadKey();
        }

        static void ProgramStart()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Card Sorter 1.0 started");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void HelpDisplay()
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