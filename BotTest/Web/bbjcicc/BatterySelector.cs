using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

#pragma warning disable 649


namespace bbjcicc
{
    public enum yearOptions { y2013, y2014, y2015, y2016, y2017 };


    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".")] 
    [Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your Battery? {||}")]
    public class BatterySelector
     {



        //public yearOptions? years;
        

        [Prompt("What Model is your vehicle? {||}")]
        public string mmodel;

        [Prompt("What Make is your vehicle? {||}")]
        public string mmake;

        [Prompt("What year is your vehicle? {||}")]
        public string myear;

        [Prompt("What Liter is your vehicle? {||}")]
        public string mliter;

        [Prompt("Do you have aftermarket accessories in your vehicle? {||}")]
        public string maftermarket;


        public static IForm<BatterySelector> BuildForm()
         {

            OnCompletionAsyncDelegate<BatterySelector> processOrder = async (context, state) =>
             {
                 string tmpmodel = state.mmodel;
                 string tmpmake = state.mmake;
                 string tmpyear = state.myear;
                 string tmpmliter = state.mliter;
                 string tmpreturn = String.Empty;
                 string tmpaftermarkey = state.maftermarket;
                 tmpreturn = getbatteryInfo(tmpmodel, tmpmake, tmpyear, tmpmliter, tmpaftermarkey);

                 if(tmpreturn == string.Empty || tmpreturn == "")
                 {
                     tmpreturn = "Oops we couldn't find a battery for your request please try again";
                 }

                 await context.PostAsync("Then we would recommend " + tmpreturn + " Have a great day!");
             };


            return new FormBuilder<BatterySelector>() 
                     .Message("Hi welcome to optima, how can I help you?")
                     .Message("Please Select the options below to find your battery.")
                     .Field(nameof(BatterySelector.mmake))
                     .Field(nameof(BatterySelector.mmodel))
                     .Field(nameof(BatterySelector.myear))
                     .Field(nameof(BatterySelector.mliter))
                     .Field(nameof(BatterySelector.maftermarket))
                     .Confirm("The below battery options you have selection {mmake} {mmodel} {myear} {mliter} with aftermarket accessories {maftermarket}?")
                     .OnCompletion(processOrder)
                     //.Message("Thanks for selecting the battery and have a great day!")
                     .Build(); 
         } 


        public static string getbatteryInfo(string tmpmodel, string tmpmake, string tmpyear, string tmpmliter = "2.5", string tmpaftermarkey = "no")
        {
            string retval = string.Empty;
            try
            {
                

                // Provide the query string with a parameter placeholder.
                //string queryString = "select Top 1 b.Make,b.Model,b.ModelYear, b.Liter, b.Cylinders, b.BlockType,b.Class, c.Technology,c.GroupSize";
                //queryString += " from dw.FactVehicleGroupSizeFitments a join";
                //queryString += " dw.DimVehicle b on a.vehicleID = b.vehicleID";
                //queryString += " join [dw].[DimBatteryGroupsize] c on a.batteryGroupSizeID = c.batteryGroupSizeID";
                //queryString += " where b.Make = '" + tmpmake + "' and b.Model = '" + tmpmodel + "' and b.ModelYear = '" + tmpyear + "' and b.Liter = '" + tmpmliter + "';";

                string queryString = "select Top 1 a.vehyear,a.vehmake,a.vehmodel, a.vehengine, b.producttype";
                queryString += " FROM dw.FactProductListDetail1 a join";
                queryString += " dw.DimProductList b on a.part_number = b.part_number";
                queryString += " where a.vehmake = '" + tmpmake + "' and a.vehmodel = '" + tmpmodel + "' and a.vehyear = '" + tmpyear + "' and a.vehengine like '%" + tmpmliter + "%';";

                if(tmpaftermarkey.ToLower() == "no")
                {
                    queryString = "select Top 1 a.vehyear,a.vehmake,a.vehmodel, a.vehengine, b.producttype";
                    queryString += " FROM dw.FactProductListDetail1 a join";
                    queryString += " dw.DimProductList b on a.part_number = b.part_number";
                    queryString += " where a.vehmake = '" + tmpmake + "' and a.vehmodel = '" + tmpmodel + "' and a.vehyear = '" + tmpyear + "' and a.vehengine like '%" + tmpmliter + "%';";
                }
                else
                {
                    queryString = "select Top 2 a.vehyear,a.vehmake,a.vehmodel, a.vehengine, b.producttype";
                    queryString += " FROM dw.FactProductListDetail1 a join";
                    queryString += " dw.DimProductList b on a.part_number = b.part_number";
                    queryString += " where a.vehmake = '" + tmpmake + "' and a.vehmodel = '" + tmpmodel + "' and a.vehyear = '" + tmpyear + "' and a.vehengine like '%" + tmpmliter + "%';";
                }

                retval = getdatasql(queryString);

                // Specify the parameter value.
                //int paramValue = 5;

                // Create and open the connection in a using block. This
                // ensures that all resources will be closed and disposed
                // when the code exits.



            }
            catch (Exception ex)
            {

                retval = ex.Message.ToString();
            }

            return retval;
        }

        public static string getdatasql(string strquery)
        {
            string retval = string.Empty;

            try
            {

                string connectionString = ConfigurationManager.AppSettings["SQLDWCon"].ToString();

                using (SqlConnection connection =
    new SqlConnection(connectionString))
                {
                    // Create the Command and Parameter objects.
                    SqlCommand command = new SqlCommand(strquery, connection);
                    //command.Parameters.AddWithValue("@pricePoint", paramValue);

                    // Open the connection in a try/catch block. 
                    // Create and execute the DataReader, writing the result
                    // set to the console window.
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        retval = reader[0].ToString() + "," + reader[1].ToString();
                        retval += "," + reader[2].ToString() + "," + reader[3].ToString();
                        retval += "," + reader[4].ToString();
                    }
                    reader.Close();

                }

            }
            catch (Exception ex)
            {

                //throw ex;

                try
                {

                }
                catch (Exception ex1)
                {
                    string connectionString = ConfigurationManager.AppSettings["SQLDWCon"].ToString();

                    using (SqlConnection connection =
        new SqlConnection(connectionString))
                    {
                        // Create the Command and Parameter objects.
                        SqlCommand command = new SqlCommand(strquery, connection);
                        //command.Parameters.AddWithValue("@pricePoint", paramValue);

                        // Open the connection in a try/catch block. 
                        // Create and execute the DataReader, writing the result
                        // set to the console window.
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            retval = reader[0].ToString() + "," + reader[1].ToString();
                            retval += "," + reader[2].ToString() + "," + reader[3].ToString();
                            retval += "," + reader[4].ToString();
                        }
                        reader.Close();

                    }

                    retval = ex1.Message.ToString();
                }

            }
            return retval;
        }

     }; 

}