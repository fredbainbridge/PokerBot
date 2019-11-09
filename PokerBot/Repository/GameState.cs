using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokerBot.Models;

namespace PokerBot.Repository
{
    public class GameState : IGameState
    {
        private DateTime _gameStart;
        private DateTime _lastGameStartAlert;
        private List<Player> _seatedPlayers;
        private string _lastMessage;
        public GameState()
        {
            _lastGameStartAlert = DateTime.Now.AddMinutes(-15);
            _seatedPlayers = new List<Player>();
        }
        public void SetLastMessage(string message)
        {
            _lastMessage = message;
        }
        public string GetLastMessage()
        {
            return _lastMessage;
        }
        public DateTime GameStart()
        {
            return _gameStart;
        }
        public void SetGameStart()
        {
            _gameStart = DateTime.Now;
        }

        public DateTime LastGameStartAlert()
        {
            return _lastGameStartAlert;
        }
        public void SetLastGameStartAlert()
        {
            _lastGameStartAlert = DateTime.Now;
        }
        public bool IsSeated(string Name)
        {
            if(_seatedPlayers.Select(p => p.Name).Contains(Name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void AddPlayer(Player player)
        {
            _seatedPlayers.Add(player);
        }
        public void RemovePlayer(string Name)
        {
            Player p = _seatedPlayers.Where(p => p.Name.Equals(Name)).FirstOrDefault();
            _seatedPlayers.Remove(p);
        }
        public Player GetSeatedPlayer(string Name)
        {
            return _seatedPlayers.Where(p => p.Name.Equals(Name)).FirstOrDefault();
        }
    }
    
}
