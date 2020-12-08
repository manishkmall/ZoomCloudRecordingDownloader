using System;
using System.Collections.Generic;
using System.Text;

namespace ZoomRecording.App_Code
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class RecordingFile
    {
        public string download_url { get; set; }
        public int file_size { get; set; }
        public string file_type { get; set; }
        public string id { get; set; }
        public string meeting_id { get; set; }
        public string play_url { get; set; }
        public DateTime recording_end { get; set; }
        public DateTime recording_start { get; set; }
        public string recording_type { get; set; }
        public string status { get; set; }
    }

    public class Meeting
    {
        public string account_id { get; set; }
        public int duration { get; set; }
        public string host_id { get; set; }
        public long id { get; set; }
        public int recording_count { get; set; }
        public List<RecordingFile> recording_files { get; set; }
        public string share_url { get; set; }
        public DateTime start_time { get; set; }
        public string timezone { get; set; }
        public string topic { get; set; }
        public int total_size { get; set; }
        public int type { get; set; }
        public string uuid { get; set; }
    }

    public class Root
    {
        public string from { get; set; }
        public List<Meeting> meetings { get; set; }
        public string next_page_token { get; set; }
        public int page_count { get; set; }
        public int page_size { get; set; }
        public string to { get; set; }
        public int total_records { get; set; }
    }
}
