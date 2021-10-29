using Moq;
using PokerBot.Models;
using PokerBot.Repository;
using PokerBot.Repository.Mavens;
using Xunit;

namespace PokerBot.Tests.Repository.Maven
{
    public class MavenAccountsEditTests
    {
        private ISecrets secrets;
        public MavenAccountsEditTests()
        {
            secrets = MavenTestSetup.GetSecrets();
        }
        [Fact]
        public async void test_change_password_unknown_user()
        {
            var dbContext = await MavenTestSetup.GetDatabaseContext();
            var httpClient = MavenTestSetup.GetHttpClient();
            
            MavenAccountsEdit mae = new MavenAccountsEdit(httpClient, dbContext, secrets);
            Assert.False(mae.ChangePassword("abc", "abc"));
        }
        [Fact]
        public void test_change_password_known_user()
        {
            var options = MavenTestSetup.GetSqlLightContext();
            var httpClient = MavenTestSetup.GetHttpClient();
            using (var dbContext = new PokerDBContext(options))
            {
                MavenAccountsEdit mae = new MavenAccountsEdit(httpClient, dbContext, secrets);
                Assert.True(mae.ChangePassword("slackid", "abc"));
            }            
        }

        [Fact]
        public void test_account_avatar_change()
        {
            var options = MavenTestSetup.GetSqlLightContext();
            var httpClient = MavenTestSetup.GetHttpClient();
            using (var dbContext = new PokerDBContext(options))
            {
                MavenAccountsEdit mae = new MavenAccountsEdit(httpClient, dbContext, secrets);
                mae.SetAvatarPath("name", "path");
            }
        }

        [Fact]
        public void test_set_account_primary_balance()
        {
            var options = MavenTestSetup.GetSqlLightContext();
            var httpClient = MavenTestSetup.GetHttpClient();
            using (var dbContext = new PokerDBContext(options))
            {
                MavenAccountsEdit mae = new MavenAccountsEdit(httpClient, dbContext, secrets);
                mae.SetPrimaryBalance("name", 1);
            }
        }
    }
}
