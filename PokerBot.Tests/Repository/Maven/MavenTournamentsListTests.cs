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
    public class MavenTournamentsListTests
    {
        [Fact]
        public async void test_get_waiting_players()
        {
            var dbContext = await MavenTestSetup.GetDatabaseContext();
            TournamentsList tpIn = new TournamentsList();
            tpIn.Game = new List<string>() { "test" };
            var httpClient = MavenTestSetup.GetHttpClient(tpIn);
            var secrets = MavenTestSetup.GetSecrets();
            MavenTournamentsList mtl = new MavenTournamentsList(httpClient, secrets);
            TournamentsList returnedTournamentList = mtl.Tournaments();
            Assert.True(returnedTournamentList.Game.Count() == tpIn.Game.Count());
        }
    }
}
