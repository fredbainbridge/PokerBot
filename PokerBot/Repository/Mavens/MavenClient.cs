using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using PokerBot.Repository;
using System;

namespace PokerBot.Repository.Mavens {
    public class MavenClient<T> : IMavenClient<T>
    {
        public string URL  {get; set;}
        private string Password {get; set;}
        public MavenClient(ISecrets secrets) {
            this.URL = secrets.PokerURL();
            this.Password = secrets.Password();
        }
        private async Task<string> Request(HttpClient client, Dictionary<string, string> Parameters) 
        {
            Parameters.Add("Password", Password);
            Parameters.Add("JSON", "Yes");
            var encodedContent = new FormUrlEncodedContent (Parameters);
            var response = await client.PostAsync(URL, encodedContent).ConfigureAwait (false);
            if (response.StatusCode == HttpStatusCode.OK) {
                // Do something with response. Example get content:
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait (false);
                return responseContent;
            }
            else {
                return "Shit be fucked.";
            }
        }
        public T Post(HttpClient client, Dictionary<string,string> Parameters) {
            string response = Request(client, Parameters).Result;
            var serializationOptions = new JsonSerializerOptions();
            serializationOptions.Converters.Add(new StringConverter());
            return JsonSerializer.Deserialize<T>(response, serializationOptions);
        }
    }

    public class StringConverter : System.Text.Json.Serialization.JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
 
            if (reader.TokenType == JsonTokenType.Number)
            {
                var stringValue = reader.GetInt32();
                return stringValue.ToString();
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
 
            throw new System.Text.Json.JsonException();
        }
 
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
 
    }

}
//A c# class to interface with Maeven Poker software 6.11
