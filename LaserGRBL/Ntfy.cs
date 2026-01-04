using System;
using System.Net;
using System.Text;

namespace LaserGRBL
{
    public class Ntfy
    {
        private const string NTFY_SERVER = "https://ntfy.sh";

        public static void NotifyEvent(string message)
        {
            if (!Settings.GetObject("Ntfy.Enabled", false))
                return;

            string topic = Settings.GetObject("Ntfy.Topic", "");
            if (string.IsNullOrWhiteSpace(topic))
                return;

            NotifyEvent(topic, message);
        }

        public static void NotifyEvent(string topic, string message)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return;

            var data = new NtfyData
            {
                Topic = topic.Trim(),
                Message = message,
                Title = "LaserGRBL Job Complete",
                Priority = "high",
                Tags = "white_check_mark,fire"
            };

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(InternalNotifyEvent), data);
        }

        private static void InternalNotifyEvent(object data)
        {
            try
            {
                NtfyData ntfyData = data as NtfyData;
                using (MyWebClient client = new MyWebClient())
                {
                    client.Headers["Title"] = ntfyData.Title;
                    client.Headers["Priority"] = ntfyData.Priority;
                    client.Headers["Tags"] = ntfyData.Tags;

                    string url = $"{NTFY_SERVER}/{ntfyData.Topic}";
                    byte[] response = client.UploadData(url, "POST", Encoding.UTF8.GetBytes(ntfyData.Message));
                }
            }
            catch (Exception ex)
            {
                // Silently fail - notification is not critical
            }
        }

        private class NtfyData
        {
            public string Topic { get; set; }
            public string Message { get; set; }
            public string Title { get; set; }
            public string Priority { get; set; }
            public string Tags { get; set; }
        }

        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 5000; // milliseconds
                return w;
            }
        }
    }
}
