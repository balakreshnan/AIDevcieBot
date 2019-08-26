using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using Microsoft.ProjectOxford.Emotion;
//using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Configuration;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.ProjectOxford.Common.Contract;

namespace DeviceBotWeb
{
    public partial class Face : System.Web.UI.Page
    {
        string secret = ConfigurationManager.AppSettings["AppSecret"].ToString();
        string appid = ConfigurationManager.AppSettings["AppId"].ToString();
        string faceapikey = ConfigurationManager.AppSettings["FaceAPIKey"].ToString();
        string visioneapikey = ConfigurationManager.AppSettings["VisionAPIKey"].ToString();
        string visioneurl = ConfigurationManager.AppSettings["VisionURL"].ToString();

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
                    TextBox1.Text += "Emotions API Identified: ";
                    String tmptext = string.Empty;
                    float tmpscore = 0.0F;
                    foreach (Emotion ea in emotionResult)
                    {
                        tmpscore = ea.Scores.Anger;
                        if (ea.Scores.Anger >= tmpscore)
                        {
                            tmpscore = ea.Scores.Anger;
                            tmptext = "Anger " + ea.Scores.Anger.ToString();
                        }

                        if (ea.Scores.Contempt >= tmpscore)
                        {
                            tmpscore = ea.Scores.Contempt;
                            tmptext = "Contempt " + ea.Scores.Contempt.ToString();
                        }

                        if (ea.Scores.Disgust >= tmpscore)
                        {
                            tmpscore = ea.Scores.Disgust;
                            tmptext = "Disgust " + ea.Scores.Disgust.ToString();
                        }

                        if (ea.Scores.Fear >= tmpscore)
                        {
                            tmpscore = ea.Scores.Fear;
                            tmptext = "Fear " + ea.Scores.Fear.ToString();
                        }

                        if (ea.Scores.Happiness >= tmpscore)
                        {
                            tmpscore = ea.Scores.Happiness;
                            tmptext = "Happiness " + ea.Scores.Happiness.ToString();
                        }

                        if (ea.Scores.Neutral >= tmpscore)
                        {
                            tmpscore = ea.Scores.Neutral;
                            tmptext = "Neutral " + ea.Scores.Neutral.ToString();
                        }

                        if (ea.Scores.Sadness >= tmpscore)
                        {
                            tmpscore = ea.Scores.Sadness;
                            tmptext += "Sadness " + ea.Scores.Sadness.ToString();
                        }

                        if (ea.Scores.Surprise >= tmpscore)
                        {
                            tmpscore = ea.Scores.Surprise;
                            tmptext = "Surprise " + ea.Scores.Surprise.ToString();
                        }

                        TextBox1.Text += tmptext + " scored";

                    }
                    //TextBox1.Text = emotionResult.ToString();

                    //try to get the face information as well.
                    //FaceServiceClient faceClient = new FaceServiceClient("0f79d48df54e47b5b3a298187b22bfca");




                }

                using (Stream imageFileStream = new MemoryStream(data))
                {
                    using (FaceServiceClient faceClient = new FaceServiceClient(faceapikey))
                    {
                        Microsoft.ProjectOxford.Face.Contract.Face[] faces = await faceClient.DetectAsync(imageFileStream, true, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses });
                        //TextBox2.Text = String.Empty;
                        TextBox1.Text += System.Environment.NewLine;
                        TextBox1.Text += "Face API Detection Characteristic: " + System.Environment.NewLine;

                        foreach (var face in faces)
                        {
                            var id = face.FaceId;
                            var attributes = face.FaceAttributes;

                            TextBox1.Text += "Rectangle: Left: " + face.FaceRectangle.Left.ToString();
                            TextBox1.Text += " Top: " + face.FaceRectangle.Top.ToString();
                            TextBox1.Text += " Width: " + face.FaceRectangle.Width.ToString();
                            TextBox1.Text += " Heght: " + face.FaceRectangle.Height.ToString() + System.Environment.NewLine;
                            //TextBox1.Text += " FaceId: " + face.FaceId.ToString() + System.Environment.NewLine;
                            TextBox1.Text += " Gender : " + face.FaceAttributes.Gender.ToString() + System.Environment.NewLine;
                            TextBox1.Text += " Age : " + face.FaceAttributes.Age.ToString() + System.Environment.NewLine;
                            if (face.FaceAttributes.Smile > 0)
                            {
                                TextBox1.Text += " IsSmiling: " + "Smile" + System.Environment.NewLine;
                            }
                            else
                            {
                                TextBox1.Text += " IsSmiling: " + "Not Smile" + System.Environment.NewLine;
                            }

                            TextBox1.Text += " Glasses: " + face.FaceAttributes.Glasses.ToString() + System.Environment.NewLine;
                            try
                            {
                                if (attributes != null)
                                {
                                    TextBox1.Text += " Head pose Pitch : " + attributes.HeadPose.Pitch.ToString() + System.Environment.NewLine;
                                    TextBox1.Text += " Head pose Roll : " + attributes.HeadPose.Roll.ToString() + System.Environment.NewLine;
                                    TextBox1.Text += " Head pose Yaw : " + attributes.HeadPose.Yaw.ToString() + System.Environment.NewLine;

                                    TextBox1.Text += " Facial Hair Beard : " + attributes.FacialHair.Beard.ToString() + System.Environment.NewLine;
                                    TextBox1.Text += " Facial Hair Moustache : " + attributes.FacialHair.Moustache.ToString() + System.Environment.NewLine;
                                    TextBox1.Text += " Facial Hair Sideburns : " + attributes.FacialHair.Sideburns.ToString() + System.Environment.NewLine;
                                }
                            }
                            catch (Exception)
                            {

                                //throw;
                            }



                            var rect = face.FaceRectangle;
                            var landmarks = face.FaceLandmarks;

                            double noseX = landmarks.NoseTip.X;
                            double noseY = landmarks.NoseTip.Y;

                            double leftPupilX = landmarks.PupilLeft.X;
                            double leftPupilY = landmarks.PupilLeft.Y;

                            double rightPupilX = landmarks.PupilRight.X;
                            double rightPupilY = landmarks.PupilRight.Y;

                            var upperLipBottom = landmarks.UpperLipBottom;
                            var underLipTop = landmarks.UnderLipTop;

                            //var centerOfMouth = new Point(
                            //    (upperLipBottom.X + underLipTop.X) / 2,
                            //    (upperLipBottom.Y + underLipTop.Y) / 2);

                            var eyeLeftInner = landmarks.EyeLeftInner;
                            var eyeRightInner = landmarks.EyeRightInner;

                            //var centerOfTwoEyes = new Point(
                            //    (eyeLeftInner.X + eyeRightInner.X) / 2,
                            //    (eyeLeftInner.Y + eyeRightInner.Y) / 2);

                            //Vector faceDirection = new Vector(
                            //    centerOfTwoEyes.X - centerOfMouth.X,
                            //    centerOfTwoEyes.Y - centerOfMouth.Y);

                            TextBox1.Text += " Node Tip X : " + noseX.ToString() + " Y " + noseY.ToString() + System.Environment.NewLine;
                            TextBox1.Text += " Left Pupil Tip X : " + leftPupilX.ToString() + " Y " + leftPupilY.ToString() + System.Environment.NewLine;
                            TextBox1.Text += " Right Pupil Tip X : " + rightPupilX.ToString() + " Y " + rightPupilY.ToString() + System.Environment.NewLine;
                            TextBox1.Text += " Upper Lip Bottom : " + upperLipBottom.X.ToString() + " " + upperLipBottom.Y.ToString() + System.Environment.NewLine;
                            TextBox1.Text += " Under Lip Bottom : " + underLipTop.X.ToString() + " " + underLipTop.Y.ToString() + System.Environment.NewLine;
                            TextBox1.Text += " Eye Left Inner : " + eyeLeftInner.X.ToString() + " " + eyeLeftInner.Y.ToString() + System.Environment.NewLine;
                            TextBox1.Text += " Eye Right Inner : " + eyeRightInner.X.ToString() + " " + eyeRightInner.Y.ToString() + System.Environment.NewLine;



                        }
                    }

                }

                using (Stream imageFileStream = new MemoryStream(data))
                {
                    //now get the computer vision api

                    try
                    {
                        VisionServiceClient VisionServiceClient = new VisionServiceClient(visioneapikey, visioneurl);
                        VisualFeature[] visualFeatures = new VisualFeature[]
                        { VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color,
                        VisualFeature.Description, VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags };

                        AnalysisResult analysisResult = await VisionServiceClient.DescribeAsync(imageFileStream, 3);

                        LogAnalysisResult(analysisResult);
                    }
                    catch (Exception ex)
                    {

                        //TextBox1.Text += "Error in Vision API: " + ex.Message.ToString();
                        showerror(ex);
                    }
                }

            }
            catch (Exception exception)
            {
                //TextBox1.Text += "Error " + exception.Message.ToString();
                //return null;
                showerror(exception);
            }
        }

        protected void LogAnalysisResult(AnalysisResult result)
        {
            if (result == null)
            {
                Log("null");
                return;
            }

            Log("Computer Vision API:");

            if (result.Metadata != null)
            {
                Log("Image Format : " + result.Metadata.Format);
                Log("Image Dimensions : " + result.Metadata.Width + " x " + result.Metadata.Height);
            }

            if (result.ImageType != null)
            {
                string clipArtType;
                switch (result.ImageType.ClipArtType)
                {
                    case 0:
                        clipArtType = "0 Non-clipart";
                        break;
                    case 1:
                        clipArtType = "1 ambiguous";
                        break;
                    case 2:
                        clipArtType = "2 normal-clipart";
                        break;
                    case 3:
                        clipArtType = "3 good-clipart";
                        break;
                    default:
                        clipArtType = "Unknown";
                        break;
                }
                Log("Clip Art Type : " + clipArtType);

                string lineDrawingType;
                switch (result.ImageType.LineDrawingType)
                {
                    case 0:
                        lineDrawingType = "0 Non-LineDrawing";
                        break;
                    case 1:
                        lineDrawingType = "1 LineDrawing";
                        break;
                    default:
                        lineDrawingType = "Unknown";
                        break;
                }
                Log("Line Drawing Type : " + lineDrawingType);
            }

            if (result.Adult != null)
            {
                Log("Is Adult Content : " + result.Adult.IsAdultContent);
                Log("Adult Score : " + result.Adult.AdultScore);
                Log("Is Racy Content : " + result.Adult.IsRacyContent);
                Log("Racy Score : " + result.Adult.RacyScore);
            }

            if (result.Categories != null && result.Categories.Length > 0)
            {
                Log("Categories : ");
                foreach (var category in result.Categories)
                {
                    Log("   Name : " + category.Name + "; Score : " + category.Score);
                }
            }

            if (result.Faces != null && result.Faces.Length > 0)
            {
                Log("Faces : ");
                foreach (var face in result.Faces)
                {
                    Log("   Age : " + face.Age + "; Gender : " + face.Gender);
                }
            }

            if (result.Color != null)
            {
                Log("AccentColor : " + result.Color.AccentColor);
                Log("Dominant Color Background : " + result.Color.DominantColorBackground);
                Log("Dominant Color Foreground : " + result.Color.DominantColorForeground);

                if (result.Color.DominantColors != null && result.Color.DominantColors.Length > 0)
                {
                    string colors = "Dominant Colors : ";
                    foreach (var color in result.Color.DominantColors)
                    {
                        colors += color + " ";
                    }
                    Log(colors);
                }
            }

            if (result.Description != null)
            {
                Log("Description : ");
                foreach (var caption in result.Description.Captions)
                {
                    Log("   Caption : " + caption.Text + "; Confidence : " + caption.Confidence);
                }
                string tags = "   Tags : ";
                foreach (var tag in result.Description.Tags)
                {
                    tags += tag + ", ";
                }
                Log(tags);

            }

            if (result.Tags != null)
            {
                Log("Tags : ");
                foreach (var tag in result.Tags)
                {
                    Log("   Name : " + tag.Name + "; Confidence : " + tag.Confidence + "; Hint : " + tag.Hint);
                }
            }

        }

        internal void Log(string message)
        {
            TextBox1.Text += String.Format(message) + System.Environment.NewLine;
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

        public void showerror(Exception ex)
        {
            try
            {
                TextBox1.Text += " Error: " + ex.Message.ToString() + System.Environment.NewLine;
                if (ex.InnerException != null)
                {
                    TextBox1.Text += " Inner Exception Error: " + ex.InnerException.Message.ToString() + System.Environment.NewLine;
                }
                if (ex.StackTrace != null)
                {
                    TextBox1.Text += " StackTrace Error: " + ex.StackTrace.ToString() + System.Environment.NewLine;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

    }
}