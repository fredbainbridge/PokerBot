using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using PokerBot.Repository.EventHub;

//A simple C# class to post messages to a Slack channel

namespace PokerBot.Repository {
	public class SlackClient : ISlackClient
	{
		private List<Uri> _uris;
		private readonly Encoding _encoding = new UTF8Encoding();
        private string _token;
        private ISecrets _secrets;
        private HttpClient _client;
        private readonly ILogger<SlackClient> _logger;
        private IPokerEventHub _pokerEventHub;

		public SlackClient(ISecrets secrets, HttpClient client, ILogger<SlackClient> logger, IPokerEventHub pokerEventHub)
		{
            _pokerEventHub = pokerEventHub;
            _logger = logger;
            _client = client;
            _token = secrets.Token();
            _secrets = secrets;
            _uris = new List<Uri>();
            foreach(var url in secrets.SlackURLs()) {
                _uris.Add(new Uri(url));
            }
		}
        
		
        public void PostAPIMessage(string text, string channel = null)
        {
            Payload payload = new Payload()
            {
                //Token = _secrets.Token(),
                Channel = channel,
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
        public async void PostWebhookMessage(Payload payload)
        {
            foreach(var _uri in _secrets.SlackURLs()) {
                var request = new HttpRequestMessage {
                    RequestUri = new Uri(_uri),
                    Method = HttpMethod.Post
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secrets.Token()); 
                request.Content = new StringContent(JsonSerializer.Serialize(payload));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                try
                {
                    var responseMessage = await _client.SendAsync(request);
                    var responseBody = await responseMessage.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError("\nException Caught!");
                    _logger.LogError("Message :{0} ", e.Message);
                }
            }        
        }
        public async void PostAPIMessage(Payload payload)
		{
            string payloadJson = JsonSerializer.Serialize(payload);
			var request = new HttpRequestMessage {
                RequestUri = new Uri("https://slack.com/api/chat.postMessage"),
                Method = HttpMethod.Post
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secrets.Token()); 
            request.Content = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(payloadJson));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            try
            {
                var responseMessage = await _client.SendAsync(request);
                var responseBody = await responseMessage.Content.ReadAsStringAsync();
                _pokerEventHub.SendEvent(responseBody);
                _logger.LogInformation(responseBody);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("\nException Caught!");
                _logger.LogError("Message :{0} ", e.Message);
            }
		}
        //for direct messages U0Y7697U1 https://stackoverflow.com/questions/48347073/using-slack-how-do-you-send-direct-message-to-user-based-on-their-member-id
    }

    //This class serializes into the Json payload required by Slack Incoming WebHooks
    public class Payload
	{
        //[JsonProperty("token")]
        //public string Token { get; set; }

		[JsonPropertyName("channel")]
		public string Channel { get; set; }
		
		[JsonPropertyName("text")]
		public string Text { get; set; }

        [JsonPropertyName("as_user")]
        public bool As_User { get; set; }
 
	}
}
