using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppUpdater.Log
{
    public class EmptyLog : ILog
    {
        private static readonly EmptyLog instance = new EmptyLog();

        public static EmptyLog Instance
        {
            get { return instance; }
        }

        public void Info(string message, params object[] values)
        {
        }

        public void Warn(string message, params object[] values)
        {
        }

        public void Error(string message, params object[] values)
        {
        }

        public void Debug(string message, params object[] values)
        {
        }
    }
}
