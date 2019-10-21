using PokerBot.Repository;
using PokerBot.Models;
using PokerMavensAPI;
using System.Collections.Generic;
using System.Linq;

public class PokerRepository : IPokerRepository {
    private ISecrets _secrets;
    public PokerRepository(ISecrets Secrets) {
        _secrets = Secrets;
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
    public LogsHandHistory GetHandHistory(string HandID) {
        var client = new MaevenClient<LogsHandHistory>(_secrets.PokerURL(), _secrets.Password());
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Command", "LogsHandHistory");
        dict.Add("Hand", HandID);
        var request = client.Post(dict);
        return request;
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
}