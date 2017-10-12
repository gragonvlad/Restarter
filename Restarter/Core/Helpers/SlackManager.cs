using System;
using System.Collections.Generic;
using System.Text;
using Restarter.Core.Objects;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace Restarter.Core.Helpers
{
    public static class SlackManager
    {
        public static void SendSlackMessage(string alertType, string alertText, string alertWarning)
        {
            return;
            var request = (HttpWebRequest)WebRequest.Create("https://hooks.slack.com/services/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");

            SlackEmbed embed = new SlackEmbed();
            Attachment attachment = new Attachment()
            {
                text = $"Sever Alert - {alertType}",
                color = "#764FA5",
                fields = new List<Field>()
                {
                    new Field()
                    {
                        title = alertWarning,
                        value = alertText
                    }
                }
            };
            embed.attachments = new List<Attachment>() { attachment };
            var data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(embed));

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            var response = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd();
            if(!string.IsNullOrEmpty(response)) Console.WriteLine($"Response Received From Slack: {response}");
        }
    }
}
