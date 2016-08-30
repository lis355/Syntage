using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Syntage.Framework.Tools
{
    public class Log
    {
        public static Log Instance { get; } = new Log();

        private string _path;
        private bool _writeToFile;

        public event Action<string> OnLog;

        private Log()
        {
        }

        public bool WriteToFile
        {
            get { return _writeToFile; }
            set
            {
                if (value == _writeToFile)
                    return;

                _writeToFile = value;
                if (_writeToFile)
                {
                    _path = string.Format("{0}\\{1}{2}", 
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                        Assembly.GetExecutingAssembly().GetName().Name,
                        "Log.txt");

                    if (File.Exists(_path))
                        File.Delete(_path);

                    File.WriteAllText(_path, string.Empty);
                }
            }
        }

        public static void Print(object message)
        {
            Instance.Write(message);
        }

        private void Write(object message)
        {
            if (WriteToFile)
                File.AppendAllText(_path, string.Format("{0} : {1}{2}",
                    DateTime.Now.ToString(CultureInfo.InvariantCulture), message, Environment.NewLine));

            OnLog?.Invoke(message.ToString());
        }
    }
}
