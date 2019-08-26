using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace DeviceBot
{
    [Serializable]
    public class welcome : IDialog<object>
    {
        protected int count = 1;
        public async Task StartAsync(IDialogContext context)
        {
            string message = "Welcome to LUIS BOT IoT Device Management - Admin/Monitoring";
            await context.PostAsync(message);

            //context.Wait(this.MessageReceivedAsync);
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            //var message = await argument;
            //if (message.Text == "reset")
            //{
            //    PromptDialog.Confirm(
            //        context,
            //        AfterResetAsync,
            //        "Are you sure you want to reset?",
            //        "Didn't get that!",
            //        promptStyle: PromptStyle.None);
            //}
            //else
            //{
            //    await context.PostAsync($"Please continue with your conversation");
            //    //context.Wait(MessageReceivedAsync);
            //    //context.Reset();
            //}
            await context.PostAsync($"Please continue with your conversation");
            context.Done(this);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset.");
            }
            else
            {
                await context.PostAsync("Did not reset.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }

}