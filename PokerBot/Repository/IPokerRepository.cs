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
        Hand GetHandHistory(string HandID);
        void SendAdminMessage(string Message, string TableName);
        List<vSession> GetSessions(int? Top = null);
        List<UserBalance> GetUserBalances();
        bool AnySeatedOrWaitingPlayers();
        List<vHand> GetHands(string handID = null, int minSize = 0, string winner = null, string tableName = null);
        List<Session> UpdateBalances();
        bool IsHOF(Hand hand);
        List<vHand> AddArtToHands(List<vHand> hands);
        bool isMainGame(string tableName);
        string GetNextHand(string Number);
        string GetPreviousHand(string Number);
    }
}
