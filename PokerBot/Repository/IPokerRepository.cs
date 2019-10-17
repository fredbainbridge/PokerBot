using PokerBot.Models;
using PokerMavensAPI;
using System.Collections.Generic;
namespace PokerBot.Repository {
    public interface IPokerRepository {
        List<Player> GetSeatedPlayers(string TableName);
        RingGamesGet GetTable(string TableName);
        int OpenSeats(string TableName);
        string RemainingSeatsMessage(string TableName);
        LogsHandHistory GetHandHistory(string HandID);
    }
}
