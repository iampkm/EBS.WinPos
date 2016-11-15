using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBS.Infrastructure.Log
{
    public interface ILogger
    {
        void Info(string message);
        void Info(string formateMessage, params object[] args);

        void Error(Exception exception);
        void Error(Exception exception,string message);
        void Error(Exception exception, string formateMessage,params object[] args);

    }
}
