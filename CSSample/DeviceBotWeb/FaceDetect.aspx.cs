using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeviceBotWeb
{
    public partial class FaceDetect : System.Web.UI.Page
    {
        string faceapikey = ConfigurationManager.AppSettings["FaceAPIKey"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Page.IsPostBack)
            {
                facedetect();
            }
        }

        public async void facedetect()
        {
            try
            {
                //string testImageFile = @"D:\Pictures\test_img1.jpg";

                string subscriptionKey = ConfigurationManager.AppSettings["subscriptionKey"].ToString();

                byte[] data = System.Convert.FromBase64String(Request.Form["formfield"]);

                using (Stream s = new MemoryStream(data))
                {
                    using (FaceServiceClient faceClient = new FaceServiceClient(faceapikey))
                    {
                        var faces = await faceClient.DetectAsync(s);
                        var faceIds = faces.Select(face => face.FaceId).ToArray();
                        string personGroupId = "myfriends";
                        var results = await faceClient.IdentifyAsync(personGroupId, faceIds);
                        foreach (var identifyResult in results)
                        {
                            if (identifyResult.Candidates.Length == 0)
                            {
                                TextBox1.Text = string.Format("No one identified");
                            }
                            else
                            {
                                // Get top 1 among all candidates returned
                                var candidateId = identifyResult.Candidates[0].PersonId;
                                var person = await faceClient.GetPersonAsync(personGroupId, candidateId);
                                TextBox1.Text = string.Format("Identified as {0}", person.Name);
                            }
                        }
                    }
                       


                }

            }
            catch (Exception ex)
            {
                showerror(ex);
                //throw ex;
            }
        }

        public void showerror(Exception ex)
        {
            try
            {
                TextBox1.Text += " Error: " + ex.Message.ToString() + System.Environment.NewLine;
                if(ex.InnerException != null)
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