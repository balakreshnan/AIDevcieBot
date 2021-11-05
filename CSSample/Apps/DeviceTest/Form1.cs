using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure.Devices;
using System.Threading;

namespace DeviceTest
{
    public partial class Form1 : Form
    {
        
        
        static string connectionString = "HostName=testdevice.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xxxxxxxxxxx";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            InvokeMethod();
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        private static void InvokeMethod()
        {
            try
            {
                ServiceClient serviceClient;

                serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

                var methodInvocation = new CloudToDeviceMethod("Getsensordata") { ResponseTimeout = TimeSpan.FromSeconds(5) };
                methodInvocation.SetPayloadJson("'{ \"MethodPayload\": \"Payload\" }'");
                //methodInvocation.ConnectionTimeout = TimeSpan.FromSeconds(5);
                CancellationToken canceltoken = new CancellationToken();

                var response = serviceClient.InvokeDeviceMethodAsync("device1", methodInvocation, canceltoken).Result;


                Console.WriteLine("Response status: {0}, payload:", response.Status);
                Console.WriteLine(response.GetPayloadAsJson());
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
