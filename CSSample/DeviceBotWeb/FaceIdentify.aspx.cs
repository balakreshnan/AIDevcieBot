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
using System.Collections.ObjectModel;

namespace DeviceBotWeb
{
    public partial class FaceIdentify : System.Web.UI.Page
    {

        string secret = ConfigurationManager.AppSettings["AppSecret"].ToString();
        string appid = ConfigurationManager.AppSettings["AppId"].ToString();

        public static string retval = String.Empty;
        public string conversationid = string.Empty;

        byte[] data;
        byte[] data1;

        private ObservableCollection<Face> _faces = new ObservableCollection<Face>();

        public ObservableCollection<Face> TargetFaces
        {
            get
            {
                return _faces;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if(Page.IsPostBack)
                {
                    //if(System.Convert.FromBase64String(Request.Form["formfield"]).Length > 1)
                    //{
                    //    data = System.Convert.FromBase64String(Request.Form["formfield"]);
                    //}
                    //if (System.Convert.FromBase64String(Request.Form["formfield1"]).Length > 1)
                    //{
                    //    data1 = System.Convert.FromBase64String(Request.Form["formfield1"]);
                    //}
                    Identify();


                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            try
            {
                Identify();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async void Identify()
        {
            try
            {
                string subscriptionKey = ConfigurationManager.AppSettings["subscriptionKey"].ToString();

                byte[] data = System.Convert.FromBase64String(Request.Form["formfield"]);

                Guid face1 = new Guid();
                Guid face2 =  new Guid();

                //byte[] data1 = System.Convert.FromBase64String(Request.Form["formfield1"]);

                //using (Stream imageFileStream = new MemoryStream(data))
                using (Stream imageFileStream = new MemoryStream(data))
                {
                    using (var imageFileStream1 = File.OpenRead(Server.MapPath("~/images/People/bala/babal.jpg")))
                    {
                        using (FaceServiceClient faceClient = new FaceServiceClient("0f79d48df54e47b5b3a298187b22bfca"))
                        {
                            Microsoft.ProjectOxford.Face.Contract.Face[] faces = await faceClient.DetectAsync(imageFileStream, true, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.FacialHair, FaceAttributeType.Emotion });
                            Microsoft.ProjectOxford.Face.Contract.Face[] faces1 = await faceClient.DetectAsync(imageFileStream1, true, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.FacialHair, FaceAttributeType.Emotion });


                            foreach (var f in faces)
                            {
                                face1 = f.FaceId;
                            }

                            foreach (var f in faces1)
                            {
                                face2 = f.FaceId;
                            }

                            try
                            {
                                var res = await faceClient.VerifyAsync(face1, face2);

                                TextBox1.Text = string.Format("Confidence = {0:0.00}, {1}", res.Confidence, res.IsIdentical ? "two faces belong to same person" : "two faces not belong to same person");
                            }
                            catch (Exception ex)
                            {

                                TextBox1.Text = "Error : " + ex.Message.ToString() + " Stack: " + ex.StackTrace.ToString() + " Inner:" + ex.InnerException.ToString();
                            }
;


                        }
                    }

                }



            }
            catch (Exception ex)
            {
                TextBox1.Text = "Error : " + ex.Message.ToString() + " Stack: " + ex.StackTrace.ToString() + " Inner:" + ex.InnerException.ToString();
                //throw ex;
            }
        }

        //public static IEnumerable<Face> CalculateFaceRectangleForRendering(IEnumerable<Microsoft.ProjectOxford.Face.Contract.Face> faces, int maxSize, Tuple<int, int> imageInfo)
        //{
        //    var imageWidth = imageInfo.Item1;
        //    var imageHeight = imageInfo.Item2;
        //    float ratio = (float)imageWidth / imageHeight;
        //    int uiWidth = 0;
        //    int uiHeight = 0;
        //    if (ratio > 1.0)
        //    {
        //        uiWidth = maxSize;
        //        uiHeight = (int)(maxSize / ratio);
        //    }
        //    else
        //    {
        //        uiHeight = maxSize;
        //        uiWidth = (int)(ratio * uiHeight);
        //    }

        //    int uiXOffset = (maxSize - uiWidth) / 2;
        //    int uiYOffset = (maxSize - uiHeight) / 2;
        //    float scale = (float)uiWidth / imageWidth;

        //    foreach (var face in faces)
        //    {
        //        yield return new Face()
        //        {
        //            FaceId = face.FaceId.ToString(),
        //            Left = (int)((face.FaceRectangle.Left * scale) + uiXOffset),
        //            Top = (int)((face.FaceRectangle.Top * scale) + uiYOffset),
        //            Height = (int)(face.FaceRectangle.Height * scale),
        //            Width = (int)(face.FaceRectangle.Width * scale),
        //        };
        //    }
        //}


    }
}