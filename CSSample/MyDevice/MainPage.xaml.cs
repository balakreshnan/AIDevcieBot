using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Devices.Management;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Devices.Tpm;
using Emmellsoft.IoT.Rpi.SenseHat;
using Emmellsoft.IoT.Rpi.SenseHat.Fonts;
using Emmellsoft.IoT.Rpi.SenseHat.Fonts.MultiColor;
using Windows.UI;
using Emmellsoft.IoT.Rpi.SenseHat.Fonts.SingleColor;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MyDevice
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        DeviceManagementClient deviceManagementClient;
        private readonly string DeviceConnectionString = "HostName=bbdevicehub.azure-devices.net;DeviceId=device1;SharedAccessKey=xxxxx";

        DeviceClient deviceClient;

        public MainPage()
        {
            this.InitializeComponent();
            this.InitializeDeviceClientAsync();
            Task.Delay(2000);
            //this.CheckDeviceStatus();
            //this.ReceiveCloudToDeviceMessageAsync();
            //this.UpdateDeviceStatus();

            Task.Delay(1000);

            //this will enable real device data
            this.SendDeviceToCloudMessagesAsync();

            //if data needs to be simulated use this
            //this.SendDeviceToCloudMessagesAsyncSimulated();

            Task.Delay(2000);
            this.ReceiveCloudToDeviceMessageAsync();

            //Start().Wait();     
            //this.Loaded += MainPage_Loaded;

            Task.Delay(2000);
            this.display();


        }

        private void ClickMe_Click(object sender, RoutedEventArgs e)
        {
            AzureIoTHub.SendDeviceToCloudMessageAsync(txtSend.Text);
            HelloMessage.Text = "Sent Message to Cloud " + DateTime.Now.ToString();
        }

        private async void Receive_Click(object sender, RoutedEventArgs e)
        {
            String retval = await AzureIoTHub.ReceiveCloudToDeviceMessageAsyncButton();
            ReceiveMessage.Text = "Received Message From Cloud " + retval + " " + DateTime.Now.ToString();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            HelloMessage.Text = "Ready Send data ";
            ReceiveMessage.Text = "Ready To receive data ";
        }

        public async Task Start()
        {
            try
            {

                deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);

                await deviceClient.OpenAsync();

                await deviceClient.SetMethodHandlerAsync("Getsensordata", Getsensordata, null);

            }
            catch (Exception ex)
            {

                Debug.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        public Task<MethodResponse> Getsensordata(MethodRequest methodRequest, object userContext)
        {
            string retval = string.Empty;
            MethodResponse retValue;

            try
            {
                JObject obj = JObject.Parse(methodRequest.DataAsJson);
                List<sensorreading> alllist = new List<sensorreading>();
                Random rd = new Random(1);
                foreach (var tmpo in obj)
                {
                    String key = tmpo.Key.ToString();
                    string sensortype = tmpo.Value.ToString().ToLower();

                    

                    //get a reference to SenseHat
                    senseHat = SenseHatFactory.GetSenseHat().Result;

                    senseHat.Sensors.ImuSensor.Update();
                    senseHat.Sensors.HumiditySensor.Update();
                    senseHat.Sensors.PressureSensor.Update();
                    

                    sensorreading one = new sensorreading();

                    switch (sensortype)
                    {
                        case "temperature":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.Temperature.ToString();
                            break;
                        case "humidity":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.Humidity.ToString();
                            break;
                        case "pressure":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.Pressure.ToString();
                            break;
                        case "weight":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.Pressure.ToString();
                            break;
                        case "acceleration":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.Acceleration.Value.ToString();
                            break;
                        case "speed":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.Acceleration.Value.ToString();
                            break;
                        case "gyro":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.Gyro.Value.ToString();
                            break;
                        case "orientation":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.Gyro.Value.ToString();
                            break;
                        case "pose":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.Pose.Value.ToString();
                            break;
                        case "magneticfield":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.MagneticField.Value.ToString();
                            break;
                        case "compass":
                            one.sensortype = sensortype;
                            one.sensorvalue = senseHat.Sensors.MagneticField.Value.ToString();
                            break;
                        default:
                            one.sensortype = sensortype;
                            one.sensorvalue = one.sensorvalue = rd.Next(1, 200).ToString();
                            break;
                    }

                    alllist.Add(one);
                    
                    
                    //retval = "{\"name\":\"" + rd.Next(1, 200).ToString() + "\"}"; // + "," + methodRequest.DataAsJson.ToString();
                    //one.sensorvalue = rd.Next(1, 200).ToString();
                    


                }
                
                retval = JsonConvert.SerializeObject(alllist);
                
                retValue = new MethodResponse(Encoding.UTF8.GetBytes(retval), 200);

            }
            catch (Exception ex)
            {

                //throw;
                retval = "No Sensor data found for this device ";
                retValue = new MethodResponse(Encoding.UTF8.GetBytes(retval), 200);
            }


            //return Task.FromResult(new MethodResponse(new byte[0], 200));
            return Task.FromResult(retValue);

        }

        public Task<MethodResponse> reboot(MethodRequest methodRequest, object userContext)
        {
            string retval = string.Empty;
            MethodResponse retValue;

            try 
            {

                

                retValue = new MethodResponse(Encoding.UTF8.GetBytes(retval), 200);
            }
            catch (Exception ex)
            {

                retValue = new MethodResponse(Encoding.UTF8.GetBytes(retval), 200);
            }
            return Task.FromResult(retValue);
        }

        public async Task<bool> IsSystemRebootAllowed()
        {
            return true;
        }

        private async Task InitializeDeviceClientAsync()
        {
            // Create DeviceClient. Application uses DeviceClient for telemetry messages, device twin
            // as well as device management
            //DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
            deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
            //deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

            // IDeviceTwin abstracts away communication with the back-end.
            // AzureIoTHubDeviceTwinProxy is an implementation of Azure IoT Hub
            IDeviceTwin deviceTwinProxy = new AzureIoTHubDeviceTwinProxy(deviceClient);

            try
            {

                // IDeviceManagementRequestHandler handles device management-specific requests to the app,
                // such as whether it is OK to perform a reboot at any givem moment, according to the app 
                // business logic.
                // DMRequestHandler is the Toaster app implementation of the interface
                IDeviceManagementRequestHandler appRequestHandler = new DMRequestHandler(this);

                // Create the DeviceManagementClient, the main entry point into device management
                this.deviceManagementClient = await DeviceManagementClient.CreateAsync(deviceTwinProxy, appRequestHandler);

                // Set the callback for desired properties update. The callback will be invoked
                // for all desired properties -- including those specific to device management
                await deviceClient.SetDesiredPropertyUpdateCallback(OnDesiredPropertyUpdate, null);

                await this.deviceClient.OpenAsync();
                connectionstatus.Text = "Connected";

                // Set up callbacks:
                await deviceClient.SetMethodHandlerAsync("Getsensordata", Getsensordata, null);

                //this.UpdateDeviceStatus();

            }
            catch (Exception ex )
            {

                Debug.WriteLine("Error in sample InitializeDeviceClientAsync: {0}", ex.Message);
            }

            

        }

        public Task OnDesiredPropertyUpdate(TwinCollection desiredProperties, object userContext)
        {
            // Let the device management client process properties specific to device management
            this.deviceManagementClient.ProcessDeviceManagementProperties(desiredProperties);

            // Application developer can process all the top-level nodes here
            return Task.CompletedTask;
        }

        public Task CloseAsync()
        {
            return this.deviceClient.CloseAsync();
        }

        private async Task CheckDeviceStatus()
        {
            try
            {
                deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
                //deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

                // IDeviceTwin abstracts away communication with the back-end.
                // AzureIoTHubDeviceTwinProxy is an implementation of Azure IoT Hub
                //IDeviceTwin deviceTwinProxy = new AzureIoTHubDeviceTwinProxy(deviceClient);

                await this.deviceClient.OpenAsync();
                connectionstatus.Text = "Connected";

            }
            catch (Exception ex)
            {
                connectionstatus.Text = "DisConnected";
                Debug.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        public async Task ReceiveCloudToDeviceMessageAsync()
        {
            string retval = string.Empty;
            try
            {


                //Microsoft.Devices.Tpm.TpmDevice myDevice = new Microsoft.Devices.Tpm.TpmDevice(0); // Use logical device 0 on the TPM by default
                //string hubUri = myDevice.GetHostName();
                //string deviceId = myDevice.GetDeviceId();
                //string sasToken = myDevice.GetSASToken();

                //var deviceClientrev = DeviceClient.Create(
                //    hubUri,
                //    AuthenticationMethodFactory.
                //        CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);

                

                while (true)
                {
                    try
                    {
                        var deviceClientrev = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
                        await deviceClientrev.SetMethodHandlerAsync("Getsensordata", Getsensordata, null);

                        Message receivedMessage = await deviceClientrev.ReceiveAsync();
                        if (receivedMessage == null)
                        {
                            Task.Delay(1000).Wait();
                            continue;                            
                        }
                        var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        await deviceClientrev.CompleteAsync(receivedMessage);
                        //return messageData;
                        retval = messageData.ToString();
                        //MainPage.ReceiveMessage.Text = retval;
                        ReceiveMessage.Text = retval;

                    }
                    catch (Exception ex)
                    {

                        Debug.WriteLine("Error in sample ReceiveCloudToDeviceMessageAsync while: {0}", ex.Message);
                    }

                    Task.Delay(1000).Wait();
                }

            }
            catch (Exception ex)
            {
                retval = ex.Message.ToString();
                Debug.WriteLine("Error in sample ReceiveCloudToDeviceMessageAsync: {0}", ex.Message);
            }
        }

        public async Task UpdateDeviceStatus()
        {
            try
            {
                while(true)
                {
                    var deviceClientupd = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
                    //deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

                    // IDeviceTwin abstracts away communication with the back-end.
                    // AzureIoTHubDeviceTwinProxy is an implementation of Azure IoT Hub
                    //IDeviceTwin deviceTwinProxy = new AzureIoTHubDeviceTwinProxy(deviceClient);

                    try
                    {
                        if(deviceClientupd == null)
                        {
                            deviceClientupd = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
                        }
                        deviceClientupd.RetryPolicy = RetryPolicyType.No_Retry;
                        await deviceClientupd.OpenAsync();
                        connectionstatus.Text = "Connected " + DateTime.Now.ToString();
                    }
                    catch (Exception ex)
                    {

                        connectionstatus.Text = "DisConnected";
                        Debug.WriteLine("Error in sample: {0}", ex.Message);
                    }
                    await deviceClientupd.CloseAsync();
                    //update status every minute.
                    await Task.Delay(60000);
                    

                }
                

            }
            catch (Exception ex)
            {
                connectionstatus.Text = "Cannot connect";
                Debug.WriteLine("Error in sample UpdateDeviceStatus: {0}", ex.Message);
            }
        }

        private async void SendDeviceToCloudMessagesAsyncSimulated()
        {

            Microsoft.Devices.Tpm.TpmDevice myDevice = new Microsoft.Devices.Tpm.TpmDevice(0); // Use logical device 0 on the TPM by default
            string hubUri = myDevice.GetHostName();
            string deviceId = myDevice.GetDeviceId();
            string sasToken = myDevice.GetSASToken();

            double minTemperature = 20;
            double minHumidity = 60;
            double minPressure = 30;
            Random rand = new Random();

            //var deviceClient = DeviceClient.Create(
            //       hubUri,
            //       AuthenticationMethodFactory.
            //           CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);
            //String _deviceConnectionString = "HostName=bbdevicehub.azure-devices.net;DeviceId=device1;SharedAccessKey=aF5CUyJjhbVGPnIUVxFdmJAgCsWXpVsaEgwcTmRCMwQ=";
            //String _deviceConnectionString = "HostName=bbdevicehub.azure-devices.net;DeviceId=device1;SharedAccessKey=Bw3S/LKNLxawjlSY4cm25ILDwQQtYvSA5AR0wTl+Q70=";
            


            while (true)
            {
                try
                {
                    //deviceClient = DeviceClient.CreateFromConnectionString(_deviceConnectionString, TransportType.Mqtt);
                    deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Http1);

                    double currentTemperature = minTemperature + rand.NextDouble() * 15;
                    double currentHumidity = minHumidity + rand.NextDouble() * 20;
                    double currentPressure = minPressure + rand.NextDouble() * 5;

                    //notify UI
                    TempText.Text = currentTemperature.ToString();
                    TempHumidity.Text = currentHumidity.ToString();
                    TempPressure.Text = currentPressure.ToString();

                    var telemetryDataPoint = new
                    {
                        deviceId = deviceId,
                        temperature = currentTemperature,
                        humidity = currentHumidity,
                        pressure = currentPressure
                    };
                    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    string levelValue;

                    if (rand.NextDouble() > 0.7)
                    {
                        messageString = "This is a critical message";
                        levelValue = "critical";
                    }
                    else
                    {
                        levelValue = "normal";
                    }

                    var message = new Message(Encoding.ASCII.GetBytes(messageString));
                    message.Properties.Add("level", levelValue);

                    await deviceClient.SendEventAsync(message);
                    //Console.WriteLine("{0} > Sent message: {1}", DateTime.Now, messageString);

                    
                }
                catch (Exception ex)
                {

                    Debug.WriteLine("Error in sample: {0}", ex.Message);
                }
                await Task.Delay(20000);
            }
        }

        private async void SendDeviceToCloudMessagesAsync()
        {

            Microsoft.Devices.Tpm.TpmDevice myDevice = new Microsoft.Devices.Tpm.TpmDevice(0); // Use logical device 0 on the TPM by default
            string hubUri = myDevice.GetHostName();
            string deviceId = myDevice.GetDeviceId();
            string sasToken = myDevice.GetSASToken();

            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                try
                {
                    //deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
                    var deviceClientsend = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Http1);

                    double currentTemperature = minTemperature + rand.NextDouble() * 15;
                    double currentHumidity = minHumidity + rand.NextDouble() * 20;

                    //get a reference to SenseHat
                    senseHat = await SenseHatFactory.GetSenseHat();

                    senseHat.Sensors.ImuSensor.Update();
                    senseHat.Sensors.HumiditySensor.Update();
                    senseHat.Sensors.PressureSensor.Update();
                    
                    

                    //gather data
                    SenseHatData data = new SenseHatData();
                    data.Temperature = senseHat.Sensors.Temperature;
                    data.Humidity = senseHat.Sensors.Humidity;
                    data.Pressure = senseHat.Sensors.Pressure;

                    //notify UI
                    TempText.Text = data.Temperature.ToString();
                    TempHumidity.Text = data.Humidity.ToString();
                    TempPressure.Text = data.Pressure.ToString();

                    if (senseHat.Sensors.Acceleration.HasValue)
                    {
                        Acceleration al = new Acceleration();
                        al.X = senseHat.Sensors.Acceleration.Value.X;
                        al.Y = senseHat.Sensors.Acceleration.Value.Y;
                        al.Z = senseHat.Sensors.Acceleration.Value.Z;

                        TempAcceleration.Text = " X= " + al.X.ToString() + " Y= " + al.Y.ToString() + " Z= " + al.Z.ToString(); 

                        data.Acceleration = al;
                    }
                    if (senseHat.Sensors.Gyro.HasValue)
                    {
                        Gyro gy = new Gyro();
                        gy.X = senseHat.Sensors.Gyro.Value.X;
                        gy.Y = senseHat.Sensors.Gyro.Value.Y;
                        gy.Z = senseHat.Sensors.Gyro.Value.Z;

                        TempGyro.Text = " X= " + gy.X.ToString() + " Y= " + gy.Y.ToString() + " Z= " + gy.Z.ToString();

                        data.Gyro = gy;
                    }
                    if (senseHat.Sensors.Pose.HasValue)
                    {
                        Pose po = new Pose();
                        po.X = senseHat.Sensors.Pose.Value.X;
                        po.Y = senseHat.Sensors.Pose.Value.Y;
                        po.Z = senseHat.Sensors.Pose.Value.Z;

                        TempPose.Text = " X= " + po.X.ToString() + " Y= " + po.Y.ToString() + " Z= " + po.Z.ToString();

                        data.po = po;
                    }
                    if (senseHat.Sensors.MagneticField.HasValue)
                    {
                        MagneticField mf = new MagneticField();
                        mf.X = senseHat.Sensors.MagneticField.Value.X;
                        mf.Y = senseHat.Sensors.MagneticField.Value.Y;
                        mf.Z = senseHat.Sensors.MagneticField.Value.Z;

                        TempMagneticField.Text = " X= " + mf.X.ToString() + " Y= " + mf.Y.ToString() + " Z= " + mf.Z.ToString();

                        data.mf = mf;
                    }


                    data.deviceID = deviceId;

                    //send them to the cloud
                    //await AzureIoTHub.SendSenseHatDataToCloudAsync(data);



                    var messageString = JsonConvert.SerializeObject(data);
                    string levelValue;

                    //if (rand.NextDouble() > 0.7)
                    //{
                    //    messageString = "This is a critical message";
                    //    levelValue = "critical";
                    //}
                    //else
                    //{
                    //    levelValue = "normal";
                    //}
                    levelValue = "normal";
                    var message = new Message(Encoding.ASCII.GetBytes(messageString));
                    message.Properties.Add("level", levelValue);

                    if (deviceClientsend == null)
                    {
                        deviceClientsend = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
                    }

                    await deviceClientsend.SendEventAsync(message);
                    //Console.WriteLine("{0} > Sent message: {1}", DateTime.Now, messageString);

                    //string _scrollText = "Hello Pi 3";

                    ////now update the Sensor HAT UI
                    //// Create the font from the image.
                    //MultiColorFont font = MultiColorFont.LoadFromImage(
                    //    new Uri("ms-appx:///Assets/ColorFont.png"),
                    //    " ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖÉÜabcdefghijklmnopqrstuvwxyzåäöéü0123456789.,?!\"#$%&-+*:;/\\<>()'`=",
                    //    Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF)).Result;

                    //// Get the characters to scroll.
                    //IEnumerable<MultiColorCharacter> characters = font.GetChars(_scrollText);

                    //// Choose a background color (or draw your own more complex background!)
                    //Color backgroundColor = Color.FromArgb(0xFF, 0x00, 0x20, 0x00);

                    //// Create the character renderer.
                    //var characterRenderer = new MultiColorCharacterRenderer();

                    //// Create the text scroller.
                    //var textScroller = new TextScroller<MultiColorCharacter>(
                    //    senseHat.Display,
                    //    characterRenderer,
                    //    characters);

                    //// Clear the display.
                    //senseHat.Display.Fill(backgroundColor);

                    //// Draw the scroll text.
                    //textScroller.Render();

                    //// Update the physical display.
                    //senseHat.Display.Update();

                    connectionstatus.Text = "Connected " + DateTime.Now.ToString();

                }
                catch (Exception ex)
                {

                    Debug.WriteLine("Error in sample SendDeviceToCloudMessagesAsync: {0}", ex.Message);
                }
                await Task.Delay(30000);
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //get a reference to SenseHat
            senseHat = await SenseHatFactory.GetSenseHat();
            //initialize the timer
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
        {
            senseHat.Sensors.HumiditySensor.Update();
            senseHat.Sensors.PressureSensor.Update();

            //gather data
            SenseHatData data = new SenseHatData();
            data.Temperature = senseHat.Sensors.Temperature;
            data.Humidity = senseHat.Sensors.Humidity;
            data.Pressure = senseHat.Sensors.Pressure;

            //send them to the cloud
            await AzureIoTHub.SendSenseHatDataToCloudAsync(data);

            //notify UI
            TempText.Text = data.Temperature.ToString();
            TempHumidity.Text = data.Humidity.ToString();
            TempPressure.Text = data.Pressure.ToString();
        }

        ISenseHat senseHat;

        private string _scrollText;

        private enum RenderMode
        {
            YellowOnBlue,
            BlackOnStaticRainbow,
            BlackOnMovingRainbow,
            StaticRainbowOnBlack,
            MovingRainbowOnBlack,
        }

        private readonly Color[,] _rainbowColors = new Color[8, 8];
        private RenderMode _currentMode;

        public async Task display()
        {
            try
            {
                _scrollText = "Device Bot - Azure IoT Hub";
                // Get a copy of the rainbow colors.
                senseHat = await SenseHatFactory.GetSenseHat();
                senseHat.Display.Reset();
                senseHat.Display.CopyScreenToColors(_rainbowColors);

                // Recreate the font from the serialized bytes.
                SingleColorFont font = SingleColorFont.Deserialize(FontBytes);

                // Get the characters to scroll.
                IEnumerable<SingleColorCharacter> characters = font.GetChars(_scrollText);

                // Create the character renderer.
                SingleColorCharacterRenderer characterRenderer = new SingleColorCharacterRenderer(GetCharacterColor);

                // Create the text scroller.
                var textScroller = new TextScroller<SingleColorCharacter>(
                    senseHat.Display,
                    characterRenderer,
                    characters);

                while (true)
                {
                    // Step the scroller.
                    if (!textScroller.Step())
                    {
                        // Reset the scroller when reaching the end.
                        textScroller.Reset();
                    }

                    // Draw the background.
                    FillDisplay(textScroller.ScrollPixelOffset);

                    // Draw the scroll text.
                    textScroller.Render();

                    // Update the physical display.
                    senseHat.Display.Update();

                    // Should the drawing mode change?
                    if (senseHat.Joystick.Update() && (senseHat.Joystick.EnterKey == KeyState.Pressing))
                    {
                        // The middle button is just pressed.
                        SwitchToNextScrollMode();
                    }

                    // Pause for a short while.
                    //Task.wait(TimeSpan.FromMilliseconds(50));
                    await Task.Delay(500);

                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private Color GetCharacterColor(SingleColorCharacterRendererPixelMap pixelMap)
        {
            switch (_currentMode)
            {
                case RenderMode.YellowOnBlue:
                    return Colors.Yellow;

                case RenderMode.BlackOnStaticRainbow:
                    return Colors.Black;

                case RenderMode.BlackOnMovingRainbow:
                    return Colors.Black;

                case RenderMode.StaticRainbowOnBlack:
                    // Let the rainbow colors be "pinned" to the display.
                    return _rainbowColors[pixelMap.DisplayPixelX, pixelMap.DisplayPixelY];

                case RenderMode.MovingRainbowOnBlack:
                    // Let the rainbow colors move with the characters ("restarting" on each character).
                    return _rainbowColors[pixelMap.CharPixelX, pixelMap.CharPixelY];

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<byte> FontBytes
        {
            get
            {
                // The following bytes are generated using the 'SingleColorFontBuilder' class found in the 'RPi.SenseHat.Tools' project!
                // (In short it takes a bitmap of the font and generates a byte array that can be used like in this example.)
                return new byte[]
                {
                    0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0x00, 0x41, 0x00, 0x00, 0x7c, 0x7e, 0x0b, 0x0b, 0x7e, 0x7c,
                    0x00, 0xff, 0x00, 0x42, 0x00, 0x00, 0x7f, 0x7f, 0x49, 0x49, 0x7f, 0x36, 0x00, 0xff, 0x00, 0x43, 0x00, 0x00, 0x3e,
                    0x7f, 0x41, 0x41, 0x63, 0x22, 0x00, 0xff, 0x00, 0x44, 0x00, 0x00, 0x7f, 0x7f, 0x41, 0x63, 0x3e, 0x1c, 0x00, 0xff,
                    0x00, 0x45, 0x00, 0x00, 0x7f, 0x7f, 0x49, 0x49, 0x41, 0x41, 0x00, 0xff, 0x00, 0x46, 0x00, 0x00, 0x7f, 0x7f, 0x09,
                    0x09, 0x01, 0x01, 0x00, 0xff, 0x00, 0x47, 0x00, 0x00, 0x3e, 0x7f, 0x41, 0x49, 0x7b, 0x3a, 0x00, 0xff, 0x00, 0x48,
                    0x00, 0x00, 0x7f, 0x7f, 0x08, 0x08, 0x7f, 0x7f, 0x00, 0xff, 0x00, 0x49, 0x00, 0x00, 0x41, 0x7f, 0x7f, 0x41, 0x00,
                    0xff, 0x00, 0x4a, 0x00, 0x00, 0x20, 0x60, 0x41, 0x7f, 0x3f, 0x01, 0x00, 0xff, 0x00, 0x4b, 0x00, 0x00, 0x7f, 0x7f,
                    0x1c, 0x36, 0x63, 0x41, 0x00, 0xff, 0x00, 0x4c, 0x00, 0x00, 0x7f, 0x7f, 0x40, 0x40, 0x40, 0x40, 0x00, 0xff, 0x00,
                    0x4d, 0x00, 0x00, 0x7f, 0x7f, 0x06, 0x0c, 0x06, 0x7f, 0x7f, 0x00, 0xff, 0x00, 0x4e, 0x00, 0x00, 0x7f, 0x7f, 0x0e,
                    0x1c, 0x7f, 0x7f, 0x00, 0xff, 0x00, 0x4f, 0x00, 0x00, 0x3e, 0x7f, 0x41, 0x41, 0x7f, 0x3e, 0x00, 0xff, 0x00, 0x50,
                    0x00, 0x00, 0x7f, 0x7f, 0x09, 0x09, 0x0f, 0x06, 0x00, 0xff, 0x00, 0x51, 0x00, 0x00, 0x1e, 0x3f, 0x21, 0x61, 0x7f,
                    0x5e, 0x00, 0xff, 0x00, 0x52, 0x00, 0x00, 0x7f, 0x7f, 0x19, 0x39, 0x6f, 0x46, 0x00, 0xff, 0x00, 0x53, 0x00, 0x00,
                    0x26, 0x6f, 0x49, 0x49, 0x7b, 0x32, 0x00, 0xff, 0x00, 0x54, 0x00, 0x00, 0x01, 0x01, 0x7f, 0x7f, 0x01, 0x01, 0x00,
                    0xff, 0x00, 0x55, 0x00, 0x00, 0x3f, 0x7f, 0x40, 0x40, 0x7f, 0x3f, 0x00, 0xff, 0x00, 0x56, 0x00, 0x00, 0x1f, 0x3f,
                    0x60, 0x60, 0x3f, 0x1f, 0x00, 0xff, 0x00, 0x57, 0x00, 0x00, 0x7f, 0x7f, 0x30, 0x18, 0x30, 0x7f, 0x7f, 0x00, 0xff,
                    0x00, 0x58, 0x00, 0x00, 0x63, 0x77, 0x1c, 0x1c, 0x77, 0x63, 0x00, 0xff, 0x00, 0x59, 0x00, 0x00, 0x07, 0x0f, 0x78,
                    0x78, 0x0f, 0x07, 0x00, 0xff, 0x00, 0x5a, 0x00, 0x00, 0x61, 0x71, 0x59, 0x4d, 0x47, 0x43, 0x00, 0xff, 0x00, 0xc5,
                    0x00, 0x00, 0x70, 0x7a, 0x2d, 0x2d, 0x7a, 0x70, 0x00, 0xff, 0x00, 0xc4, 0x00, 0x00, 0x71, 0x79, 0x2c, 0x2c, 0x79,
                    0x71, 0x00, 0xff, 0x00, 0xd6, 0x00, 0x00, 0x39, 0x7d, 0x44, 0x44, 0x7d, 0x39, 0x00, 0xff, 0x00, 0xc9, 0x00, 0x00,
                    0x7c, 0x7c, 0x54, 0x56, 0x45, 0x45, 0x00, 0xff, 0x00, 0xdc, 0x00, 0x00, 0x3d, 0x7d, 0x40, 0x40, 0x7d, 0x3d, 0x00,
                    0xff, 0x00, 0x61, 0x00, 0x20, 0x74, 0x54, 0x54, 0x7c, 0x78, 0x00, 0xff, 0x00, 0x62, 0x00, 0x00, 0x7f, 0x7f, 0x48,
                    0x48, 0x78, 0x30, 0x00, 0xff, 0x00, 0x63, 0x00, 0x00, 0x38, 0x7c, 0x44, 0x44, 0x44, 0x00, 0xff, 0x00, 0x64, 0x00,
                    0x00, 0x38, 0x7c, 0x44, 0x44, 0x7f, 0x7f, 0x00, 0xff, 0x00, 0x65, 0x00, 0x00, 0x38, 0x7c, 0x54, 0x54, 0x5c, 0x18,
                    0x00, 0xff, 0x00, 0x66, 0x00, 0x00, 0x04, 0x7e, 0x7f, 0x05, 0x05, 0x00, 0xff, 0x00, 0x67, 0x00, 0x00, 0x98, 0xbc,
                    0xa4, 0xa4, 0xfc, 0x7c, 0x00, 0xff, 0x00, 0x68, 0x00, 0x00, 0x7f, 0x7f, 0x08, 0x08, 0x78, 0x70, 0x00, 0xff, 0x00,
                    0x69, 0x00, 0x00, 0x48, 0x7a, 0x7a, 0x40, 0x00, 0xff, 0x00, 0x6a, 0x00, 0x80, 0x80, 0x80, 0xfa, 0x7a, 0x00, 0xff,
                    0x00, 0x6b, 0x00, 0x00, 0x7f, 0x7f, 0x10, 0x38, 0x68, 0x40, 0x00, 0xff, 0x00, 0x6c, 0x00, 0x00, 0x41, 0x7f, 0x7f,
                    0x40, 0x00, 0xff, 0x00, 0x6d, 0x00, 0x00, 0x7c, 0x7c, 0x18, 0x38, 0x1c, 0x7c, 0x78, 0x00, 0xff, 0x00, 0x6e, 0x00,
                    0x00, 0x7c, 0x7c, 0x04, 0x04, 0x7c, 0x78, 0x00, 0xff, 0x00, 0x6f, 0x00, 0x00, 0x38, 0x7c, 0x44, 0x44, 0x7c, 0x38,
                    0x00, 0xff, 0x00, 0x70, 0x00, 0x00, 0xfc, 0xfc, 0x24, 0x24, 0x3c, 0x18, 0x00, 0xff, 0x00, 0x71, 0x00, 0x00, 0x18,
                    0x3c, 0x24, 0x24, 0xfc, 0xfc, 0x00, 0xff, 0x00, 0x72, 0x00, 0x00, 0x7c, 0x7c, 0x04, 0x04, 0x0c, 0x08, 0x00, 0xff,
                    0x00, 0x73, 0x00, 0x00, 0x48, 0x5c, 0x54, 0x54, 0x74, 0x24, 0x00, 0xff, 0x00, 0x74, 0x00, 0x00, 0x04, 0x04, 0x3f,
                    0x7f, 0x44, 0x44, 0x00, 0xff, 0x00, 0x75, 0x00, 0x00, 0x3c, 0x7c, 0x40, 0x40, 0x7c, 0x7c, 0x00, 0xff, 0x00, 0x76,
                    0x00, 0x00, 0x1c, 0x3c, 0x60, 0x60, 0x3c, 0x1c, 0x00, 0xff, 0x00, 0x77, 0x00, 0x00, 0x1c, 0x7c, 0x70, 0x38, 0x70,
                    0x7c, 0x1c, 0x00, 0xff, 0x00, 0x78, 0x00, 0x00, 0x44, 0x6c, 0x38, 0x38, 0x6c, 0x44, 0x00, 0xff, 0x00, 0x79, 0x00,
                    0x00, 0x9c, 0xbc, 0xa0, 0xe0, 0x7c, 0x3c, 0x00, 0xff, 0x00, 0x7a, 0x00, 0x00, 0x44, 0x64, 0x74, 0x5c, 0x4c, 0x44,
                    0x00, 0xff, 0x00, 0xe5, 0x00, 0x20, 0x74, 0x55, 0x55, 0x7c, 0x78, 0x00, 0xff, 0x00, 0xe4, 0x00, 0x20, 0x75, 0x54,
                    0x54, 0x7d, 0x78, 0x00, 0xff, 0x00, 0xf6, 0x00, 0x00, 0x30, 0x7a, 0x48, 0x48, 0x7a, 0x30, 0x00, 0xff, 0x00, 0xe9,
                    0x00, 0x00, 0x38, 0x7c, 0x54, 0x56, 0x5d, 0x19, 0x00, 0xff, 0x00, 0xfc, 0x00, 0x00, 0x3a, 0x7a, 0x40, 0x40, 0x7a,
                    0x7a, 0x00, 0xff, 0x00, 0x30, 0x00, 0x00, 0x3e, 0x7f, 0x49, 0x45, 0x7f, 0x3e, 0x00, 0xff, 0x00, 0x31, 0x00, 0x00,
                    0x40, 0x44, 0x7f, 0x7f, 0x40, 0x40, 0x00, 0xff, 0x00, 0x32, 0x00, 0x00, 0x62, 0x73, 0x51, 0x49, 0x4f, 0x46, 0x00,
                    0xff, 0x00, 0x33, 0x00, 0x00, 0x22, 0x63, 0x49, 0x49, 0x7f, 0x36, 0x00, 0xff, 0x00, 0x34, 0x00, 0x00, 0x18, 0x18,
                    0x14, 0x16, 0x7f, 0x7f, 0x10, 0xff, 0x00, 0x35, 0x00, 0x00, 0x27, 0x67, 0x45, 0x45, 0x7d, 0x39, 0x00, 0xff, 0x00,
                    0x36, 0x00, 0x00, 0x3e, 0x7f, 0x49, 0x49, 0x7b, 0x32, 0x00, 0xff, 0x00, 0x37, 0x00, 0x00, 0x03, 0x03, 0x79, 0x7d,
                    0x07, 0x03, 0x00, 0xff, 0x00, 0x38, 0x00, 0x00, 0x36, 0x7f, 0x49, 0x49, 0x7f, 0x36, 0x00, 0xff, 0x00, 0x39, 0x00,
                    0x00, 0x26, 0x6f, 0x49, 0x49, 0x7f, 0x3e, 0x00, 0xff, 0x00, 0x2e, 0x00, 0x00, 0x60, 0x60, 0x00, 0xff, 0x00, 0x2c,
                    0x00, 0x00, 0x80, 0xe0, 0x60, 0x00, 0xff, 0x00, 0x3f, 0x00, 0x00, 0x02, 0x03, 0x51, 0x59, 0x0f, 0x06, 0x00, 0xff,
                    0x00, 0x21, 0x00, 0x00, 0x4f, 0x4f, 0x00, 0xff, 0x00, 0x22, 0x00, 0x00, 0x07, 0x07, 0x00, 0x00, 0x07, 0x07, 0x00,
                    0xff, 0x00, 0x23, 0x00, 0x00, 0x14, 0x7f, 0x7f, 0x14, 0x14, 0x7f, 0x7f, 0x14, 0x00, 0xff, 0x00, 0x24, 0x00, 0x00,
                    0x24, 0x2e, 0x6b, 0x6b, 0x3a, 0x12, 0x00, 0xff, 0x00, 0x25, 0x00, 0x00, 0x63, 0x33, 0x18, 0x0c, 0x66, 0x63, 0x00,
                    0xff, 0x00, 0x26, 0x00, 0x00, 0x32, 0x7f, 0x4d, 0x4d, 0x77, 0x72, 0x50, 0x00, 0xff, 0x00, 0x2d, 0x00, 0x00, 0x08,
                    0x08, 0x08, 0x08, 0x08, 0x08, 0x00, 0xff, 0x00, 0x2b, 0x00, 0x00, 0x08, 0x08, 0x3e, 0x3e, 0x08, 0x08, 0x00, 0xff,
                    0x00, 0x2a, 0x00, 0x00, 0x08, 0x2a, 0x3e, 0x1c, 0x1c, 0x3e, 0x2a, 0x08, 0x00, 0xff, 0x00, 0x3a, 0x00, 0x00, 0x66,
                    0x66, 0x00, 0xff, 0x00, 0x3b, 0x00, 0x00, 0x80, 0xe6, 0x66, 0x00, 0xff, 0x00, 0x2f, 0x00, 0x00, 0x40, 0x60, 0x30,
                    0x18, 0x0c, 0x06, 0x02, 0x00, 0xff, 0x00, 0x5c, 0x00, 0x00, 0x02, 0x06, 0x0c, 0x18, 0x30, 0x60, 0x40, 0x00, 0xff,
                    0x00, 0x3c, 0x00, 0x00, 0x08, 0x1c, 0x36, 0x63, 0x41, 0x41, 0x00, 0xff, 0x00, 0x3e, 0x00, 0x00, 0x41, 0x41, 0x63,
                    0x36, 0x1c, 0x08, 0x00, 0xff, 0x00, 0x28, 0x00, 0x00, 0x1c, 0x3e, 0x63, 0x41, 0x00, 0xff, 0x00, 0x29, 0x00, 0x00,
                    0x41, 0x63, 0x3e, 0x1c, 0x00, 0xff, 0x00, 0x27, 0x00, 0x00, 0x04, 0x06, 0x03, 0x01, 0x00, 0xff, 0x00, 0x60, 0x00,
                    0x00, 0x01, 0x03, 0x06, 0x04, 0x00, 0xff, 0x00, 0x3d, 0x00, 0x00, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x00
                };
            }
        }

        private void FillDisplay(int scrollPixelOffset)
        {
            switch (_currentMode)
            {
                case RenderMode.YellowOnBlue:
                    senseHat.Display.Fill(Colors.Blue);
                    break;

                case RenderMode.BlackOnStaticRainbow:
                    // Reset to the rainbow colors.
                    senseHat.Display.Reset();
                    break;

                case RenderMode.BlackOnMovingRainbow:
                    // Copy the rainbow colors -- but with an offset so that it "moves" with the characters.
                    senseHat.Display.CopyColorsToScreen(_rainbowColors, -scrollPixelOffset);
                    break;

                case RenderMode.StaticRainbowOnBlack:
                    senseHat.Display.Fill(Colors.Black);
                    break;

                case RenderMode.MovingRainbowOnBlack:
                    senseHat.Display.Fill(Colors.Black);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SwitchToNextScrollMode()
        {
            _currentMode++;
            if (_currentMode > RenderMode.MovingRainbowOnBlack)
            {
                _currentMode = RenderMode.YellowOnBlue;
            }
        }



    }

    class DMRequestHandler : IDeviceManagementRequestHandler
    {
        MainPage mainPage;

        public DMRequestHandler(MainPage mainPage)
        {
            this.mainPage = mainPage;
        }

        // Answer the question "is it OK to reboot the device"
        async Task<bool> IDeviceManagementRequestHandler.IsSystemRebootAllowed()
        {
            bool answer = await this.mainPage.IsSystemRebootAllowed();
            return answer;
        }
    }


    class sensorreading
    {
        public string sensortype { get; set; }
        public string sensorvalue { get; set; }
    }


}
