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
                    DateTime now = DateTime.Now;
                    AccountsList accountList = _pokerRepo.GetAccounts();
                    for (int i = 0; i < accountList.Accounts; i++) {
                        if(accountList.Balance[i] != "100000")
                        {
                            User u = _pokerDB.User.Where(u => u.UserName.Equals(accountList.Player[i])).FirstOrDefault();
                            Console.WriteLine("Recording session for " + u.RealName);
                            int balance = Int32.Parse(accountList.Balance[i]);
                            int chips = balance - 100000;
                            Session s = new Session();
                            s.Chips = chips;
                            s.Date = now;
                            s.User = u;
                            
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
                
                await Task.Delay(3600000, stoppingToken);  //run every 1 hours
            }
        }
    }
}
