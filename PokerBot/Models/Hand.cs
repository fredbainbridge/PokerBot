using System.ComponentModel.DataAnnotations.Schema;

namespace PokerBot.Models
{
    public class Hand
    {
        public Hand()
        {
            Data = "";
        }
        public int ID { get; set; }
        public string Number { get; set; }
        public int WinnerID { get; set; }

        [ForeignKey("WinnerID")]
        [InverseProperty("Winners")] 
        public User Winner { get; set; }
        public int WinningAmount { get; set; }
        public string Data { get; set; }
    }
}
