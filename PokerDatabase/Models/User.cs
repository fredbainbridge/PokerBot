using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokerBot.Models
{
    public class User
    {
        public int ID { get; set; }
        public string SlackID { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string RealName { get; set; }
        public int AvatarIndex { get; set; }
        public string AvatarHash { get; set; }
        public string SlackUserName { get; set; }
        
        [InverseProperty("User")]
        public virtual ICollection<Session> Sessions { get; set; } 

        [InverseProperty("Payee")]
        public virtual ICollection<Payment> Payees { get; set; }

        [InverseProperty("Payer")]
        public virtual ICollection<Payment> Payers { get; set; }

        [InverseProperty("Winner")]
        public virtual ICollection<Hand> Winners { get; set; }
    }
}
