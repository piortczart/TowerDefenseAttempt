using System;
using System.Collections.Generic;

namespace TowerDefenseColab.Logging
{
    public class ApplicationLogger : IApplicationLogger
    {
        private enum ApplicationLogType
        {
            Debug,
            Info,
            Error
        }

        public List<string> Logs { get; }

        public ApplicationLogger()
        {
            Logs = new List<string>();
        }

        public void LogInfo(string message)
        {
            AddLog(GetLogFormatted(ApplicationLogType.Info, message));
        }

        public void LogDebug(string message)
        {
            AddLog(GetLogFormatted(ApplicationLogType.Debug, message));
        }

        public void LogError(string message, Exception exception)
        {
            AddLog(GetLogFormatted(ApplicationLogType.Error, message, exception));
        }

        private string GetLogFormatted(ApplicationLogType logType, string message, Exception exception = null)
        {
            return $"{DateTime.Now:HH:mm:ss.fff} {logType}: {message} {exception}";
        }

        public void AddLog(string log)
        {
            Logs.Add(log);
            if (Logs.Count > 10)
            {
                Logs.RemoveRange(0, Logs.Count - 10);
            }
        }
    }
}