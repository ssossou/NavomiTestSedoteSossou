using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NavomiApi.Interfaces
{
    public interface ILogService
    {
        void LogInformation(string message, string tripNumber = null);
        void LogWarning(string message, string tripNumber = null);
        void LogError(Exception ex, string message, string tripNumber = null);
    }
}
