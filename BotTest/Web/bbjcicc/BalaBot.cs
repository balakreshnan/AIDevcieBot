using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#pragma warning disable 649

namespace bbjcicc
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".")]
    [Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your options? {||}")]
    public class BalaBot
    {

        [Prompt("What is your issue? {||}")]
        public string missue;

        [Prompt("Would like to contact Azure Sales? {||}")]
        public string msales;

        [Prompt("Would like to contact Cloud Solution Architect? {||}")]
        public string mcsa;

        [Prompt("Would like to contact Data Soulution Architect? {||}")]
        public string mdsa;

        [Prompt("Can you explain about your project? {||}")]
        public string mproject;


        public static IForm<BalaBot> BuildForm()
        {

            OnCompletionAsyncDelegate<BalaBot> processOrder = async (context, state) =>
            {
                string tmpissue = state.missue;
                string tmpsales = state.msales;
                string tmpcsa = state.mcsa;
                string tmpdsa = state.mdsa;
                string tmpproject = state.mproject;
                //string tmpaftermarkey = state.maftermarket;
                string tmpreturn = string.Empty;
                tmpreturn = "Azure " + tmpissue + " " + tmpsales + " " + tmpcsa + " " + tmpdsa + " " + tmpproject;
                //tmpreturn = getbatteryInfo(tmpmodel, tmpmake, tmpyear, tmpmliter, tmpaftermarkey);

                if (tmpreturn == string.Empty || tmpreturn == "")
                {
                    tmpreturn = "Oops we couldn't find a battery for your request please try again";
                }

                await context.PostAsync("Then we would recommend " + tmpreturn + " Have a great day!");
            };


            return new FormBuilder<BalaBot>()
                     .Message("Hi welcome to BalaBot, how can I help you?")
                     .Message("Please answer this question to get help.")
                     .Field(nameof(BalaBot.missue))
                     .Field(nameof(BalaBot.msales))
                     .Field(nameof(BalaBot.mcsa))
                     .Field(nameof(BalaBot.mdsa))
                     .Field(nameof(BalaBot.mproject))
                     .Confirm("The below you have selected {missue} {msales} {mcsa} {mdsa} with project details as {mproject}?")
                     .OnCompletion(processOrder)
                     //.Message("Thanks for selecting the battery and have a great day!")
                     .Build();
        }



    }


}