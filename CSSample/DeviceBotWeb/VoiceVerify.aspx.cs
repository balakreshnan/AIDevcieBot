using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Verification;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text;
using System.Configuration;

namespace DeviceBotWeb
{
    public partial class VoiceVerify : System.Web.UI.Page
    {

        string secret = ConfigurationManager.AppSettings["AppSecret"].ToString();
        string appid = ConfigurationManager.AppSettings["AppId"].ToString();

        public static string retval = String.Empty;
        public string conversationid = string.Empty;

        byte[] data;
        byte[] data1;

        string blobconnectionstring = "DefaultEndpointsProtocol=https;AccountName=bbiotstore;AccountKey=5b9ZqSnpeKwswgoh5tvKuhYGsgzB8/Yl3wZcLyHkds+URjpp5ZYK4WQfs7niCt2jfjnHX8R5vg2Dl97u5Q2pbg==;EndpointSuffix=core.windows.net";


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ss_Click(object sender, EventArgs e)
        {
            try
            {
                saveimage();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void saveimage()
        {
            try
            {
                string subscriptionKey = ConfigurationManager.AppSettings["subscriptionKey"].ToString();

                string data1 = Request.Form["formfield"];

                string[] spdata = data1.Split(',');


                Encoding encoding = Encoding.UTF8;


                //byte[] data = Convert.FromBase64String(Request.Form["formfield"]);
                //byte[] data = encoding.GetBytes(Request.Form["formfield"]);
                byte[] data = System.Convert.FromBase64String(spdata[1]);

                if (data != null)
                {
                    using (MemoryStream ms = new MemoryStream(data))
                    //using (var imageFileStream1 = File.OpenRead(Server.MapPath("~/images/People/bala/babal.jpg")))
                    {
                        //System.Drawing.Image i = System.Drawing.Image.FromStream(ms);




                        string imagefname = name.Text.ToString() + "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav";

                        string blobname = name.Text.ToString() + "/" + name.Text.ToString() + "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav";

                        // Parse the connection string and return a reference to the storage account.
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                            CloudConfigurationManager.GetSetting("StorageConnectionString"));
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        // Retrieve a reference to a container.
                        CloudBlobContainer container = blobClient.GetContainerReference("bbvoice1");

                        // Create the container if it doesn't already exist.
                        container.CreateIfNotExists();

                        container.SetPermissions(
    new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                        // Retrieve reference to a blob named "myblob".
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(imagefname);
                        blockBlob.Properties.ContentType = "audio/wav";
                        //blockBlob.UploadFromStream(ms);
                        blockBlob.UploadFromByteArray(data, 0, data.Length);

                        verifySpeaker(data, imagefname);

                        //i.Save(Server.MapPath("~/images/People/" + name.Text.ToString() + "/" + imagefname), System.Drawing.Imaging.ImageFormat.Jpeg);

                    }
                }



            }
            catch (Exception ex)
            {

                //throw ex; 
            }
        }

        public async void verifySpeaker(byte[] data,string profilename)
        {
            try
            {
                SpeakerVerificationServiceClient _serviceClient = new SpeakerVerificationServiceClient("7afe1cdfee8243c0aca9fadc7d42f978");
                Guid _speakerId = Guid.Empty;
                using (Stream audioStream = new MemoryStream(data))
                {
                    
                    Verification response = await _serviceClient.VerifyAsync(audioStream, _speakerId);
                    if (response.Result == Result.Accept)
                    {
                        TextBox1.Text = "Accept";
                        
                    }
                    else
                    {
                        TextBox1.Text = "Rejected";
                        
                    }
                }
                    
            }
            catch (Exception ex )
            {

                throw ex;
            }
        }


    }
}