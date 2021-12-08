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
        public string AvatarDir { get; set; }
        public string EventHubConnectionString {get; set;}
        public string EventHubName {get; set;}
    }
    public class Secrets : ISecrets {
        private Secret _secret;
        public Secrets() {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Secret secret = new Secret();
            secret.AvatarDir = Environment.GetEnvironmentVariable("AVATAR_DIR");
            secret.Balance = Int32.Parse(Environment.GetEnvironmentVariable("BALANCE"));
            secret.GameNames =  Environment.GetEnvironmentVariable("GAME_NAMES").Split(',').ToList();
            secret.GameURL = Environment.GetEnvironmentVariable("GAME_URL");
            secret.Password = Environment.GetEnvironmentVariable("PASSWORD");
            secret.ServerURL = Environment.GetEnvironmentVariable("SERVER_URL");
            secret.Silence = bool.Parse(Environment.GetEnvironmentVariable("SILENCE"));
            secret.SlackURLs =  Environment.GetEnvironmentVariable("SLACK_URLS").Split(',').ToList();
            secret.Token = Environment.GetEnvironmentVariable("TOKEN");
            secret.UserToken = Environment.GetEnvironmentVariable("USER_TOKEN");
            secret.WebsiteURL = Environment.GetEnvironmentVariable("WEBSITE_URL");
            secret.EventHubConnectionString = Environment.GetEnvironmentVariable("EVENTHUB_CONNECTION_STRING");
            secret.EventHubName = Environment.GetEnvironmentVariable("EVENTHUB_NAME");
            _secret = secret;
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
        public string AvatarDir()
        {
            return _secret.AvatarDir;
        }

        public string EventHubConnectionString() {
            return _secret.EventHubConnectionString;
        }

        public string EventHubName() {
            return _secret.EventHubName;
        }
    }
}
