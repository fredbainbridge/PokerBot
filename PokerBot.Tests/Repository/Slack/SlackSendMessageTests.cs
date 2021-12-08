using Moq;
using PokerBot.Models;
using PokerBot.Repository;
using PokerBot.Repository.Mavens;
using PokerMavensAPI;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace PokerBot.Tests.Repository.Slack {
    public class SlackSendMessageTest {
        private HttpClient httpClient;
        private ISecrets secrets;
        public SlackSendMessageTest()
        {
            //secrets = MavenTestSetup.GetSecrets();
        }
        [Fact]
        public void test_send_message()
        {
            ISecrets secrets = new Secrets();
            HttpClient httpClient = new HttpClient();
            SlackClient client = new SlackClient(secrets);
            client.PostAPIMessage("test", "U02EGTBLZ5L");
        }
    }
        
}