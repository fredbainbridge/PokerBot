using System.Collections.Generic;
namespace PokerBot.Repository {
    public interface ISecrets{
        string PokerURL();
        string Password();
        string SlackURL();
        bool Silence();
        string Token();
        string WebsiteURL();
        string GameURL();
        int Balance();
        List<string> GameName();
    }
}