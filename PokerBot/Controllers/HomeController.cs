using System;
using System.Diagnostics;
using PokerBot.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PokerBot.Models;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using PokerMavensAPI;

namespace PokerBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IPokerRepository _pokerRepository;
        private ISecrets _secrets;
        public HomeController(ILogger<HomeController> logger, IPokerRepository PokerRepository, ISecrets Secrets)
        {
            _logger = logger;
            _pokerRepository = PokerRepository;
            _secrets = Secrets;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public JsonResult Poker(IFormCollection Form) {
            string url = _secrets.SlackURL();
            SlackClient client = new SlackClient(url);
            string externalip = new System.Net.WebClient().DownloadString("http://bot.whatismyipaddress.com");            
            string gameUrl = "http://" + externalip + ":8087";
            string message = "";
            if(Form["Event"] == "RingGameLeave") {
                string TableName = Form["Name"];
                string PlayerName = Form["Player"];
                string Amount = Form["Amount"];
                string Time = Form["Time"];

                string remainingSeatsMsg = _pokerRepository.RemainingSeatsMessage(TableName);
                message = PlayerName + " has left the table. " + remainingSeatsMsg;
            }
            if(Form["Event"] == "RingGameJoin") {
                string TableName = Form["Name"];
                string PlayerName = Form["Player"];
                string Amount = Form["Amount"];
                string Time = Form["Time"];
                
                string remainingSeatsMsg = _pokerRepository.RemainingSeatsMessage(TableName);
                message = PlayerName + " has sat down with $" + Amount + "! " + remainingSeatsMsg + gameUrl;
            }
            if(Form["Event"] == "RingGameStart") {
                string TableName = Form["Name"];
                string Time = Form["Time"];
                
                message = "A game has started! " + gameUrl;
            }
            if(Form["Event"] == "Hand") {
                Console.WriteLine("Tommy Like Wingie");
            }
            if(!string.IsNullOrEmpty(message)) {
                Console.WriteLine(message);
                if(!_secrets.Silence()){
                    client.PostMessage(
                        text: message
                    );
                }
            }
            return Json(new EmptyResult());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
