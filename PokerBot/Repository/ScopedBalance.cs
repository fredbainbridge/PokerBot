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
                bool gameOn = false;
                List<Session> sessions = new List<Session>();
                //Console.WriteLine("Scoped Processing Service is working");
                //is the table empty?
                List<RingGamesGet> tables = _pokerRepo.GetTable();
                foreach(RingGamesGet t in tables)
                {
                    gameOn = _pokerRepo.AnySeatedPlayers();
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
                var balances = _pokerRepo.GetUserBalances();
                foreach(Session s in sessions)
                {
                    var b = balances.Where(b => b.UserID == s.UserID).FirstOrDefault();
                    var text = "New session recorded! Session total: " + s.Chips.ToString() + " Balance: " + b.Balance;
                    _slackClient.PostAPIMessage(
                        text: text,
                        channel: s.User.SlackID
                    ); 
                }
                
                await Task.Delay(1800000, stoppingToken);  //run every 1 hours
            }
        }
    }
}
