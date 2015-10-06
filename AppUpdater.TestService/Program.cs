using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;

namespace AppUpdater.TestService
{
    public class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<AppService>(s =>
                {
                    s.ConstructUsing(name => new AppService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();

                x.SetDescription("Teste");
                x.SetDisplayName("Teste");
                x.SetServiceName("Teste");
            });
        }
    }
}
