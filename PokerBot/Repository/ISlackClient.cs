namespace PokerBot.Repository
{
    public interface ISlackClient
    {
        void PostMessage(Payload payload);
        void PostMessage(string text, string username = null, string channel = null);
    }
}
