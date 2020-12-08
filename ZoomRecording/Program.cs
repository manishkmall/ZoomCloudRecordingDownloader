using System;
using ZoomNet;
using ZoomNet.Models;
using ZoomNet.Utilities;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;
using ZoomRecording.App_Code;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using System.Net.Mime;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Pastel;


namespace ZoomRecording
{
    class Program
    {
        protected static void Main(string[] args)
        {

            // Create JWT App and set the API Details below.
            // https://marketplace.zoom.us/docs/guides/build/jwt-app
            string ApiKey = "IJ4_K_M4TbKlmfXat4fWHg";
            string ApiSecret = "5vJ4a1EjFmCYbJK3EiHu5uPH6MH2mVIeHB7v";

            //Load all users in the account.
            List<User> userlist = App_Code.ZoomUtil.GetZoomUsers(ApiKey, ApiSecret);

            //Read the month and year from command prompt 
            // default load for current month.
            int year = args.Length > 0 ? int.Parse(args[0]) :  DateTime.Now.Year;
            int month = args.Length > 1 ? int.Parse(args[1]): DateTime.Now.Month;

            DateTime now = new DateTime(year, month, 1);
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            Console.WriteLine("Starting Zoom Recording Copy Utility {0:yyyy-MM-dd} : {1:yyyy-MM-dd}", startDate,endDate);

            //Iterate throught all users in the account and load all recordings.
            foreach (User user in userlist)
            {
                try
                {
                    Console.Write("Downloading Cloud Recordings for " + user.Email + ": ");
                    App_Code.ZoomUtil.SaveRecordings(ZoomToken.GetZoomToken(ApiKey, ApiSecret), user.Email, startDate, endDate, AppDomain.CurrentDomain.BaseDirectory, "meeting_recordings");
                    App_Code.ZoomUtil.SaveRecordings(ZoomToken.GetZoomToken(ApiKey, ApiSecret), user.Email, startDate, endDate, AppDomain.CurrentDomain.BaseDirectory, "recording_file");
                    Console.WriteLine("[Done]");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Error]" + ex.Message);
                }
            }
        }
    }
}
