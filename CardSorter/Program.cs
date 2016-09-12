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
            UserInterface.ProgramStart();
            Console.ReadKey();
            string pathFrom = @"D:\prog\C#\Visual Studio\real\For SorterCard\1";
            string pathTo = pathFrom;//temporary, replace with another folder!!!
#region subscribing to events
            FileSystemTasks.AnalyzeStarted += UserInterface.AsyncProgressDisplayer;
#endregion

            FileSystemTasks.Analyzer(pathFrom).GetAwaiter().GetResult();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            UserInterface.AsyncProgressDisplayer("Analyzing").GetAwaiter().GetResult();
            Console.ReadKey();
            Console.WriteLine("!");
            Console.ReadKey();
            

            List<LogItem> LogsCollection = FileSystemTasks.Analyzer(pathFrom).GetAwaiter().GetResult();
            FileSystemTasks.AsyncMover(LogsCollection, pathTo).GetAwaiter().GetResult();//invoke of mover to move files into different folders
            FileSystemTasks.AsyncMassiveArchiver(pathFrom).GetAwaiter().GetResult();

            Console.ReadKey();
        }

        
    }
}