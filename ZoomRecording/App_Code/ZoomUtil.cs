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


namespace ZoomRecording.App_Code
{
    public static class ZoomUtil
    {

        public static string RemoveInvalidChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static void Download(RecordingFile file, string folderpath, string token, string topic)
        {
            //File nameing pattern defined here.
            string filename = "GMT" + file.recording_start.ToString("yyyy-MM-dd-HHmmss") + "_" + topic + "." + file.file_type;
            filename = RemoveInvalidChars(filename);

            //Do not re-download the file if run the utility again.
            if (File.Exists(Path.Join(folderpath, filename)))
            {
                return;
            }
            //Download link need access token in the url to download the file.
            WebClient client = new WebClient();
            client.DownloadFile(file.download_url + "?access_token=" + token, Path.Join(folderpath, filename)); //  folderpath
        }

        public static List<User> GetZoomUsers(string apikey, string apisecret)
        {
            var connectionInfo = new JwtConnectionInfo(apikey, apisecret);
            var zoomClient = new ZoomClient(connectionInfo);

            List<User> userlist = new List<User>();
            int pagenumber = 1;
            // Iterate through all pages and load all users in the account. As Zoom provide maximum 300 users to be loaded in one api call.
            while (true)
            {

                PaginatedResponse<User> paginateduserlist = zoomClient.Users.GetAllAsync(UserStatus.Active, null, 300, pagenumber).Result;
                userlist.AddRange(paginateduserlist.Records);

                if (paginateduserlist.PageCount == pagenumber)
                {
                    break;
                }
                pagenumber++;
            }


            return userlist;

        }


        public static void SaveRecordings(string token, string email, DateTime startDate, DateTime endDate, string basepath, string trash_type)
        {
            string apiurl = string.Format("https://api.zoom.us/v2/users/{0}/recordings?trash_type={3}&mc=false&page_size=30&from={1}&to={2}", email, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), trash_type);
            var client = new RestClient(apiurl);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "Bearer " + token);
            IRestResponse response = client.Execute(request);

            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response.Content);
            if (myDeserializedClass.meetings != null && myDeserializedClass.meetings.Count > 0)
            {
                foreach (App_Code.Meeting meeting in myDeserializedClass.meetings.Where(t => t.recording_count > 0))
                {
                    string folderpath = System.IO.Path.Join(basepath, "Recording", email);
                    if (!System.IO.Directory.Exists(folderpath))
                    {
                        System.IO.Directory.CreateDirectory(folderpath);
                    }
                    folderpath = System.IO.Path.Join(folderpath, meeting.id.ToString());
                    if (!System.IO.Directory.Exists(folderpath))
                    {
                        System.IO.Directory.CreateDirectory(folderpath);
                    }

                    File.WriteAllText(System.IO.Path.Join(folderpath, meeting.id + ".json"), Newtonsoft.Json.JsonConvert.SerializeObject(meeting));

                    foreach (App_Code.RecordingFile file in meeting.recording_files)
                    {
                        Download(file, folderpath, token, meeting.topic);
                    }
                }
            }

        }




    }
}
