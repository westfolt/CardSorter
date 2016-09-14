using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Ionic.BZip2;
using Ionic.Zip;
using Ionic.Zlib;

namespace CardSorter
{
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();//clear command prompt on program start
            UserInterface.ProgramStart();
            Console.ReadKey();//убрать позже, задержки быть не должно!!!!!!!!!!!!!

#region Data Input
            string[] argumentsHandled = UserInterface.InputHandle(args);
            if (argumentsHandled==null)//если после обработки аргументов нет - завершаем программу
                return;
            string pathFrom = argumentsHandled[0]; //@"D:\prog\C#\Visual Studio\real\For SorterCard\1";
            string pathTo = argumentsHandled[1];//добавить забрать степень сжатия!!!!!!
#endregion

#region File Analyzing
            FileSystemTasks fileSystem = new FileSystemTasks(pathFrom,pathTo);//Объект для работы с файлами
            fileSystem.AnalyzeIt();//запускаем анализ папки с логами
            if (fileSystem.LogsCollection.Count == 0)
            {
                Console.WriteLine("Press any key to close program");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
#endregion

#region File Moving
            fileSystem.MoveIt();//запускаем логику создания папок и перемещения файлов
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
#endregion

#region File Archiving
            fileSystem.ArchivateIt();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
#endregion
            Console.WriteLine("!");
            Console.ReadKey();
        }
    }
}