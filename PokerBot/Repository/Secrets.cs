using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.FileProviders.Embedded;

namespace PokerBot.Repository {
    class Secret {
        public string ServerURL {get; set;}
        public string Password {get; set;}
        public string SlackURL {get; set;}
        public bool Silence { get; set; }
        public string Token { get; set; }
    }
    public class Secrets : ISecrets {
        private Secret _secret;
        public Secrets() {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("PokerBot.secrets.json");
            var secretJSON = "";
            using(var reader = new StreamReader(stream)) {
                secretJSON = reader.ReadToEnd();
            }
            _secret = JsonConvert.DeserializeObject<Secret>(secretJSON);
        }
        public string PokerURL(){
            return _secret.ServerURL;
        }
        public string Password(){
            return _secret.Password;
        }
        public string SlackURL() {
            return _secret.SlackURL;
        }
        public bool Silence() {
            return _secret.Silence;
        }
        public string Token()
        {
            return _secret.Token;
        }
    }
}
