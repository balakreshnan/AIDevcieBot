using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BalaBot
{
    [Serializable]
    public class ContactCard : IDialog<object>
    {
        public IList<Attachment> all;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }



        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var reply = context.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetContactCard();

            await context.PostAsync(reply);
            context.Wait(this.MessageReceivedAsync);

        }

        private static IList<Attachment> GetContactCard()
        {
            //var resultMessage = context.MakeMessage();
            //resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            //resultMessage.Attachments = new List<Attachment>();

            var resultMessage = new List<Attachment>();

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

    }



}