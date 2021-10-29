using Moq;
using PokerBot.Models;
using PokerBot.Repository;
using PokerBot.Repository.Mavens;
using Xunit;
using PokerMavensAPI;

namespace PokerBot.Tests.Repository.Maven
{
    public class MavenAccountsListTests
    {
        private ISecrets secrets;
        public MavenAccountsListTests()
        {
            secrets = MavenTestSetup.GetSecrets();
        }
        [Fact]
        public void test_get_accounts()
        {
            AccountsList listIn = new AccountsList();
            listIn.Accounts = 1;
            var httpClient = MavenTestSetup.GetHttpClient(listIn); 
            MavenAccountsList mal = new MavenAccountsList(httpClient, secrets);
            var listOut = mal.GetAccounts();
            Assert.True(listOut.Accounts == listIn.Accounts);
        }
        
    }
}
