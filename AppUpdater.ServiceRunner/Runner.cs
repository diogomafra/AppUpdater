using System;
using System.IO;
using System.Reflection;

namespace AppUpdater.ServiceRunner
{
    public class Runner
    {
        private readonly string serviceAssemblyFilename;
        private AppDomain appDomain;
        private ServiceInsideAppDomainProxy serviceProxy;

        public Runner()
        {
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.serviceAssemblyFilename = GetServiceAssemblyFilename(baseDirectory);
        }

        public Runner(string serviceAssemblyFilename)
        {
            this.serviceAssemblyFilename = serviceAssemblyFilename;
        }

        public void Start()
        {
            appDomain = CreateAppDomain();
            var serviceProxyType = typeof(ServiceInsideAppDomainProxy);
            serviceProxy = (ServiceInsideAppDomainProxy)appDomain.CreateInstanceFromAndUnwrap(serviceProxyType.Assembly.Location, serviceProxyType.FullName);
            serviceProxy.LoadService(serviceAssemblyFilename);
            serviceProxy.Start();
        }

        public void Stop()
        {
            if (serviceProxy != null)
            {
                serviceProxy.Stop();
                serviceProxy = null;
                appDomain = null;
            }
        }

        private AppDomain CreateAppDomain()
        {
            var setup = new AppDomainSetup();
            setup.ApplicationBase = Path.GetDirectoryName(serviceAssemblyFilename);
            var appDomain = AppDomain.CreateDomain("AppUpdaterServiceRunner", AppDomain.CurrentDomain.Evidence, setup);
            return appDomain;
        }

        private static string GetServiceAssemblyFilename(string baseDirectory)
        {
            var configInfo = ConfigLoader.LoadConfig(baseDirectory);
            return Path.Combine(configInfo.BaseDirectory, configInfo.Version, configInfo.Executable);
        }
    }
}
