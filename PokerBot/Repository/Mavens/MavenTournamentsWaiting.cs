using PokerMavensAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokerBot.Repository.Mavens
{
    public interface IMavenTournamentsWaiting
    {
        bool AnyWaitingTournamentPlayers();
    }
    public class MavenTournamentsWaiting : IMavenTournamentsWaiting
    {
        private HttpClient _client;
        private ISecrets _secrets;
        private IMavenTournamentsList _mavenTournamentsList;
        public MavenTournamentsWaiting(HttpClient client, ISecrets secrets, IMavenTournamentsList mavenTournamentsList)
        {
            _client = client;
            _secrets = secrets;
            _mavenTournamentsList = mavenTournamentsList;
        }
        public bool AnyWaitingTournamentPlayers()
        {
            var request = _mavenTournamentsList.Tournaments();
            Dictionary<string, string> dict;
            if (request.Name != null)
            {
                for (int i = 0; i < request.Name.Count(); i++)
                {
                    var tClient = new MavenClient<TournamentsWaiting>(_secrets);
                    dict = new Dictionary<string, string>();
                    dict.Add("Command", "TournamentsWaiting");
                    dict.Add("Name", request.Name[i]);
                    var tRequest = tClient.Post(_client, dict);

                    if (tRequest.Count != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
