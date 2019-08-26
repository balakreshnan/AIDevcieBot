using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace BalaBot
{

    public enum ContactOptions { Kari, Kenny, Bala, Julie, Beth, Chris };


    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".")]
    [Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your options? {||}")]
    public class BalaBot
    {
        private const string ContactCard = "Contact card";

        [Prompt("What is your use case/issue? {||}")]
        public string musecase;

        [Prompt("Would like to contact Azure Sales? {||}")]
        public string msales;

        [Prompt("Would like to contact Cloud Solution Architect? {||}")]
        public string mcsa;

        [Prompt("Would like to contact Data Soulution Architect? {||}")]
        public string mdsa;

        [Prompt("Can you explain about your Use Case/Issue? {||}")]
        public string mproject;

        [Prompt("What is the time frame? {||}")]
        public string mtimeframe;

        [Prompt("What is your priority? {||}")]
        public string mpriority;

        [Prompt("Are you considering Cloud First Strategy? {||}")]
        public string mcloudfirst;

        [Prompt("Are you considering Mobile First Strategy? {||}")]
        public string mmobilefirst;

        [Prompt("Does the Use Case involves IoT Schenraio or Sensor based data? {||}")]
        public string miot;

        [Prompt("Would you consider Converstation as a service? {||}")]
        public string mcaas;

        [Prompt("Would you consider Platform as a service? {||}")]
        public string mpaas;

        [Prompt("Would you like Intelligence added? {||}")]
        public string mintelli;

        [Prompt("Enter your email address {||}")]
        public string memail;

        //[Prompt("Who would you like to connect with?")]
        //public List<ContactOptions> Contact;
        public ContactOptions? Contact;



        //public IList<Attachment> mall = GetContactCard();

        public static IForm<BalaBot> BuildForm()
        {

            OnCompletionAsyncDelegate<BalaBot> processOrder = async (context, state) =>
            {
                string tmpissue = state.musecase;
                //string tmpsales = state.msales;
                //string tmpcsa = state.mcsa;
                //string tmpdsa = state.mdsa;
                string tmpproject = state.mproject;
                string tmptimeframe = state.mtimeframe;
                string tmppriority = state.mpriority;
                string tmpcloudfirst = state.mcloudfirst;
                string tmpmobilefirst = state.mmobilefirst;
                string tmpcaas = state.mcaas;
                string tmppaas = state.mpaas;
                string tmpintelli = state.mintelli;
                string tmpiot = state.miot;
                string tmpemail = state.memail;
                string tmpcontact = state.Contact.ToString();

                //string tmpaftermarkey = state.maftermarket;
                string tmpreturn = string.Empty;
                tmpreturn = "Azure " + tmpissue + " " + tmpproject + " " + tmptimeframe + " " + tmppriority + " " + tmpcloudfirst;
                tmpreturn += " " + tmpmobilefirst + " " + tmpcaas + " " + tmppaas + " " + tmpintelli + " " + tmpiot + " " + tmpemail;
                //tmpreturn = getbatteryInfo(tmpmodel, tmpmake, tmpyear, tmpmliter, tmpaftermarkey);

                if (tmpreturn == string.Empty || tmpreturn == "")
                {
                    tmpreturn = "Oops we couldn't find a option for your request please try again";
                }

                if (tmpiot.ToLower().Equals("yes"))
                {
                    tmpreturn = "Check Azure IOT Suite - https://www.microsoft.com/en-us/cloud-platform/internet-of-things-azure-iot-suite";
                }
                else if (tmpcloudfirst.ToLower().Equals("yes") || tmpmobilefirst.ToLower().Equals("yes") || tmpintelli.ToLower().Equals("yes"))
                {
                    tmpreturn = "Check CIS - Cortana Intelligence Suite - https://www.microsoft.com/en-us/cloud-platform/cortana-intelligence-suite";
                }
                else
                {
                    tmpreturn = "Please look at some on premise solutions available from our portfolio of products";
                }

                //try
                //{
                //    sendemail(tmpissue, tmpproject, tmptimeframe, tmppriority, tmpcloudfirst, tmpmobilefirst, tmpcaas, tmppaas, tmpintelli, tmpiot, tmpemail);
                //}
                //catch (Exception ex)
                //{

                //    throw ex;
                //}
                


                await context.PostAsync("We will have someone reach out to you. Recommendations are " + tmpreturn + ". ");
            };


            return new FormBuilder<BalaBot>()
                     .Message("Welcome to BalaBot. How are you today?")
                     .Message("Please answer these questions to Qualify the Solution to use.")
                     .Field(nameof(BalaBot.musecase))
                     //.Field(nameof(BalaBot.msales))
                     //.Field(nameof(BalaBot.mcsa))
                     //.Field(nameof(BalaBot.mdsa))
                     .Field(nameof(BalaBot.mproject))
                     .Field(nameof(BalaBot.mtimeframe))
                     .Field(nameof(BalaBot.mpriority))
                     .Field(nameof(BalaBot.mcloudfirst))
                     .Field(nameof(BalaBot.mmobilefirst))
                     .Field(nameof(BalaBot.miot))
                     .Field(nameof(BalaBot.mcaas))
                     .Field(nameof(BalaBot.mpaas))
                     .Field(nameof(BalaBot.mintelli))
                     .Field(nameof(BalaBot.memail))
                     //.Field(nameof(BalaBot.Contact),
                     //       validate: async (state, value) =>
                     //       {
                     //           var values = ((List<object>)value).OfType<ContactOptions>();
                     //           var result = new ValidateResult { IsValid = true, Value = values };
                     //           if (values != null)
                     //           {
                     //               result.Value = (from ContactOptions ct in Enum.GetValues(typeof(ContactOptions))
                     //                               where ct.Equals(values.Contains(ct))
                     //                               select ct).ToList();
                     //           }
                     //           return result;
                     //       })
                     //   .Message("For Contacts you have selected {Contact}.")
                     .Field(nameof(BalaBot.Contact))
                     //.Confirm("The below you have selected Use Case: {musecase} Time Frame: {mtimeframe} Priority: {mpriority} {mcloudfirst} {mmobilefirst} {mcaas} {mpaas} {mintelli}?")
                     //.Confirm("The below you have selected Use Case: {musecase} Time Frame: {mtimeframe} Priority: {mpriority} {mcloudfirst} {mmobilefirst} {mcaas} {mpaas} {mintelli}?")
                     .OnCompletion(processOrder)
                     //.Message("Thanks for selecting the battery and have a great day!")
                     .Build();
        }

        private static IList<Microsoft.Bot.Connector.Attachment> GetContactCard()
        {
            //var resultMessage = context.MakeMessage();
            //resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            //resultMessage.Attachments = new List<Attachment>();

            var resultMessage = new List<Microsoft.Bot.Connector.Attachment>();

            var contactcard = new ThumbnailCard
            {
                Title = "Kari Bolger",
                Subtitle = "PSS - Azure Solution Specialists",
                Text = "kabolger@microsoft.com",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://azure.microsoft.com/") }
            };

            resultMessage.Add(contactcard.ToAttachment());

            contactcard = new ThumbnailCard
            {
                Title = "Kenny Young",
                Subtitle = "CSA - Cloud Solution Architect",
                Text = "keyo@microsoft.com",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://azure.microsoft.com/") }
            };

            resultMessage.Add(contactcard.ToAttachment());

            contactcard = new ThumbnailCard
            {
                Title = "Balamurugan Balakreshnan",
                Subtitle = "DSA - Data Solution Architect",
                Text = "babal@microsoft.com",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://azure.microsoft.com/") }
            };

            resultMessage.Add(contactcard.ToAttachment());

            //return contactcard.ToAttachment();
            return resultMessage;
        }

        public static void sendemail(string tmpissue, string tmpproject,
            string tmptimeframe, string tmppriority, string tmpcloudfirst,string tmpmobilefirst,
            string tmpcaas, string tmppaas, string tmpintelli, string tmpiot,string tmpemail)
        {
            try
            {
                //MailMessage mail = new MailMessage("babal@microsoft.com", tmpemail);
                //SmtpClient client = new SmtpClient();
                //client.Port = 25;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.UseDefaultCredentials = false;
                //client.Host = "smtp.live.com";
                //client.UseDefaultCredentials = false;
                //client.Timeout = 10000;
                //client.Credentials = new System.Net.NetworkCredential("bala_b@hotmail.com", "Bala!234");
                //mail.BodyEncoding = UTF8Encoding.UTF8;
                //mail.Subject = "Lead Generated from Bot ";
                String msg = String.Empty;
                msg += " Issue: " + tmpissue + System.Environment.NewLine;
                msg += " Issue Detail: " + tmpproject + System.Environment.NewLine;
                msg += " Time Frame: " + tmptimeframe + System.Environment.NewLine;
                msg += " Priority: " + tmppriority + System.Environment.NewLine;
                msg += " Cloud First: " + tmpcloudfirst + System.Environment.NewLine;
                msg += " Mobile First: " + tmpmobilefirst + System.Environment.NewLine;
                msg += " CaaS: " + tmpcaas + System.Environment.NewLine;
                msg += " PaaS: " + tmppaas + System.Environment.NewLine;
                msg += " ML (Intelligent) : " + tmpintelli + System.Environment.NewLine;
                msg += " IOT : " + tmpiot + System.Environment.NewLine;
                msg += " Email : " + tmpemail + System.Environment.NewLine;


                //mail.Body = msg;
                //client.Send(mail);

                string smtpAddress = "smtp.live.com";
                int portNumber = 587;
                bool enableSSL = true;

                string emailFrom = "bala_b@hotmailcom";
                string password = "Bala!234";
                string emailTo = "babal@microsoft.com";
                string subject = "Lead Generated from Bot";
                string body = msg;


                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    // Can set to false, if you are sending pure text.

                    //mail.Attachments.Add(new Attachment("C:\\SomeFile.txt"));
                    //mail.Attachments.Add(new Attachment("C:\\SomeZip.zip"));

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }

}