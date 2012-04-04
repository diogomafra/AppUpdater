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
                if (args.Length != 3)
                {
                    Console.WriteLine("Usage:\r\n\tAppUpdate.Publisher.exe source destination version");
                }
                else
                {
                    string sourceDirectory = Path.GetFullPath(args[0]);
                    string destinationDirectory = Path.GetFullPath(args[1]);
                    string version = args[2];

                    Console.WriteLine();
                    Console.WriteLine("Publishing version \"{0}\"...", version);
                    Console.WriteLine("Source directory: {0}", sourceDirectory);
                    Console.WriteLine("Destionation directory: {0}", destinationDirectory);
                    Console.WriteLine();

                    new AppPublisher().Publish(sourceDirectory, destinationDirectory, version);

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
