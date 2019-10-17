using PokerBot.Models;
using System.Collections.Generic;

namespace PokerBot.Repository {
    public class TableState : ITableState {
        private List<Player> _players;
        private IPokerRepository _pokerRepo; 
        private bool _wasMonster;

        public TableState(IPokerRepository PokerRepository) {
            _pokerRepo = PokerRepository;
            _wasMonster = false;
            _players = new List<Player>();
        }
        public List<Player> GetPlayers(string TableName) {
            return _players;
        }
        public void BalanceEvent(string TableName) {
            _players = _pokerRepo.GetSeatedPlayers(TableName);
        }
        public void HandEvent(string TableName, bool ProcessHandEvents = false) {
            List<Player> newTableState = _pokerRepo.GetSeatedPlayers(TableName);
            if(ProcessHandEvents) {
                //determine if hand was a monster!
            }
            
            
        }
        

        public bool WasMonster() {
            return _wasMonster;
        }
    }
}