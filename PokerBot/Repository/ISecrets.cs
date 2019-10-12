namespace PokerBot.Repository {
    public interface ISecrets{
        string PokerURL();
        string Password();
        string SlackURL();
    }
}