using System;
using PokerBot.Repository;
using Microsoft.AspNetCore.Mvc;
using PokerBot.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;


namespace PokerBot.Controllers
{
    public class SlackController : Controller
    {
        private ISlackClient _slackClient;
        private PokerDBContext _pokerContext;
        private IPokerRepository _pokerRepo;
        //private ISecrets _secrets;

        public SlackController(ISlackClient SlackClient, PokerDBContext pokerContext, IPokerRepository PokerRepo)
        {
            _slackClient = SlackClient;
            _pokerContext = pokerContext;
            _pokerRepo = PokerRepo;
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
            var sessions = _pokerRepo.UpdateBalances();
            Console.WriteLine(u.RealName + " has requested their balance."); 
            if (sessions == null)
            {
                string gameOnText = "There is a game going on so your balance may not be accurate.";
                _slackClient.PostAPIMessage(
                    text: gameOnText,
                    channel: userID
                );
            } 
            var balances = _pokerRepo.GetUserBalances();
            try
            {
                foreach (Session s in sessions)
                {
                    var b = balances.Where(b => b.UserID == s.UserID).FirstOrDefault();
                    var text2 = "Poker Session Total: " + s.Chips.ToString() + " Balance: " + b.Balance;
                    _slackClient.PostAPIMessage(
                        text: text2,
                        channel: s.User.SlackID
                    );
                }
            }
            catch
            {

            }
            string text = "";
            try
            {
                text = "Your balance is: " + balance.Balance;
            }
            catch
            {
                text = "You have never played in a cash game.";
            }
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

            string prevBalance = _pokerContext.UserBalance.Where(b => b.UserID == Payer.ID).Select(b => b.Balance).FirstOrDefault();

            Payment payment = new Payment();
            payment.Chips = chipint;
            payment.DateRequested = DateTime.Now;
            payment.Payee = Payee;
            payment.Payer = Payer;
            _pokerContext.Payment.Add(payment);
            _pokerContext.SaveChanges();

            string curBalance = _pokerContext.UserBalance.Where(b => b.UserID == Payer.ID).Select(b => b.Balance).FirstOrDefault();

            string chipsMoney = "$" + string.Format("{0:#.00}", Convert.ToDecimal(chipint) / 100 );

            //slack to payer
            string payerMsg = $"Please pay <@{Payee.SlackID}> {chipsMoney}.  When the payment is sent do: ```/paid <@{Payee.SlackID}> [method of payment]```";
            _slackClient.PostAPIMessage(payerMsg, null, payerID);

            payerMsg = "Previous balance: " + prevBalance + ".  Current Balance: " + curBalance;
            _slackClient.PostAPIMessage(payerMsg, null, payerID);

            //slack to payee
            string payeeMsg = $"<@{Payer.SlackID}> has been requested to pay you {chipsMoney}.  You will be notified when the payment is sent.";
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
                string msg = $"There is no existing payment between you and <@{payee.SlackID}>.";
                _slackClient.PostAPIMessage(msg, null, userID);
                return new EmptyResult();
            }
            existingPayment.Sent = DateTime.Now;
            _pokerContext.SaveChanges();

            string payeeMsg = $"<@{payer.SlackID}> has sent you a payment via: " + notes;
            _slackClient.PostAPIMessage(payeeMsg, null, to);
            payeeMsg = $"Confirm payment has been received with /confirm <@{payer.SlackID}>";
            _slackClient.PostAPIMessage(payeeMsg, null, to);

            string payerMsg = toName + $"<@{payee.SlackID}> has been notified of your payment.  You will be notified when <@{payee.SlackID}> confirms the payment.";
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
                _slackClient.PostAPIMessage($"No open payment found from <@{payer.SlackID}> to you.",null, userID);
                return new EmptyResult();
            }
            payment.Confirmed = DateTime.Now;
            _pokerContext.SaveChanges();
            string fromMessage = userName + $"<@{payee.SlackID}> confirmed receipt of your payment.";
            _slackClient.PostAPIMessage(fromMessage, null, fromID);

            string toMessage = "Payment receipt confirmed, thank you.";
            _slackClient.PostAPIMessage(toMessage, null, userID);
            return new EmptyResult();
            
        }
        public IActionResult ChangePassword(IFormCollection Form)
        {
            string userID = Form["user_id"];
            string userName = Form["user_name"];
            string text = Form["Text"];
            bool success = _pokerRepo.ChangePassword(userID, text);
            string message;
            if(success)
            {
                message = "Password updated.";
            }
            else
            {
                message = "Failed to update password, user not found maybe? ";
            }
            _slackClient.PostAPIMessage(message, null, userID);
            return new EmptyResult();
        }
        public IActionResult Register(IFormCollection Form)
        {
            
            //params (optional): username, realname
            string userID = Form["user_id"];
            string userName = Form["user_name"];
            User u = _pokerContext.User.Where(u => u.SlackID.Equals(userID)).FirstOrDefault();
            if (u != null)
            {
                //account already exists.
                string msg = "Account already exists.  UserName: " + u.UserName;
                _slackClient.PostAPIMessage(msg, null, userID);
                return new EmptyResult();
            }

            string Player = "";
            string RealName = "";
            
            string text = Form["Text"];
            if (!string.IsNullOrEmpty(text))
            {
                string[] parameters = text.Split(' ');
                Player = parameters[0];
                for (int i = 1; i < parameters.Length; i++)
                {
                    RealName += parameters[i];
                }
            }
            if(Player.Length < 3 || Player.Length > 12)
            {
                string errmsg = $"{Player} is too long or too short. Trying something with more than 3 character or less than 12.";
                _slackClient.PostAPIMessage(errmsg, null, userID);
                return new EmptyResult();
            }
            string Location = "The internet";
            string Email = "not@real.address";
            if(string.IsNullOrEmpty(Player))
            {
                Player = userName;
            }
            if(string.IsNullOrEmpty(RealName))
            {
                RealName = userName;
            }
            var response =  _pokerRepo.CreateNewUser(userID, Player, RealName, Location, Email);
            string message = "Poker account created!  UserName: " + Player + ". Password: password";
            _slackClient.PostAPIMessage(message, null, userID);
            message = "Please change your password using /changepw.";
            _slackClient.PostAPIMessage(message, null, userID);
            return new EmptyResult();
        }
    }
}