using PokerBot.Repository;
using PokerBot.Models;
using PokerMavensAPI;
using System.Collections.Generic;
using System.Linq;
using System;
using PokerBot.Repository.Mavens;
using Microsoft.Extensions.Logging;

public class PokerRepository : IPokerRepository {
    private ISecrets _secrets;
    private PokerDBContext _pokerContext;
    private IMavenRingGamesPlaying _mavenRingGamesPlaying;
    private IMavenRingGamesList _mavenRingGamesList;
    private IMavenRingGamesGet _mavenRingGamesGet;
    private IMavenRingGamesMessage _mavenRingGamesMessage;
    private IMavenTournamentsPlaying _mavenTournamentsPlaying;
    private IMavenTournamentsWaiting _mavenTournamentsWaiting;
    private IMavenAccountsEdit _mavenAccountsEdit;
    private IMavenAccountsList _mavenAccountsList;
    private IMavenLogsHandHistory _mavenLogsHandHistory;
    private readonly ILogger<PokerRepository> _logger;

    public PokerRepository(
        ISecrets Secrets, 
        PokerDBContext PokerContext, 
        IMavenRingGamesPlaying mavenRingGamesPlaying,
        IMavenRingGamesList mavenRingGamesList,
        IMavenRingGamesGet mavenRingGamesGet,
        IMavenRingGamesMessage mavenRingGamesMessage,
        IMavenTournamentsPlaying mavenTournamentsPlaying, 
        IMavenTournamentsWaiting mavenTournamentsWaiting, 
        IMavenAccountsEdit mavenAccountsEdit,
        IMavenAccountsList mavenAccountsList,
        IMavenLogsHandHistory mavenLogsHandHistory,
        ILogger<PokerRepository> logger) {
        _secrets = Secrets;
        _pokerContext = PokerContext;
        _mavenRingGamesPlaying = mavenRingGamesPlaying;
        _mavenRingGamesList = mavenRingGamesList;
        _mavenRingGamesGet = mavenRingGamesGet;
        _mavenRingGamesMessage = mavenRingGamesMessage;
        _mavenTournamentsPlaying = mavenTournamentsPlaying;
        _mavenTournamentsWaiting = mavenTournamentsWaiting;
        _mavenAccountsEdit = mavenAccountsEdit;
        _mavenAccountsList = mavenAccountsList;
        _mavenLogsHandHistory = mavenLogsHandHistory;
        _logger = logger;
    }
    public bool AnySeatedOrWaitingPlayers()
    {
        if(_mavenRingGamesPlaying.AnySeatedRingGamePlayers())
        {
            return true;
        }
        if(_mavenTournamentsPlaying.AnySeatedTournamentPlayers())
        {
            return true;
        }
        if(_mavenTournamentsWaiting.AnyWaitingTournamentPlayers())
        {
            return true;
        }
        return false;
    }
    
    public List<UserBalance> GetUserBalances()
    {
        return _pokerContext.UserBalance.ToList();
    }
    public List<vSession> GetSessions(int? Top = null)
    {
        return _pokerContext.vSession.Take(Top.HasValue ? Top.Value : int.MaxValue).ToList();
    }
    
    public List<Player> GetSeatedPlayers(string TableName, string Type = "RingGame") {
        List<Player> players = new List<Player>();
        if(Type.Equals("RingGame"))
        {
            players.AddRange(_mavenRingGamesPlaying.GetSeatedPlayers(TableName));
        }
        if(Type.Equals("Tournament"))
        {
            players.AddRange(_mavenTournamentsPlaying.GetSeatedPlayers(TableName));
        }
        return players;
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
        LogsHandHistory request = _mavenLogsHandHistory.GetHistory(HandID);
        if(request.Result.Equals("Error")) {
            _logger.LogError(request.Error);
            return  null;
        }
        //"Hand #10001-1 - 2019-12-09 21:05:05",

        Hand hand = new Hand();
        Dictionary<string, int> winners = new Dictionary<string, int>();
            
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
            //wins Side Pot
            //wins Main Pot
            //wins Pot
            
            if (s.Contains(" wins Pot (") || s.Contains(" wins Main Pot (") || s.Contains(" wins Side Pot ") )
            { //winner declaration
              //"Fred wins Pot (40)"
                int index1 = s.IndexOf(" wins ");
                string player = s.Substring(0, index1);
                //hand.Winner = _pokerContext.User.Where(u => u.UserName.Equals(player)).FirstOrDefault();

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
                    //hand.WinningAmount = winningAmountInt;
                    if(winners.Keys.Contains(player)) {
                        winners[player] = winners[player] + winningAmountInt;
                    }
                    else {
                        winners.Add(player, winningAmountInt);
                    }
                }   
            }
        }
        var totalWinner = winners.OrderByDescending(w => w.Value).FirstOrDefault();
        hand.Winner = _pokerContext.User.Where(u => u.UserName.Equals(totalWinner.Key)).FirstOrDefault();
        hand.WinningAmount = totalWinner.Value; 
        _pokerContext.Hand.Add(hand);
        _pokerContext.SaveChanges();
        return hand;
    }
    public List<RingGamesGet> GetTable(string TableName = null) {
        RingGamesList list = new RingGamesList();
        if (string.IsNullOrEmpty(TableName))
        {
            list = _mavenRingGamesList.RingGames();
        }
        else
        {
            list.Name = new List<string>();
            list.Name.Add(TableName);
        }

        List<RingGamesGet> ringGames = new List<RingGamesGet>();
        foreach(var tableName in list.Name)
        {
            ringGames.Add(_mavenRingGamesGet.GetRingGame(tableName));
        }
        return ringGames;
    }
    public void SendMessageToAllRingGames(string message)
    {
        var tables = GetTable();
        foreach (var t in tables)
        {
            _logger.LogInformation("RingGame Admin Message: " + message);
            SendAdminMessage(message, t.Name);
        }
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
        string remainingSeatsMsg = "There are " + remaining.ToString() + " seats remaining. ";
        if(remaining == 0) {
            remainingSeatsMsg = "The table is full! ";
        }
        else if(remaining == 1) {
            remainingSeatsMsg = "There is one seat remaining! ";
        }
        return remainingSeatsMsg;
    }

    public void SendAdminMessage(string Message, string TableName)
    {
        _mavenRingGamesMessage.SendAdminMessage(Message, TableName);

    }
    public List<vHand> GetHands(string handID = null, int minSize = 0, string winner = null, string tableName = null) 
    {
        IQueryable<vHand> query = _pokerContext.vHand;

        if(!string.IsNullOrEmpty(winner))
        {
            query = query.Where(h => h.Winner.ToUpper().Equals(winner.ToUpper()));   
        }
        if(!string.IsNullOrEmpty(handID))
        {
            query = query.Where(h => h.Number.Equals(handID));
        }
        if(minSize != 0)
        {
            query = query.Where(h => h.Amount >= minSize);
        }
        if(!string.IsNullOrEmpty(tableName)) {
            string gameType = tableName.Split("(").First().Trim();
            query = query.Where(h => h.TableName != null && h.TableName.StartsWith(gameType));
        }
        return query.OrderByDescending(h => h.Date).Take(1000).ToList();
    }
    public List<Session> UpdateBalances()
    {
        List<Session> sessions = new List<Session>();
        try
        {
            List<RingGamesGet> tables = GetTable();
        }
        catch
        {
            _logger.LogInformation("Waiting for server to start");
            return null;
        }
        bool gameOn = false;
        gameOn = AnySeatedOrWaitingPlayers(); 
        if (gameOn)
        {
            _logger.LogInformation("A game is happening, balance changes will not be recorded.");
            return null;
        }
        
        DateTime now = DateTime.Now;
        AccountsList accountList = _mavenAccountsList.GetAccounts();
        for (int i = 0; i < accountList.Accounts; i++)
        {
            if (accountList.Balance[i] != "100000")
            {
                User u = _pokerContext.User.Where(u => u.UserName.Equals(accountList.Player[i])).FirstOrDefault();
                _logger.LogInformation("Recording session for " + u.RealName);
                bool success = false;
                int balance;
                success = Int32.TryParse(accountList.Balance[i], out balance);
                if(!success)
                {
                    _logger.LogWarning("Non integer balance found, is the smallest chip size set correctly on the table?");
                    continue;
                }
                int chips = balance - 100000;
                Session s = new Session();
                s.Chips = chips;
                s.Date = now;
                s.User = u;

                //s.Name = accountList.RealName[i];
                sessions.Add(s);

                _pokerContext.Sessions.Add(s);
                _pokerContext.SaveChanges();
                _mavenAccountsEdit.SetPrimaryBalance(accountList.Player[i], 100000);

            }
        }
        return sessions;
    }
    public bool IsHOF(Hand hand)
    {
        var hands = GetHands(null, 10000, null, hand.TableName)
            .OrderByDescending(h => h.Amount)
            .Take(20);
        foreach(var h in hands)
        {
            if (h.Number.Equals(hand.Number))
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
    public string GetNextHand(string Number) 
    {
        var hand = _pokerContext.Hand.Where(h => h.Number.Equals(Number)).FirstOrDefault() ;
        if(hand != null)
        {
            var nextHand = _pokerContext.Hand.Where(h => h.ID == hand.ID + 1).FirstOrDefault();
            if (nextHand != null)
            {
                return nextHand.Number;
            }
        }
        return "na";
    }
    public string GetPreviousHand(string Number)
    {
        var hand = _pokerContext.Hand.Where(h => h.Number.Equals(Number)).FirstOrDefault();
        if (hand != null)
        {
            var prevHand = _pokerContext.Hand.Where(h => h.ID == hand.ID - 1).FirstOrDefault();
            if (prevHand != null)
            {
                return prevHand.Number;
            }
            return prevHand.Number;
        }
        return "na";
    }
}