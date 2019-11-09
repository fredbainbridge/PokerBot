using PokerBot.Models;
using PokerMavensAPI;
using System.Collections.Generic;
namespace PokerBot.Repository {
    public interface IPokerRepository {
        List<Player> GetSeatedPlayers(string TableName);
        List<RingGamesGet> GetTable(string TableName = null);
        int OpenSeats(string TableName);
        string RemainingSeatsMessage(string TableName);
        LogsHandHistory GetHandHistory(string HandID);
        void SendAdminMessage(string Message, string TableName);
        AccountsList GetAccounts();
        void SetPrimaryBalance(string Name, int Balance);
        List<vSession> GetSessions(int? Top = null);
        List<UserBalance> GetUserBalances();
        User CreateNewUser(string SlackID, string Player, string RealName, string Location, string Email);
        bool ChangePassword(string SlackID, string password);
    }
}
