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
        private PokerDBContext _pokerDB;

        public ScopedBalance(ISecrets secrets, IPokerRepository pokerRepository, PokerDBContext pokerDBContext) {
            _secrets = secrets;
            _pokerRepo = pokerRepository;
            _pokerDB = pokerDBContext;
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
                            s.User = _pokerDB.User.Where(u => u.UserName.Equals(accountList.Player[i])).FirstOrDefault();
                            
                            //s.Name = accountList.RealName[i];
                            sessions.Add(s);
                            _pokerDB.Sessions.Add(s);
                            _pokerDB.SaveChanges();
                            _pokerRepo.SetPrimaryBalance(accountList.Player[i], 100000);

                        }
                    }
                }
                else
                {
                    Console.WriteLine("A game is happening, balance changes will not be recorded.");
                }
                
                await Task.Delay(14400000, stoppingToken);  //run every 4 hours
            }
        }
    }
}
