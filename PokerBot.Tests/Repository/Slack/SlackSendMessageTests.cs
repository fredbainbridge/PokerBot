using Microsoft.Extensions.Logging;
using Moq;
using PokerBot.Models;
using PokerBot.Repository;
using PokerBot.Repository.Mavens;
using PokerMavensAPI;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;
using PokerBot.Repository.EventHub;
namespace PokerBot.Tests.Repository.Slack {
    public class SlackSendMessageTest {
        public SlackSendMessageTest()
        {
            //secrets = MavenTestSetup.GetSecrets();
        }
        [Fact]
        public void test_send_message()
        {
            ISecrets secrets = new Secrets();
            HttpClient httpClient = new HttpClient();
            Mock<ILogger<SlackClient>> logger = new Mock<ILogger<SlackClient>>();
            Mock<IPokerEventHub> eventHob = new Mock<IPokerEventHub>();
            SlackClient client = new SlackClient(secrets, httpClient, logger.Object, eventHob.Object);
            client.PostAPIMessage("test", "U02EGTBLZ5L");
        }
    }
        
}