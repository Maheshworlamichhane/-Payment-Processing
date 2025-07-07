using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure
{
    public class PaymentDbContext : IdentityDbContext 
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Transaction>(entity =>
        //    {
        //        entity.HasKey(t => t.TransactionID);

        //        entity.Property(t => t.Currency).IsRequired().HasMaxLength(10);
        //        entity.Property(t => t.Amount).IsRequired();
        //        entity.Property(t => t.Status).HasConversion<string>();

        //        entity.HasOne(t => t.User)
        //              .WithMany()  // or .WithMany(u => u.Transactions) if you add collection
        //              .HasForeignKey(t => t.UserId)
        //              .OnDelete(DeleteBehavior.Restrict);
        //    });

        //}
    }
}

