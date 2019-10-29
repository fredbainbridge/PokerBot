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
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PokerBot.Controllers
{
    public class SlackController : Controller
    {
        private ISlackClient _slackClient;
        private PokerDBContext _pokerContext;
        //private ISecrets _secrets;

        public SlackController(ISlackClient SlackClient, PokerDBContext pokerContext)
        {
            _slackClient = SlackClient;
            _pokerContext = pokerContext;
        }
        [HttpPost]
        public IActionResult Index(IFormCollection Form)
        {
            string userID = Form["user_id"];
            _slackClient.PostAPIMessage(
                        text: "The server is up!",
                        //username: userID,
                        channel: userID
                    );
            return new EmptyResult();
        }
        [HttpGet]
        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult Balance(IFormCollection Form)
        {
            string userID = Form["user_id"];
            User u = _pokerContext.User.Where(u => u.SlackID.Equals(userID)).FirstOrDefault();
            UserBalance balance = _pokerContext.UserBalance.Where(b => b.UserID == u.ID).FirstOrDefault();
            Console.WriteLine(u.RealName + " has requested their balance.");
            string text = "Your balance is: " + balance.Balance;
            _slackClient.PostAPIMessage(
                text: text,
                channel: userID
                );
            return new EmptyResult();
        }
    }
}