using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace BotClientWeb
{
    delegate void SetTextCallback(string text);

    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        static async Task<String> MakeRequest(String searchquery)
        {
            string retval = string.Empty;

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "31a15bf5cd7a449dacb8b6454ec87590");

            // Request parameters
            //queryString["q"] = "bill gates";
            queryString["q"] = searchquery;
            queryString["count"] = "10";
            queryString["offset"] = "0";
            queryString["mkt"] = "en-us";
            queryString["safesearch"] = "Moderate";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/search?" + queryString;

            //var response = await client.GetAsync(uri);
            //if(response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    retval = response.Content.ToString();

            //}
            var response = await client.GetStringAsync(uri);
            var result = JsonConvert.DeserializeObject<Object>(response);

            retval = response;

            return retval;
        }



        protected async void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                String results = await MakeRequest(TextBox2.Text);
                TextBox1.Text = results + System.Environment.NewLine;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}