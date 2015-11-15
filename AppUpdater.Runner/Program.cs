using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace AppUpdater.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var configInfo = ConfigLoader.LoadConfig(Path.GetDirectoryName(typeof(Program).Assembly.Location));

            bool runLast = args.Any(x => x.Equals("-last", StringComparison.CurrentCultureIgnoreCase));
            if (runLast && configInfo.LastVersion == null)
            {
                Console.WriteLine("Last version is not defined.");
                runLast = false;
            }

            if (runLast)
            {
                ExecuteApplication(configInfo.BaseDirectory, configInfo.LastVersion, configInfo.Executable, args);
            }
            else
            {
                ExecuteApplication(configInfo.BaseDirectory, configInfo.Version, configInfo.Executable, args);
            }
        }

        private static void ExecuteApplication(string baseDir, string version, string executable, string[] args)
        {
            string path = Path.Combine(baseDir, version, executable);
            Process.Start(path, String.Join(" ", args));
        }
    }
}
