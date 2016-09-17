using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using Ionic.Zlib;

namespace CardSorter
{
    class FileSystemTasks
    {
        private readonly List<LogItem> _logsCollection;
        private string _pathFrom;//change fields down!!!!!!!!!!!testing!!!!!!!!!!!!!!
        private string _pathTo;
        private readonly List<string> _yearFoldersFinal;//for storage of yearFolders in destination path

        //constructor
        public FileSystemTasks(string pathFrom,string pathTo, int compressionLevel)
        {
            PathFrom = pathFrom;
            PathTo = pathTo;
            _logsCollection = new List<LogItem>();
            _yearFoldersFinal = new List<string>();
#region compressionLevel
            switch (compressionLevel)
            {
                case 0:
                    Level = CompressionLevel.Level0;
                    break;
                case 1:
                    Level = CompressionLevel.Level1;
                    break;
                case 2:
                    Level = CompressionLevel.Level2;
                    break;
                case 3:
                    Level = CompressionLevel.Level3;
                    break;
                case 4:
                    Level = CompressionLevel.Level4;
                    break;
                case 5:
                    Level = CompressionLevel.Level5;
                    break;
                case 6:
                    Level = CompressionLevel.Level6;
                    break;
                case 7:
                    Level = CompressionLevel.Level7;
                    break;
                case 8:
                    Level = CompressionLevel.Level8;
                    break;
                case 9:
                    Level = CompressionLevel.Level9;
                    break;
                default:
                    Level = CompressionLevel.Level5;
                    break;
            }
#endregion

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

        public CompressionLevel Level { get; private set; }

        static string MonthNameGiver(int number)
        {
            return Enum.GetName(typeof(MonthsNames), number);
        }//gives month name instead of number
        public void AnalyzeIt()//starts analyzer and progressbar in parallel
        {
            UserInterface.WordAction = "Analyzing";
            Parallel.Invoke(UserInterface.ProgressDisplayer,Analyzer);
        }
        public void MoveIt()//starts mover and progressbar in parallel
        {
            UserInterface.WordAction = "Moving files";
            Parallel.Invoke(UserInterface.ProgressDisplayer,Mover);
        }
        public void ArchivateIt()//starts archiver and progressbar in parallel
        {
            UserInterface.WordAction = "Archiving files";
            Parallel.Invoke(UserInterface.ProgressDisplayer,MassiveArchiver);
        }
        private void Analyzer()
        {
            UserInterface.Logger.LogWrite("File analyze started");//to log
            string[] filesInFolder = Directory.GetFiles(_pathFrom);
            double percentCompletion = 0;
            double oneAnalyzeCost = Math.Round((100.0/filesInFolder.Length),2);
            foreach (string file in filesInFolder)
            {
                FileInfo oneFile = new FileInfo(file);
                if (oneFile.Extension == ".log" && oneFile.Name.StartsWith("Ast"))//checking, if file is log of needed program (starts with "Ast" and has .log extension)
                {
                    _logsCollection.Add(new LogItem(file));//if yes - adding it to collection
                }
                percentCompletion += oneAnalyzeCost;//every file adds completion percent
                UserInterface.PercentCompleted = Math.Round(percentCompletion);
                Thread.Sleep(50);//for testing purpose only!!!!!!!!!!!!!!
            }
            UserInterface.PercentCompleted = 100;//analyze completed, setting percent to 100
            Thread.Sleep(2000);
            UserInterface.StopProgressBar();//calling progressbar stop
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(100);
            }
            UserInterface.PercentCompleted = 0;//after progressbar stop returning percent to 0
            if (_logsCollection.Count != 0)//if log files were found
            {
                Console.ForegroundColor = ConsoleColor.Green;
                string message = string.Format("Analyze finished, {0} files will be sorted and archived",
                    _logsCollection.Count);
                Console.WriteLine(message);
                UserInterface.Logger.LogWrite(message);//to log
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else//if not
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                string message = "No card logs were found in selected folder";
                Console.WriteLine(message);
                UserInterface.Logger.LogWrite(message);//to log
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        public void Mover()//sorts files to folders like this: year\month\date
        {
            double processCompletion = 0;
            double oneMoveCost = Math.Round((100.0/LogsCollection.Count),2);//calculating percent increase with every folder move
            foreach (LogItem logItem in LogsCollection)
            {
                string tempDirectory = _pathTo + @"\" + logItem.Year;
                if (!_yearFoldersFinal.Contains(tempDirectory))
                {
                    _yearFoldersFinal.Add(tempDirectory);    
                }
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

                try//moving file to folder created
                {
                    logItem.Info.MoveTo(tempDirectory + @"\" + logItem.Info.Name);
                }
                catch (Exception ex)
                {
                    UserInterface.Logger.LogWrite("Error occured moving file " + logItem.Info.Name + " to folder " + tempDirectory);//to log
                    UserInterface.Logger.LogWrite("Error message: " + ex.Message);//to log
                }
                UserInterface.Logger.LogWrite("File " + logItem.Info.Name + " was successfully moved to folder: " + tempDirectory);//to log
                processCompletion += oneMoveCost;//increasing percent with every file move
                UserInterface.PercentCompleted = Math.Round(processCompletion);
                Thread.Sleep(100);//for testing only!!!!!!!!!!!!1
            }
            UserInterface.PercentCompleted = 100;//completed - percent 100
            Thread.Sleep(2000);
            UserInterface.StopProgressBar();//stopping progressbar
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            UserInterface.PercentCompleted = 0;//making percent 0 again
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Files have been moved succesfully to folders");
            Console.ForegroundColor = ConsoleColor.Gray;
            UserInterface.Logger.LogWrite("Moving completed succesfully, all files moved");//to log
        }
        public void MassiveArchiver()
        {
            int archivesCounter = 0;
            foreach (string yearsDirectory in _yearFoldersFinal)//узнаем количество папок для архивации для расчета процента готовности
            {
                string[] monthDirectoriesInYear = Directory.GetDirectories(yearsDirectory);
                archivesCounter += monthDirectoriesInYear.Length;
            }
            double processCompletion = 0;
            double oneArchiveCost = Math.Round((100.0/archivesCounter),2);//на столько будет увеличиваться процент при архивировании каждой папки
            foreach (string yearDirectory in _yearFoldersFinal)//перебор по годам
            {
                string year = yearDirectory.Substring(yearDirectory.Length - 4);
                string message = string.Format("Started archiving in folder {0}", year);//to log
                UserInterface.Logger.LogWrite(message);//to log
                string[] directoriesForArchiving = Directory.GetDirectories(yearDirectory);
                for (int i = 0; i < directoriesForArchiving.Length; i++)//перебор по месяцам
                {
                    try
                    {
                        OneFolderArchiver(directoriesForArchiving[i]);
                        processCompletion += oneArchiveCost;
                        UserInterface.PercentCompleted = Math.Round(processCompletion);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        string errorMessage = string.Format("Error occured in working with: {0}",
                            directoriesForArchiving[i]);
                        Console.WriteLine(errorMessage);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        UserInterface.Logger.LogWrite(errorMessage);//to log
                        UserInterface.Logger.LogWrite("Error message: " + ex.Message);//to log
                    }
                }
                string successMessage = string.Format("Finished archiving of folder {0}", year);//to log
                UserInterface.Logger.LogWrite(successMessage);
            }
            UserInterface.PercentCompleted = 100;//задача завершена, процент выполнения=100
            Thread.Sleep(2000);
            UserInterface.StopProgressBar();
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            UserInterface.PercentCompleted = 0;//после завершения прогрессбара возвращаем обратно значение процента
            Console.ForegroundColor = ConsoleColor.Green;
            string archivingFinishedMessage = string.Format(
                "All files were succcesfully archived, {0} archives created", archivesCounter);
            Console.WriteLine(archivingFinishedMessage);
            Console.ForegroundColor = ConsoleColor.Gray;
            UserInterface.Logger.LogWrite(archivingFinishedMessage);//to log
            Console.ReadKey();//убрать в конце!!!!!!!!!!!!!!
        }//uses onefolderarchiver to archive all months folders

        private void OneFolderArchiver(string directoryToArchive) //archiving of one folder
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;//low priority, because operation takes long time, it is not possible to use CPU much for so long
            if (File.Exists(directoryToArchive + ".zip"))//if archive already exists in destination folder - only adding files
            {
                using (ZipFile zip = ZipFile.Read(directoryToArchive + ".zip"))
                {
                    DirectoryInfo dirToArchive = new DirectoryInfo(directoryToArchive);
                    DirectoryInfo[] dirs = dirToArchive.GetDirectories();
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        zip.UpdateDirectory(subdir.FullName,subdir.Name);
                    }
                    zip.Save();
                }
            }
            else//if not exists - creating new one
            {
                using (ZipFile zip = new ZipFile(directoryToArchive))
                {
                    zip.CompressionLevel = Level;
                    zip.AddDirectory(directoryToArchive);
                    zip.Save(directoryToArchive + ".zip");

                }
            }
            Thread.CurrentThread.Priority = ThreadPriority.Normal;//setting priority default
            Directory.Delete(directoryToArchive, true);//deleting archived directory
        }
    }
}