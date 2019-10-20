using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerBot.Repository
{
    public interface IGameState
    {
        DateTime GameStart();
        DateTime LastGameStartAlert();
        void SetLastGameStartAlert();
        public void SetGameStart();

    }
}
