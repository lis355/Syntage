using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Syntage.Framework.Tools
{
    public class Log
    {
        private readonly string _path;

        private Log()
        {
            _path = string.Format("{0}\\{1}{2}",Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Assembly.GetExecutingAssembly().GetName().Name, "Log.txt");

            if (File.Exists(_path))
                File.Delete(_path);

            File.WriteAllText(_path, string.Empty);
        }

        public event Action<string> OnLog;

        public static Log Instance { get; } = new Log();

        public static void Print(object message)
        {
            Instance.Write(message);
        }

        private void Write(object message)
        {
#if LOG_ENABLE
            File.AppendAllText(_path, string.Format("{0} : {1}{2}", DateTime.Now.ToString(CultureInfo.InvariantCulture), message, Environment.NewLine));

            OnLog?.Invoke(message.ToString());
#endif
        }
    }
}
