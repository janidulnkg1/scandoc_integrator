using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanDoc.Logger
{
    public class Logger: ILogger
    {
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public void Log(string message)
        {
            _logger.Info(message);
        }
    }
}
