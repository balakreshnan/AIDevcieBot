using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Runtime.Serialization;
using System.Web;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;

namespace DeviceBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                //var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                string message;
                await Microsoft.Bot.Builder.Dialogs.Conversation.SendAsync(activity, () => new device());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
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

        private static bool TryParseAnchorTag(string text, out string url)
        {
            var regex = new Regex("^<a href=\"(?<href>[^\"]*)\">[^<]*</a>$", RegexOptions.IgnoreCase);
            url = regex.Matches(text).OfType<Match>().Select(m => m.Groups["href"].Value).FirstOrDefault();
            return url != null;
        }

        private static async Task<string> GetTokenAsync(ConnectorClient connector)
        {
            var credentials = connector.Credentials as MicrosoftAppCredentials;
            if (credentials != null)
            {
                return await credentials.GetTokenAsync();
            }

            return null;
        }

        private async Task<string> GetCaptionAsync(Activity activity, ConnectorClient connector)
        {
            var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
            img im = new img();
            if (imageAttachment != null)
            {
                using (var stream = await GetImageStream(connector, imageAttachment))
                {
                    return await im.GetCaptionAsync(stream);
                    
                }
            }

            string url;
            if (TryParseAnchorTag(activity.Text, out url))
            {
                return await im.GetCaptionAsync(url);
            }

            if (Uri.IsWellFormedUriString(activity.Text, UriKind.Absolute))
            {
                return await im.GetCaptionAsync(activity.Text);
            }

            // If we reach here then the activity is neither an image attachment nor an image URL.
            throw new ArgumentException("The activity doesn't contain a valid image attachment or an image URL.");
        }


        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                if (message.MembersAdded.Any(m => m.Id == message.Recipient.Id))
                {
                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));

                    var response = message.CreateReply();
                    response.Text = "Welcome to IoT Device Management LUIS BOT";

                    connector.Conversations.ReplyToActivityAsync(response);
                   
                }

            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private string DoSpeechReco(Attachment attachment)
        {
            AccessTokenInfo token;
            string headerValue;
            // Note: Sign up at https://microsoft.com/cognitive to get a subscription key.  
            // Use the subscription key as Client secret below.
            Authentication auth = new Authentication("bala_b@hotmail.com", "d41475ca4ca04d7883daf52bf0d5d3f3");
            string requestUri = "https://speech.platform.bing.com/recognize";

            //URI Params. Refer to the Speech API documentation for more information.
            requestUri += @"?scenarios=smd";                                // websearch is the other main option.
            requestUri += @"&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5";   // You must use this ID.
            requestUri += @"&locale=en-US";                                 // read docs, for other supported languages. 
            requestUri += @"&device.os=wp7";
            requestUri += @"&version=3.0";
            requestUri += @"&format=json";
            requestUri += @"&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3";
            requestUri += @"&requestid=" + Guid.NewGuid().ToString();

            string host = @"speech.platform.bing.com";
            string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";
            var wav = HttpWebRequest.Create(attachment.ContentUrl);
            string responseString = string.Empty;

            try
            {
                token = auth.GetAccessToken();
                Console.WriteLine("Token: {0}\n", token.access_token);

                //Create a header with the access_token property of the returned token
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

                using (Stream wavStream = wav.GetResponse().GetResponseStream())
                {
                    byte[] buffer = null;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        int count = 0;
                        do
                        {
                            buffer = new byte[1024];
                            count = wavStream.Read(buffer, 0, 1024);
                            requestStream.Write(buffer, 0, count);
                        } while (wavStream.CanRead && count > 0);
                        // Flush
                        requestStream.Flush();
                    }
                    //Get the response from the service.
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
            dynamic data = JObject.Parse(responseString);
            return data.header.name;
        }

        

        //public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        //{
        //    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

        //    var text = activity.Text;

        //    if (activity.Type == ActivityTypes.Message)
        //    {
        //        if (activity.Attachments.Any())
        //        {
        //            var reco = DoSpeechReco(activity.Attachments.First());

        //            if (activity.Text.ToUpper().Contains("WORD"))
        //            {
        //                text = "You said : " + reco + " Word Count: " + reco.Split(' ').Count();
        //            }
        //            else if (activity.Text.ToUpper().Contains("CHARACTER"))
        //            {
        //                var nospacereco = reco.ToCharArray().Where(c => c != ' ').Count();
        //                text = "You said : " + reco + " Character Count: " + nospacereco;
        //            }
        //            else if (activity.Text.ToUpper().Contains("SPACE"))
        //            {
        //                var spacereco = reco.ToCharArray().Where(c => c == ' ').Count();
        //                text = "You said : " + reco + " Space Count: " + spacereco;
        //            }
        //            else if (activity.Text.ToUpper().Contains("VOWEL"))
        //            {
        //                var vowelreco = reco.ToUpper().ToCharArray().Where(c => c == 'A' || c == 'E' ||
        //                                                                   c == 'O' || c == 'I' || c == 'U').Count();
        //                text = "You said : " + reco + " Vowel Count: " + vowelreco;
        //            }
        //            else if (!String.IsNullOrEmpty(activity.Text))
        //            {
        //                var keywordreco = reco.ToUpper().Split(' ').Where(w => w == activity.Text.ToUpper()).Count();
        //                text = "You said : " + reco + " Keyword " + activity.Text + " found " + keywordreco + " times.";
        //            }
        //            else
        //            {
        //                text = "You said : " + reco;
        //            }
        //        }
        //        Activity reply = activity.CreateReply(text);
        //        await connector.Conversations.ReplyToActivityAsync(reply);
        //    }
        //    else
        //    {
        //        return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted); //HandleSystemMessage(activity);
        //    }
        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    return response;
        //}


    }


    [DataContract]
    public class AccessTokenInfo
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    public class Authentication
    {
        public static readonly string AccessUri = "https://oxford-speech.cloudapp.net/token/issueToken";
        private string clientId;
        private string clientSecret;
        private string request;
        private AccessTokenInfo token;
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public Authentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;

            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope={2}",
                                              HttpUtility.UrlEncode(clientId),
                                              HttpUtility.UrlEncode(clientSecret),
                                              HttpUtility.UrlEncode("https://speech.platform.bing.com"));

            this.token = HttpPost(AccessUri, this.request);

            // renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                           this,
                                           TimeSpan.FromMinutes(RefreshTokenDuration),
                                           TimeSpan.FromMilliseconds(-1));
        }

        //Return the access token
        public AccessTokenInfo GetAccessToken()
        {
            return this.token;
        }

        //Renew the access token
        private void RenewAccessToken()
        {
            AccessTokenInfo newAccessToken = HttpPost(AccessUri, this.request);
            //swap the new token with old one
            //Note: the swap is thread unsafe
            this.token = newAccessToken;
            Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}",
                              this.clientId,
                              this.token.access_token));
        }
        //Call-back when we determine the access token has expired 
        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        //Helper function to get new access token
        private AccessTokenInfo HttpPost(string accessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(accessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccessTokenInfo));
                //Get deserialized object from JSON stream
                AccessTokenInfo token = (AccessTokenInfo)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }


}