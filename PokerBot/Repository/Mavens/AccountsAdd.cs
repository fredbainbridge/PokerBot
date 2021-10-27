using System.Collections.Generic;
using System.Linq;
using PokerBot.Models;
using PokerMavensAPI;


namespace PokerBot.Repository.Mavens {
    public class AccountsAdd {
        private IMavensClient<AccountsAdd> _client;
        private PokerDBContext _pokerContext;
        public AccountsAdd(IMavensClient<AccountsAdd> client, PokerDBContext PokerContext) {
            _client = client;
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
            var response = _client.Post(dict);
            return true;
        }
    }
}