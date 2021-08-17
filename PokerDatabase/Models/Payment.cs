using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokerBot.Models
{
    public class Payment
    {
        public int ID { get; set; }
        public int PayeeID { get; set; }
        public int PayerID { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime? Sent { get; set; }
        public DateTime? Confirmed { get; set; }
        public int Chips { get; set; }

        [ForeignKey("PayeeID")]
        [InverseProperty("Payees")]
        public virtual User Payee{ get; set; }

        [ForeignKey("PayerID")]
        [InverseProperty("Payers")]
        public virtual User Payer { get; set; }
    }
}
