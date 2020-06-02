using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SysMonitorTest
{
    public static class ServiceMonitor
    {
        public static Service CheckServices(JobServiceModel jobSvcModel)
        {
            using (ServiceController sc = new ServiceController())
            {
                sc.ServiceName = jobSvcModel.Service.SvcName;
                jobSvcModel.Service.SvcStatus = sc.Status.ToString();
                return jobSvcModel.Service;
            }
        }

        public static ServiceController CheckServices(string svcName)
        {
            return new ServiceController(svcName);
        }
    }
}
