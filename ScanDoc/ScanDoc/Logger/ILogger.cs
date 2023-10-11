using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanDoc.Logger
{
    public interface ILogger
    {
        void Log(string message);
    }
}
