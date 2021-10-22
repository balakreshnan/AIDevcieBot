using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Threading;

namespace DeviceTestconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "HostName=bbdevicehub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xxx";

            ServiceClient serviceClient;

            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            var methodInvocation = new CloudToDeviceMethod("Getsensordata") { ResponseTimeout = TimeSpan.FromSeconds(5) };
            methodInvocation.SetPayloadJson("{ \"MethodPayload\": \"Temperature\" }");
            //methodInvocation.ConnectionTimeout = TimeSpan.FromSeconds(5);
            CancellationToken canceltoken = new CancellationToken();

            var response = serviceClient.InvokeDeviceMethodAsync("device1", methodInvocation, canceltoken).Result;


            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());

            Console.ReadLine();

        }
    }
}
