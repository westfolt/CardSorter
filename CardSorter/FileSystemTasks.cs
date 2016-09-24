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
        static string MonthNameGiver(int number)
        {
            return Enum.GetName(typeof(MonthsNames), number);
        }//gives month name instead of number
#region Properties
        private string PathFrom { get; set; }
        public List<LogItem> LogsCollection
        {
            get { return _logsCollection; }
        }
        private string PathTo { get; set; }
        private CompressionLevel Level { get; set; }
#endregion
        public void AnalyzeIt()//starts analyzer and progressbar in parallel
        {
            UserInterface.WordAction = "Analyzing files";
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

#region Methods that do required actions
        private void Analyzer()
        {
            UserInterface.Logger.LogWrite("File analyze started");//to log
            string[] filesInFolder = Directory.GetFiles(PathFrom);
            UserInterface.TotalFiles = filesInFolder.Length;//sending to progressbar total amount of files
            UserInterface.CompletedFiles = 0;
            foreach (string file in filesInFolder)
            {
                FileInfo oneFile = new FileInfo(file);
                if (oneFile.Extension == ".log" && oneFile.Name.StartsWith("Ast"))//checking, if file is log of needed program (starts with "Ast" and has .log extension)
                {
                    _logsCollection.Add(new LogItem(file));//if yes - adding it to collection
                }
                UserInterface.CompletedFiles++;//every file adds completion
                Thread.Sleep(1);//extra sleep, gives ability to watch file analyzing progress, can be deleted for higher speed
            }
            UserInterface.CompletedFiles = UserInterface.TotalFiles;//analyze completed
            Thread.Sleep(2000);
            UserInterface.StopProgressBar();//calling progressbar stop
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(100);
            }
            //after progressbar stop returning values to 0
            UserInterface.CompletedFiles=0;
            UserInterface.TotalFiles = 0;
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
        private void Mover()//sorts files to folders like this: year\month\date
        {
            UserInterface.TotalFiles = LogsCollection.Count;//sending to progressbar total amount of files
            UserInterface.CompletedFiles = 0;
            foreach (LogItem logItem in LogsCollection)
            {
                string tempDirectory = PathTo + @"\" + logItem.Year;
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
                UserInterface.CompletedFiles++;//increasing percent with every file move
            }
            UserInterface.CompletedFiles = UserInterface.TotalFiles;//completed all
            Thread.Sleep(2000);
            UserInterface.StopProgressBar();//stopping progressbar
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            //making percent 0 again
            UserInterface.TotalFiles = 0;
            UserInterface.CompletedFiles = 0;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Files have been moved succesfully to folders");
            Console.ForegroundColor = ConsoleColor.Gray;
            UserInterface.Logger.LogWrite("Moving completed succesfully, all files moved");//to log
        }

        private void MassiveArchiver()//uses onefolderarchiver to archive all months folders
        {
            int archivesCounter = 0;
            foreach (string yearsDirectory in _yearFoldersFinal)//узнаем количество папок для архивации для расчета процента готовности
            {
                string[] monthDirectoriesInYear = Directory.GetDirectories(yearsDirectory);
                archivesCounter += monthDirectoriesInYear.Length;
            }
            UserInterface.TotalFiles = archivesCounter;//sending to progressbar total amount of files
            UserInterface.CompletedFiles = 0;
            foreach (string yearDirectory in _yearFoldersFinal)//перебор по годам
            {
                string year = yearDirectory.Substring(yearDirectory.Length - 4);
                string message = string.Format("Started archiving in folder {0}", year);//to log
                UserInterface.Logger.LogWrite(message);//to log
                string[] directoriesForArchiving = Directory.GetDirectories(yearDirectory);
                foreach (string dir in directoriesForArchiving)
                {
                    try
                    {
                        OneFolderArchiver(dir);
                        UserInterface.CompletedFiles++;
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        string errorMessage = string.Format("Error occured in working with: {0}",
                            dir);
                        Console.WriteLine(errorMessage);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        UserInterface.Logger.LogWrite(errorMessage);//to log
                        UserInterface.Logger.LogWrite("Error message: " + ex.Message);//to log
                    }
                }
                string successMessage = string.Format("Finished archiving of folder {0}", year);//to log
                UserInterface.Logger.LogWrite(successMessage);
            }
            UserInterface.CompletedFiles = UserInterface.TotalFiles;//задача завершена, процент выполнения=100
            Thread.Sleep(2000);
            UserInterface.StopProgressBar();
            while (!UserInterface.Stopped)
            {
                Thread.Sleep(50);
            }
            //после завершения прогрессбара возвращаем обратно значение процента
            UserInterface.CompletedFiles = 0;
            UserInterface.TotalFiles = 0;
            Console.ForegroundColor = ConsoleColor.Green;
            string archivingFinishedMessage = string.Format(
                "All files were succcesfully archived, {0} archives created", archivesCounter);
            Console.WriteLine(archivingFinishedMessage);
            Console.ForegroundColor = ConsoleColor.Gray;
            UserInterface.Logger.LogWrite(archivingFinishedMessage);//to log
        }
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
#endregion
    }
}