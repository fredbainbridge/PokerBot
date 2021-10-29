using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using PokerBot.Models;
using PokerMavensAPI;


namespace PokerBot.Repository.Mavens {
    public interface IMavenAccountsEdit {
        bool ChangePassword(string SlackID, string password);
        void SetAvatarPath(string name, string path);
        void SetPrimaryBalance(string Name, int Balance);
    }
    public class MavenAccountsEdit : IMavenAccountsEdit{
        private HttpClient _client;
        private PokerDBContext _pokerContext;
        private ISecrets _secrets;
        public MavenAccountsEdit(HttpClient client, PokerDBContext pokerContext, ISecrets secrets) {
            _client = client;
            _pokerContext = pokerContext;
            _secrets = secrets;
        }

        public bool ChangePassword(string SlackID, string password)
        {
            User u = _pokerContext.User.Where(u => u.SlackID.Equals(SlackID)).FirstOrDefault();
            if(u == null)
            {
                return false; 
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "AccountsEdit");
            dict.Add("Player", u.UserName);
            dict.Add("PW", password);
            MavenClient<AccountsEdit> mClient = new MavenClient<AccountsEdit>(_secrets);
            var response = mClient.Post(_client, dict);
            return true;
        }

        public void SetAvatarPath(string name, string path)
        {
            var client = new MavenClient<AccountsEdit>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "AccountsEdit");
            dict.Add("Player", name);
            dict.Add("AvatarFile", path);
            dict.Add("Avatar", "0");
            client.Post(_client, dict);
        }

        public void SetPrimaryBalance(string Name, int Balance)
        {
            var client = new MavenClient<AccountsEdit>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "AccountsEdit");
            dict.Add("Player", Name);
            dict.Add("Balance", Balance.ToString());
            client.Post(_client, dict);
        }
    }
}