using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerBot.Models
{
    public class vHand
    {
        public string Number { get; set; }
        public string Winner { get; set; }
        public int Amount { get; set; }
        public string Data { get; set; }
        public DateTime? Date { get; set; }

    }
}
