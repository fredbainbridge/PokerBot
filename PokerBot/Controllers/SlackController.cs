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
using System.Collections.Generic;

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
        public IActionResult RequestPayment(IFormCollection Form)
        {
            string userID = Form["user_id"];
            User u = _pokerContext.User.Where(u => u.SlackID.Equals(userID)).FirstOrDefault();
            if (!u.RealName.Equals("Fred"))
            {
                throw new Exception("You aren't Fred.");
            }
            //payment.
            
            string text = Form["Text"];
            string[] parameters = text.Split(' ');
            string payeeID = "";
            string payeeName = "";
            string payerID = "";
            string payerName = "";
            string chips = "";
            int chipint;
            for (int i = 0; i < parameters.Length; i++)
            {
                if(i == 0)
                {
                    try
                    {
                        payerID = parameters[i].Trim('<').Trim('>').Trim('@').Split('|')[0];
                        payerName = parameters[i].Trim('<').Trim('>').Trim('@').Split('|')[1];
                    }
                    catch
                    {
                        _slackClient.PostAPIMessage("Unable to determine the payer.", null, userID);
                        return new EmptyResult();
                    }
                    
                }
                if(i == 1)
                {
                    try
                    {
                        payeeID = parameters[i].Trim('<').Trim('>').Trim('@').Split('|')[0];
                        payeeName = parameters[i].Trim('<').Trim('>').Trim('@').Split('|')[1];
                    }
                    catch
                    {
                        _slackClient.PostAPIMessage("Unable to determine the payee.", null, userID);
                        return new EmptyResult();
                    }
                    
                }
                if(i == 2)
                {
                    chips = parameters[i];
                }
            }
            if(String.IsNullOrEmpty(payeeID) || String.IsNullOrEmpty(payerID))
            {
                throw new Exception("Payee or Payer missing.");
            }
            if(String.IsNullOrEmpty(chips))
            {
                chipint = 10000;
            }
            else
            {
                try
                {
                    chipint = Int32.Parse(chips);
                }
                catch
                {
                    throw new Exception("Unable to determine the chip amount.");
                }
            }
            User Payer = _pokerContext.User.Where(u => u.SlackID.Equals(payerID)).FirstOrDefault();
            User Payee = _pokerContext.User.Where(u => u.SlackID.Equals(payeeID)).FirstOrDefault();
            Payment payment = new Payment();
            payment.Chips = chipint;
            payment.DateRequested = DateTime.Now;
            payment.Payee = Payee;
            payment.Payer = Payer;
            _pokerContext.Payment.Add(payment);
            _pokerContext.SaveChanges();

            string chipsMoney = "$" + string.Format("{0:#.00}", Convert.ToDecimal(chipint) / 100 );

            //slack to payer
            string payerMsg = "Please pay " + payeeName + " " + chipsMoney + ".  When the payment is sent do: ```/paid @" + payeeName + " [method of payment]```";
            _slackClient.PostAPIMessage(payerMsg, null, payerID);

            //slack to payee
            string payeeMsg = payerName + " has been requested to pay you " + chipsMoney + ".  You will be notified when the payment is sent.";
            _slackClient.PostAPIMessage(payeeMsg, null, payeeID);
            return new EmptyResult();       
        }
        public IActionResult PaymentSent(IFormCollection Form)
        {
            //<@U0XTFV6KU|fredbainbridge> Venmo or whatever
            string userID = Form["user_id"];
            string userName = Form["user_name"];
            string text = Form["Text"];
            //who is being paid?
            string to = text.Substring(0, text.LastIndexOf(">")).Trim('<').Trim('>').Trim('@').Split('|')[0];
            string toName = text.Substring(0, text.LastIndexOf(">")).Trim('<').Trim('>').Trim('@').Split('|')[1];
            string notes = text.Substring(text.IndexOf(' ') + 1);
            User payee = _pokerContext.User.Where(u => u.SlackID == to).FirstOrDefault();
            User payer = _pokerContext.User.Where(u => u.SlackID == userID).FirstOrDefault();

            //is there an open payment between this two?
            Payment existingPayment = _pokerContext.Payment.Where(p => p.Payee == payee && p.Payer == payer && p.Confirmed == null).FirstOrDefault();
            if(existingPayment == null)
            {
                string msg = "There is no existing payment between you and " + toName + ".";
                _slackClient.PostAPIMessage(msg, null, userID);
                return new EmptyResult();
            }
            existingPayment.Sent = DateTime.Now;
            _pokerContext.SaveChanges();

            string payeeMsg = userName + " has sent you a payment via: " + notes;
            _slackClient.PostAPIMessage(payeeMsg, null, to);
            payeeMsg = "Confirm payment has been received with /confirm @" + userName;
            _slackClient.PostAPIMessage(payeeMsg, null, to);

            string payerMsg = toName + " has been notified of your payment.  You will be notified when " + toName + " confirms the payment.";
            _slackClient.PostAPIMessage(payerMsg, null, userID);
            return new EmptyResult();
        }
        public IActionResult ConfirmPayment(IFormCollection Form)
        {
            string userID = Form["user_id"];
            string userName = Form["user_name"];
            string text = Form["Text"];
            string fromID = text.Substring(0, text.LastIndexOf(">")).Trim('<').Trim('>').Trim('@').Split('|')[0];
            string fromName = text.Substring(0, text.LastIndexOf(">")).Trim('<').Trim('>').Trim('@').Split('|')[1];
            if(string.IsNullOrEmpty(fromID))
            {
                _slackClient.PostAPIMessage("No user specified.  Who are you confirming payment from?  usage: /confirm @user  ", null, userID);
                return new EmptyResult();
            }
            User payer = _pokerContext.User.Where(u => u.SlackID == fromID).FirstOrDefault();
            User payee = _pokerContext.User.Where(u => u.SlackID == userID).FirstOrDefault();
            Payment payment = _pokerContext.Payment.Where(p => p.Payee == payee && p.Payer == payer && p.Confirmed == null).FirstOrDefault();
            if(payment == null)
            {
                _slackClient.PostAPIMessage("No open payment found from " + userName + " to you.");
                return new EmptyResult();
            }
            payment.Confirmed = DateTime.Now;
            _pokerContext.SaveChanges();
            string fromMessage = userName + " confirmed receipt of your payment.";
            _slackClient.PostAPIMessage(fromMessage, null, fromID);

            string toMessage = "Payment receipt confirmed, thank you.";
            _slackClient.PostAPIMessage(toMessage, null, userID);
            return new EmptyResult();
            
        }
    }
}