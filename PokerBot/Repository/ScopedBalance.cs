﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using PokerMavensAPI;
using PokerBot.Models;


namespace PokerBot.Repository
{
    internal interface IScopedBalance
    {
        Task DoWork(CancellationToken stoppingToken);
    }
    internal class ScopedBalance : IScopedBalance
    {
        private ISecrets _secrets;
        private IPokerRepository _pokerRepo;
        private PokerDBContext _pokerDB;
        private ISlackClient _slackClient;

        public ScopedBalance(ISecrets secrets, IPokerRepository pokerRepository, PokerDBContext pokerDBContext, ISlackClient slackClient) {
            _secrets = secrets;
            _pokerRepo = pokerRepository;
            _pokerDB = pokerDBContext;
            _slackClient = slackClient;
        }
        public async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                List<Session> sessions = new List<Session>();
                sessions = _pokerRepo.UpdateBalances();
                if(sessions == null)
                {
                    Console.WriteLine("A game is happening, balance changes will not be recorded.");
                }
                else
                {
                    var balances = _pokerRepo.GetUserBalances();
                    foreach (Session s in sessions)
                    {
                        var b = balances.Where(b => b.UserID == s.UserID).FirstOrDefault();
                        var text = "Poker Session Total: " + s.Chips.ToString() + " Balance: " + b.Balance;
                        _slackClient.PostAPIMessage(
                            text: text,
                            channel: s.User.SlackID
                        );
                    }

                }
                
                
                await Task.Delay(900000, stoppingToken);  //run every 15 minutes
            }
        }
    }
}
