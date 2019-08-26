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
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.Text;

namespace DeviceBotWeb
{
    public partial class FaceTrain : System.Web.UI.Page
    {

        string secret = ConfigurationManager.AppSettings["AppSecret"].ToString();
        string appid = ConfigurationManager.AppSettings["AppId"].ToString();

        public static string retval = String.Empty;
        public string conversationid = string.Empty;

        byte[] data;
        byte[] data1;

        string blobconnectionstring = "DefaultEndpointsProtocol=https;AccountName=bbiotstore;AccountKey=5b9ZqSnpeKwswgoh5tvKuhYGsgzB8/Yl3wZcLyHkds+URjpp5ZYK4WQfs7niCt2jfjnHX8R5vg2Dl97u5Q2pbg==;EndpointSuffix=core.windows.net";

        string faceapikey = ConfigurationManager.AppSettings["FaceAPIKey"].ToString();


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
                    saveimage();
                }
            }
            catch (Exception ex)
            {
                showerror(ex);
                //throw ex;
            }
        }

        public void saveimage()
        {
            try
            {
                string subscriptionKey = ConfigurationManager.AppSettings["subscriptionKey"].ToString();

                byte[] data = System.Convert.FromBase64String(Request.Form["formfield"]);
                if(data != null)
                {
                    using (MemoryStream ms = new MemoryStream(data))
                    //using (var imageFileStream1 = File.OpenRead(Server.MapPath("~/images/People/bala/babal.jpg")))
                    {
                        System.Drawing.Image i = System.Drawing.Image.FromStream(ms);

                        string imagefname = name.Text.ToString() + "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

                        string blobname = name.Text.ToString() + "/" + name.Text.ToString() + "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

                        // Parse the connection string and return a reference to the storage account.
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                            CloudConfigurationManager.GetSetting("StorageConnectionString"));
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        // Retrieve a reference to a container.
                        CloudBlobContainer container = blobClient.GetContainerReference("bbpeople");

                        // Create the container if it doesn't already exist.
                        container.CreateIfNotExists();

                        container.SetPermissions(
    new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                        // Retrieve reference to a blob named "myblob".
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(imagefname);
                        blockBlob.Properties.ContentType = "Jpeg";
                        //blockBlob.UploadFromStream(ms);
                        blockBlob.UploadFromByteArray(data, 0, data.Length);

                        //i.Save(Server.MapPath("~/images/People/" + name.Text.ToString() + "/" + imagefname), System.Drawing.Imaging.ImageFormat.Jpeg);

                    }
                }



            }
            catch (Exception ex)
            {
                //showerror(ex);
                //throw ex;
                //throw ex; 
            }
        }

        public async void trainmodel()
        {
            try
            {
                // Create an empty person group
                string personGroupId = "myfriends";

                using (FaceServiceClient faceClient = new FaceServiceClient(faceapikey))
                {
                    //await faceClient.CreatePersonGroupAsync(personGroupId, "myfriends");

                    // Define name here
                    CreatePersonResult friend1 = await faceClient.CreatePersonAsync(
                        // Id of the person group that the person belonged to
                        personGroupId,
                        // Name of the person
                        name.Text.ToString()
                    );

                    // Define Bill and Clare in the same way
                    // Directory contains image files of Anna
                    //const string friend1ImageDir = Server.MapPath("~/images/People/" + name.Text.ToString() + "/");

                    //foreach (string imagePath in Directory.GetFiles(Server.MapPath("~/images/People/" + name.Text.ToString() + "/"), "*.jpg"))
                    //{
                    //    using (Stream s = File.OpenRead(imagePath))
                    //    {
                    //        // Detect faces in the image and add to Anna
                    //        await faceClient.AddPersonFaceAsync(
                    //            personGroupId, friend1.PersonId, s);
                    //    }
                    //}

                    // Retrieve storage account from connection string.
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                        CloudConfigurationManager.GetSetting("StorageConnectionString"));

                    // Create the blob client.
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    // Retrieve reference to a previously created container.
                    CloudBlobContainer container = blobClient.GetContainerReference("bbpeople");

                    // Loop over items within the container and output the length and URI.
                    foreach (IListBlobItem item in container.ListBlobs(null, false))
                    {
                        if (item.GetType() == typeof(CloudBlockBlob))
                        {
                            CloudBlockBlob blob = (CloudBlockBlob)item;
                            if(blob.Name.StartsWith(name.Text))
                            {
                                CloudBlockBlob blockBlob2 = container.GetBlockBlobReference(blob.Name);
                                string text;
                                using (var memoryStream = new MemoryStream())
                                {
                                    //memoryStream.Position = 0;

                                    blockBlob2.DownloadToStream(memoryStream);
                                    //text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray(),0,(int)memoryStream.Length);
                                    //var mem = new MemoryStream();
                                    //memoryStream.Position = 0;
                                    //memoryStream.CopyTo(mem, (int)memoryStream.Length);


                                    //MemoryStream mStrm = new MemoryStream(Encoding.UTF8.GetBytes(text));

                                    //text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                                    // Detect faces in the image and add to Anna
                                    //await faceClient.AddPersonFaceAsync(personGroupId, friend1.PersonId, memoryStream);
                                    //await faceClient.AddPersonFaceAsync(personGroupId, friend1.PersonId, blob.Name);
                                    try
                                    {
                                        await faceClient.AddPersonFaceAsync(personGroupId, friend1.PersonId, blob.Uri.ToString());
                                    }
                                    catch (Exception e)
                                    {
                                        showerror(e);
                                        //throw e;
                                   }
                                    
                                }

                                                            

                                
                            }

                            //Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

                        }
                        else if (item.GetType() == typeof(CloudPageBlob))
                        {
                            CloudPageBlob pageBlob = (CloudPageBlob)item;

                            //Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

                        }
                        else if (item.GetType() == typeof(CloudBlobDirectory))
                        {
                            CloudBlobDirectory directory = (CloudBlobDirectory)item;

                            //Console.WriteLine("Directory: {0}", directory.Uri);
                        }
                    }

                        // Do the same for Other Users as well and Clare
                        await faceClient.TrainPersonGroupAsync(personGroupId);

                    TrainingStatus trainingStatus = null;
                    while (true)
                    {
                        trainingStatus = await faceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                        if (trainingStatus.Status.ToString() != "running")
                        {
                            break;
                        }

                        await Task.Delay(1000);
                    }


                }




            }
            catch (Exception ex)
            {
                showerror(ex);
                //throw ex;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                trainmodel();
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

    }
}