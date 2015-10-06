using AppUpdater.Service;
using System;
using System.Linq;
using System.Reflection;

namespace AppUpdater.ServiceRunner
{
    internal class ServiceInsideAppDomainProxy : MarshalByRefObject
    {
        private IService service;

        public void LoadService(string path)
        {
            var assembly = Assembly.LoadFrom(path);
            var serviceType = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(IService))).FirstOrDefault();
            if (serviceType == null)
            {
                throw new Exception("Could not find the implementation of IService.");
            }

            service = (IService)Activator.CreateInstance(serviceType);
        }

        public void Start()
        {
            if (service == null)
            {
                throw new InvalidOperationException("The service must be loaded. Call LoadService first.");
            }

            service.Start();
        }

        public void Stop()
        {
            if (service == null)
            {
                throw new InvalidOperationException("The service must be loaded. Call LoadService first.");
            }

            service.Stop();
        }
    }
}
