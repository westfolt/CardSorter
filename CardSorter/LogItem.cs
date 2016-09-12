using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CardSorter
{
    class LogItem
    {
        private int year;
        private int month;
        private int day_of_month;
        private string path;
        private FileInfo fileInfo;

        public LogItem(string path)
        {
            Path = path;
            fileInfo = new FileInfo(path);
            Int32.TryParse(fileInfo.Name.Substring(5, 4),out year);
            Int32.TryParse(fileInfo.Name.Substring(10, 2), out month);
            Int32.TryParse(fileInfo.Name.Substring(13, 2), out day_of_month);
        }
        public string Path
        {
            get { return path; }
            private set { path = value; }
        }

        public FileInfo Info
        {
            get { return fileInfo; }
        }

        public int Year
        {
            get { return year; }
        }

        public int Month
        {
            get { return month; }
        }

        public int DayOfMonth
        {
            get { return day_of_month; }
        }
    }
}
