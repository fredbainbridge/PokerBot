using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerBot.Models
{
    public class UserBalance
    {
        public int UserID { get; set; }
        public string RealName { get; set; }
        public string Balance { get; set; }

        public int Priority { get; set; }
        public string SlackID { get; set; }
    }
}
