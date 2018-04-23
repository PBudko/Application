using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SecondWebApplication
{

    public class FileLogger : ILogger
    {
        string path;

        object locking = new object();

        public FileLogger(string path)
        {
            this.path = path;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //throw new NotImplementedException();
            if (formatter == null)
            {
                lock (locking)
                { File.AppendAllText(path, $"loglevel: {logLevel}, EventId: Id: {eventId.Id} nameevent: {eventId.Name}, Exception: {exception.Message}"); }
            }
            else if(formatter!=null)
            {
                lock(locking)
                {
                    File.AppendAllText(path, formatter(state, exception) + Environment.NewLine);
                }
            }
        }
    }
}
