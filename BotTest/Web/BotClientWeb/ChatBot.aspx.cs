using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net.Http;
using System.IO;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace BotClientWeb
{
    public partial class ChatBot : System.Web.UI.Page
    {

        string secret = ConfigurationManager.AppSettings["AppSecret"].ToString();
        string appid = ConfigurationManager.AppSettings["AppId"].ToString();

        public static string retval = String.Empty;
        public string conversationid = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                if (Page.IsPostBack)
                {
                    // HTTP Post
                    getemotions();
                }
                else
                {
                    // HTTP Get
                    //getemotions();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async void getemotions()
        {
            string subscriptionKey = ConfigurationManager.AppSettings["subscriptionKey"].ToString();

            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(subscriptionKey);

            try
            {
                Emotion[] emotionResult;
                byte[] data = System.Convert.FromBase64String(Request.Form["formfield"]);

                using (Stream imageFileStream = new MemoryStream(data)) //File.OpenRead(Request.Form["image"]))
                {
                    //
                    // Detect the emotions in the URL
                    //
                    emotionResult = await emotionServiceClient.RecognizeAsync(imageFileStream);
                    //return emotionResult;
                    TextBox1.Text = String.Empty;
                    float tmpscore = 0.0F;
                    foreach (Emotion ea in emotionResult)
                    {
                        tmpscore = ea.Scores.Anger;
                        if (ea.Scores.Anger >= tmpscore)
                        {
                            tmpscore = ea.Scores.Anger;
                            TextBox1.Text = "Anger " + ea.Scores.Anger.ToString();
                        }

                        if (ea.Scores.Contempt >= tmpscore)
                        {
                            tmpscore = ea.Scores.Contempt;
                            TextBox1.Text = "Contempt " + ea.Scores.Contempt.ToString();
                        }

                        if (ea.Scores.Disgust >= tmpscore)
                        {
                            tmpscore = ea.Scores.Disgust;
                            TextBox1.Text = "Disgust " + ea.Scores.Disgust.ToString();
                        }

                        if (ea.Scores.Fear >= tmpscore)
                        {
                            tmpscore = ea.Scores.Fear;
                            TextBox1.Text = "Fear " + ea.Scores.Fear.ToString();
                        }

                        if (ea.Scores.Happiness >= tmpscore)
                        {
                            tmpscore = ea.Scores.Happiness;
                            TextBox1.Text = "Happiness " + ea.Scores.Happiness.ToString();
                        }

                        if (ea.Scores.Neutral >= tmpscore)
                        {
                            tmpscore = ea.Scores.Neutral;
                            TextBox1.Text = "Neutral " + ea.Scores.Neutral.ToString();
                        }

                        if (ea.Scores.Sadness >= tmpscore)
                        {
                            tmpscore = ea.Scores.Sadness;
                            TextBox1.Text = "Sadness " + ea.Scores.Sadness.ToString();
                        }

                        if (ea.Scores.Surprise >= tmpscore)
                        {
                            tmpscore = ea.Scores.Surprise;
                            TextBox1.Text = "Surprise " + ea.Scores.Surprise.ToString();
                        }
                       
       

                    }
                    //TextBox1.Text = emotionResult.ToString();

                    //try to get the face information as well.
                    //FaceServiceClient faceClient = new FaceServiceClient("0f79d48df54e47b5b3a298187b22bfca");

                 


                }

                using (Stream imageFileStream = new MemoryStream(data))
                {
                    using (FaceServiceClient faceClient = new FaceServiceClient("0f79d48df54e47b5b3a298187b22bfca"))
                    {
                        Face[] faces = await faceClient.DetectAsync(imageFileStream, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses });
                        TextBox2.Text = String.Empty;

                        foreach (var face in faces)
                        {
                            TextBox2.Text += " Left: " + face.FaceRectangle.Left.ToString() + System.Environment.NewLine;
                            TextBox2.Text += " Top: " + face.FaceRectangle.Top.ToString() + System.Environment.NewLine;
                            TextBox2.Text += " Width: " + face.FaceRectangle.Width.ToString() + System.Environment.NewLine;
                            TextBox2.Text += " Heght: " + face.FaceRectangle.Height.ToString() + System.Environment.NewLine;
                            //TextBox2.Text += " FaceId: " + face.FaceId.ToString() + System.Environment.NewLine;
                            TextBox2.Text += " Gender : " + face.FaceAttributes.Gender.ToString() + System.Environment.NewLine;
                            TextBox2.Text += " Age : " + face.FaceAttributes.Age.ToString() + System.Environment.NewLine;
                            if (face.FaceAttributes.Smile > 0)
                            {
                                TextBox2.Text += " IsSmiling: " + "Smile" + System.Environment.NewLine;
                            }
                            else
                            {
                                TextBox2.Text += " IsSmiling: " + "Not Smile" + System.Environment.NewLine;
                            }

                            TextBox2.Text += " Glasses: " + face.FaceAttributes.Glasses.ToString() + System.Environment.NewLine;

                            TextBox2.Text += " Head pose Pitch : " + face.FaceAttributes.HeadPose.Pitch.ToString() + System.Environment.NewLine;
                            TextBox2.Text += " Head pose Roll : " + face.FaceAttributes.HeadPose.Roll.ToString() + System.Environment.NewLine;
                            TextBox2.Text += " Head pose Yaw : " + face.FaceAttributes.HeadPose.Yaw.ToString() + System.Environment.NewLine;

                            TextBox2.Text += " Facial Hair Beard : " + face.FaceAttributes.FacialHair.Beard.ToString() + System.Environment.NewLine;
                            TextBox2.Text += " Facial Hair Moustache : " + face.FaceAttributes.FacialHair.Moustache.ToString() + System.Environment.NewLine;
                            TextBox2.Text += " Facial Hair Sideburns : " + face.FaceAttributes.FacialHair.Sideburns.ToString() + System.Environment.NewLine;



                        }
                    }
                }

            }
            catch (Exception exception)
            {

                //return null;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                //string secretout = appid + ":" + secret;
                ////var uri = new Uri("https://directline.botframework.com");
                //var uri = new Uri(ConfigurationManager.AppSettings["DirectLineURL"].ToString());

                ////string newsecret = "zHhveP4jk4o.cwA.cIA.QERY2t7safaD557XAYfyFp0WZrpQji3s2nI7QbPunSQ";

                //string newsecret = ConfigurationManager.AppSettings["DirectLineSecret"].ToString();

                //DirectLineClientCredentials creds = new DirectLineClientCredentials(newsecret);

                //DirectLineClient client = new DirectLineClient(uri, creds);
                //Microsoft.Bot.Connector.DirectLine.Conversations convs = new Microsoft.Bot.Connector.DirectLine.Conversations(client);

                //string waterMark = string.Empty;
                //MessageSet set = null;

                //if (conversationid == string.Empty)
                //{
                //    var conv = convs.NewConversation();
                //    set = convs.GetMessages(conv.ConversationId);
                //    waterMark = set.Watermark;
                //    conversationid = conv.ConversationId.ToString();
                //}

                

                //Microsoft.Bot.Connector.DirectLine.Models.Message message = new Microsoft.Bot.Connector.DirectLine.Models.Message(conversationId: conversationid, text: TextBox1.Text);
                ////Console.WriteLine(message.Text);
                //convs.PostMessage(conversationid, message);

                //set = convs.GetMessages(conversationid, waterMark);
                //PrintResponse(set);
                //waterMark = set.Watermark;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //void PrintResponse(MessageSet set)
        //{
        //    var q = from x in set.Messages
        //            where x.FromProperty == appid
        //            select x.Text;

        //    foreach (var str in q.ToList())
        //    {
        //        //TextBox2.Text += "Responce from bot >> " + str + System.Environment.NewLine;
        //    }


        //}

        static async void MakeRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "31a15bf5cd7a449dacb8b6454ec87590");

            // Request parameters
            queryString["q"] = "bill gates";
            queryString["count"] = "10";
            queryString["offset"] = "0";
            queryString["mkt"] = "en-us";
            queryString["safesearch"] = "Moderate";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/search?" + queryString;

            var response = await client.GetAsync(uri);
        }


        //end of class
    }

    //end of class
}