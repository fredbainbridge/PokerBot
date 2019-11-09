using System;
using System.Diagnostics;
using PokerBot.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PokerBot.Models;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using PokerMavensAPI;

namespace PokerBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IPokerRepository _pokerRepository;
        private ISecrets _secrets;
        private ISlackClient _slackClient;
        private IGameState _gameState;
        
        public HomeController(ILogger<HomeController> logger, IPokerRepository PokerRepository, ISecrets Secrets, ISlackClient SlackClient, IGameState GameState)
        {
            _logger = logger;
            _pokerRepository = PokerRepository;
            _secrets = Secrets;
            _slackClient = SlackClient;
            _gameState = GameState;
        }

        public IActionResult Index()
        {
            List<vSession> sessions = _pokerRepository.GetSessions();
            return View(sessions.OrderByDescending(s => s.Date).ToList());
        }

        public IActionResult Help()
        {
            return View();
        }
        public IActionResult Balance()
        {
            List<UserBalance> balances = _pokerRepository.GetUserBalances();
            return View(balances.OrderByDescending(b => b.Priority).ToList());
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public JsonResult Poker(IFormCollection Form) {
            //string externalip = new System.Net.WebClient().DownloadString("http://bot.whatismyipaddress.com");            
            //string gameUrl = "http://" + externalip + ":8087";
            string gameUrl = "http://poker.hopto.org:8087";
            string message = "";
            if(Form["Event"] == "RingGameLeave") {
                string TableName = Form["Name"];
                string PlayerName = Form["Player"];
                string Amount = Form["Amount"];
                string Time = Form["Time"];

                string remainingSeatsMsg = _pokerRepository.RemainingSeatsMessage(TableName);
                message = PlayerName + " has left the table. " + remainingSeatsMsg;
                _gameState.RemovePlayer(PlayerName);
            }
            if(Form["Event"] == "RingGameJoin") {
                string TableName = Form["Name"];
                string PlayerName = Form["Player"];
                string Amount = Form["Amount"];
                string Time = Form["Time"];
                
                string remainingSeats = _pokerRepository.RemainingSeatsMessage(TableName);
                
                string remainingSeatsMsg = _pokerRepository.RemainingSeatsMessage(TableName);
                message = PlayerName + " has sat down with $" + Amount + "! " + remainingSeatsMsg + gameUrl;
                if(remainingSeats.Equals("There are 6 seats remaining. "))
                {
                    _pokerRepository.SendAdminMessage("We have 4 players, now is a good time to click your Straddle button!", TableName);
                }
                Player p = new Player();
                p.Name = PlayerName;
                p.TimeSeated = DateTime.Now;
                _gameState.AddPlayer(p);
            }
            if(Form["Event"] == "RingGameStart") {
                string TableName = Form["Name"];
                string Time = Form["Time"];
                TimeSpan ts = DateTime.Now - _gameState.LastGameStartAlert();
                if(ts.TotalMinutes > 15)
                {
                    if(!_gameState.GetLastMessage().Equals("A game has started! " + gameUrl)) {
                        message = "A game has started! " + gameUrl;
                        _gameState.SetLastGameStartAlert();
                    }
                }
                _gameState.SetGameStart();

            }
            if(Form["Event"] == "Hand") {
                string Hand = Form["Hand"];
                string Table = Form["Table"];
                string TableName = Form["Name"];
                //get the hand number.
                //get the hand history
                //determine if its a monster hand!!
                LogsHandHistory hand = _pokerRepository.GetHandHistory(Hand);
                foreach(string s in hand.Data) {
                    if(s.Contains(" wins Pot (")) { //winner declaration
                        //"Fred wins Pot (40)"
                        int index1 = s.IndexOf(" wins Pot (");
                        string player = s.Substring(0, index1);
                        index1 = s.LastIndexOf(" wins Pot (") + 11;
                        int index2 = s.LastIndexOf(")");;
                        string winningAmountString = s.Substring(index1,index2 - index1);
                        //"Seat 5: Fred (+20) [2d 3h] Won without Showdown"
                        int winningAmountInt;
                        bool success = Int32.TryParse(winningAmountString, out winningAmountInt);
                        if(success) {
                            string type = "";
                            if(winningAmountInt > 50000) {
                                type = "FUCKING HUGE";
                            }
                            else if(winningAmountInt > 20000) {
                                type = "MONSTER";
                            }
                            if(!string.IsNullOrEmpty(type)) {
                                string amount = String.Format("{0:n0}", winningAmountInt);
                                message = player + " just won a " + type + " pot! (" + amount +")";
                            }
                            
                        }
                        
                    }
                }
            }
            if(Form["Event"] == "Balance") {
                string player = Form["Player"];
                string source = Form["Source"];
                string change = Form["Change"];
                int changeInt = Int32.Parse(change);
                if(changeInt < 0)
                {
                    //if they were seated more than 1 minute ago.
                    Player p = _gameState.GetSeatedPlayer(player);
                    if (p != null)
                    {
                        TimeSpan ts = DateTime.Now - p.TimeSeated;
                        if (ts.TotalSeconds > 5)
                        {
                            string adminmessage = player + " added " + String.Format("{0:n0}",(changeInt * -1)) + " chips.";
                            Console.WriteLine("(" + System.DateTime.Now.ToString() + ") " + adminmessage); 
                            _pokerRepository.SendAdminMessage(adminmessage, source);
                        }
                    }
                }
                
                //if they are not seated, do nothing.

            }
            if (Form["Event"] == "Login")
            {
                string player = Form["Player"];
                var tables = _pokerRepository.GetTable();
                foreach(var t in tables)
                {
                    Console.WriteLine("(" + System.DateTime.Now.ToString() + ") " + player + " has logged in.");
                    _pokerRepository.SendAdminMessage(player + " has logged in.", t.Name);
                }
                
                //update the table state with the new balanaces.
            }
            
            if (!string.IsNullOrEmpty(message)) {
                Console.WriteLine("(" + System.DateTime.Now.ToString() + ") " + message);
                if(!_secrets.Silence()){
                    _slackClient.PostWebhookMessage(
                        text: message
                    );
                }
                _gameState.SetLastMessage(message);
            }
            return Json(new EmptyResult());
        }

        public JsonResult AdminMessage(string Message)
           {
            string TableName = "10/20 No Limit ($1000 min buy in)";
            _pokerRepository.SendAdminMessage("We have 4 players, now is a good time to click your Straddle button!", TableName);
            return Json(new EmptyResult());
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
