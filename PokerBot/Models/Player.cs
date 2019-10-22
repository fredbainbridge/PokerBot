using System;
namespace PokerBot.Models {
    public class Player {
        public string Name{get; set;}
        public double Chips {get; set;}
        public int SeatNumber {get; set;}
        public DateTime TimeSeated { get; set; }
    }
}