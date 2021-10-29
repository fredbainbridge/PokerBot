using PokerMavensAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokerBot.Repository.Mavens
{
    public interface IMavenLogsHandHistory
    {
        LogsHandHistory GetHistory(string handId);
    }
    public class MavenLogsHandHistory : IMavenLogsHandHistory
    {
        private HttpClient _client;
        private ISecrets _secrets;
        public MavenLogsHandHistory(HttpClient client, ISecrets secrets)
        {
            _client = client;
            _secrets = secrets;
        }
        public LogsHandHistory GetHistory(string handId)
        {
            var client = new MavenClient<LogsHandHistory>(_secrets);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "LogsHandHistory");
            dict.Add("Hand", handId);
            return client.Post(_client, dict);
        }
    }
}
