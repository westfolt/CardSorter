using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using Ionic.Zlib;

namespace CardSorter
{
    class FileSystemTasks
    {
        private List<LogItem> _logsCollection;
        private string _pathFrom;//ввел поле, переделать методы ниже!!!!!
        private string _pathTo;
        

        //constructor
        public FileSystemTasks(string pathFrom,string pathTo)
        {
            PathFrom = pathFrom;
            PathTo = pathTo;
            _logsCollection = new List<LogItem>();
        }
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

        public string PathFrom
        {
            get { return _pathFrom; }
            private set
            {
                _pathFrom = value;
            }
        }

        public List<LogItem> LogsCollection
        {
            get { return _logsCollection; }
        }

        public string PathTo
        {
            get { return _pathTo; }
            private set { _pathTo = value; }
        }

        static string MonthNameGiver(int number)
        {
            return Enum.GetName(typeof(MonthsNames), number);
        }
        public void AnalyzeIt()
        {
            UserInterface.wordAction = "Analyzing";
            Parallel.Invoke(UserInterface.ProgressDisplayer,Analyzer);
        }
        public void MoveIt()
        {
            UserInterface.wordAction = "Moving files";
            Parallel.Invoke(UserInterface.ProgressDisplayer,Mover);
        }

        public void ArchivateIt()
        {
            UserInterface.wordAction = "Archiving files";
            Parallel.Invoke(UserInterface.ProgressDisplayer,MassiveArchiver);
        }
        private void Analyzer()
        {
            string[] filesInFolder = Directory.GetFiles(_pathFrom);
            foreach (string file in filesInFolder)
            {
                FileInfo oneFile = new FileInfo(file);
                if (oneFile.Extension == ".log" && oneFile.Name.StartsWith("Ast"))//проверяем, является ли файл логом card-write
                {
                    _logsCollection.Add(new LogItem(file));//если да - заносим в коллекцию
                }
            }
            Thread.Sleep(2000);
            UserInterface.stopProgressBar();
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            if (_logsCollection.Count != 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Analyze finished, {0} files will be sorted and archived", _logsCollection.Count);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No card logs were found in selected folder");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        public void Mover()//sorts files to folders like this: year\month\date
        {
            
            if (LogsCollection == null || LogsCollection.Count == 0)//лишняя проверка!!!!!!!!
                throw new NotImplementedException("There are no items in collection");
            foreach (LogItem logItem in LogsCollection)
            {
                string tempDirectory = _pathTo + @"\" + logItem.Year;
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
            Thread.Sleep(2000);
            UserInterface.stopProgressBar();
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Files have been moved succesfully to folders");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public void MassiveArchiver()
        {
            string[] yearsDirectories = Directory.GetDirectories(_pathFrom);
            int archivesCounter = 0;
            foreach (string yearDirectory in yearsDirectories)
            {
                string year = yearDirectory.Substring(yearDirectory.Length - 4);
                Console.WriteLine("Started archiving in folder {0}", year);
                string[] directoriesForArchiving = Directory.GetDirectories(yearDirectory);
                double percentDone = 0;
                double percentRate = 100.0/directoriesForArchiving.Length;
                for (int i = 0; i < directoriesForArchiving.Length; i++)
                {
                    try
                    {
                        OneFolderArchiver(directoriesForArchiving[i]);
                        archivesCounter++;
                        percentDone += percentRate;
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Error occured in working with: {0}", directoriesForArchiving[i]);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                //Console.WriteLine("Finished archiving of folder {0}", year);только в лог!!!!!!!!!!
            }
            Thread.Sleep(2000);
            UserInterface.stopProgressBar();
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All files were succcesfully archived, {0} archives created",archivesCounter);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadKey();
        }//uses onefolderarchiver to archive all months folders
        void OneFolderArchiver(string directoryToArchive)//archiving of one folder
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