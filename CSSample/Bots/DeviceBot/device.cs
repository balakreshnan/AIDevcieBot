using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using Microsoft.Azure.Devices.Shared;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.ProjectOxford.Vision;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using AdaptiveCards;
//using Microsoft.Azure.EventHubs;
//using Microsoft.Azure.EventHubs.Processor;
using Twilio;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;

namespace DeviceBot
{
    //[LuisModel("fb463f12-f18d-4e5a-944e-2fa83471c16e", "xxxxxxxxxxxxxxxxxxxxx")]
    [Serializable]
    public class device : LuisDialog<object>
    {


        public struct DeviceTwinData
        {
            public string deviceJson;
            public string tagsJson;
            public string reportedPropertiesJson;
            public string desiredPropertiesJson;
        }

        private const string Entitycurrent = "current";

        private const string Entitytemperatures = "temperatures";
        private const string Entityuptime = "uptime";
        private const string Entityvoltage = "voltage";
        private const string Entityfaults = "faults";
        private const string Entityconnecttodevice = "connecttodevice";

        public static string strdevice = string.Empty;

        static RegistryManager registryManager;
        static string connectionString = "HostName=devicename.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xxxxxxxxxxxxxxxxxxxxxxxx";
        static string iotHubD2cEndpoint = "messages/events";
        static Microsoft.ServiceBus.Messaging.EventHubClient eventHubClient;

        readonly ICaptionService captionService = new MicrosoftCognitiveCaptionService();
        static readonly VisualFeature[] VisualFeatures = { VisualFeature.Description };

        string faceapikey = ConfigurationManager.AppSettings["FaceAPIKey"].ToString();
        string visioneapikey = ConfigurationManager.AppSettings["VisionAPIKey"].ToString();
        string visioneurl = ConfigurationManager.AppSettings["VisionURL"].ToString();
        string trainingKey = ConfigurationManager.AppSettings["CustomVisionTraining"].ToString();
        string predictionKey = ConfigurationManager.AppSettings["CustomVisionPrediction"].ToString();

        private string SouthCentralUsEndpoint = ConfigurationManager.AppSettings["CustomVisionURL"].ToString();

        Guid projectid = new Guid(ConfigurationManager.AppSettings["customvisionprojectid"].ToString());

        public device() : base(new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }



        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Connect to Device")]
        public async Task connecttodevice(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = "";

            PromptDialog.Text(context, AfterConfirming_setdevice, "Enter the Device Serial Number: ");


            foreach (var ea in result.Entities)
            {
                retval = "Connected to " + ea.Type.ToString() + ":" + ea.Entity.ToString();
                strdevice = ea.Entity.ToString().ToLower();
            }

            //string retval = gettotalyield(message.Text.ToString());


            await context.PostAsync(retval);

        }

        public async Task AfterConfirming_setdevice(IDialogContext context, IAwaitable<string> confirmation)
        {
            //if (await confirmation)
            //{
            //    this.alarmByWhat.Remove(this.turnOff.What);
            //    await context.PostAsync($"Ok, alarm {this.turnOff} disabled.");
            //}
            //else
            //{
            //    await context.PostAsync("Ok! We haven't modified your alarms!");
            //}

            string retval = await confirmation;
            string ret = string.Empty;
            if (retval != String.Empty)
            {
                strdevice = retval;
                ret = "Looking for device .. found it and Assign it ... " + retval + " ..Done.";
            }


            //context.Wait(MessageReceived);
            await context.PostAsync(ret);
        }

        [LuisIntent("Disconnect")]
        public async Task Disconnect(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = "";

            //foreach (var ea in result.Entities)
            //{
            //    retval = "Disconnect to " + ea.Type.ToString() + ":" + ea.Entity.ToString();
            //}
            if (strdevice != string.Empty)
            {
                retval = "Disconnected " + strdevice;
                strdevice = string.Empty;
            }

            //string retval = gettotalyield(message.Text.ToString());


            await context.PostAsync(retval);

        }

        [LuisIntent("current")]
        public async Task current(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = String.Empty;
            //string retval = gettotalyield(message.Text.ToString());
            if (strdevice != string.Empty)
            {
                retval = "Current is 20 Amps";
            }
            else
            {
                retval = "Connect to device to get reading";
            }


            await context.PostAsync(retval);

        }

        [LuisIntent("Rpm")]
        public async Task Rpm(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = String.Empty;
            //string retval = gettotalyield(message.Text.ToString());
            if (strdevice != string.Empty)
            {
                retval = "Rpm is 2500";
            }
            else
            {
                retval = "Connect to device to get reading";
            }


            await context.PostAsync(retval);

        }

        [LuisIntent("torque")]
        public async Task torque(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = String.Empty;
            //string retval = gettotalyield(message.Text.ToString());
            if (strdevice != string.Empty)
            {
                retval = "torque is 1500";
            }
            else
            {
                retval = "Connect to device to get reading";
            }


            await context.PostAsync(retval);

        }

        [LuisIntent("temperature")]
        public async Task temperatures(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");

            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;

            try
            {
                if (strdevice != string.Empty)
                {
                    retval = "Temperature is 80 Degrees F"; //temperature is denoted as C
                    CancellationToken canceltoken;
                    retval = await InvokeMethod("temperature", canceltoken);
                    retval = retval.Replace("temperature:", "");
                    double cf = 0;
                    double.TryParse(retval, out cf);
                    cf = cf * (9 / 5) + 32;

                    retval = cf.ToString();

                    retval += " F";

                }
                else
                {
                    retval = "Connect to device to get reading";
                }

                await context.PostAsync(retval);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }

        }

        [LuisIntent("uptime")]
        public async Task uptime(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            //await context.PostAsync($"Processing: '{message.Text}'...");

            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            //AddTagsAndQuery().Wait();

            //string retmsg = await AddTagsAndQuery();
            string retmsg = await GetDeviceStatus();

            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;

            if (strdevice != string.Empty)
            {
                retval = retmsg;
            }
            else
            {
                retval = "Connect to device to get reading";
            }

            await context.PostAsync(retval);

        }

        [LuisIntent("voltage")]
        public async Task voltage(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"processing: '{message.Text}'...");

            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;
            if (strdevice != string.Empty)
            {
                retval = "Voltage is 510 Volts";
                CancellationToken canceltoken = new CancellationToken();

                

            }
            else
            {
                retval = "Connect to device to get reading";
            }

            await context.PostAsync(retval);

        }

        [LuisIntent("faults")]
        public async Task faults(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"processing: '{message.Text}'...");

            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;
            if (strdevice != string.Empty)
            {
                retval = "WARN: device temperature 70 deg F, INFO: Device running at optimal config; Alarm: Overheating";
                //logic to find critical alarm.If critical Alarm ask customer to take action
                PromptDialog.Confirm(context, AfterConfirming_Alarm, "Found an Alarm do you want to create a Maintanance work order", null, 1, PromptStyle.Auto);

            }
            else
            {
                retval = "Connect to device to get reading";
            }

            await context.PostAsync(retval);

        }

        public async Task AfterConfirming_Alarm(IDialogContext context, IAwaitable<bool> confirmation)
        {

            bool retval = await confirmation;
            string ret = string.Empty;
            if (retval)
            {
                ret = "Placing Work Order and alerting Maintanace. Estimating 2 or 3 days to Fix it. Meanwhile please try to keep the area little cooler and move it closer to AC";
                var client = new TwilioRestClient("xxxxxxxxxxxxxxxxxxxx", "xxxxxxxxxxxxxxxxx");
                client.SendMessage("+14142061344", "+16313271000", "Overheating Alert detected and Work order is created");
                //await Task.Delay(1000);
                client.SendMessage("+14142061344", "+12625272801", "Overheating Alert detected and Work order is created");
                //await Task.Delay(1000);
                client.SendMessage("+14142061344", "+14145077748", "Overheating Alert detected and Work order is created");

            }


            //context.Wait(MessageReceived);
            await context.PostAsync(ret);
        }

        [LuisIntent("AnalyzeImage")]
        public async Task AnalyzeImage(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            
            var connector = new ConnectorClient(new Uri(message.ServiceUrl));

            PromptDialog.Attachment(context, AfterConfirming_image, "Upload a Picture");


            //await context.PostAsync($"processing: '{message.Text}'...");
            //await this.GetCaptionAsync(message, connector);



            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;
            if (strdevice != string.Empty)
            {
                retval = "still in development";
            }
            else
            {
                retval = "Connect to device to get reading";
            }

            await context.PostAsync(retval);

        }

        public async Task AfterConfirming_image(IDialogContext context, IAwaitable<IEnumerable<Attachment>> confirmation)
        {
            string ret = string.Empty;

            try
            {
                IEnumerable<Attachment> retval = await confirmation;
                Activity a = (Activity)context.Activity;

                foreach (var img in retval)
                {
                    if (img != null)
                    {
                        var client = new VisionServiceClient(visioneapikey, visioneurl);

                        try
                        {
                            System.Drawing.Image attachedImage;

                            if (img.ContentType == "image/png" || img.ContentType == "image/jpeg" || img.ContentType == "image/*")
                            {
                                using (var wc = new WebClient())
                                {
                                    //attachedImage = Image.FromStream(wc.OpenRead(img.ContentUrl));

                                    var result = await client.AnalyzeImageAsync(wc.OpenRead(img.ContentUrl), null);
                                    //ret = result.ToString();
                                    foreach (var cat in result.Categories)
                                    {
                                        ret += "Name : " + cat.Name.ToString();
                                        ret += "Score : " + cat.Score.ToString();
                                        if (cat.Detail != null)
                                        {
                                            ret += "Details : " + cat.Detail.ToString();
                                        }

                                        ret += System.Environment.NewLine;
                                    }

                                    var result1 = await client.DescribeAsync(wc.OpenRead(img.ContentUrl));

                                    foreach (var ea in result1.Description.Captions)
                                    {
                                        ret += "Text : " + ea.Text.ToString();
                                        ret += "Confidence : " + ea.Confidence.ToString();
                                    }

                                    foreach (var ea in result1.Description.Tags)
                                    {
                                        ret += "Tags : " + ea.ToString();

                                    }



                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            ret = ex.Message.ToString() + " - " + ex.StackTrace.ToString();
                            //throw;
                        }

                        //return ProcessAnalysisResult(result);


                    }
                }

                //if (retval)
                //{
                //    ret = "Placing Work Order and alerting Maintanace. Estimating 2 or 3 days to Fix it. Meanwhile please try to keep the area little cooler and move it closer to AC";
                //}


                //context.Wait(MessageReceived);
                await context.PostAsync(ret);
                await context.PostAsync("End of Analyzing Image.");
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }
        }

        private static async Task<Stream> GetImageStream(ConnectorClient connector, Attachment imageAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(imageAttachment.ContentUrl);
                if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                }

                return await httpClient.GetStreamAsync(uri);
            }
        }

        /// <summary>
        /// Gets the href value in an anchor element.
        /// </summary>
        ///  Skype transforms raw urls to html. Here we extract the href value from the url
        /// <param name="text">Anchor tag html.</param>
        /// <param name="url">Url if valid anchor tag, null otherwise</param>
        /// <returns>True if valid anchor element</returns>
        private static bool TryParseAnchorTag(string text, out string url)
        {
            var regex = new Regex("^<a href=\"(?<href>[^\"]*)\">[^<]*</a>$", RegexOptions.IgnoreCase);
            url = regex.Matches(text).OfType<Match>().Select(m => m.Groups["href"].Value).FirstOrDefault();
            return url != null;
        }

        /// <summary>
        /// Gets the JwT token of the bot. 
        /// </summary>
        /// <param name="connector"></param>
        /// <returns>JwT token of the bot</returns>
        private static async Task<string> GetTokenAsync(ConnectorClient connector)
        {
            var credentials = connector.Credentials as MicrosoftAppCredentials;
            if (credentials != null)
            {
                return await credentials.GetTokenAsync();
            }

            return null;
        }

        /// <summary>
        /// Gets the caption asynchronously by checking the type of the image (stream vs URL)
        /// and calling the appropriate caption service method.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="connector">The connector.</param>
        /// <returns>The caption if found</returns>
        /// <exception cref="ArgumentException">The activity doesn't contain a valid image attachment or an image URL.</exception>
        private async Task<string> GetCaptionAsync(Activity activity, ConnectorClient connector)
        {
            var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
            if (imageAttachment != null)
            {
                using (var stream = await GetImageStream(connector, imageAttachment))
                {
                    return await this.captionService.GetCaptionAsync(stream);
                }
            }

            string url;
            if (TryParseAnchorTag(activity.Text, out url))
            {
                return await this.captionService.GetCaptionAsync(url);
            }

            if (Uri.IsWellFormedUriString(activity.Text, UriKind.Absolute))
            {
                return await this.captionService.GetCaptionAsync(activity.Text);
            }

            // If we reach here then the activity is neither an image attachment nor an image URL.
            throw new ArgumentException("The activity doesn't contain a valid image attachment or an image URL.");
        }


        //public async Task<string> GetCaptionAsync(Stream stream)
        //{
        //    var client = new VisionServiceClient(ApiKey);
        //    var result = await client.AnalyzeImageAsync(stream, VisualFeatures);
        //    return ProcessAnalysisResult(result);
        //}

        //private static string ProcessAnalysisResult(AnalysisResult result)
        //{
        //    string message = result?.Description?.Captions.FirstOrDefault()?.Text;

        //    return string.IsNullOrEmpty(message) ?
        //    "Couldn't find a caption for this one" :
        //    "I think it's " + message;
        //}


        [LuisIntent("Gyroscope")]
        public async Task Gyroscope(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"processing: '{message.Text}'...");

            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;
            try
            {
                if (strdevice != string.Empty)
                {
                    retval = "Gyroscope is stable "; //angular rate sensor: ±245/500/2000dps
                    CancellationToken canceltoken;
                    retval = await InvokeMethod("gyro", canceltoken);
                    retval = retval.Replace("gyro:", "");
                    //retval += " C";
                }
                else
                {
                    retval = "Connect to device to get reading";
                }

                await context.PostAsync(retval);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }

        }

        [LuisIntent("Accelerometer")]
        public async Task Accelerometer(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"processing: '{message.Text}'...");

            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;
            try
            {
                if (strdevice != string.Empty)
                {
                    retval = "Accelerometer - speed is 2 Miles/hr"; //Linear acceleration sensor: ±-2/4/8/16 g
                    CancellationToken canceltoken;
                    retval = await InvokeMethod("speed", canceltoken);
                    retval = retval.Replace("speed:", "");
                    retval += " Miles/hr";
                }
                else
                {
                    retval = "Connect to device to get reading";
                }

                await context.PostAsync(retval);
            }
            catch (Exception ex )
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }

        }

        [LuisIntent("Magnetometer")]
        public async Task Magnetometer(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"processing: '{message.Text}'...");

            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;
            try
            {
                if (strdevice != string.Empty)
                {
                    retval = "Compass readings are: "; //reading in gauss
                    CancellationToken canceltoken;
                    retval = await InvokeMethod("magneticfield", canceltoken);
                    retval = retval.Replace("magneticfield:", "");
                    //retval += " C";
                }
                else
                {
                    retval = "Connect to device to get reading";
                }

                await context.PostAsync(retval);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }

        }

        [LuisIntent("Barometricpressure")]
        public async Task Barometricpressure(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"processing: '{message.Text}'...");

            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;
            try
            {
                if (strdevice != string.Empty)
                {
                    retval = "Barometric pressure reading is:"; //pressure reported in millibars
                    CancellationToken canceltoken;
                    retval = await InvokeMethod("pressure", canceltoken);
                    retval = retval.Replace("pressure:", "");
                    double ccv = 0;
                    double.TryParse(retval, out ccv);
                    ccv = ccv * 0.02953;
                    retval = ccv.ToString();

                    retval += " inches of mercury";
                }
                else
                {
                    retval = "Connect to device to get reading";
                }

                await context.PostAsync(retval);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw ex;
            }

        }

        [LuisIntent("Humidity")]
        public async Task Humidity(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"processing: '{message.Text}'...");

            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;
            try
            {
                if (strdevice != string.Empty)
                {
                    retval = "Humidity Reading is: "; //humidity is representated as %
                    CancellationToken canceltoken;
                    retval = await InvokeMethod("humidity", canceltoken);
                    retval = retval.Replace("humidity:", "");

                    //double ch = 0;
                    //double.TryParse(retval, out ch);
                    //ch = ch * 10;
                    //retval = ch.ToString();

                    retval += " %";
                }
                else
                {
                    retval = "Connect to device to get reading";
                }

                await context.PostAsync(retval);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }

        }

        [LuisIntent("deviceproperties")]
        public async Task deviceproperties(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
           
            

            //registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            ////AddTagsAndQuery().Wait();

            ////string retmsg = await AddTagsAndQuery();
            //string retmsg = await GetDeviceInfo();

            var strlist = new string[] { "Temperature", "Pressure", "Humidity", "Gyro", "Compass", "Speed", "pose" };

            try
            {
                CancellationToken cancelToken;
                string retval = await InvokeMethod(strlist, cancelToken);

                dynamic dynObj = JsonConvert.DeserializeObject(retval.Substring(4).ToString());

                var resultMessage = context.MakeMessage();

                if (strdevice != string.Empty)
                {
                    resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    resultMessage.Attachments = new List<Attachment>();

                    //var res = retval.Split(',');

                    foreach (var fc in dynObj)
                    {
                        //HeroCard heroCard = new HeroCard()
                        //{
                        //    Title = fc.sensortype,
                        //    Subtitle = fc.sensorvalue,
                        //    Buttons = new List<CardAction>()
                        //    {
                        //        new CardAction()
                        //        {
                        //            Title = "More details",
                        //            Type = ActionTypes.OpenUrl,
                        //            Value = $"https://www.microsoft.com"
                        //        }
                        //    }
                        //};
                        var card = new AdaptiveCard();
                        card.Title = fc.sensortype;

                        card.Speak = "<s>Here are the device properties.</s>";
                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = fc.sensorvalue,
                            Size = TextSize.Large,
                            Weight = TextWeight.Bolder
                        });



                        // Add buttons to the card.
                        card.Actions.Add(new HttpAction()
                        {
                            Url = "http://www.microsoft.com",
                            Title = fc.sensortype
                        });


                        // Create the attachment.
                        Attachment attachment = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = card
                        };
                        //var card = new AdaptiveCard()
                        //{

                        //};
                        //resultMessage.Attachments.Add(heroCard.ToAttachment());
                        resultMessage.Attachments.Add(attachment);


                    }
                }
                else
                {
                    resultMessage = null;
                }


                await context.PostAsync(resultMessage);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }

        }

        [LuisIntent("deviceinfo")]
        public async Task deviceinfo(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {

            try
            {

                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                //AddTagsAndQuery().Wait();

                //string retmsg = await AddTagsAndQuery();
                string retmsg = await GetDeviceInfo();

                var message = await activity;
                //await context.PostAsync($"processing: '{message.Text}'...");

                var tdata = await GetDeviceTwinData();

                JObject obj = JObject.Parse(tdata.deviceJson);

                //string retval = gettotalyield(message.Text.ToString());
                string retval = string.Empty;
                if (strdevice != string.Empty)
                {
                    retval = retmsg + " " + tdata.deviceJson.ToString();
                }
                else
                {
                    retval = "Connect to device to get reading";
                }

                await context.PostAsync(retval);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }


        }

        public async Task<DeviceTwinData> GetDeviceTwinData()
        {
            DeviceTwinData result = new DeviceTwinData();

            dynamic registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            try
            {
                var deviceTwin = await registryManager.GetTwinAsync(strdevice);
                if (deviceTwin != null)
                {
                    result.deviceJson = deviceTwin.ToJson();
                    result.tagsJson = deviceTwin.Tags.ToJson();
                    result.reportedPropertiesJson = deviceTwin.Properties.Reported.ToJson();
                    result.desiredPropertiesJson = deviceTwin.Properties.Desired.ToJson();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + Environment.NewLine + "Make sure you are using the latest Microsoft.Azure.Devices package.", "Device Twin Properties");
                //await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
            }
            return result;
        }

        [LuisIntent("reboot")]
        public async Task reboot(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {

            try
            {
                ServiceClient client;
                JobClient jobClient;
                string targetDevice = strdevice;
                string retval = "Device Rebooting...";

                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                //AddTagsAndQuery().Wait();

                //string retmsg = await AddTagsAndQuery();
                Twin twin = await registryManager.GetTwinAsync(targetDevice);
                client = ServiceClient.CreateFromConnectionString(connectionString);
                CloudToDeviceMethod method = new CloudToDeviceMethod("reboot");
                method.ResponseTimeout = TimeSpan.FromSeconds(30);


                CloudToDeviceMethodResult result1 = await client.InvokeDeviceMethodAsync(targetDevice, method);

                retval += result1.Status.ToString();
                retval += result1.GetPayloadAsJson();

                await context.PostAsync(retval);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }

        }

        [LuisIntent("devicemsg")]
        public async Task devicemsg(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {

            try
            {
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                //AddTagsAndQuery().Wait();

                //string retmsg = await AddTagsAndQuery();
                string retmsg = await GetDeviceMsg();

                var message = await activity;
                //await context.PostAsync($"processing: '{message.Text}'...");

                //string retval = gettotalyield(message.Text.ToString());
                string retval = string.Empty;
                if (strdevice != string.Empty)
                {
                    retval = retmsg;
                }
                else
                {
                    retval = "Connect to device to get reading";
                }

                await context.PostAsync(retval);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }

            

        }

        [LuisIntent("SendToDevice")]
        public async Task SendToDevice(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            try
            {
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                //AddTagsAndQuery().Wait();
                var message = await activity;
                string command = message.Text;
                command = command.ToLower().Replace("send", "");
                string sensor = "";
                string sensorvalue = "";

                foreach (var ea in result.Entities)
                {
                    //retval = "Connected to " + ea.Type.ToString() + ":" + ea.Entity.ToString();
                    if (ea.Type.ToString().Equals("valuetoset"))
                    {
                        sensorvalue = ea.Entity.ToString();
                    }
                    if (ea.Type.ToString().Equals("sensor"))
                    {
                        sensor = ea.Entity.ToString();
                    }
                    //command = ea.Entity.ToString();

                }

                command = sensor + ":" + sensorvalue;

                //string retmsg = await AddTagsAndQuery();
                string retmsg = await SendToDevice(strdevice, command);

                //await context.PostAsync($"processing: '{message.Text}'...");

                //string retval = gettotalyield(message.Text.ToString());
                string retval = string.Empty;
                if (strdevice != string.Empty)
                {
                    retval = retmsg;
                }
                else
                {
                    retval = "Connect to device to get reading";
                }

                await context.PostAsync(retval);
            }
            catch (Exception ex)
            {
                await context.PostAsync("Error: " + ex.Message.ToString() + " - " + ex.StackTrace.ToString());
                //throw;
            }
            

        }


        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi! Try asking me things like 'show me the current', 'what is the uptime' or 'show me the voltage'");

            context.Wait(this.MessageReceived);
        }


        public static async Task<string> AddTagsAndQuery()
        {
            string retval = string.Empty;
            var twin = await registryManager.GetTwinAsync("device1");
            var patch =
                @"{
             tags: {
                 }
         }";
            await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);

            var query = registryManager.CreateQuery("SELECT * FROM devices", 100);
            var twinsInRedmond43 = await query.GetNextAsTwinAsync();
            retval += string.Format("Devices : {0}", string.Join(", ", twinsInRedmond43.Select(t => t.DeviceId)));

            //query = registryManager.CreateQuery("SELECT * FROM devices WHERE tags.location.plant = 'Redmond43' AND properties.reported.connectivity.type = 'cellular'", 100);
            query = registryManager.CreateQuery("SELECT * FROM devices", 100);
            var twinsInRedmond43UsingCellular = await query.GetNextAsTwinAsync();
            retval += string.Format("Devices network: {0}", string.Join(", ", twinsInRedmond43UsingCellular.Select(t => t.DeviceId)));

            return retval;
        }

        public static async Task<string> GetDeviceStatus()
        {
            string retval = string.Empty;
            //var twin =  registryManager.GetTwinAsync("device1");
            var dd = await registryManager.GetDeviceAsync("device1");

            if (dd != null)
            {
                //retval += "Device Id: " + dd.Id.ToString() + System.Environment.NewLine;
                //retval += "Device ETag: " + dd.ETag.ToString() + System.Environment.NewLine;
                //retval += "Device Status: " + dd.Status.ToString() + System.Environment.NewLine;
                //if (dd.StatusReason != null)
                //{
                //    retval += "Device StatusReason: " + dd.StatusReason.ToString() + System.Environment.NewLine;
                //    retval += "Device StatusUpdatedTime: " + dd.StatusUpdatedTime.ToString() + System.Environment.NewLine;
                //}

                //retval += "Device ConnectionStateUpdatedTime: " + dd.ConnectionStateUpdatedTime.ToString() + System.Environment.NewLine;

                //retval += "Device LastActivityTime: " + dd.LastActivityTime.ToString() + System.Environment.NewLine;
                retval += "Device ConnectionState: " + dd.ConnectionState.ToString() + System.Environment.NewLine;

            }



            return retval;

        }

        public static async Task<string> GetDeviceInfo()
        {
            string retval = string.Empty;
            //var twin =  registryManager.GetTwinAsync("device1");
            var dd = await registryManager.GetDeviceAsync("device1");

            if(dd != null)
            {
                retval += "Device Id: " + dd.Id.ToString() + System.Environment.NewLine;
                retval += "Device ETag: " + dd.ETag.ToString() + System.Environment.NewLine;
                retval += "Device Status: " + dd.Status.ToString() + System.Environment.NewLine;
                if(dd.StatusReason != null)
                {
                    retval += "Device StatusReason: " + dd.StatusReason.ToString() + System.Environment.NewLine;
                    retval += "Device StatusUpdatedTime: " + dd.StatusUpdatedTime.ToString() + System.Environment.NewLine;
                }
                
                retval += "Device ConnectionStateUpdatedTime: " + dd.ConnectionStateUpdatedTime.ToString() + System.Environment.NewLine;
                
                retval += "Device LastActivityTime: " + dd.LastActivityTime.ToString() + System.Environment.NewLine;
                retval += "Device ConnectionState: " + dd.ConnectionState.ToString() + System.Environment.NewLine;
                
            }

           

            return retval;

        }

        public static async Task<string> GetDeviceMsg()
        {
            string retval = string.Empty;
            string iotHubD2cEndpoint = "messages/events";

            // string EhConnectionString = "Endpoint=sb://xxxxxx.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessxxxx";
            // string EhEntityPath = "iothub-ehub-bbdevicehu-146694-05d48bcf4c";
            // string StorageContainerName = "iotstore";
            // string StorageAccountName = "bbiotstore";
            // string StorageAccountKey = "xxxxxxxx";

            //String StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

            var eventHubClient = Microsoft.ServiceBus.Messaging.EventHubClient.
            CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            //var receiver = eventHubClient.GetDefaultConsumerGroup().
            //    CreateReceiver("1", DateTime.Now.AddDays(-1));

            var receiver = eventHubClient.GetDefaultConsumerGroup().
                CreateReceiver("1", DateTime.UtcNow.AddMinutes(-60));

            int reccount = receiver.PrefetchCount;
            //Microsoft.ServiceBus.Messaging.EventData eventData = await receiver.ReceiveAsync();
            //Microsoft.ServiceBus.Messaging.EventData eventData = receiver.Receive();
            //if (eventData != null)
            //{
            //    string data = Encoding.UTF8.GetString(eventData.GetBytes());
            //    retval += String.Format("Message received: '{0}'", data) + System.Environment.NewLine;
            //}


            IEnumerable<Microsoft.ServiceBus.Messaging.EventData> eventData = receiver.Receive(20);

            foreach (EventData ev in eventData)
            {
                if (ev != null)
                {
                    string data = Encoding.UTF8.GetString(ev.GetBytes());
                    //retval += String.Format("Message received: '{0}'", data) + System.Environment.NewLine;
                    retval = String.Format("Message received: '{0}'", data) + System.Environment.NewLine;

                }
            }




            //foreach (string partition in d2cPartitions)
            //{
            //    var receiver = eventHubClient.GetDefaultConsumerGroup().
            //    CreateReceiver(partition, DateTime.Now.AddDays(-1));
            //    receiver.PrefetchCount = 10;
            //    //Microsoft.ServiceBus.Messaging.EventData eventData = await receiver.ReceiveAsync();
            //    Microsoft.ServiceBus.Messaging.EventData eventData = receiver.Receive();

            //    if (eventData != null)
            //    {
            //        string data = Encoding.UTF8.GetString(eventData.GetBytes());
            //        retval += String.Format("Message received: '{0}'", data) + System.Environment.NewLine;
            //    }

            //}
            //CancellationToken canceltoken = new CancellationToken();

            //InvokeMethod(canceltoken);
            //   var eventProcessorHost = new EventProcessorHost(
            //EhEntityPath,
            //PartitionReceiver.DefaultConsumerGroupName,
            //EhConnectionString,
            //StorageConnectionString,
            //StorageContainerName);

            //   // Registers the Event Processor Host and starts receiving messages
            //   await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();


            //   // Disposes of the Event Processor Host
            //   await eventProcessorHost.UnregisterEventProcessorAsync();

            return retval;
        }

        public static async Task<string> SendToDevice(String deviceid, string devicemsg)
        {
            string retval = string.Empty;

            ServiceClient serviceClient;
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var commandMessage = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(devicemsg));
            await serviceClient.SendAsync(deviceid, commandMessage);
            retval = "Sent information to Device " + DateTime.Now.ToString();

            return retval;

        }

        async static Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
        {
            while (true)
            {
                Microsoft.ServiceBus.Messaging.EventData eventData = await receiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received: '{0}'", data);
            }
        }

        static ServiceClient _serviceClient;
        private static async Task<string> InvokeMethod(string payload,CancellationToken cancellationToken)
        {
            string retval = string.Empty;
            try
            {
                _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
                

                var methodInvocation = new CloudToDeviceMethod("Getsensordata") { ResponseTimeout = TimeSpan.FromSeconds(10) };
                methodInvocation.SetPayloadJson("{ \"MethodPayload\": \"" + payload + "\" }");
                methodInvocation.ConnectionTimeout = TimeSpan.FromSeconds(10);
                

                CloudToDeviceMethodResult response = await _serviceClient.InvokeDeviceMethodAsync(strdevice, methodInvocation, cancellationToken);
                retval = response.Status.ToString();
                retval += " " + response.GetPayloadAsJson();
                //Console.WriteLine("Response status: {0}, payload:", response.Status);
                //Console.WriteLine(response.GetPayloadAsJson());

                dynamic dynObj = JsonConvert.DeserializeObject(response.GetPayloadAsJson());

                foreach (var fc in dynObj)
                {
                    retval = fc.sensortype;
                    retval += ":" + fc.sensorvalue;
                }


                //_serviceClient.Dispose();
            }
            catch (Exception ex)
            {

                retval = " Error: " + ex.Message.ToString() + " Stack Trace: " + ex.StackTrace.ToString();
            }

            return retval;

        }

        private static async Task<string> InvokeMethod(string[] payload, CancellationToken cancellationToken)
        {
            string retval = string.Empty;
            try
            {
                _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
                int i = 1;

                var methodInvocation = new CloudToDeviceMethod("Getsensordata") { ResponseTimeout = TimeSpan.FromSeconds(10) };
                string payloadstring = "{";

                foreach (string str in payload)
                {
                    payloadstring += "\"MethodPayload" + i.ToString() + "\": \"" + str + "\",";
                    i++;
                }
                payloadstring = payloadstring.TrimEnd(',');
                payloadstring += "}";
                //methodInvocation.SetPayloadJson("{ \"MethodPayload\": \"" + payload + "\" }");
                methodInvocation.SetPayloadJson(payloadstring);
                methodInvocation.ConnectionTimeout = TimeSpan.FromSeconds(10);


                CloudToDeviceMethodResult response = await _serviceClient.InvokeDeviceMethodAsync(strdevice, methodInvocation, cancellationToken);
                retval = response.Status.ToString();
                retval += " " + response.GetPayloadAsJson();
                //Console.WriteLine("Response status: {0}, payload:", response.Status);
                //Console.WriteLine(response.GetPayloadAsJson());
                //_serviceClient.Dispose();
            }
            catch (Exception ex)
            {

                retval = " Error: " + ex.Message.ToString() + " Stack Trace: " + ex.StackTrace.ToString();
            }

            return retval;

        }

        [LuisIntent("DetectObject")]
        public async Task DetectImage(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;

            var connector = new ConnectorClient(new Uri(message.ServiceUrl));

            PromptDialog.Attachment(context, Afterdetect_image, "Upload a Picture");


            //await context.PostAsync($"processing: '{message.Text}'...");
            //await this.GetCaptionAsync(message, connector);



            //string retval = gettotalyield(message.Text.ToString());
            string retval = string.Empty;
            if (strdevice != string.Empty)
            {
                retval = "still in development";
            }
            else
            {
                retval = "Connect to device to get reading";
            }

            await context.PostAsync(retval);

        }

        public async Task Afterdetect_image(IDialogContext context, IAwaitable<IEnumerable<Attachment>> confirmation)
        {
            string ret = string.Empty;

            try
            {
                IEnumerable<Attachment> retval = await confirmation;
                Activity a = (Activity)context.Activity;

                foreach (var img in retval)
                {
                    if (img != null)
                    {
                        var client = new VisionServiceClient(visioneapikey, visioneurl);

                        try
                        {
                            System.Drawing.Image attachedImage;

                            if (img.ContentType == "image/png" || img.ContentType == "image/jpeg" || img.ContentType == "image/*")
                            {
                                using (var wc = new WebClient())
                                {
                                    //attachedImage = Image.FromStream(wc.OpenRead(img.ContentUrl));
                                    //using (Stream s = new MemoryStream(wc.OpenRead(img.ContentUrl)))
                                    //{




                                    //}
                                    // Create a prediction endpoint, passing in obtained prediction key
                                    CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
                                    {
                                        ApiKey = predictionKey,
                                        Endpoint = SouthCentralUsEndpoint
                                    };

                                    // Make a prediction against the new project
                                    ret = string.Format("Making a prediction:") + System.Environment.NewLine;
                                    var result = endpoint.PredictImage(projectid, wc.OpenRead(img.ContentUrl));

                                    // Loop over each prediction and write out the results
                                    foreach (var c in result.Predictions)
                                    {
                                        ret += string.Format($"\t{c.TagName}: {c.Probability:P1}" + System.Environment.NewLine);
                                    }



                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            ret = ex.Message.ToString() + " - " + ex.StackTrace.ToString();
                            //throw;
                        }

                        //return ProcessAnalysisResult(result);


                    }
                }

                //if (retval)
                //{
                //    ret = "Placing Work Order and alerting Maintanace. Estimating 2 or 3 days to Fix it. Meanwhile please try to keep the area little cooler and move it closer to AC";
                //}


                //context.Wait(MessageReceived);
                await context.PostAsync(ret);
                await context.PostAsync("End of Detecting Object.");
            }
            catch (Exception ex)
            {
                await context.PostAsync(" Error: " + ex.Message.ToString() + " Stack Trace: " + ex.StackTrace.ToString());
                //throw;
            }
        }

        [LuisIntent("DeviceList")]
        public async Task devicelist(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {


            try
            {
                //registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                ////AddTagsAndQuery().Wait();

                ////string retmsg = await AddTagsAndQuery();
                //string retmsg = await GetDeviceInfo();

                var strlist = new string[] { "device1", "device2", "iotedge1", "iotedge2" };

                CancellationToken cancelToken;
                //string retval = await InvokeMethod(strlist, cancelToken);

                //dynamic dynObj = JsonConvert.DeserializeObject(retval.Substring(4).ToString());

                var resultMessage = context.MakeMessage();

                if (strdevice != string.Empty)
                {
                    resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    resultMessage.Attachments = new List<Attachment>();

                    //var res = retval.Split(',');

                    foreach (var fc in strlist)
                    {
                        var card = new AdaptiveCard();
                        card.Title = fc.ToString();

                        card.Speak = "<s>List of Devices available.</s>";
                        // Add text to the card.
                        card.Body.Add(new TextBlock()
                        {
                            Text = fc.ToString(),
                            Size = TextSize.Large,
                            Weight = TextWeight.Bolder
                        });



                        // Add buttons to the card.
                        card.Actions.Add(new HttpAction()
                        {
                            Url = "http://www.microsoft.com",
                            Title = fc.ToString()
                        });


                        // Create the attachment.
                        Attachment attachment = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = card
                        };
                        //var card = new AdaptiveCard()
                        //{

                        //};
                        //resultMessage.Attachments.Add(heroCard.ToAttachment());
                        resultMessage.Attachments.Add(attachment);


                    }
                }
                else
                {
                    resultMessage = null;
                }


                await context.PostAsync(resultMessage);
                await context.PostAsync("List Devices completed.");
            }
            catch (Exception ex)
            {

                await context.PostAsync(ex.Message.ToString() + " - " + ex.StackTrace.ToString());
            }
 

            
        }


        [LuisIntent("Availability")]
        public async Task Availability(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = String.Empty;
            //string retval = gettotalyield(message.Text.ToString());
            if (strdevice != string.Empty)
            {
                retval = "Availability is at 80%";
            }
            else
            {
                retval = "Connect to device to get reading";
            }


            await context.PostAsync(retval);

        }

        [LuisIntent("Yield")]
        public async Task Yield(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = String.Empty;
            //string retval = gettotalyield(message.Text.ToString());
            if (strdevice != string.Empty)
            {
                retval = "Yield is at 77%";
            }
            else
            {
                retval = "Connect to device to get reading";
            }


            await context.PostAsync(retval);

        }

        [LuisIntent("performance")]
        public async Task performance(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = String.Empty;
            //string retval = gettotalyield(message.Text.ToString());
            if (strdevice != string.Empty)
            {
                retval = "performance is at 70%";
            }
            else
            {
                retval = "Connect to device to get reading";
            }


            await context.PostAsync(retval);

        }

        [LuisIntent("Quality")]
        public async Task Quality(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = String.Empty;
            //string retval = gettotalyield(message.Text.ToString());
            if (strdevice != string.Empty)
            {
                retval = "Quality is at 70%";
            }
            else
            {
                retval = "Connect to device to get reading";
            }


            await context.PostAsync(retval);

        }

        [LuisIntent("energyconsumption")]
        public async Task energyconsumption(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = String.Empty;
            //string retval = gettotalyield(message.Text.ToString());
            if (strdevice != string.Empty)
            {
                retval = "energy consumption is at 70%";
            }
            else
            {
                retval = "Connect to device to get reading";
            }


            await context.PostAsync(retval);

        }

        [LuisIntent("productivity")]
        public async Task productivity(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Processing: '{message.Text}'...");
            string retval = String.Empty;
            //string retval = gettotalyield(message.Text.ToString());
            if (strdevice != string.Empty)
            {
                retval = "Productivity is at 74%";
            }
            else
            {
                retval = "Connect to device to get reading";
            }


            await context.PostAsync(retval);

        }

        //end of clss

    }

    //end of namespace

}
