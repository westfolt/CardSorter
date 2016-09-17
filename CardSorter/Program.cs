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
#region Program start
            Console.Clear();//clear command prompt on program start
            UserInterface.ProgramStart();
#endregion

#region Data Input
            string[] argumentsHandled = UserInterface.InputHandle(args);
            if (argumentsHandled == null) //if there are no arguments - shutting down program
            {
                UserInterface.Logger.LogWrite("Program was shut down because of wrong arguments input");//to log
                return;
            }
            string pathFrom = argumentsHandled[0];
            string pathTo = argumentsHandled[1];
            int compressionLevel = Convert.ToInt32(argumentsHandled[2]);
            UserInterface.Logger.LogWrite("Successfull program start. Input folder: " + pathFrom + " , output folder: " + pathTo + 
                " , compression level: " + compressionLevel);//to log
#endregion

#region File Analyzing
            FileSystemTasks fileSystem = new FileSystemTasks(pathFrom,pathTo,compressionLevel);//object for work with filesystem
            fileSystem.AnalyzeIt();//starting analyze of input folder
            if (fileSystem.LogsCollection.Count == 0)
            {
                Console.WriteLine("Press any key to close program");
                Console.ReadKey();
                UserInterface.Logger.LogWrite("Program stopped because no *.log files were found in selected folder");//to log
                return;
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
#endregion

#region File Moving
            fileSystem.MoveIt();//starting logic of folder creation and files moving
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
#endregion

#region File Archiving
            fileSystem.ArchivateIt();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
#endregion
            UserInterface.Logger.LogWrite("Program finished correctly");//to log
        }
    }
}