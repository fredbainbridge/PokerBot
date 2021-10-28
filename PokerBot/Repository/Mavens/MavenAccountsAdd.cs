using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using PokerBot.Models;
using PokerMavensAPI;


namespace PokerBot.Repository.Mavens {
    public interface IMavenAccountsAdd {
        User CreateNewUser(string SlackID, string Player, string RealName, string Location, string Email);
    }
    public class MavenAccountsAdd : IMavenAccountsAdd{
        private HttpClient _client;
        private PokerDBContext _pokerContext;
        private ISecrets _secrets;
        public MavenAccountsAdd(HttpClient client, PokerDBContext pokerContext, ISecrets secrets) {
            _client = client;
            _pokerContext = pokerContext;
            _secrets = secrets;
        }

        public User CreateNewUser(string SlackID, string Player, string RealName, string Location, string Email)
        {
            var client = new MavenClient<AccountsAdd>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "AccountsAdd");
            dict.Add("Player", Player);
            dict.Add("RealName", RealName);
            dict.Add("PW", "password");
            dict.Add("Location", Location);
            dict.Add("Email", Email);
            var response = client.Post(_client, dict);

            User u = new User();
            u.EmailAddress = Email;
            u.RealName = RealName;
            u.SlackID = SlackID;
            u.UserName = Player;
            _pokerContext.User.Add(u);
            _pokerContext.SaveChanges();            
            return u;
        }
    }
}