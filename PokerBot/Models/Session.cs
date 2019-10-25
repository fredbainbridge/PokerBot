using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerBot.Models
{
    public class Session
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Chips { get; set; }
        public DateTime Date { get; set; }
    }
}
