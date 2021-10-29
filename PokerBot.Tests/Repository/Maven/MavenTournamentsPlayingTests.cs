using Moq;
using PokerBot.Models;
using PokerBot.Repository;
using PokerBot.Repository.Mavens;
using PokerMavensAPI;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace PokerBot.Tests.Repository.Maven
{
    public class MavenTournamentsPlayingTests
    {
        private HttpClient httpClient;
        private ISecrets secrets;
        public MavenTournamentsPlayingTests()
        {
            secrets = MavenTestSetup.GetSecrets();
        }
        [Fact]
        public async void test_get_seated_players()
        {
            var dbContext = await MavenTestSetup.GetDatabaseContext();
            TournamentsPlaying tpIn = new TournamentsPlaying();
            tpIn.Count = 1;
            tpIn.Chips = new List<double>() { 1 };
            tpIn.Player = new List<string>() { "test" };
            httpClient = MavenTestSetup.GetHttpClient(tpIn);
            MavenTournamentsPlaying mtp = new MavenTournamentsPlaying(httpClient, secrets, new Mock<IMavenTournamentsList>().Object);
            List<Player> returnedPlayers = mtp.GetSeatedPlayers("test");
            Assert.True(returnedPlayers.Count == tpIn.Count);
        }
    }
}
