using PokerMavensAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokerBot.Repository.Mavens
{
    public interface IMavenRingGamesGet
    {
        RingGamesGet GetRingGame(string tableName);
    }
    public class MavenRingGamesGet : IMavenRingGamesGet
    {
        private HttpClient _client;
        private ISecrets _secrets;
        public MavenRingGamesGet(HttpClient client, ISecrets secrets)
        {
            _client = client;
            _secrets = secrets;
        }
        public RingGamesGet GetRingGame(string tableName)
        {
            var client = new MavenClient<RingGamesGet>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "RingGamesGet");
            dict.Add("Name", tableName);
            return client.Post(_client, dict);
        }
    }
}
