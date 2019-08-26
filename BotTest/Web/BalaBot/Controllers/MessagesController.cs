using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Diagnostics;
using System.Collections.Generic;

namespace BalaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        internal static IDialog<BalaBot> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(BalaBot.BuildForm))
                .Do(async (context, order) =>
                {
                    try
                    {
                        var completed = await order;
                         // Actually process the sandwich order... 
                         await context.PostAsync("Processed your Azure Selection! Have a great day!");
                    }
                    catch (FormCanceledException<BalaBot> e)
                    {
                        string reply;
                        if (e.InnerException == null)
                        {
                            reply = $"You quit on {e.Last}--maybe you can finish next time!";
                        }
                        else
                        {
                            reply = "Sorry, I've had a short circuit.  Please try again." + e.Message.ToString();
                        }
                        await context.PostAsync(reply);
                    }
                });
        }

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            // Detect if this is a Message activity
            if (activity.Type == ActivityTypes.Message)
            {
                // Call our FormFlow by calling MakeRootDialog
                await Conversation.SendAsync(activity, MakeRootDialog);
                //await Conversation.SendAsync(activity, () => new ContactCard());
            }
            else
            {
                // This was not a Message activity
                HandleSystemMessage(activity);
            }

            // Return response
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        //[ResponseType(typeof(void))]
        //public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        //{
        //    if (activity != null)
        //    {
        //        // one of these will have an interface and process it 
        //        switch (activity.GetActivityType())
        //        {
        //            case ActivityTypes.Message:
        //                await Conversation.SendAsync(activity, MakeRootDialog);
        //                //await Conversation.SendAsync(activity, () => new ContactCard());
        //                break;

                        
        //            case ActivityTypes.ConversationUpdate:
        //            case ActivityTypes.ContactRelationUpdate:
        //            case ActivityTypes.Typing:
        //            case ActivityTypes.DeleteUserData:
        //            default:
        //                Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
        //                break;
        //        }
        //    }
        //    return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        //}

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
            return resultMessage.ToList();
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        //public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        //{
        //    if (activity.Type == ActivityTypes.Message)
        //    {
        //        ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
        //        // calculate something for us to return
        //        int length = (activity.Text ?? string.Empty).Length;

        //        // return our reply to the user
        //        Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
        //        await connector.Conversations.ReplyToActivityAsync(reply);
        //    }
        //    else
        //    {
        //        HandleSystemMessage(activity);
        //    }
        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    return response;
        //}

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}