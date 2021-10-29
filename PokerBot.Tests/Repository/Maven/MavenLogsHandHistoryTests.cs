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
    public class MavenLogsHandHistoryTests
    {
        [Fact]
        public void test_get_hand_history()
        {
            LogsHandHistory logsIn = new LogsHandHistory();
            logsIn.Data = new List<string>() { "hand data" };
            var httpClient = MavenTestSetup.GetHttpClient(logsIn);
            var secrets = MavenTestSetup.GetSecrets();
            MavenLogsHandHistory mrp = new MavenLogsHandHistory(httpClient, secrets);
            LogsHandHistory logsOut = mrp.GetHistory("test");
            Assert.True(logsIn.Data[0].Equals(logsOut.Data[0]));
        }
    }
}
