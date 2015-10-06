using AppUpdater.ServiceRunner;
using System.IO;
using System.Reflection;

namespace AppUpdater.TestService
{
    public class AppService
    {
        private Runner serviceRunner;

        public void Start()
        {
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var serviceFilename = Path.Combine(baseDirectory, @"..\..\..\TestApp\bin\Debug\1.2.3\TestApp.exe");
            //var serviceFilename = @"C:\Teste\AppUpdater\TestApp\bin\Debug\1.2.3\TestApp.exe";
            serviceRunner = new Runner();
            serviceRunner.Start();
        }

        public void Stop()
        {
            if (serviceRunner != null)
            {
                serviceRunner.Stop();
                serviceRunner = null;
            }
        }
    }
}
