using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace PokerBot.Models
{
    public class PokerDBContext : DbContext
    {
        public PokerDBContext(DbContextOptions<PokerDBContext> options) : base(options)
        {

        }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserBalance> UserBalance {get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<vSession> vSession { get; set; }
        public virtual DbSet<Hand> Hand { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBalance>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("v_Balances");
            });
            modelBuilder.Entity<vSession>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("v_Sessions");
            });
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(u => u.Payee).WithMany(user => user.Payees).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(u => u.Payer).WithMany(user => user.Payers).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Hand>(entity =>
            {
                entity.HasOne(h => h.Winner).WithMany(user => user.Winners).OnDelete(DeleteBehavior.NoAction);
            });
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
    }
}
