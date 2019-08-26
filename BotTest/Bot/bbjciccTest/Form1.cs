using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Bot.Connector;
using System.Configuration;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Bot.Connector.DirectLine.Models;
using System.Web;

namespace bbjciccTest
{
    

    public partial class Form1 : Form
    {
        string secret = ConfigurationManager.AppSettings["AppSecret"].ToString();
        string appid = ConfigurationManager.AppSettings["AppId"].ToString();

        public static string retval = String.Empty;

        public delegate void UpdateStatusDelegate(string data);


        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string secretout = appid + ":" + secret;
                var uri = new Uri("https://directline.botframework.com");

                string newsecret = "zHhveP4jk4o.cwA.cIA.QERY2t7safaD557XAYfyFp0WZrpQji3s2nI7QbPunSQ";

                DirectLineClientCredentials creds = new DirectLineClientCredentials(newsecret);

                DirectLineClient client = new DirectLineClient(uri, creds);
                Microsoft.Bot.Connector.DirectLine.Conversations convs = new Microsoft.Bot.Connector.DirectLine.Conversations(client);

                string waterMark;

                var conv = convs.NewConversation();
                var set = convs.GetMessages(conv.ConversationId);
                waterMark = set.Watermark;

                Microsoft.Bot.Connector.DirectLine.Models.Message message = new Microsoft.Bot.Connector.DirectLine.Models.Message(conversationId: conv.ConversationId, text: textBox1.Text);
                //Console.WriteLine(message.Text);
                convs.PostMessage(conv.ConversationId, message);

                set = convs.GetMessages(conv.ConversationId, waterMark);
                PrintResponse(set);
                waterMark = set.Watermark;

                string x = await MakeRequest(textBox1.Text);

                //System.Threading.Thread.Sleep(15000);

                if (x != String.Empty)
                {
                    textBox2.Text += " Return JSON: " + x + System.Environment.NewLine;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        void PrintResponse(MessageSet set)
        {
            var q = from x in set.Messages
                    where x.FromProperty == appid
                    select x.Text;

            foreach (var str in q.ToList())
            {
                textBox3.Text += "Responce from bot >> " + str + System.Environment.NewLine;
            }


        }

        static async Task<string> MakeRequest(string strquery)
        {

            string retval = string.Empty;

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            //bbjcicclang

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "3aaab437f88a4371988b6e09028dbc55");

            //var uri = "https://api.projectoxford.ai/luis/v1.0/prog/apps/9bff0685-3fd6-4249-bfa3-ae1be37f10ee/actions?" + queryString;

            //HttpResponseMessage response;

            //// Request body
            //byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            //using (var content = new ByteArrayContent(byteData))
            //{
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //    response = await client.PostAsync(uri, content);
            //}

            //var url = "https://api.projectoxford.ai/luis/v1/application?id=9bff0685-3fd6-4249-bfa3-ae1be37f10ee&subscription-key=3aaab437f88a4371988b6e09028dbc55"


            // Request parameters
            queryString["id"] = "9bff0685-3fd6-4249-bfa3-ae1be37f10ee";
            //queryString["q"] = "Hi welcome to optima, how can I help you?";
            queryString["q"] = strquery;
            //var uri = "https://api.projectoxford.ai/luis/v1/application/preview?" + queryString;
            var uri = "https://api.projectoxford.ai/luis/v1/application?" + queryString;

            var response = await client.GetAsync(uri);

            //string contentback = response.Content.ToString();
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();
                retval = result;

                //AppendTextBox(retval);

                //AppendTextBox(retval.ToString());

                // ... Display the result.
                if (result != null &&
                result.Length >= 50)
                {
                    //Console.WriteLine(result.Substring(0, 50) + "...");
                    object ret = JsonConvert.DeserializeObject<object>(result);



                }
            }

            return retval;
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            textBox2.Text += value;
        }

        public void UpdateStatus(string data)
        {
            if (this.textBox2.InvokeRequired)
            {
                UpdateStatusDelegate d = new UpdateStatusDelegate(UpdateStatus);
                this.Invoke(d, new object[] { data });
            }
            else
            {
                this.textBox2.Text = data;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string x = await MakeRequest(textBox1.Text);

                //System.Threading.Thread.Sleep(15000);

                if (x != String.Empty)
                {
                    textBox2.Text += " Return JSON: " + x + System.Environment.NewLine;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string secretout = appid + ":" + secret;
                //var uri = new Uri("https://directline.botframework.com");
                var uri = new Uri(ConfigurationManager.AppSettings["DirectLineURL"].ToString());

                //string newsecret = "zHhveP4jk4o.cwA.cIA.QERY2t7safaD557XAYfyFp0WZrpQji3s2nI7QbPunSQ";

                string newsecret = ConfigurationManager.AppSettings["DirectLineSecret"].ToString();

                DirectLineClientCredentials creds = new DirectLineClientCredentials(newsecret);

                DirectLineClient client = new DirectLineClient(uri, creds);
                Microsoft.Bot.Connector.DirectLine.Conversations convs = new Microsoft.Bot.Connector.DirectLine.Conversations(client);

                string waterMark;

                var conv = convs.NewConversation();
                var set = convs.GetMessages(conv.ConversationId);
                waterMark = set.Watermark;

                Microsoft.Bot.Connector.DirectLine.Models.Message message = new Microsoft.Bot.Connector.DirectLine.Models.Message(conversationId: conv.ConversationId, text: textBox1.Text);
                //Console.WriteLine(message.Text);
                convs.PostMessage(conv.ConversationId, message);

                set = convs.GetMessages(conv.ConversationId, waterMark);
                PrintResponse(set);
                waterMark = set.Watermark;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        //end of class
    }

    //end of namespace
}
