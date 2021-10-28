using Moq;
using PokerBot.Models;
using PokerBot.Repository;
using PokerBot.Repository.Mavens;
using Xunit;

namespace PokerBot.Tests.Repository.Maven
{
    public class MavenAccountsEditTests
    {        
        [Fact]
        public async void test_change_password_unknown_user()
        {
            var dbContext = await MavenTestSetup.GetDatabaseContext();
            var httpClient = MavenTestSetup.GetHttpClient();
            var secrets = MavenTestSetup.GetSecrets();
            MavenAccountsEdit mae = new MavenAccountsEdit(httpClient, dbContext, secrets);
            Assert.False(mae.ChangePassword("abc", "abc"));
        }
        [Fact]
        public void test_change_password_known_user()
        {
            var options = MavenTestSetup.GetSqlLightContext();
            var httpClient = MavenTestSetup.GetHttpClient();
            var secrets = MavenTestSetup.GetSecrets();
            using (var dbContext = new PokerDBContext(options))
            {
                MavenAccountsEdit mae = new MavenAccountsEdit(httpClient, dbContext, secrets);
                Assert.True(mae.ChangePassword("slackid", "abc"));
            }
            
            
        }

    }
}
