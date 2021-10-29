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
    public class MavenRingGamesGetTests
    {
        [Fact]
        public void test_get_ringgames()
        {
            RingGamesGet rgIn = new RingGamesGet();
            rgIn.Name = "test";
            var httpClient = MavenTestSetup.GetHttpClient(rgIn);
            var secrets = MavenTestSetup.GetSecrets();
            MavenRingGamesGet mrg = new MavenRingGamesGet(httpClient, secrets);
            RingGamesGet returnedRingGameGet = mrg.GetRingGame("test");
            Assert.True(returnedRingGameGet.Name.Equals(rgIn.Name));
        }
    }
}
