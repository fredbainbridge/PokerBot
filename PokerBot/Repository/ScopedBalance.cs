using System;
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

        public ScopedBalance(ISecrets secrets, IPokerRepository pokerRepository) {
            _secrets = secrets;
            _pokerRepo = pokerRepository;
        }
        public async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                bool gameOn = false;
                List<Session> sessions = new List<Session>();
                Console.WriteLine("Scoped Processing Service is working");
                //is the table empty?
                List<RingGamesGet> tables = _pokerRepo.GetTable();
                foreach(RingGamesGet t in tables)
                {
                    List<Player> seatedPlayers = _pokerRepo.GetSeatedPlayers(t.Name);
                    if(seatedPlayers.Count() != 0)
                    {
                        gameOn = true;
                    }
                }
                if(!gameOn)
                {
                    AccountsList accountList = _pokerRepo.GetAccounts();
                    for (int i = 0; i < accountList.Accounts; i++) {
                        if(accountList.Balance[i] != "100000")
                        {
                            int balance = Int32.Parse(accountList.Balance[i]);
                            int chips = balance - 100000;
                            Session s = new Session();
                            s.Chips = chips;
                            s.Date = DateTime.Today;
                            s.Name = accountList.RealName[i];
                            sessions.Add(s);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("A game is happening, balance changes will not be recorded.");
                }
                
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
