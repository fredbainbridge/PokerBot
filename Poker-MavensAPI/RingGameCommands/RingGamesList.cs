using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PokerMavensAPI
{
    /// <summary>   retrieves the list of all ring games. Calling this with no parameters will just return the number of ring games. Set the Fields parameter to a comma separated list of field names that you want returned. Do not include spaces in the list. You may choose any combination of these fields: Name, Status, Description, Auto, Game, MixedList, PW, Private, PermPlay, PermObserve, PermPlayerChat, PermObserverChat, SuspendChatAllIn, Seats, SmallestChip, BuyInMin, BuyInMax, BuyInDef, RakePercent, RakeCap, TurnClock, TimeBank, BankReset, DisProtect, SmallBlind, BigBlind, AllowStraddle, SmallBet, BigBet, Ante, AnteAll, BringIn, DupeIPs, RatholeMinutes, SitoutMinutes, SitoutRelaxed  </summary>
    public class RingGamesList
    {
        /// <summary>  Used to get Result from JSON reply</summary>
        public string Result { get; set; }

        /// <summary>  get the JSON response for Result  </summary>
        public int RingGames { get; set; }
        public List<string> Name { get; set; }
        
    }
}
