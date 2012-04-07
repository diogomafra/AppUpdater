using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AppUpdater.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 3 || args.Length > 4)
                {
                    Console.WriteLine("Usage:\r\n\tAppUpdate.Publisher.exe source destination [diff_versions] version");
                }
                else
                {
                    string sourceDirectory = Path.GetFullPath(args[0]);
                    string destinationDirectory = Path.GetFullPath(args[1]);
                    string version = args.Length == 3 ? args[2] : args[3];
                    int numberOfVersionsAsDelta = args.Length == 3 ? 0 : int.Parse(args[2]);

                    Console.WriteLine();
                    Console.WriteLine("Publishing version \"{0}\"...", version);
                    Console.WriteLine("Source directory: {0}", sourceDirectory);
                    Console.WriteLine("Destionation directory: {0}", destinationDirectory);
                    Console.WriteLine("Generating delta information for the latest {0} versions.", numberOfVersionsAsDelta);
                    Console.WriteLine();

                    new AppPublisher().Publish(sourceDirectory, destinationDirectory, version, numberOfVersionsAsDelta);

                    Console.WriteLine("Publish succeeded.");
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Erro: " + err.ToString());
            }
        }
    }
}
