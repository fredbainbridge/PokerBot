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
    public class MavenRingGamesMessageTests
    {
        [Fact]
        public void test_get_ringgameslist()
        {
            var httpClient = MavenTestSetup.GetHttpClient();
            var secrets = MavenTestSetup.GetSecrets();
            MavenRingGamesMessage mrm = new MavenRingGamesMessage(httpClient, secrets);
            mrm.SendAdminMessage("test", "test");
        }
    }
}
