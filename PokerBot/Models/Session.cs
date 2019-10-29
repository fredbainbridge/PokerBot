using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokerBot.Models
{
    public class Session
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int Chips { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("UserID")]
        [InverseProperty("Sessions")]
        public virtual User User { get; set; }
    }
}
