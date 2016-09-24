using System;
using System.IO;
using System.Threading;

namespace CardSorter
{
    static class UserInterface
    {
        private static bool _stopFlag = true;//flag that stops progressbar
        public static string WordAction = "";
        public static int TotalFiles = 0;
        public static int CompletedFiles = 0;
        public static Logger Logger;//object for logging

        //static constructor
        static UserInterface()
        {
            Stopped = true;
            ProgramOwnPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public static bool Stopped { get; private set; }//shows if progressbar is stopped
        public static string ProgramOwnPath { get; private set; }//shows program own path


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
            if (File.Exists(ProgramOwnPath +"\\help.txt"))
            {
                using (StreamReader sr = new StreamReader(ProgramOwnPath + "\\help.txt"))
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
            string word = WordAction + ":";
            string completion = "";
            CompletedFiles = 0;
            Console.Write(word);
            _stopFlag = true;
            Stopped = false;
            while (_stopFlag)
            {
                for (int i = 0; i <= 5; i++)
                {
                    completion = String.Format(" {0} of {1} completed", CompletedFiles, TotalFiles);//shows files operation completion
                    Console.Write(completion);
                    for (int j = 0; j <= i; j++)
                    {
                        Console.Write(".");    
                    }
                    if(!_stopFlag)//checking, if operation ended
                        break;
                    Thread.Sleep(200);
                    Console.SetCursorPosition(word.Length, Console.CursorTop);
                    Console.ForegroundColor = ConsoleColor.Black;
                    for (int k = 0; k <= 5+completion.Length; k++)
                    {
                        Console.Write(Convert.ToChar(219).ToString());//clearing string
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition(word.Length, Console.CursorTop);
                }
            }
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Black;
            for (int i = 0; i <= word.Length+completion.Length + 7; i++)//initial word length + completion + dots
            {
                Console.Write(Convert.ToChar(219).ToString());
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(0,Console.CursorTop);
            Stopped = true;
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
                    if (s == "y" || s == "Y")
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
                            Logger.LogWrite("Error: Directory cannot be created, error message: " + ex.Message);
                                //to log
                            return null;
                        }
                        ShowToUser("Directory was succesfully created", ConsoleColor.Green);
                        Logger.LogWrite("Directory " + argumnetsHandled[1] + " was succesfully created"); //to log
                    }
                    else //user doesnt want to create directory entered, shutting down...
                    {
                        Console.WriteLine("Shutting down program...");
                        Console.WriteLine("Press any key to exit");
                        Console.ReadKey();
                        Logger.LogWrite("Program was shut down by user choice"); //to log
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
