using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Cognitive.CustomVision.Prediction.Models;
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
    public partial class customvision : System.Web.UI.Page
    {
        // Add your training & prediction key from the settings page of the portal
        string trainingKey = ConfigurationManager.AppSettings["CustomVisionTraining"].ToString();
        string predictionKey = ConfigurationManager.AppSettings["CustomVisionPrediction"].ToString();

        private string SouthCentralUsEndpoint = ConfigurationManager.AppSettings["CustomVisionURL"].ToString();

        Guid projectid = new Guid(ConfigurationManager.AppSettings["customvisionprojectid"].ToString());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                objectdetect();
            }
        }

        public async void objectdetect()
        {
            try
            {
                //string testImageFile = @"D:\Pictures\test_img1.jpg";

                //string subscriptionKey = ConfigurationManager.AppSettings["subscriptionKey"].ToString();

                byte[] data = System.Convert.FromBase64String(Request.Form["formfield"]);

                if(data.Length > 0)
                {
                    using (Stream s = new MemoryStream(data))
                    {

                        // Create a prediction endpoint, passing in obtained prediction key
                        CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
                        {
                            ApiKey = predictionKey,
                            Endpoint = SouthCentralUsEndpoint
                        };

                        // Make a prediction against the new project
                        TextBox1.Text = string.Format("Making a prediction:") + System.Environment.NewLine;
                        var result = endpoint.PredictImage(projectid, s);

                        // Loop over each prediction and write out the results
                        foreach (var c in result.Predictions)
                        {
                            TextBox1.Text += string.Format($"\t{c.TagName}: {c.Probability:P1}" + System.Environment.NewLine);
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                //trainmodel();
                if (FileUpload1.HasFile)
                {
                    string fileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    FileUpload1.PostedFile.SaveAs(Server.MapPath("~/Images/") + fileName);
                    //Response.Redirect(Request.Url.AbsoluteUri);

                    // Create the Api, passing in the training key
                    //CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient()
                    //{
                    //    ApiKey = trainingKey,
                    //    Endpoint = SouthCentralUsEndpoint
                    //};

                    //var project = trainingApi.GetProject(projectid);


                    using (var stream = new MemoryStream(File.ReadAllBytes(Server.MapPath("~/Images/") + fileName)))
                    {
                       
                        // Create a prediction endpoint, passing in obtained prediction key
                        CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
                        {
                            ApiKey = predictionKey,
                            Endpoint = SouthCentralUsEndpoint
                        };

                        var predictions = new ImagePredictionResultModel();

                        // Make a prediction against the new project
                        TextBox1.Text = string.Format("Making a prediction:") + System.Environment.NewLine;
                        var result = endpoint.PredictImage(projectid, stream);

                        

                        // Loop over each prediction and write out the results
                        foreach (var c in result.Predictions)
                        {
                            TextBox1.Text += string.Format($"\t{c.TagName}: {c.Probability:P1}" + System.Environment.NewLine);
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


    }
}