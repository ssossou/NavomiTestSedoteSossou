using NLog;
using NavomiApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NavomiApi.Models;

namespace NavomiApi.Services
{
    public class LogService : ILogService
    {
        public void LogInformation(string message, string user = null)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var log = new LogEventInfo(LogLevel.Info, "", message);
            log.Properties["user"] = user;

            logger.Log(log);
        }

        public void LogWarning(string message, string user = null)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var log = new LogEventInfo(LogLevel.Warn, "", message);
            log.Properties["user"] = user;

            logger.Log(log);
        }

        public void LogError(Exception ex, string message, string user = null)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var log = new LogEventInfo(LogLevel.Error, "", message);
            log.Exception = ex;

            log.Properties["user"] = user;

            logger.Log(log);
        }
    }
}
