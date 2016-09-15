using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CardSorter
{
    class UserInterface
    {
        private static bool stopped = true;
        private static bool stopFlag = true;
        public static string wordAction = "";
        public static double percentCompleted = 0;
        public static string nextArchive = "";
        public static Logger logger;//объект для записи логов

        public static bool Stopped
        {
            get { return stopped; }
        }
        

        public static void ProgramStart()
        {
            logger = Logger.GetLogger();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Card Sorter 1.0 started");
            Console.ForegroundColor = ConsoleColor.Gray;
            logger.LogWrite("Program started");//to log
        }
        public static void HelpDisplay()
        {
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
                logger.LogWrite("Helper was corrupted or deleted and cannot be displayed");//to log
            }
        }
        public static void ProgressDisplayer()
        {
            string word = wordAction;
            percentCompleted = 0;
            Console.Write(word);
            stopFlag = true;
            stopped = false;
            while (stopFlag)
            {
                for (int i = 0; i <= 5; i++)
                {
                    string percents = String.Format(", {0}% completed", percentCompleted);
                    Console.Write(percents);
                    for (int j = 0; j <= i; j++)
                    {
                        Console.Write(".");    
                    }
                    if(!stopFlag)
                        break;
                    Thread.Sleep(200);
                    Console.SetCursorPosition(word.Length, Console.CursorTop);
                    Console.ForegroundColor = ConsoleColor.Black;
                    for (int k = 0; k <= 5+percents.Length; k++)
                    {
                        Console.Write(Convert.ToChar(219).ToString());
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition(word.Length, Console.CursorTop);
                }
            }
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Black;
            for (int i = 0; i <= word.Length+21; i++)//длина слова +процент+точки
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
        
        private static void ShowToUser(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static string[] InputHandle(string[] args)
        {
            string[] argumnetsHandled = new string[3];
            if (args.Length == 0)
            {
                ShowToUser("Command cannot run without arguments", ConsoleColor.Red);
                HelpDisplay();
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                logger.LogWrite("Error: running without arguments");//to log
                return null;
            }
            else if (args.Length > 3)
            {
                ShowToUser("Not more than three arguments accepted!",ConsoleColor.Red);
                HelpDisplay();
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                logger.LogWrite("Error: running with more arguments than accepted");//to log
                return null;
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    argumnetsHandled[i] = args[i];
                }
            }
            //проверяем по очереди введенные пользователем аргументы
            if (!Directory.Exists(argumnetsHandled[0]))//входящая папка
            {
                ShowToUser("There is no such input directory", ConsoleColor.Red);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                logger.LogWrite("Error: No such input dictionary");//to log
                return null;
            }
            else if(!Directory.Exists(argumnetsHandled[1]))//исходящая папка
            {
                if (argumnetsHandled[1] == null)
                {
                    ShowToUser("You have not specified output folder and compression level", ConsoleColor.Yellow);
                    ShowToUser("Output will go to the input folder with default compression level 5",
                        ConsoleColor.Yellow);
                    Console.WriteLine("Press any key to continue");
                    argumnetsHandled[1] = argumnetsHandled[0];
                    Console.ReadKey();
                }
                else
                {
                    Console.Write("There is no such output directory on the drive, try to create it? (y/n): ");
                    string s = Console.ReadLine();
                    if (s == "y")
                    {
                        try
                        {
                            Directory.CreateDirectory(argumnetsHandled[1]);
                        }
                        catch (Exception ex)//доделать!!!! запись в лог
                        {
                            ShowToUser("Cannot create directory!!!", ConsoleColor.Red);
                            Console.WriteLine("Press any key to exit");
                            Console.ReadKey();
                            logger.LogWrite("Error: Directory cannot be created, error message: " + ex.Message);//to log
                            return null;
                        }
                        ShowToUser("Directory was succesfully created", ConsoleColor.Green);
                        logger.LogWrite("Directory " + argumnetsHandled[1] + " was succesfully created");//to log
                    }
                    else
                    {
                        Console.WriteLine("Shutting down program...");
                        Console.WriteLine("Press any key to exit");
                        Console.ReadKey();
                        logger.LogWrite("Program was shut down by user choice");//to log
                        return null;
                    }
                }
            }
            else if (argumnetsHandled[2] == null)//степень сжатия архиватора
            {
                ShowToUser("Archiving will be done with default compression level 5", ConsoleColor.Yellow);
                Console.WriteLine("Press any key to continue");
                argumnetsHandled[2] = "5";//setting default value
                Console.ReadKey();
                return argumnetsHandled;
            }
            int compressionLevel = 10;//compression level
            try
            {
                compressionLevel = Convert.ToInt32(argumnetsHandled[2]);
            }
            catch (Exception ex)
            {
                ShowToUser("Wrong compression level entered! Only ingers [0-9] allowed", ConsoleColor.Red);
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                logger.LogWrite("Error: wrong compression level input");//to log
                return null;
            }
            if (compressionLevel > 9 || compressionLevel < 0)
            {
                ShowToUser("Wrong compression level entered! Only ingers [0-9] allowed", ConsoleColor.Red);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                logger.LogWrite("Error: wrong compression level input");//to log
                return null;
            }
            return argumnetsHandled;
        }
    }
}
