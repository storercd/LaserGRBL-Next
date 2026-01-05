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
            string topic = (string)Settings.GetObject("Ntfy.Topic", string.Empty);
            if (string.IsNullOrWhiteSpace(topic))
                return;

            NotifyEvent(topic, message);
        }

        public static void NotifyEvent(string topic, string message)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return;

            bool isError = message.Contains("Job Issue") || message.Contains("Issue") || message.Contains("Alarm") || message.Contains("Error");

            var data = new NtfyData
            {
                Topic = topic.Trim(),
                Message = message,
                Title = isError ? "⚠️ LaserGRBL Job FAILED" : "✅ LaserGRBL Job Complete",
                Priority = isError ? "urgent" : "high",
                Tags = isError ? "warning,rotating_light,x" : "white_check_mark,fire"
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
