using PokerMavensAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokerBot.Repository.Mavens
{
    public interface IMavenRingGamesMessage
    {
        void SendAdminMessage(string Message, string TableName);
    }
    public class MavenRingGamesMessage : IMavenRingGamesMessage
    {
        private HttpClient _client;
        private ISecrets _secrets;
        public MavenRingGamesMessage(HttpClient client, ISecrets secrets)
        {
            _client = client;
            _secrets = secrets;
        }
        public void SendAdminMessage(string Message, string TableName)
        {
            var client = new MavenClient<RingGamesMessage>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "RingGamesMessage");
            dict.Add("Name", TableName);
            dict.Add("Message", Message);
            client.Post(_client, dict);
        }
    }
}
