using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace AppUpdater.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(dir, "config.xml"));

            string version = doc.SelectSingleNode("config/version").InnerText;
            string executable = doc.SelectSingleNode("config/executable").InnerText;

            string executableFile = Path.Combine(Path.Combine(dir, version), executable);
            Process.Start(executableFile, String.Join(" ", args));
        }
    }
}
