using Expense_Tracker.DTO.CategoryDtos;
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
        public DbSet<RecurringExpense> RecurringExpenses { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<CategorySpendingDto>().HasNoKey();

            // User-Expense relationship (1-to-many)
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)              // Expense belongs to User
                .WithMany(u => u.Expenses)        // User has many Expenses
                .HasForeignKey(e => e.UserId)     // FK property
                .OnDelete(DeleteBehavior.Cascade); // Delete user => delete their expenses

            // User-RecurringExpense relationship (1-to-many)
            modelBuilder.Entity<RecurringExpense>()
                .HasOne(re => re.User)               // RecurringExpense belongs to User
                .WithMany(u => u.RecurringExpenses)  // User has many RecurringExpenses
                .HasForeignKey(re => re.UserId)      // FK property
                .OnDelete(DeleteBehavior.Cascade);    // Delete user => delete their recurring expenses

            //// User-Categories relationship (1-to-many)
            //modelBuilder.Entity<Categories>()
            //    .HasOne(c => c.User)               // Category belongs to User
            //    .WithMany(u => u.Categories)       // User has many Categories
            //    .HasForeignKey(c => c.UserId)      // FK property
            //    .OnDelete(DeleteBehavior.Cascade);  // Delete user => delete their categories
            //expense -category relationship
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Category)              // Expense belongs to Category
                .WithMany()        // Category has many Expenses
                .HasForeignKey(e => e.CategoryId)     // FK property
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if referenced

            // Ensure Email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Categories>().HasData(
                new Categories { Id = 1, Name = "Food", Description = "Expenses for food,dining and groceries",CreatedAt = new DateTime(2025,10,20) },
                new Categories { Id = 2, Name = "Transportation", Description = "Expenses for transportation and travel", CreatedAt = new DateTime(2025, 10, 20) },
                new Categories { Id = 3, Name = "Utilities", Description = "Expenses for utilities like electricity, water, etc.", CreatedAt = new DateTime(2025, 10, 20) },
                new Categories { Id = 4, Name = "Entertainment", Description = "Expenses for entertainment and leisure activities", CreatedAt = new DateTime(2025, 10, 20) },
                new Categories { Id = 5, Name = "Healthcare", Description = "Expenses for medical and healthcare services", CreatedAt = new DateTime(2025, 10, 20) },
                new Categories { Id = 6, Name = "Education", Description = "Expenses for education and learning materials", CreatedAt = new DateTime(2025, 10, 20) },
                new Categories { Id = 7, Name = "Miscellaneous", Description = "Other expenses that do not fit into the above categories", CreatedAt = new DateTime(2025, 10, 20) }
            );
        }
    }
}
