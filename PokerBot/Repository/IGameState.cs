﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokerBot.Models;

namespace PokerBot.Repository
{
    public interface IGameState
    {
        DateTime GameStart();
        DateTime LastGameStartAlert();
        void SetLastGameStartAlert();
        public void SetGameStart();
        void RemovePlayer(string Name);
        void AddPlayer(Player player);
        Player GetSeatedPlayer(string Name);
        void SetLastMessage(string message);
        string GetLastMessage();

    }
}
