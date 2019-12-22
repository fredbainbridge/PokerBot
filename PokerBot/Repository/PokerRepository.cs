using PokerBot.Repository;
using PokerBot.Models;
using PokerMavensAPI;
using System.Collections.Generic;
using System.Linq;
using System;

public class PokerRepository : IPokerRepository {
    private ISecrets _secrets;
    private PokerDBContext _pokerContext;
    public PokerRepository(ISecrets Secrets, PokerDBContext PokerContext) {
        _secrets = Secrets;
        _pokerContext = PokerContext;
    }
    public bool ChangePassword(string SlackID, string password)
    {
        var client = new MaevenClient<AccountsEdit>(_secrets.PokerURL(), _secrets.Password());
        User u = _pokerContext.User.Where(u => u.SlackID.Equals(SlackID)).FirstOrDefault();
        if(u == null)
        {
            return false; 
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Command", "AccountsEdit");
        dict.Add("Player", u.UserName);
        dict.Add("PW", password);
        var response = client.Post(dict);
        return true;
    }
    public User CreateNewUser(string SlackID, string Player, string RealName, string Location, string Email)
    {
        var client = new MaevenClient<AccountsAdd>(_secrets.PokerURL(), _secrets.Password());
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Command", "AccountsAdd");
        dict.Add("Player", Player);
        dict.Add("RealName", RealName);
        dict.Add("PW", "password");
        dict.Add("Location", Location);
        dict.Add("Email", Email);
        var response = client.Post(dict);

        User u = new User();
        u.EmailAddress = Email;
        u.RealName = RealName;
        u.SlackID = SlackID;
        u.UserName = Player;
        _pokerContext.User.Add(u);
        _pokerContext.SaveChanges();

        
        return u;

    }
    public List<UserBalance> GetUserBalances()
    {
        return _pokerContext.UserBalance.ToList();
    }
    public List<vSession> GetSessions(int? Top = null)
    {
        return _pokerContext.vSession.Take(Top.HasValue ? Top.Value : int.MaxValue).ToList();
    }
    public void SetPrimaryBalance(string Name, int Balance)
    {
        var client = new MaevenClient<AccountsEdit>(_secrets.PokerURL(), _secrets.Password());
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Command", "AccountsEdit");
        dict.Add("Player", Name);
        dict.Add("Balance", Balance.ToString());
        client.Post(dict);

    }
    public AccountsList GetAccounts()
    {
        var client = new MaevenClient<AccountsList>(_secrets.PokerURL(), _secrets.Password());
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Command", "AccountsList");
        dict.Add("Fields", "RealName,Balance,Player");
        return client.Post(dict);
    }
    public List<Player> GetSeatedPlayers(string TableName) {
        var client = new MaevenClient<RingGamesPlaying>(_secrets.PokerURL(), _secrets.Password());
        Dictionary<string, string> dict = new Dictionary<string,string>();
        dict.Add("Command", "RingGamesPlaying");
        dict.Add("Name", TableName);
        var request = client.Post(dict);
        List<Player> players = new List<Player>();
        for(int i = 0; i<request.Count; i++) {
            Player p = new Player();
            p.Name = request.Player[i];
            p.Chips = request.Chips[i];
            players.Add(p);
        }
        return players;
    }
    public bool AnySeatedPlayers()
    {
        var client = new MaevenClient<TournamentsList>(_secrets.PokerURL(), _secrets.Password());
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Command", "TournamentsList");
        dict.Add("Fields", "Name");
        var request = client.Post(dict);
        if(request.Name != null)
        {
            for (int i = 0; i < request.Name.Count(); i++)
            {
                var tClient = new MaevenClient<TournamentsPlaying>(_secrets.PokerURL(), _secrets.Password());
                dict = new Dictionary<string, string>();
                dict.Add("Command", "TournamentsPlaying");
                dict.Add("Name", request.Name[i]);
                var tRequest = client.Post(dict);

                if(tRequest.Name != null)
                {
                    return true;
                }
            }
        }
        
        var rClient = new MaevenClient<RingGamesList>(_secrets.PokerURL(), _secrets.Password());
        dict = new Dictionary<string, string>();
        dict.Add("Command", "RingGamesList");
        dict.Add("Fields", "Name");
        request = client.Post(dict);
        if(request.Name != null)
        {
            for (int i = 0; i < request.Name.Count(); i++)
            {
                if (GetSeatedPlayers(request.Name[i]).Count() > 0)
                {
                    return true;
                }
            }
        }          
        return false;
    }
    public Hand GetHandHistory(string HandID) {
        var client = new MaevenClient<LogsHandHistory>(_secrets.PokerURL(), _secrets.Password());
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Command", "LogsHandHistory");
        dict.Add("Hand", HandID);
        var request = client.Post(dict);
        //"Hand #10001-1 - 2019-12-09 21:05:05",

        Hand hand = new Hand();
        
        foreach(string s in request.Data)
        {
            if(s.Contains("** Summary **"))
            {
                break;
            }
            hand.Data += s + "\n";
            string s2;
            if(s.StartsWith("Hand #"))
            {
                
                int i = s.IndexOf('-');
                s2 = s.Substring(i + 1, s.Length - i - 1);
                i = s2.IndexOf('-');
                s2 = s2.Substring(i + 1, s2.Length - i - 1);
                s2 = s2.Trim();

                hand.Date = DateTime.Parse(s2);
                
                s2 = s.Replace("Hand #","");
                int index = s2.IndexOf('-') + 3;
                hand.Number = s2.Substring(0, index).Trim();
            }

            if (s.Contains(" wins Pot ("))
            { //winner declaration
              //"Fred wins Pot (40)"
                int index1 = s.IndexOf(" wins Pot (");
                string player = s.Substring(0, index1);
                hand.Winner = _pokerContext.User.Where(u => u.UserName.Equals(player)).FirstOrDefault();

                index1 = s.LastIndexOf(" wins Pot (") + 11;
                int index2 = s.LastIndexOf(")"); ;
                string winningAmountString = s.Substring(index1, index2 - index1);
                
                //"Seat 5: Fred (+20) [2d 3h] Won without Showdown"
                int winningAmountInt;
                bool success = Int32.TryParse(winningAmountString, out winningAmountInt);
                if (success)
                {
                    hand.WinningAmount = winningAmountInt;
                }
            }
        }
        _pokerContext.Hand.Add(hand);
        _pokerContext.SaveChanges();
        return hand;
    }
    public List<RingGamesGet> GetTable(string TableName = null) {
        RingGamesList list = new RingGamesList();
        if(string.IsNullOrEmpty(TableName)) {  //get all ring games
            var clientGet = new MaevenClient<RingGamesList>(_secrets.PokerURL(), _secrets.Password());
            Dictionary<string, string> dictGet = new Dictionary<string, string>();
            dictGet.Add("Command", "RingGamesList"); 
            dictGet.Add("Fields", "Name");
            list = clientGet.Post(dictGet);
        }
        else
        {
            list.Name = new List<string>();
            list.Name.Add(TableName);
        }

        var client = new MaevenClient<RingGamesGet>(_secrets.PokerURL(), _secrets.Password());
        List<RingGamesGet> ringGames = new List<RingGamesGet>();
        foreach(var l in list.Name)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Command", "RingGamesGet");
            dict.Add("Name", l);
            ringGames.Add(client.Post(dict));
        }
        return ringGames;
        
    }
    public int OpenSeats(string TableName) {
        string maxPlayers = GetTable(TableName).FirstOrDefault().Seats;
        int intMaxPlayers;
        System.Int32.TryParse(maxPlayers, out intMaxPlayers);
        int totalPlayers = GetSeatedPlayers(TableName).Count;        
        return intMaxPlayers - totalPlayers;
    }
    public string RemainingSeatsMessage(string TableName) {
        int remaining = OpenSeats(TableName);
        string remainingSeatsMsg = "";
        if(remaining == 0) {
            remainingSeatsMsg = "The table is full! ";
        }
        else if(remaining == 1) {
            remainingSeatsMsg = "There is one seat remaining! ";
        }
        else {
            remainingSeatsMsg = "There are " + remaining.ToString() + " seats remaining. ";
        }
        return remainingSeatsMsg;
    }

    public void SendAdminMessage(string Message, string TableName)
    {
        var client = new MaevenClient<RingGamesMessage>(_secrets.PokerURL(), _secrets.Password());
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Command", "RingGamesMessage");
        dict.Add("Name", TableName);
        dict.Add("Message", Message);
        client.Post(dict);

    }
    public List<vHand> GetHands(string handID = null, int minSize = 0, string winner = null) 
    {
        
        if(!string.IsNullOrEmpty(winner))
        {   
            if(minSize != 0 )
            {
                return _pokerContext.vHand.Where(h => h.Amount >= minSize && h.Winner.Equals(winner)).OrderByDescending(h => h.Number).ToList();
            }
            else
            {
                return _pokerContext.vHand.Where(h => h.Winner.ToUpper().Equals(winner.ToUpper())).OrderByDescending(h => h.Number).ToList();
            }
        }
        if(!string.IsNullOrEmpty(handID))
        {
            return _pokerContext.vHand.Where(h => h.Number.Equals(handID)).OrderByDescending(h => h.Number).ToList();
        }
        if(minSize != 0)
        {
            return _pokerContext.vHand.Where(h => h.Amount >= minSize).OrderByDescending(h => h.Number).ToList();
        }
        return _pokerContext.vHand.OrderByDescending(h => h.Number).Take(1000).ToList();
    }
}