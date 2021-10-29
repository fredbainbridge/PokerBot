using System.Collections.Generic;
using System.Net.Http;
using PokerMavensAPI;


namespace PokerBot.Repository.Mavens {
    public interface IMavenAccountsList {
        AccountsList GetAccounts();
    }
    public class MavenAccountsList : IMavenAccountsList{
        private HttpClient _client;
        private ISecrets _secrets;
        public MavenAccountsList(HttpClient client, ISecrets secrets) {
            _client = client;
            _secrets = secrets;
        }

        public AccountsList GetAccounts()
        {
            var client = new MavenClient<AccountsList>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "AccountsList");
            dict.Add("Fields", "RealName,Balance,Player");
            return client.Post(_client, dict);
        }
    }
}