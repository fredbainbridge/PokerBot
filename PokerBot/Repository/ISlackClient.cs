namespace PokerBot.Repository
{
    public interface ISlackClient
    {
        //void PostMessage(Payload payload);
        void PostWebhookMessage(string text, string username = null, string channel = null);
        void PostAPIMessage(string text, string username = null, string channel = null);
        void PostWebhookMessage(Payload payload);
        void PostAPIMessage(Payload payload);
    }
}
