using System;

namespace TowerDefenseColab.Logging
{
    public interface IApplicationLogger
    {
        void LogInfo(string message);
        void LogDebug(string message);
        void LogError(string message, Exception exception);
    }
}
