using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using System.Threading.Tasks;

namespace DeviceBotWeb
{


    public partial class VoiceIdentification : System.Web.UI.Page
    {
        string secret = ConfigurationManager.AppSettings["AppSecret"].ToString();
        string appid = ConfigurationManager.AppSettings["AppId"].ToString();

        public static string retval = String.Empty;
        public string conversationid = string.Empty;

        byte[] data;
        byte[] data1;

        string blobconnectionstring = "DefaultEndpointsProtocol=https;AccountName=bbiotstore;AccountKey=xxxx;EndpointSuffix=core.windows.net";


        protected void Page_Load(object sender, EventArgs e)
        {

        }


        //try to save the audio file
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
                        voicetoprofile(data, imagefname);
                        //i.Save(Server.MapPath("~/images/People/" + name.Text.ToString() + "/" + imagefname), System.Drawing.Imaging.ImageFormat.Jpeg);

                    }
                }



            }
            catch (Exception ex)
            {

                //throw ex; 
            }
        }

        public async void voicetoprofile(byte[] data, string filename)
        {
            try
            {
                SpeakerIdentificationServiceClient _serviceClient = new SpeakerIdentificationServiceClient("7afe1cdfee8243c0aca9fadc7d42f978");
                CreateProfileResponse creationResponse = await _serviceClient.CreateProfileAsync(name.Text.ToString());
                Profile profile = await _serviceClient.GetProfileAsync(creationResponse.ProfileId);
                //SpeakersListPage.SpeakersList.AddSpeaker(profile);
                OperationLocation processPollingLocation;
                using (Stream audioStream = new MemoryStream(data))
                {
                    //_selectedFile = "";
                    processPollingLocation = await _serviceClient.EnrollAsync(audioStream, profile.ProfileId);
                }

                EnrollmentOperation enrollmentResult;
                int numOfRetries = 10;
                TimeSpan timeBetweenRetries = TimeSpan.FromSeconds(5.0);
                while (numOfRetries > 0)
                {
                    await Task.Delay(timeBetweenRetries);
                    enrollmentResult = await _serviceClient.CheckEnrollmentStatusAsync(processPollingLocation);

                    if (enrollmentResult.Status == Status.Succeeded)
                    {
                        break;
                    }
                    else if (enrollmentResult.Status == Status.Failed)
                    {
                        throw new EnrollmentException(enrollmentResult.Message);
                    }
                    numOfRetries--;
                }
                if (numOfRetries <= 0)
                {
                    throw new EnrollmentException("Enrollment operation timeout.");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
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



    }
}
