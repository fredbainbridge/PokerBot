using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerMavensAPI
{
    /// <summary>   This command returns the current balance of all house accounts plus the sum total of all player accounts. A House parameter is also returned with the total of all house accounts (not including the Players account). There are no calling parameters for this command.   </summary>
    public class SystemBalance
    {
        /// <summary>  Used to get Result from JSON reply</summary>
        public string Result { get; set; }

        /// <summary>  Used to get Master Account from JSON reply</summary>
        public string Master { get; set; }

        /// <summary>  Used to get Ring Account from JSON reply</summary>
        public string Ring { get; set; }

        /// <summary>  Used to get Rake Account from JSON reply</summary>
        public string Rake { get; set; }

        /// <summary>  Used to get Tourney Account from JSON reply</summary>
        public string Tourney { get; set; }

        /// <summary>  Used to get EntryFee Account from JSON reply</summary>
        public string EntryFee { get; set; }

        /// <summary>  Used to get House Accounts from JSON reply</summary>
        public string House { get; set; }

        /// <summary>  Used to get Sum of Player Accounts from JSON reply</summary>
        public string Players { get; set; }


    }
}
