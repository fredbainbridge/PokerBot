using Moq;
using PokerBot.Models;
using PokerBot.Repository;
using PokerBot.Repository.Mavens;
using PokerMavensAPI;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PokerBot.Tests.Repository.Maven
{
    public class MavenRingGamesPlayingTests
    {
        [Fact]
        public async void test_get_seated_players()
        {
            var dbContext = await MavenTestSetup.GetDatabaseContext();
            RingGamesPlaying rgpIn = new RingGamesPlaying();
            rgpIn.Count = 1;
            rgpIn.Chips = new List<double>() { 1 };
            rgpIn.Player = new List<string>() { "test" };
            var httpClient = MavenTestSetup.GetHttpClient(rgpIn);
            var secrets = MavenTestSetup.GetSecrets();
            MavenRingGamesPlaying mrp = new MavenRingGamesPlaying(httpClient, secrets, new Mock<IMavenRingGamesList>().Object);
            List<Player> returnedPlayers = mrp.GetSeatedPlayers("test");
            Assert.True(returnedPlayers.Count == rgpIn.Count);
        }
    }
}
