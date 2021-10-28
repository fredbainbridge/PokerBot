using PokerMavensAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokerBot.Repository.Mavens
{
    public interface IMavenTournamentsList
    {
        TournamentsList Tournaments();
    }
    public class MavenTournamentsList : IMavenTournamentsList
    {
        private HttpClient _client;
        private ISecrets _secrets;
        public MavenTournamentsList(HttpClient client, ISecrets secrets)
        {
            _client = client;
            _secrets = secrets;
        }
        public TournamentsList Tournaments()
        {
            var client = new MavenClient<TournamentsList>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "TournamentsList");
            dict.Add("Fields", "Name");
            return client.Post(_client, dict);
        }
    }
}
