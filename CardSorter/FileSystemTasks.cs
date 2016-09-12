using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using Ionic.Zlib;

namespace CardSorter
{
    static class FileSystemTasks
    {
        public delegate Task ProcessStateHandler(string word);
        //события для оповещения прогрессбаров
        public static event ProcessStateHandler AnalyzeStarted;
        public static event ProcessStateHandler ArchivingStarted;
        public static event ProcessStateHandler AnalyzeFinished;
        public static event ProcessStateHandler ArchivingFinished;
        enum MonthsNames
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }
        static string MonthNameGiver(int number)
        {
            return Enum.GetName(typeof(MonthsNames), number);
        }

        public static async Task<List<LogItem>> Analyzer(string pathFrom)
        {
            if (!Directory.Exists(pathFrom))
            {
                throw new DirectoryNotFoundException("There is no such directory");
            }
            if (AnalyzeStarted != null)
            {
                AnalyzeStarted("Analyzing");
            }
            List<LogItem> LogsCollection = new List<LogItem>();

            string[] filesInFolder = Directory.GetFiles(pathFrom);
            foreach (string file in filesInFolder)
            {
                LogsCollection.Add(new LogItem(file));
            }
            if (AnalyzeFinished != null)
            {
                AnalyzeFinished("stop");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Analyze finished, {0} files will be sorted and archived", LogsCollection.Count);
            Console.ForegroundColor = ConsoleColor.Gray;
            return LogsCollection;
        }
        public static async Task AsyncMover(List<LogItem> LogsCollection, string pathTo)//sorts files to folders like this: year\month\date
        {
            
            if (LogsCollection == null || LogsCollection.Count == 0)
                throw new NotImplementedException("There are no items in collection");
            foreach (LogItem logItem in LogsCollection)
            {
                string tempDirectory = pathTo + @"\" + logItem.Year;
                #region Logic of folder creation
                if (!Directory.Exists(tempDirectory))
                {
                    tempDirectory += @"\" + MonthNameGiver(logItem.Month) + "_" + logItem.Year + @"\" + logItem.DayOfMonth;
                    Directory.CreateDirectory(tempDirectory);
                }
                else
                {
                    tempDirectory += @"\" + MonthNameGiver(logItem.Month) + "_" + logItem.Year;
                    if (!Directory.Exists(tempDirectory))
                    {
                        tempDirectory += @"\" + logItem.DayOfMonth;
                        Directory.CreateDirectory(tempDirectory);
                    }
                    else
                    {
                        tempDirectory += @"\" + logItem.DayOfMonth;
                        if (!Directory.Exists(tempDirectory))
                            Directory.CreateDirectory(tempDirectory);
                    }
                }
                #endregion
                logItem.Info.MoveTo(tempDirectory + @"\" + logItem.Info.Name);
            }
            Console.WriteLine("Files have been moved");
        }
        public static async Task AsyncMassiveArchiver(string pathFrom)
        {
            string[] yearsDirectories = Directory.GetDirectories(pathFrom);
            foreach (string yearDirectory in yearsDirectories)
            {
                string year = yearDirectory.Substring(yearDirectory.Length - 4);
                Console.WriteLine("Started archiving in folder {0}", year);
                string[] directoriesForArchiving = Directory.GetDirectories(yearDirectory);

                for (int i = 0; i < directoriesForArchiving.Length; i++)
                {
                    try
                    {
                        AsyncOneFolderArchiver(directoriesForArchiving[i]).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Error occured in working with: {0}", directoriesForArchiving[i]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                Console.WriteLine("Finished archiving of folder {0}", year);
            }
            Console.ReadKey();
        }//uses onefolderarchiver to archive all months folders
        static async Task AsyncOneFolderArchiver(string directoryToArchive)//archiving of one folder
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            using (ZipFile zip = new ZipFile(directoryToArchive))
            {
                zip.CompressionLevel = CompressionLevel.Level5;
                zip.AddDirectory(directoryToArchive);
                zip.Save(directoryToArchive + ".zip");
            }
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            Directory.Delete(directoryToArchive, true);
        }
    }
}
