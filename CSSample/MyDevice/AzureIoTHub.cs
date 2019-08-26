using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Devices.Tpm;
using Microsoft.Azure.Devices.Client;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Devices.Management;

namespace MyDevice
{
    static class AzureIoTHub
    {
        static DeviceManagementClient deviceManagementClient;
        private static readonly string DeviceConnectionString = "HostName=bbdevicehub.azure-devices.net;DeviceId=device1;SharedAccessKey=Bw3S/LKNLxawjlSY4cm25ILDwQQtYvSA5AR0wTl+Q70=";

        static DeviceClient deviceClient;

        public static async Task SendDeviceToCloudMessageAsync(string txtSend)
        {
           
            try
            {
                Microsoft.Devices.Tpm.TpmDevice myDevice = new Microsoft.Devices.Tpm.TpmDevice(0); // Use logical device 0 on the TPM by default
                string hubUri = myDevice.GetHostName();
                string deviceId = myDevice.GetDeviceId();
                string sasToken = myDevice.GetSASToken();

                var deviceClient = DeviceClient.Create(
                    hubUri,
                    AuthenticationMethodFactory.
                        CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);

                var str = txtSend + " from " + deviceId + " " + DateTime.Now.ToString();

                var message = new Message(Encoding.ASCII.GetBytes(str));

                await deviceClient.SendEventAsync(message);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static async Task<string> ReceiveCloudToDeviceMessageAsync()
        {
            string retval = string.Empty;
            try
            {


                Microsoft.Devices.Tpm.TpmDevice myDevice = new Microsoft.Devices.Tpm.TpmDevice(0); // Use logical device 0 on the TPM by default
                string hubUri = myDevice.GetHostName();
                string deviceId = myDevice.GetDeviceId();
                string sasToken = myDevice.GetSASToken();

                var deviceClient = DeviceClient.Create(
                    hubUri,
                    AuthenticationMethodFactory.
                        CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);

                while (true)
                {
                    try
                    {
                        Message receivedMessage = await deviceClient.ReceiveAsync();
                        if (receivedMessage == null) continue;
                        var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        await deviceClient.CompleteAsync(receivedMessage);
                        //return messageData;
                        retval = messageData.ToString();
                        //MainPage.ReceiveMessage.Text = retval;
                        return retval;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    

                }
                
            }
            catch (Exception ex)
            {
                retval = ex.Message.ToString();
                Debug.WriteLine("Error in sample: {0}", ex.Message);
            }
            return retval;
        }

        public static async Task<string> ReceiveCloudToDeviceMessageAsyncButton()
        {
            string retval = string.Empty;

            Microsoft.Devices.Tpm.TpmDevice myDevice = new Microsoft.Devices.Tpm.TpmDevice(0); // Use logical device 0 on the TPM by default
            string hubUri = myDevice.GetHostName();
            string deviceId = myDevice.GetDeviceId();
            string sasToken = myDevice.GetSASToken();

            var deviceClient = DeviceClient.Create(
                hubUri,
                AuthenticationMethodFactory.
                    CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);

            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                //return messageData;
                retval += messageData.ToString();
            }

            //while (true)
            //{
            //    var receivedMessage = await deviceClient.ReceiveAsync();

            //    if (receivedMessage != null)
            //    {
            //        var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
            //        await deviceClient.CompleteAsync(receivedMessage);
            //        return messageData;
            //    }

            //    await Task.Delay(TimeSpan.FromSeconds(1));
            //}
            return retval;
        }

        public static async Task SendSenseHatDataToCloudAsync(SenseHatData data)
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Http1);
            var messageInJson = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.UTF8.GetBytes(messageInJson));

            await deviceClient.SendEventAsync(message);
        }

    }
}

