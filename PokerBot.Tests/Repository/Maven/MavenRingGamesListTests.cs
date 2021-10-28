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
    public class MavenRingGamesListTests
    {
        [Fact]
        public void test_get_ringgameslist()
        {
            RingGamesList rglIn = new RingGamesList();
            rglIn.Name = new List<string>() { "test" };
            var httpClient = MavenTestSetup.GetHttpClient(rglIn);
            var secrets = MavenTestSetup.GetSecrets();
            MavenRingGamesList mrl = new MavenRingGamesList(httpClient, secrets);
            RingGamesList returnedRingGameList = mrl.RingGames();
            Assert.True(returnedRingGameList.Name.Count() == rglIn.Name.Count());
        }
    }
}
