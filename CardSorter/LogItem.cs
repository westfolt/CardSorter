using System.IO;

namespace CardSorter
{
    class LogItem
    {
        private readonly int _year;
        private readonly int _month;
        private readonly int _dayOfMonth;
        private readonly FileInfo _fileInfo;

        public LogItem(string path)
        {
            Path = path;
            _fileInfo = new FileInfo(path);
            int.TryParse(_fileInfo.Name.Substring(5, 4),out _year);
            int.TryParse(_fileInfo.Name.Substring(10, 2), out _month);
            int.TryParse(_fileInfo.Name.Substring(13, 2), out _dayOfMonth);
        }
        public string Path { get; private set; }

        public FileInfo Info
        {
            get { return _fileInfo; }
        }

        public int Year
        {
            get { return _year; }
        }

        public int Month
        {
            get { return _month; }
        }

        public int DayOfMonth
        {
            get { return _dayOfMonth; }
        }
    }
}
