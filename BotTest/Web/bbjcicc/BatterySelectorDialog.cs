using Microsoft.Bot.Builder.Luis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;

namespace bbjcicc
{
    [LuisModel("9bff0685-3fd6-4249-bfa3-ae1be37f10ee", "3aaab437f88a4371988b6e09028dbc55")]
    [Serializable]
    public class BatterySelectorDialog : LuisDialog<BatterySelector>
    {

        private readonly BuildFormDelegate<BatterySelector> MakeBatteryForm;

        internal BatterySelectorDialog(BuildFormDelegate<BatterySelector> MakeBatteryForm)
        {
            this.MakeBatteryForm = MakeBatteryForm;
        }



        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            context.Wait(null);
        }

        [LuisIntent("BatterySelector")]
        [LuisIntent("hi")]
        public async Task ProcessBatteryForm(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);

            if (!entities.Any((entity) => entity.Type == "Kind"))
            {

                // Infer kind

                //foreach (var entity in result.Entities)
                //{

                //    string kind = null;

                //    switch (entity.Type)
                //    {
                //        case "Signature": kind = "Signature"; break;

                //        case "GourmetDelite": kind = "Gourmet delite"; break;

                //        case "Stuffed": kind = "stuffed"; break;

                //        default:
                //            if (entity.Type.StartsWith("BYO")) kind = "byo";
                //            break;
                //    }

                //    if (kind != null)
                //    {
                //        entities.Add(new EntityRecommendation(type: "Kind") { Entity = kind });
                //        break;
                //    }

                //}

            }



            var BatteryForm = new FormDialog<BatterySelector>(new BatterySelector(), this.MakeBatteryForm, FormOptions.PromptInStart, entities);
            context.Call<BatterySelector>(BatteryForm, BatteryFormComplete);

        }



        private async Task BatteryFormComplete(IDialogContext context, IAwaitable<BatterySelector> result)
        {
            BatterySelector order = null;

            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");
                return;
            }
            
            if (order != null)
            {
                await context.PostAsync("Your Pizza Order: " + order.ToString());
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }
            
            context.Wait(null);

        }




        //end of class
    }


}