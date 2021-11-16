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
using PokerBot.Repository.EventHub;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PokerBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IPokerRepository _pokerRepository;
        private ISecrets _secrets;
        private ISlackClient _slackClient;
        private IGameState _gameState;
        private IPokerEventHub _pokerEventHub;
        
        public HomeController(ILogger<HomeController> logger, IPokerRepository PokerRepository, ISecrets Secrets, ISlackClient SlackClient, IGameState GameState, IPokerEventHub pokerEventHub)
        {
            _logger = logger;
            _pokerRepository = PokerRepository;
            _secrets = Secrets;
            _slackClient = SlackClient;
            _gameState = GameState;
            _pokerEventHub = pokerEventHub;
        }

        public IActionResult Index()
        {
            List<vSession> sessions = _pokerRepository.GetSessions();
            return View(sessions.OrderByDescending(s => s.Date).Take(100).ToList());
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
        public IActionResult Hands(string ID = null, int minSize = 0, string winner = null)
        {
            ViewBag.minSize = minSize;
            ViewBag.winner = winner;
            List<vHand> hands = _pokerRepository.GetHands(ID, minSize, winner);
            ViewBag.NextHand = "na";
            ViewBag.PrevHand = "na";
            if (hands.Count == 1)
            {
                ViewBag.NextHand = _pokerRepository.GetNextHand(ID);
                ViewBag.PrevHand = _pokerRepository.GetPreviousHand(ID);
            }
            return View(_pokerRepository.AddArtToHands(hands));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public JsonResult Poker(IFormCollection Form) {
            //string externalip = new System.Net.WebClient().DownloadString("http://bot.whatismyipaddress.com");            
            //string gameUrl = "http://" + externalip + ":8087";
            string gameUrl = _secrets.GameURL();
            string websiteURL = _secrets.WebsiteURL();
            string message = "";
            if(Form["Event"] == "RingGameLeave") {
                string TableName = Form["Name"];
                if (!_pokerRepository.isMainGame(TableName))
                {
                    return Json(new EmptyResult());
                }
                string PlayerName = Form["Player"];
                string Amount = Form["Amount"];
                string Time = Form["Time"];

                string remainingSeatsMsg = _pokerRepository.RemainingSeatsMessage(TableName);
                message = PlayerName + $" has left {TableName}. " + remainingSeatsMsg;
                _gameState.RemovePlayer(PlayerName);
            }
            if(Form["Event"] == "RingGameJoin") {
                string TableName = Form["Name"];
                if (!_pokerRepository.isMainGame(TableName))
                {
                    return Json(new EmptyResult());
                }
                string PlayerName = Form["Player"];
                string Amount = Form["Amount"];
                string Time = Form["Time"];
                
                string remainingSeats = _pokerRepository.RemainingSeatsMessage(TableName);
                
                string remainingSeatsMsg = _pokerRepository.RemainingSeatsMessage(TableName);
                message = PlayerName + " has sat down with $" + Amount + $" at {TableName}! " + remainingSeatsMsg + gameUrl;
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
                if (!_pokerRepository.isMainGame(TableName))
                {
                    return Json(new EmptyResult());
                }
                string Time = Form["Time"];
                TimeSpan ts = DateTime.Now - _gameState.LastGameStartAlert();
                if(ts.TotalMinutes > 15)
                {
                    if(!_gameState.GetLastMessage().Equals($"A game has started at {TableName}! " + gameUrl)) {
                        message = $"A game has started at {TableName}! " + gameUrl;
                        _gameState.SetLastGameStartAlert();
                    }
                }
                _gameState.SetGameStart();

            }
            if(Form["Event"] == "Hand") {
                int Unspeakable = 100000; int Huge = 50000; int Monster = 20000;
                string HandNumber = Form["Hand"];
                string TableName = Form["Name"];
                if (!_pokerRepository.isMainGame(TableName))
                {
                    return Json(new EmptyResult());
                }
                
                //get the hand number.
                //get the hand history
                //determine if its a monster hand!!
                Hand hand = _pokerRepository.GetHandHistory(HandNumber);
                if (TableName.Contains("100/200"))
                {
                    Unspeakable = Unspeakable * 10;
                    Huge = Huge * 10;
                    Monster = Monster * 10;

                }
                string handURL = websiteURL + "/Home/Hands/" + HandNumber;
                string type = "";

                if (hand.WinningAmount > Unspeakable)
                {
                    message = "Something unspeakable has happened! " + handURL;
                }
                else if (hand.WinningAmount > Huge)
                {
                    type = "FUCKING HUGE";
                }
                else if (hand.WinningAmount > Monster)
                {
                    type = "MONSTER";
                }
                if (!string.IsNullOrEmpty(type))
                {
                    string amount = String.Format("{0:n0}", hand.WinningAmount);
                    message = hand.Winner.UserName + " just won a " + type + $" pot at {TableName}! (" + amount + ") " + handURL;
                }
                if(_pokerRepository.IsHOF(hand))
                {
                    if (!_secrets.Silence())
                    {
                        _slackClient.PostWebhookMessage(
                            text: "We have a new Hall of Fame pot! " + handURL
                        );
                    }
                }
            }
            if(Form["Event"] == "Balance") {
                string player = Form["Player"];
                string source = Form["Source"];
                if (!_pokerRepository.isMainGame(source))
                {
                    return Json(new EmptyResult());
                }
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
                            string adminmessage = "";
                            if(changeInt == 1)
                            {
                                adminmessage = player + " added " + String.Format("{0:n0}", (changeInt * -1)) + " god damn chip!";
                            }
                            else
                            {
                                adminmessage = player + " added " + String.Format("{0:n0}", (changeInt * -1)) + " chips.";
                            }
                            _logger.LogInformation("(" + System.DateTime.Now.ToString() + ") " + adminmessage);
                            _pokerRepository.SendAdminMessage(adminmessage, source);
                        }
                    }
                }
                //if they are not seated, do nothing.
            }
            if(Form["Event"] == "Login")
            {
                string player = Form["Player"];
                var tables = _pokerRepository.GetTable();
                foreach(var t in tables)
                {
                    _logger.LogInformation("(" + System.DateTime.Now.ToString() + ") " + player + " has logged in.");
                    _pokerRepository.SendAdminMessage(player + " has logged in.", t.Name);
                }
                
                //update the table state with the new balanaces.
            }
            if (Form["Event"] == "TourneyRegister")
            {
                //Name, Player, Late, and Time.
                string name = Form["Name"];
                var player = Form["Player"];
                message = $"{player} has registered for {name}!";
                _pokerRepository.SendMessageToAllRingGames(message);
            }
            if (Form["Event"] == "TourneyUnregister")
            {
                string name = Form["Name"];
                var player = Form["Player"];
                message = $"{player} has unregistered for {name}!";
                _pokerRepository.SendMessageToAllRingGames(message);
            }
            if (Form["Event"] == "TourneyStart")
            {
                string name = Form["Name"];
                message = $"{name} has started!";
                _pokerRepository.SendMessageToAllRingGames(message);
            }
            if (!string.IsNullOrEmpty(message)) {
                _logger.LogInformation("(" + System.DateTime.Now.ToString() + ") " + message);
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
        
        public ActionResult HOF(string tableName = "")
        {
            
            var list = new List<SelectListItem> {
                new SelectListItem { Value = "Cash Game 10/20 No Limit", Text = "Cash Game 10/20 No Limit" },
                new SelectListItem { Value = "Cash Game 100/200 No Limit", Text = "Cash Game 100/200 No Limit" },
                new SelectListItem { Value = "Cash Game 10/20 PLO", Text = "Cash Game 10/20 PLO" }
            };

            foreach (SelectListItem item in list)
            {
                if(item.Value == tableName)
                {
                    item.Selected = true;
                }
            }
            ViewBag.Tables = list;
            if(string.IsNullOrEmpty(tableName)) { tableName = "Cash Game 10/20 No Limit";}
            var hands = _pokerRepository.GetHands(null, 10000, null, tableName)
                .OrderByDescending(h => h.Amount).Take(20).ToList();
            return View(_pokerRepository.AddArtToHands(hands));            
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
