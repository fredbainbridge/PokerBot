using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Collections.Generic;

//A simple C# class to post messages to a Slack channel
//Note: This class uses the Newtonsoft Json.NET serializer available via NuGet

namespace PokerBot.Repository {
	public class SlackClient : ISlackClient
	{
		private List<Uri> _uris;
		private readonly Encoding _encoding = new UTF8Encoding();
        private string _token;
        private ISecrets _secrets;
		public SlackClient(ISecrets secrets)
		{
            _token = secrets.Token();
            _secrets = secrets;
            _uris = new List<Uri>();
            foreach(var url in secrets.SlackURLs()) {
                _uris.Add(new Uri(url));
            }
		}
        
		
        public void PostAPIMessage(string text, string username = null, string channel = null)
        {
            Payload payload = new Payload()
            {
                //Token = _secrets.Token(),
                Channel = channel,
                //Username = username,
                Text = text,
                As_User = true

            };
            
            PostAPIMessage(payload);
        }
		//Post a message using simple strings
		public void PostWebhookMessage(string text, string username = null, string channel = null)
		{
			Payload payload = new Payload()
			{
				Channel = channel,
				//Username = username,
				Text = text
			};
			
			PostWebhookMessage(payload);
		}

        //Post a message using a Payload object
        public void PostWebhookMessage(Payload payload)
        {
            string payloadJson = JsonConvert.SerializeObject(payload);
            foreach(var _uri in _secrets.SlackURLs()) {
                using (WebClient client = new WebClient())
                {
                    NameValueCollection data = new NameValueCollection();
                    data["payload"] = payloadJson;

                    var response = client.UploadValues(_uri, "POST", data);

                    //The response text is usually "ok"
                    string responseText = _encoding.GetString(response);
                }
            }        
        }
        public void PostAPIMessage(Payload payload)
		{
            var _uri = new Uri("https://slack.com/api/chat.postMessage");
            string payloadJson = JsonConvert.SerializeObject(payload);
			
			using (WebClient client = new WebClient())
			{
                client.Headers.Set("Content-Type", "application/json");
                client.Headers.Add("Authorization", "Bearer " + _secrets.Token());
                byte[] request = System.Text.Encoding.UTF8.GetBytes(payloadJson);
                var response = client.UploadData(_uri, "POST", request);
				
				//The response text is usually "ok"
				string responseText = _encoding.GetString(response);
			}
		}
        //for direct messages U0Y7697U1 https://stackoverflow.com/questions/48347073/using-slack-how-do-you-send-direct-message-to-user-based-on-their-member-id
    }

    //This class serializes into the Json payload required by Slack Incoming WebHooks
    public class Payload
	{
        //[JsonProperty("token")]
        //public string Token { get; set; }

		[JsonProperty("channel")]
		public string Channel { get; set; }
		
		[JsonProperty("text")]
		public string Text { get; set; }

        [JsonProperty("as_user")]
        public bool As_User { get; set; }
 
	}
}
