using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using PokerBot.Models;
using PokerMavensAPI;


namespace PokerBot.Repository.Mavens
{
    public interface IMavenRingGamesPlaying
    {
        List<Player> GetSeatedPlayers(string TableName);
        bool AnySeatedRingGamePlayers();
    }
    public class MavenRingGamesPlaying : IMavenRingGamesPlaying
    {
        private HttpClient _client;
        private ISecrets _secrets;
        private IMavenRingGamesList _mavenRingGamesList;
        public MavenRingGamesPlaying(HttpClient client, ISecrets secrets, IMavenRingGamesList mavenRingGamesList)
        {
            _client = client;
            _secrets = secrets;
            _mavenRingGamesList = mavenRingGamesList;
        }

        public List<Player> GetSeatedPlayers(string TableName)
        {
            List<Player> players = new List<Player>();
            var client = new MavenClient<RingGamesPlaying>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "RingGamesPlaying");
            dict.Add("Name", TableName);
            RingGamesPlaying request = client.Post(_client, dict);
            for (int i = 0; i < request.Count; i++)
            {
                Player p = new Player();
                p.Name = request.Player[i];
                p.Chips = request.Chips[i];
                players.Add(p);
            }
            return players;
        }

        public bool AnySeatedRingGamePlayers()
        {
            var ringGames = _mavenRingGamesList.RingGames();
            if (ringGames.Name != null)
            {
                for (int i = 0; i < ringGames.Name.Count(); i++)
                {
                    if (GetSeatedPlayers(ringGames.Name[i]).Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}