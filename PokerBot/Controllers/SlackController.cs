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
    public class SlackController : Controller
    {
        private ISlackClient _slackClient;
        //private ISecrets _secrets;

        public SlackController(ISlackClient SlackClient)
        {
            _slackClient = SlackClient;
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
    }
}