using PokerBot.Models;
using System.Collections.Generic;

namespace PokerBot.Repository {
    public interface ITableState{
        List<Player> GetPlayers(string TableName);
        void BalanceEvent(string TableName);
        void HandEvent(string TableName, bool ProcessHandEvents = false);
    }
}