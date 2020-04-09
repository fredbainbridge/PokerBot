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
    public void SetAvatarPath(string name, string path)
    {
        var client = new MaevenClient<AccountsEdit>(_secrets.PokerURL(), _secrets.Password());
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Command", "AccountsEdit");
        dict.Add("Player", name);
        dict.Add("AvatarFile", path);
        dict.Add("Avatar", "0");
        client.Post(dict);

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
    public bool isMainGame(string tableName)
    {
        foreach(string mainGamename in _secrets.GameName())
        {
            if(tableName.StartsWith(mainGamename))
            {
                return true;
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
            if(s.StartsWith("Table: "))
            {
                hand.TableName = s.Substring(7);
            }

            if (s.Contains("Game: "))
            {
                int i = s.IndexOf("Game: ");
                string TableName = s.Substring(i + 6).Trim();
                hand.Game = TableName;
            }

            if (s.Contains("** Summary **"))
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
                hand.Number = HandID;
            }

            if (s.Contains(" wins Pot (") || s.Contains(" wins Main Pot ("))
            { //winner declaration
              //"Fred wins Pot (40)"
                int index1 = s.IndexOf(" wins ");
                string player = s.Substring(0, index1);
                hand.Winner = _pokerContext.User.Where(u => u.UserName.Equals(player)).FirstOrDefault();

                if(s.LastIndexOf(" wins Pot (") == -1)
                {
                    index1 = s.LastIndexOf(" wins Main Pot (") + 16;
                }
                else
                {
                    index1 = s.LastIndexOf(" wins Pot (") + 11;
                }
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
    public List<Session> UpdateBalances()
    {
        List<Session> sessions = new List<Session>();
        List<RingGamesGet> tables = GetTable();
        bool gameOn = false;
        gameOn = AnySeatedPlayers(); 
        if (gameOn)
        {
            Console.WriteLine("A game is happening, balance changes will not be recorded.");
            return null;
        }
        
        DateTime now = DateTime.Now;
        AccountsList accountList = GetAccounts();
        for (int i = 0; i < accountList.Accounts; i++)
        {
            if (accountList.Balance[i] != "100000")
            {
                User u = _pokerContext.User.Where(u => u.UserName.Equals(accountList.Player[i])).FirstOrDefault();
                Console.WriteLine("Recording session for " + u.RealName);
                int balance = Int32.Parse(accountList.Balance[i]);
                int chips = balance - 100000;
                Session s = new Session();
                s.Chips = chips;
                s.Date = now;
                s.User = u;

                //s.Name = accountList.RealName[i];
                sessions.Add(s);

                _pokerContext.Sessions.Add(s);
                _pokerContext.SaveChanges();
                SetPrimaryBalance(accountList.Player[i], 100000);

            }
        }
        return sessions;
    }
    public bool IsHOF(string number)
    {
        var hands = GetHands(null, 10000, null)
            .Where(h => !_secrets.HOFExclusions().Contains(h.TableName))
            .OrderByDescending(h => h.Amount)
            .Take(20);
        foreach(var h in hands)
        {
            if (h.Number.Equals(number))
            {
                return true;
            }
        }
        return false;
    }
    public List<vHand> AddArtToHands(List<vHand> hands) {
        string url = "/media/";
        List<string> ranks = new List<string> { "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" };
        List<string> suits = new List<string> { "s", "d", "c", "h" };
            
        foreach(var h in hands)
        {
            var lines = h.Data.Split("\n");
            string newData = "";
            int counter = 0;
            foreach(var l in lines)
            {
                counter++;
                if(counter < 5)
                {
                    continue;
                }
                string line = l;
                if(line.Contains("** Flop ** [") ||
                    line.Contains("** Turn ** [") ||
                    line.Contains("** River ** [") ||
                    line.Contains("** Pot Show Down ** [") ||
                    line.Contains(" shows ["))
                {
                    foreach (string r in ranks)
                    {
                        foreach (string s in suits)
                        {
                            int pos1 = line.IndexOf('[');
                            int pos2 = line.IndexOf(']');
                            string cards = line.Substring(pos1+1, pos2 - pos1-1);
                            string newCards = cards;
                            string artUrl = url + r + s + ".png";
                            artUrl = "<img src = \"" + artUrl + "\" width=\"42\" height=\"59\">";
                            newCards = newCards.Replace(r + s, artUrl);
                            line = line.Replace(cards, newCards);
                            
                        }
                    }
                    line = line.Replace("]", "").Replace("[", "");
                    line = line.Replace(" **", " **<br/>");
                    line = line.Replace("shows ", "shows <br/>");
                    if (line.Contains(" shows"))
                    {
                        line = line.Replace("(", "<br/>(");
                    }

                }
                
                line = line + "<br/>";
                newData = newData + line;
            }
            h.Data = newData;
        }
         
        return hands;
    }
}