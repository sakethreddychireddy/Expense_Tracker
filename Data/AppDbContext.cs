using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-Expense relationship (1-to-many)
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)              // Expense belongs to User
                .WithMany(u => u.Expenses)        // User has many Expenses
                .HasForeignKey(e => e.UserId)     // FK property
                .OnDelete(DeleteBehavior.Cascade); // Delete user => delete their expenses

            // Ensure Email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
