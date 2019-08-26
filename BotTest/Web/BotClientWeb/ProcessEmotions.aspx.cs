using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.Configuration;
using System.IO;

namespace BotClientWeb
{
    public partial class ProcessEmotions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                
                if (Page.IsPostBack)
                {
                    // HTTP Post
                }
                else
                {
                    // HTTP Get
                    getemotions();
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
                byte[] data = System.Convert.FromBase64String(Request.QueryString["formfield"]);

                using (Stream imageFileStream = new MemoryStream(data)) //File.OpenRead(Request.Form["image"]))
                {
                    //
                    // Detect the emotions in the URL
                    //
                    emotionResult = await emotionServiceClient.RecognizeAsync(imageFileStream);
                    //return emotionResult;
                }
            }
            catch (Exception exception)
            {
                
                //return null;
            }
        }


    }
}