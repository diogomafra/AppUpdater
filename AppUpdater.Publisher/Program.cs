using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AppUpdater.Publisher
{
    class Program
    {
        private string sourceDirectory;
        private string targetDirectory;
        private string sourceDirectoryPath;
        private string targetDirectoryPath;
        private string version = null;
        private int? numberOfVersionsAsDelta = null;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:\r\n\tAppUpdate.Publisher.exe -source:source_dir -target:target_dir -version:1.0.0 -deltas:2");
            }
            else
            {
                new Program().Execute(args);
            }
        }

        public void Execute(string[] args)
        {
            try
            {
                ProcessArgs(args);
                ValidateArgs();
                PublishVersion();
            }
            catch (Exception err)
            {
                Console.WriteLine();
                Console.WriteLine("Error: ");
                Console.WriteLine(err.Message);
            }
        }

        private void PublishVersion()
        {
            Console.WriteLine();
            Console.WriteLine("Publishing version \"{0}\"...", version);
            Console.WriteLine("Source directory: {0}", sourceDirectoryPath);
            Console.WriteLine("Target directory: {0}", targetDirectoryPath);
            if (numberOfVersionsAsDelta.HasValue)
            {
                Console.WriteLine("Generating delta information for the latest {0} versions.", numberOfVersionsAsDelta);
            }
            Console.WriteLine();

            new AppPublisher().Publish(sourceDirectory, targetDirectoryPath, version, numberOfVersionsAsDelta ?? 0);

            Console.WriteLine("Publish succeeded.");
        }

        private void ProcessArgs(string[] args)
        {
            foreach (var arg in args)
            {
                if (!arg.StartsWith("-"))
                {
                    throw new Exception("Invalid argument: " + arg);
                }

                string[] argValues = arg.Split(new []{':'}, 2);
                string commandName = argValues[0].Remove(0, 1);
                string commandValue = argValues.Length == 1 ? null : argValues[1];

                switch (commandName.ToLower())
                {
                    case "source":
                        sourceDirectory = commandValue;
                        break;
                    case "target":
                        targetDirectory = commandValue;
                        break;
                    case "version":
                        version = commandValue;
                        break;
                    case "deltas":
                        int deltas;
                        if (!int.TryParse(commandValue, out deltas))
                        {
                            throw new Exception("The 'delta' argument is not a valid number.");
                        }
                        numberOfVersionsAsDelta = deltas;
                        break;
                    default:
                        throw new Exception("Unknown argument: " + arg);
                }
            }
        }

        private void ValidateArgs()
        {
            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrWhiteSpace(sourceDirectory))
            {
                sb.AppendLine("The 'source' argument is required.");
            }
            else
            {
                sourceDirectoryPath = Path.GetFullPath(sourceDirectory);
                if (!Directory.Exists(sourceDirectoryPath))
                {
                    throw new Exception(String.Format("The directory '{0}' could not be found.", sourceDirectoryPath));
                }
            }

            if (String.IsNullOrWhiteSpace(targetDirectory))
            {
                sb.AppendLine("The 'target' argument is required.");
            }
            else
            {
                targetDirectoryPath = Path.GetFullPath(targetDirectory);
                if (!Directory.Exists(targetDirectoryPath))
                {
                    throw new Exception(String.Format("The directory '{0}' could not be found.", targetDirectoryPath));
                }
            }

            if (String.IsNullOrWhiteSpace(version))
            {
                sb.AppendLine("The 'version' argument is required.");
            }

            if (sb.Length > 0)
            {
                throw new Exception(sb.ToString());
            }
        }
    }
}
