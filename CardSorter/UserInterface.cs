using System;
using System.IO;
using System.Threading;

namespace CardSorter
{
    class UserInterface
    {
        private static bool _stopped = true;//indicates, that progressbar has been stopped
        private static bool _stopFlag = true;//flag that stops progressbar
        public static string WordAction = "";
        public static double PercentCompleted = 0;
        public static Logger Logger;//object for logging

        public static bool Stopped
        {
            get { return _stopped; }
        }
        

        public static void ProgramStart()
        {
            Logger = Logger.GetLogger();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Card Sorter 1.0 started");
            Console.ForegroundColor = ConsoleColor.Gray;
            Logger.LogWrite("Program started");//to log
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
                Logger.LogWrite("Helper was corrupted or deleted and cannot be displayed");//to log
            }
        }
        public static void ProgressDisplayer()//displays progressbar with percent indicator
        {
            string word = WordAction;
            PercentCompleted = 0;
            Console.Write(word);
            _stopFlag = true;
            _stopped = false;
            while (_stopFlag)
            {
                for (int i = 0; i <= 5; i++)
                {
                    string percents = String.Format(", {0}% completed", PercentCompleted);
                    Console.Write(percents);
                    for (int j = 0; j <= i; j++)
                    {
                        Console.Write(".");    
                    }
                    if(!_stopFlag)//checking, if operation ended
                        break;
                    Thread.Sleep(200);
                    Console.SetCursorPosition(word.Length, Console.CursorTop);
                    Console.ForegroundColor = ConsoleColor.Black;
                    for (int k = 0; k <= 5+percents.Length; k++)
                    {
                        Console.Write(Convert.ToChar(219).ToString());//clearing string
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition(word.Length, Console.CursorTop);
                }
            }
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Black;
            for (int i = 0; i <= word.Length+21; i++)//initial word length + percent completed + dots
            {
                Console.Write(Convert.ToChar(219).ToString());
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(0,Console.CursorTop);
            _stopped = true;
        }
        public static void StopProgressBar()//gives ability for methods in parallel thread to stop progressbar
        {
            _stopFlag = false;
        }
        
        private static void ShowToUser(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }//displays message to user with specified color
        public static string[] InputHandle(string[] args)
        {
            string[] argumnetsHandled = new string[3];//for output of method
            if (args.Length == 0)
            {
                ShowToUser("Command cannot run without arguments", ConsoleColor.Red);
                HelpDisplay();
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                Logger.LogWrite("Error: running without arguments");//to log
                return null;
            }
            else if (args.Length > 3)
            {
                ShowToUser("Not more than three arguments accepted!",ConsoleColor.Red);
                HelpDisplay();
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                Logger.LogWrite("Error: running with more arguments than accepted");//to log
                return null;
            }
            else//if arguments.Count between 1 and 3
            {
                for (int i = 0; i < args.Length; i++)
                {
                    argumnetsHandled[i] = args[i];
                }
            }
            //checking one by one arguments entered by user
            if (!Directory.Exists(argumnetsHandled[0]))//input folder
            {
                ShowToUser("There is no such input directory", ConsoleColor.Red);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                Logger.LogWrite("Error: No such input dictionary");//to log
                return null;
            }
            else if(!Directory.Exists(argumnetsHandled[1]))//output folder
            {
                if (argumnetsHandled[1] == null)//ok, than archiving to the input folder with default compression level
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
                        catch (Exception ex)
                        {
                            ShowToUser("Cannot create directory!!!", ConsoleColor.Red);
                            Console.WriteLine("Press any key to exit");
                            Console.ReadKey();
                            Logger.LogWrite("Error: Directory cannot be created, error message: " + ex.Message);//to log
                            return null;
                        }
                        ShowToUser("Directory was succesfully created", ConsoleColor.Green);
                        Logger.LogWrite("Directory " + argumnetsHandled[1] + " was succesfully created");//to log
                    }
                    else//user doesnt want to create directory entered, shutting down...
                    {
                        Console.WriteLine("Shutting down program...");
                        Console.WriteLine("Press any key to exit");
                        Console.ReadKey();
                        Logger.LogWrite("Program was shut down by user choice");//to log
                        return null;
                    }
                }
            }
            else if (argumnetsHandled[2] == null)//archiver compression level
            {
                ShowToUser("Archiving will be done with default compression level 5", ConsoleColor.Yellow);
                Console.WriteLine("Press any key to continue");
                argumnetsHandled[2] = "5";//setting default value
                Console.ReadKey();
                return argumnetsHandled;
            }
            int compressionLevel = 5;//compression level assigned to default level
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
                Logger.LogWrite("Error: wrong compression level input");//to log
                return null;
            }
            if (compressionLevel > 9 || compressionLevel < 0)
            {
                ShowToUser("Wrong compression level entered! Only ingers [0-9] allowed", ConsoleColor.Red);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                Logger.LogWrite("Error: wrong compression level input");//to log
                return null;
            }
            return argumnetsHandled;
        }//handles user input, error checking
    }
}
