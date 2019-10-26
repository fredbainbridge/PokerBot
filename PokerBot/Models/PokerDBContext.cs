using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace PokerBot.Models
{
    public class PokerDBContext : DbContext
    {
        public PokerDBContext(DbContextOptions<PokerDBContext> options) : base(options)
        {
                
        }
        public DbSet<Session> Sessions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
    }
}
