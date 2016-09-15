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
        private CompressionLevel compressionLevel;
        

        //constructor
        public FileSystemTasks(string pathFrom,string pathTo, int compressionLevel)
        {
            PathFrom = pathFrom;
            PathTo = pathTo;
            _logsCollection = new List<LogItem>();
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

        public CompressionLevel Level
        {
            get { return compressionLevel; }
            private set { compressionLevel = value; }
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
            UserInterface.logger.LogWrite("File analyze started");//to log
            string[] filesInFolder = Directory.GetFiles(_pathFrom);
            double percentCompletion = 0;
            double oneAnalyzeCost = Math.Round((100.0/filesInFolder.Length),2);
            foreach (string file in filesInFolder)
            {
                FileInfo oneFile = new FileInfo(file);
                if (oneFile.Extension == ".log" && oneFile.Name.StartsWith("Ast"))//проверяем, является ли файл логом card-write
                {
                    _logsCollection.Add(new LogItem(file));//если да - заносим в коллекцию
                }
                percentCompletion += oneAnalyzeCost;//каждый файл увеличивает процент готовности
                UserInterface.percentCompleted = Math.Round(percentCompletion);
            }
            UserInterface.percentCompleted = 100;//анализ завершен
            Thread.Sleep(2000);
            UserInterface.stopProgressBar();
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            UserInterface.percentCompleted = 0;//после завершения прогрессбара возвращаем процент в 0
            if (_logsCollection.Count != 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                string message = string.Format("Analyze finished, {0} files will be sorted and archived",
                    _logsCollection.Count);
                Console.WriteLine(message);
                UserInterface.logger.LogWrite(message);//to log
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                string message = "No card logs were found in selected folder";
                Console.WriteLine(message);
                UserInterface.logger.LogWrite(message);//to log
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        public void Mover()//sorts files to folders like this: year\month\date
        {
            double processCompletion = 0;
            double oneMoveCost = Math.Round((100.0/LogsCollection.Count),2);//на столько будет увеличиваться процент при архивировании каждой папки
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

                try
                {
                    logItem.Info.MoveTo(tempDirectory + @"\" + logItem.Info.Name);
                }
                catch (Exception ex)
                {
                    UserInterface.logger.LogWrite("Error occured moving file " + logItem.Info.Name + " to folder " + tempDirectory);//to log
                    UserInterface.logger.LogWrite("Error message: " + ex.Message);//to log
                }
                UserInterface.logger.LogWrite("File " + logItem.Info.Name + " was successfully moved to folder: " + tempDirectory);//to log
                processCompletion += oneMoveCost;//увеличиваем процент при каждом перемещении файла
                UserInterface.percentCompleted = Math.Round(processCompletion);
            }
            UserInterface.percentCompleted = 100;
            Thread.Sleep(2000);
            UserInterface.stopProgressBar();
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            UserInterface.percentCompleted = 0;//после завершения прогрессбара возвращаем обратно значение процента
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Files have been moved succesfully to folders");
            Console.ForegroundColor = ConsoleColor.Gray;
            UserInterface.logger.LogWrite("Moving completed succesfully, all files moved");//to log
        }
        public void MassiveArchiver()
        {
            string[] yearsDirectories = Directory.GetDirectories(_pathTo);
            int archivesCounter = 0;
            foreach (string yearsDirectory in yearsDirectories)//узнаем количество папок для архивации для расчета процента готовности
            {
                string[] monthDirectoriesInYear = Directory.GetDirectories(yearsDirectory);
                archivesCounter += monthDirectoriesInYear.Length;
            }
            double processCompletion = 0;
            double oneArchiveCost = Math.Round((100.0/archivesCounter),2);//на столько будет увеличиваться процент при архивировании каждой папки
            foreach (string yearDirectory in yearsDirectories)//перебор по годам
            {
                string year = yearDirectory.Substring(yearDirectory.Length - 4);
                string message = string.Format("Started archiving in folder {0}", year);//to log
                UserInterface.logger.LogWrite(message);//to log
                string[] directoriesForArchiving = Directory.GetDirectories(yearDirectory);
                for (int i = 0; i < directoriesForArchiving.Length; i++)//перебор по месяцам
                {
                    try
                    {
                        OneFolderArchiver(directoriesForArchiving[i]);
                        processCompletion += oneArchiveCost;
                        UserInterface.percentCompleted = Math.Round(processCompletion);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        string errorMessage = string.Format("Error occured in working with: {0}",
                            directoriesForArchiving[i]);
                        Console.WriteLine(errorMessage);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        UserInterface.logger.LogWrite(errorMessage);//to log
                        UserInterface.logger.LogWrite("Error message: " + ex.Message);//to log
                    }
                }
                string successMessage = string.Format("Finished archiving of folder {0}", year);//to log
                UserInterface.logger.LogWrite(successMessage);
            }
            UserInterface.percentCompleted = 100;//задача завершена, процент выполнения=100
            Thread.Sleep(2000);
            UserInterface.stopProgressBar();
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            UserInterface.percentCompleted = 0;//после завершения прогрессбара возвращаем обратно значение процента
            Console.ForegroundColor = ConsoleColor.Green;
            string archivingFinishedMessage = string.Format(
                "All files were succcesfully archived, {0} archives created", archivesCounter);
            Console.WriteLine(archivingFinishedMessage);
            Console.ForegroundColor = ConsoleColor.Gray;
            UserInterface.logger.LogWrite(archivingFinishedMessage);//to log
            Console.ReadKey();
        }//uses onefolderarchiver to archive all months folders
        void OneFolderArchiver(string directoryToArchive)//archiving of one folder
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            using (ZipFile zip = new ZipFile(directoryToArchive))
            {
                zip.CompressionLevel = Level;
                zip.AddDirectory(directoryToArchive);
                zip.Save(directoryToArchive + ".zip");
            }
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            Directory.Delete(directoryToArchive, true);
        }
    }
}