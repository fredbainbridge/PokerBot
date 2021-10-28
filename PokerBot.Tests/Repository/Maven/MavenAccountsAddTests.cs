using Moq;
using PokerBot.Models;
using PokerBot.Repository;
using PokerBot.Repository.Mavens;
using System.Linq;
using Xunit;

namespace PokerBot.Tests.Repository.Maven
{
    public class MavenAccountsAddTests
    {
        [Fact]
        public async void test_create_new_user()
        {
            var dbContext = await MavenTestSetup.GetDatabaseContext();
            var httpClient = MavenTestSetup.GetHttpClient();
            var secrets = MavenTestSetup.GetSecrets();
            MavenAccountsAdd maa = new MavenAccountsAdd(httpClient, dbContext, secrets);
            User u = maa.CreateNewUser("NewSlackId", "Player", "RealName", "Location", "Email");
            Assert.True(dbContext.User.Where(u => u.SlackID.Equals("NewSlackId")).Any());
        }
    }
}
