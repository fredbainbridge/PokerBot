using PokerBot.Models;
using PokerMavensAPI;
using System.Collections.Generic;
namespace PokerBot.Repository {
    public interface IPokerRepository {
        List<Player> GetSeatedPlayers(string TableName, string Type = "RingGame");
        List<RingGamesGet> GetTable(string TableName = null);
        void SendMessageToAllRingGames(string message);
        int OpenSeats(string TableName);
        string RemainingSeatsMessage(string TableName);
        void SetAvatarPath(string name, string path);
        Hand GetHandHistory(string HandID);
        void SendAdminMessage(string Message, string TableName);
        AccountsList GetAccounts();
        void SetPrimaryBalance(string Name, int Balance);
        List<vSession> GetSessions(int? Top = null);
        List<UserBalance> GetUserBalances();
        User CreateNewUser(string SlackID, string Player, string RealName, string Location, string Email);
        bool ChangePassword(string SlackID, string password);
        bool AnySeatedRingGamePlayers();
        bool AnySeatedTournamentPlayers();
        bool AnySeatedOrWaitingPlayers();
        bool AnyWaitingTournamentPlayers();
        List<vHand> GetHands(string handID = null, int minSize = 0, string winner = null);
        List<Session> UpdateBalances();
        bool IsHOF(string number);
        List<vHand> AddArtToHands(List<vHand> hands);
        bool isMainGame(string tableName);
        string GetNextHand(string Number);
        string GetPreviousHand(string Number);
    }
}
