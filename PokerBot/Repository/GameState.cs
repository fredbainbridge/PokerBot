using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerBot.Repository
{
    public class GameState : IGameState
    {
        private DateTime _gameStart;
        private DateTime _lastGameStartAlert;
        public GameState()
        {
            _lastGameStartAlert = DateTime.Now.AddMinutes(-15);
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
    }
}
