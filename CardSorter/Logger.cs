﻿using System;
using System.IO;
namespace CardSorter
{
    class Logger//singleton logger
    {
        private static Logger _logger;
        private readonly string _logFilePath;

        private Logger()
        {
            _logFilePath = "CardSorter.log";
            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath);
            }
        }
        public static Logger GetLogger()
        {
            if (_logger==null)
                _logger = new Logger();
            return _logger;
        }

        public void LogWrite(string message)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_logFilePath,true))//adds string to program log
                {
                    sw.WriteLine(DateTime.Now+ ": "+message);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
