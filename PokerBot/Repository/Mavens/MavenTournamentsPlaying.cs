using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using PokerBot.Models;
using PokerMavensAPI;


namespace PokerBot.Repository.Mavens
{
    public interface IMavenTournamentsPlaying
    {
        List<Player> GetSeatedPlayers(string TableName);
        bool AnySeatedTournamentPlayers();
    }
    public class MavenTournamentsPlaying : IMavenTournamentsPlaying
    {
        private HttpClient _client;
        private ISecrets _secrets;
        private IMavenTournamentsList _mavenTournamentsList;
        public MavenTournamentsPlaying(HttpClient client, ISecrets secrets, IMavenTournamentsList mavenTournamentsList)
        {
            _client = client;
            _secrets = secrets;
            _mavenTournamentsList = mavenTournamentsList;
        }

        public List<Player> GetSeatedPlayers(string TableName)
        {
            List<Player> players = new List<Player>();
            var client = new MavenClient<TournamentsPlaying>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "TournamentGamesPlaying");
            dict.Add("Name", TableName);
            TournamentsPlaying request = client.Post(_client, dict);
            for (int i = 0; i < request.Count; i++)
            {
                Player p = new Player();
                p.Name = request.Player[i];
                p.Chips = request.Chips[i];
                players.Add(p);
            }
            return players;
        }
        public bool AnySeatedTournamentPlayers()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var request = _mavenTournamentsList.Tournaments();
            if (request.Name != null)
            {
                for (int i = 0; i < request.Name.Count(); i++)
                {
                    if (GetSeatedPlayers(request.Name[i]).Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}