using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Bot.Connector.DirectLine.Models;
using System.Configuration;
using Microsoft.Bot.Connector;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Threading;
using System.Media;

namespace BotClient
{



    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string secret = ConfigurationManager.AppSettings["AppSecret"].ToString();
        string appid = ConfigurationManager.AppSettings["AppId"].ToString();

        static string retval = String.Empty;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string secretout = appid + ":" + secret;
                var uri = new Uri("https://directline.botframework.com");

                string newsecret = "WQcy4GowAQY.cwA.k3g.znNIK2YL4KHjYFlNMd0iND2ag67ONPy7jbOnUsKWhZk";

                DirectLineClientCredentials creds = new DirectLineClientCredentials(newsecret);

                DirectLineClient client = new DirectLineClient(uri, creds);
                Conversations convs = new Conversations(client);

                string waterMark;

                var conv = convs.NewConversation();
                var set = convs.GetMessages(conv.ConversationId);
                waterMark = set.Watermark;

                Microsoft.Bot.Connector.DirectLine.Models.Message message = new Microsoft.Bot.Connector.DirectLine.Models.Message(conversationId: conv.ConversationId, text: textBox3.Text);
                //Console.WriteLine(message.Text);
                convs.PostMessage(conv.ConversationId, message);

                set = convs.GetMessages(conv.ConversationId, waterMark);
                PrintResponse(set);
                waterMark = set.Watermark;

                MakeRequest();

                if(retval != String.Empty)
                {
                    textBox1.Text += " Return JSON: " + retval + System.Environment.NewLine;
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        void PrintResponse(MessageSet set)
        {
            var q = from x in set.Messages
                    where x.FromProperty == appid
                    select x.Text;

            foreach (var str in q.ToList())
            {
                 textBox1.Text += "Responce from bot >> " + str + System.Environment.NewLine;
            }


        }

        static async void MakeRequest()
        {
           

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "3aaab437f88a4371988b6e09028dbc55");

            //var uri = "https://api.projectoxford.ai/luis/v1.0/prog/apps/6a973954-c84f-4073-9c38-f2d597c40021/actions?" + queryString;

            //HttpResponseMessage response;

            //// Request body
            //byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            //using (var content = new ByteArrayContent(byteData))
            //{
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //    response = await client.PostAsync(uri, content);
            //}

            // Request parameters
            queryString["id"] = "6a973954-c84f-4073-9c38-f2d597c40021";
            queryString["q"] = "Big Data";
            //var uri = "https://api.projectoxford.ai/luis/v1/application/preview?" + queryString;
            var uri = "https://api.projectoxford.ai/luis/v1/application?" + queryString;

            var response = await client.GetAsync(uri);

            //string contentback = response.Content.ToString();
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();
                retval = result;

                // ... Display the result.
                if (result != null &&
                result.Length >= 50)
                {
                    //Console.WriteLine(result.Substring(0, 50) + "...");
                    object ret = JsonConvert.DeserializeObject<object>(result);
                    
                    

                }
            }

            
        }

        public void testtospeech()
        {
            AccessTokenInfo token;
            string headerValue;

            // Note: Sign up at http://www.projectoxford.ai to get a subscription key.  Search for Speech APIs from Azure Marketplace.  
            // Use the subscription key as Client secret below.
            Authentication auth = new Authentication("Come up with a short ClientId", "Client Secret");
            //d41475ca4ca04d7883daf52bf0d5d3f3/


            string requestUri = ""; //args[0].Trim(new char[] { '/', '?' });

            /* URI Params. Refer to the README file for more information. */
            requestUri += @"?scenarios=smd";                                  // websearch is the other main option.
            requestUri += @"&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5";     // You must use this ID.
            requestUri += @"&locale=en-US";                                   // We support several other languages.  Refer to README file.
            requestUri += @"&device.os=wp7";
            requestUri += @"&version=3.0";
            requestUri += @"&format=json";
            requestUri += @"&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3";
            requestUri += @"&requestid=" + Guid.NewGuid().ToString();

            string host = @"speech.platform.bing.com";
            string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";

            /*
             * Input your own audio file or use read from a microphone stream directly.
             */
            string audioFile = "filenamehere.wav"; //args[1];
            string responseString;
            FileStream fs = null;

            try
            {
                token = auth.GetAccessToken();
                Console.WriteLine("Token: {0}\n", token.access_token);

                /*
                 * Create a header with the access_token property of the returned token
                 */
                headerValue = "Bearer " + token.access_token;

                Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);

                HttpWebRequest request = null;
                request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.SendChunked = true;
                request.Accept = @"application/json;text/xml";
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version11;
                request.Host = host;
                request.ContentType = contentType;
                request.Headers["Authorization"] = headerValue;

                using (fs = new FileStream(audioFile, FileMode.Open, FileAccess.Read))
                {

                    /*
                     * Open a request stream and write 1024 byte chunks in the stream one at a time.
                     */
                    byte[] buffer = null;
                    int bytesRead = 0;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        /*
                         * Read 1024 raw bytes from the input audio file.
                         */
                        buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
                        }

                        // Flush
                        requestStream.Flush();
                    }

                    /*
                     * Get the response from the service.
                     */
                    Console.WriteLine("Response:");
                    using (WebResponse response = request.GetResponse())
                    {
                        Console.WriteLine(((HttpWebResponse)response).StatusCode);

                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            responseString = sr.ReadToEnd();
                        }

                        Console.WriteLine(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// This method is called once the audio returned from the service.
        /// It will then attempt to play that audio file.
        /// Note that the playback will fail if the output audio format is not pcm encoded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="GenericEventArgs{Stream}"/> instance containing the event data.</param>
        static void PlayAudio(object sender, GenericEventArgs<Stream> args)
        {
            Console.WriteLine(args.EventData);

            // For SoundPlayer to be able to play the wav file, it has to be encoded in PCM.
            // Use output audio format AudioOutputFormat.Riff16Khz16BitMonoPcm to do that.
            SoundPlayer player = new SoundPlayer(args.EventData);
            player.PlaySync();
            args.EventData.Dispose();
        }

        /// <summary>
        /// Handler an error when a TTS request failed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="GenericEventArgs{Exception}"/> instance containing the event data.</param>
        static void ErrorHandler(object sender, GenericEventArgs<Exception> e)
        {
            Console.WriteLine("Unable to complete the TTS request: [{0}]", e.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = "Starting Authtentication" + System.Environment.NewLine;
                AccessTokenInfo token;

                // Note: Sign up at http://www.projectoxford.ai for the client credentials.
                Authentication auth = new Authentication("mybbspapi1", "d41475ca4ca04d7883daf52bf0d5d3f3");

                try
                {
                    token = auth.GetAccessToken();
                    textBox1.Text += String.Format("Token: {0}\n", token.access_token) + System.Environment.NewLine;
                }
                catch (Exception ex)
                {
                    textBox2.Text += String.Format("Failed authentication.") + System.Environment.NewLine;
                    textBox2.Text += String.Format(ex.ToString()) + System.Environment.NewLine;
                    textBox2.Text += String.Format(ex.Message) + System.Environment.NewLine;
                    return;
                }

                textBox1.Text += String.Format("Starting TTSSample request code execution.") + System.Environment.NewLine;

                string requestUri = "https://speech.platform.bing.com/synthesize";

                var cortana = new Synthesize(new Synthesize.InputOptions()
                {
                    RequestUri = new Uri(requestUri),
                    // Text to be spoken.
                    Text = textBox4.Text,
                    VoiceType = Gender.Female,
                    // Refer to the documentation for complete list of supported locales.
                    Locale = "en-US",
                    // You can also customize the output voice. Refer to the documentation to view the different
                    // voices that the TTS service can output.
                    VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)",
                    // Service can return audio in different output format. 
                    OutputFormat = AudioOutputFormat.Riff16Khz16BitMonoPcm,
                    AuthorizationToken = "Bearer " + token.access_token,
                });

                cortana.OnAudioAvailable += PlayAudio;
                cortana.OnError += ErrorHandler;
                cortana.Speak(CancellationToken.None).Wait();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

 

}
