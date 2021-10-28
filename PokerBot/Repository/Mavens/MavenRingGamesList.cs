using PokerMavensAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokerBot.Repository.Mavens
{
    public interface IMavenRingGamesList
    {
        RingGamesList RingGames();
    }
    public class MavenRingGamesList : IMavenRingGamesList
    {
        private HttpClient _client;
        private ISecrets _secrets;
        public MavenRingGamesList(HttpClient client, ISecrets secrets)
        {
            _client = client;
            _secrets = secrets;
        }
        public RingGamesList RingGames()
        {
            var client = new MavenClient<RingGamesList>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "RingGamesList");
            dict.Add("Fields", "Name");
            return client.Post(_client, dict);
        }
    }
}
