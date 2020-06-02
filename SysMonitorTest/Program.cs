using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SysMonitorTest
{
    class Program
    {
        private static  HubConnection _signalRConnection;
        private static IHubProxy _hubProxy;

        static void Main(string[] args)
        {
            ConnectAsync();
            //JobServiceModel jobSvcModel = new JobServiceModel();
            //jobSvcModel.SvcStatusChanged += JobSvcModel_SvcStatusChanged;
            //jobSvcModel.Service.SvcName = "PulseJob";
            while (true)
            {
                if (_signalRConnection.State == ConnectionState.Connected)
                {
                    //int seed = Guid.NewGuid().GetHashCode();
                    //_hubProxy.Invoke("Send", RandomString(25));
                    //jobSvcModel.Service = ServiceMonitor.CheckServices(jobSvcModel);
                    SendProxyMessage(ServiceMonitor.CheckServices("PulseJob"));
                    SendProxyMessage(ServiceMonitor.CheckServices("JobsMonitor"));
                    //_hubProxy.Invoke("Send", $"{jobSvcModel.Service.SvcName} : {jobSvcModel.Service.SvcStatus}");
                    Thread.Sleep(3000);
                }
            }
        }

        private static void SendProxyMessage(ServiceController sc)
        {
            ServiceController svc = sc;
            _hubProxy.Invoke("Send", $"{svc.ServiceName} : {svc.Status}");
        }

        private static void JobSvcModel_SvcStatusChanged(object sender, ServiceChangedEventArgs e)
        {
            _hubProxy.Invoke("Send", $"{e.Service.SvcName} : {e.Service.SvcStatus}");
        }

        private static void ConnectAsync()
        {
            _signalRConnection = new HubConnection("http://localhost:8899");
            _signalRConnection.StateChanged += _signalRConnection_StateChanged;
            _hubProxy = _signalRConnection.CreateHubProxy("MessageHub");
            _hubProxy.On<string>("AddMessage", (message) => Console.WriteLine(message));

            try
            {
                _signalRConnection.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void _signalRConnection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Connected)
                Console.WriteLine("Connected");
            else if (obj.NewState == ConnectionState.Disconnected)
                Console.WriteLine("Disconnected");
        }

        private static int GetInt(RNGCryptoServiceProvider rnd, int max)
        {
            byte[] r = new byte[4];
            int value;
            do
            {
                rnd.GetBytes(r);
                value = BitConverter.ToInt32(r, 0) & Int32.MaxValue;
            } while (value >= max * (Int32.MaxValue / max));
            return value % max;
        }

        private static string RandomString(int length)
        {
            const string valid = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    res.Append(valid[GetInt(rng, valid.Length)]);
                }
            }
            return res.ToString();
        }
    }
}
