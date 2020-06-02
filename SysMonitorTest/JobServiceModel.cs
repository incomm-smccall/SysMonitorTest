using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysMonitorTest
{
    public class Service
    {
        public string SvcStatus { get; set; }
        public string SvcName { get; set; }
        public bool ManualStop { get; set; } = false;
    }

    public class ServiceChangedEventArgs : EventArgs
    {
        public readonly Service Service;
        public ServiceChangedEventArgs(Service service)
        {
            Service = service;
        }
    }

    public class JobServiceModel
    {
        Service service = new Service();
        public event EventHandler<ServiceChangedEventArgs> SvcStatusChanged;
        protected virtual void OnSvcStatusChanged(ServiceChangedEventArgs e)
        {
            SvcStatusChanged?.Invoke(this, e);
        }

        public Service Service
        {
            get => service;
            set
            {
                if (service.SvcStatus == value.SvcStatus) return;
                service = value;
                OnSvcStatusChanged(new ServiceChangedEventArgs(service));
            }
        }
    }
}
