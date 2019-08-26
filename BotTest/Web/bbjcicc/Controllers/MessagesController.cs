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
using System.Diagnostics;

namespace bbjcicc
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        //internal static IDialog<BatterySelector> MakeRootDialog()
        //{
        //    return Chain.From(() => FormDialog.FromForm(BatterySelector.BuildForm));
        //}



        internal static IDialog<BatterySelector> MakeRootDialog()
         { 
             return Chain.From(() => FormDialog.FromForm(BatterySelector.BuildForm)) 
                 .Do(async (context, order) =>
                 { 
                     try 
                     { 
                         var completed = await order; 
                         // Actually process the sandwich order... 
                         await context.PostAsync("Processed your Battery Selection! Have a great day!"); 
                     } 
                     catch (FormCanceledException<BatterySelector> e) 
                     { 
                         string reply; 
                         if (e.InnerException == null) 
                         { 
                             reply = $"You quit on {e.Last}--maybe you can finish next time!"; 
                         } 
                         else 
                         { 
                             reply = "Sorry, I've had a short circuit.  Please try again."; 
                         } 
                         await context.PostAsync(reply); 
                     } 
                 }); 
         }


        private static IForm<BatterySelector> BuildForm()
        {
            var builder = new FormBuilder<BatterySelector>();
            return builder
                // .Field(nameof(PizzaOrder.Choice))
                .Message("Hi welcome to optima, how can I help you?")
                     .Message("Please Select the options below to find your battery.")
                     .Field(nameof(BatterySelector.mmake))
                     .Field(nameof(BatterySelector.mmodel))
                     .Field(nameof(BatterySelector.myear))
                     .Field(nameof(BatterySelector.mliter))
                     .Field(nameof(BatterySelector.maftermarket))
                     .Confirm("The below battery options you have selection {mmake} {mmodel} {myear} {mliter} with aftermarket accessories {maftermarket}?")
                     //.OnCompletion(BatterySelector.processOrder)
                     //.Message("Thanks for selecting the battery and have a great day!")
                     .Build();

        }



        internal static IDialog<BatterySelector> MakeRoot()
        {
            return Chain.From(() => new BatterySelectorDialog(BuildForm));
        }


        [ResponseType(typeof(void))] 
         public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
         { 
             if (activity != null) 
             { 
                 // one of these will have an interface and process it 
                 switch (activity.GetActivityType()) 
                 { 
                     case ActivityTypes.Message: 
                         await Conversation.SendAsync(activity, MakeRootDialog); 
                         break; 
 
 
                     case ActivityTypes.ConversationUpdate: 
                     case ActivityTypes.ContactRelationUpdate: 
                     case ActivityTypes.Typing: 
                     case ActivityTypes.DeleteUserData: 
                     default: 
                         Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}"); 
                         break; 
                 } 
             } 
             return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted); 
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

        //private Activity HandleSystemMessage(Activity message)
        //{
        //    if (message.Type == ActivityTypes.DeleteUserData)
        //    {
        //        // Implement user deletion here
        //        // If we handle user deletion, return a real message
        //    }
        //    else if (message.Type == ActivityTypes.ConversationUpdate)
        //    {
        //        // Handle conversation state changes, like members being added and removed
        //        // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        //        // Not available in all channels
        //    }
        //    else if (message.Type == ActivityTypes.ContactRelationUpdate)
        //    {
        //        // Handle add/remove from contact lists
        //        // Activity.From + Activity.Action represent what happened
        //    }
        //    else if (message.Type == ActivityTypes.Typing)
        //    {
        //        // Handle knowing tha the user is typing
        //    }
        //    else if (message.Type == ActivityTypes.Ping)
        //    {
        //    }

        //    return null;
        //}


    }
}