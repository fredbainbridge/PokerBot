using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.FileProviders.Embedded;
using System.Linq;
using System.Collections.Generic;
using System;

namespace PokerBot.Repository {
    class Secret {
        public string ServerURL { get; set; }
        public string Password { get; set; }
        public List<string> SlackURLs { get; set; }
        public bool Silence { get; set; }
        public string Token { get; set; }
        public string UserToken { get; set; }
        public string WebsiteURL { get; set; }
        public string GameURL { get; set; }
        public List<string> GameNames { get; set; }
        public int Balance { get; set; }
        public List<string> HOFExclusions { get; set; }
        public string AvatarDir { get; set; }
    }
    public class Secrets : ISecrets {
        private Secret _secret;
        public Secrets() {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if(environment.Equals("Azure")) {
                Secret secret = new Secret();
                secret.AvatarDir = Environment.GetEnvironmentVariable("AVATAR_DIR");
                secret.Balance = Int32.Parse(Environment.GetEnvironmentVariable("BALANCE"));
                secret.HOFExclusions =  Environment.GetEnvironmentVariable("HOF_EXCLUSIONS").Split(',').ToList();
                secret.GameNames =  Environment.GetEnvironmentVariable("GAME_NAMES").Split(',').ToList();
                secret.GameURL = Environment.GetEnvironmentVariable("GAME_URL");
                secret.Password = Environment.GetEnvironmentVariable("PASSWORD");
                secret.ServerURL = Environment.GetEnvironmentVariable("SERVER_URL");
                secret.Silence = bool.Parse(Environment.GetEnvironmentVariable("SILENCE"));
                secret.SlackURLs =  Environment.GetEnvironmentVariable("SLACK_URLS").Split(',').ToList();
                secret.Token = Environment.GetEnvironmentVariable("TOKEN");
                secret.UserToken = Environment.GetEnvironmentVariable("USER_TOKEN");
                secret.WebsiteURL = Environment.GetEnvironmentVariable("WEBSITE_URL");
                _secret = secret;
            }
            else {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("PokerBot.secrets.json");
                var secretJSON = "";
                using(var reader = new StreamReader(stream)) {
                    secretJSON = reader.ReadToEnd();
                }
                _secret = JsonConvert.DeserializeObject<Secret>(secretJSON);
            }
            
        }
        public string PokerURL(){
            return _secret.ServerURL;
        }
        public string Password(){
            return _secret.Password;
        }
        public List<string> SlackURLs() {
            return _secret.SlackURLs;
        }
        public bool Silence() {
            return _secret.Silence;
        }
        public string Token()
        {
            return _secret.Token;
        }
        public string UserToken()
        {
            return _secret.UserToken;
        }
        public string WebsiteURL()
        {
            return _secret.WebsiteURL;
        }
        public string GameURL()
        {
            return _secret.GameURL;
        }
        public int Balance()
        {
            return _secret.Balance;
        }
        public List<string> GameName()
        {
            return _secret.GameNames;
        }
        public List<string> HOFExclusions()
        {
            return _secret.HOFExclusions;
        }
        public string AvatarDir()
        {
            return _secret.AvatarDir;
        }
    }
}
