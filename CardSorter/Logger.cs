using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardSorter
{
    class Logger//singleton logger
    {
        private static Logger logger;
        private string logFilePath;

        private Logger()
        {
            logFilePath = "CardSorter.log";
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath);
            }
        }
        public static Logger GetLogger()
        {
            if (logger==null)
                logger = new Logger();
            return logger;
        }

        public void LogWrite(string message)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(logFilePath,true))//добавляет строку в лог-файл
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
