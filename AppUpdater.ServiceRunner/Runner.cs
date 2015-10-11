using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace AppUpdater.ServiceRunner
{
    public class Runner
    {
        private readonly string serviceAssemblyFilename;
        private AppDomain appDomain;
        private ServiceInsideAppDomainProxy serviceProxy;

        public Runner() : this(null)
        {
        }

        public Runner(string serviceAssemblyFilename)
        {
            this.serviceAssemblyFilename = serviceAssemblyFilename;
        }

        public void Start()
        {
            var assemblyFilename = serviceAssemblyFilename;
            if (assemblyFilename == null)
            {
                assemblyFilename = GetServiceAssemblyFilename();
            }

            appDomain = CreateAppDomain();
            var serviceProxyType = typeof(ServiceInsideAppDomainProxy);
            serviceProxy = (ServiceInsideAppDomainProxy)appDomain.CreateInstanceFromAndUnwrap(serviceProxyType.Assembly.Location, serviceProxyType.FullName);
            serviceProxy.LoadService(assemblyFilename);
            serviceProxy.Start();
        }

        public void Stop()
        {
            if (serviceProxy != null)
            {
                serviceProxy.Stop();
                serviceProxy = null;
                AppDomain.Unload(appDomain);
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

        private static string GetServiceAssemblyFilename()
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configFilename = Path.Combine(baseDir, "config.xml");
            var doc = new XmlDocument();
            doc.Load(configFilename);

            string version = GetConfigValue(doc, "version");
            string lastVersion = GetConfigValue(doc, "last_version");
            string executable = GetConfigValue(doc, "executable");
            string customBaseDir = GetConfigValue(doc, "base_directory");
            if (!String.IsNullOrEmpty(customBaseDir))
            {
                baseDir = customBaseDir;
            }

            string servicePath = Path.Combine(baseDir, version, executable);
            return servicePath;
        }

        private static string GetConfigValue(XmlDocument doc, string name)
        {
            var node = doc.SelectSingleNode("config/" + name);
            return node == null ? null : node.InnerText;
        }
    }
}
