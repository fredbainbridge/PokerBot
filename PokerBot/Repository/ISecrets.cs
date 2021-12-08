using System.Collections.Generic;
namespace PokerBot.Repository {
    public interface ISecrets{
        string PokerURL();
        string Password();
        List<string> SlackURLs();
        bool Silence();
        string Token();
        string UserToken();
        string WebsiteURL();
        string GameURL();
        int Balance();
        List<string> GameName();
        string AvatarDir();
        string EventHubConnectionString();
        string EventHubName();
    }
}