namespace PokerBot.Repository.EventHub {
    public interface IPokerEventHub {
        void SendEvent(string message);
    }
}