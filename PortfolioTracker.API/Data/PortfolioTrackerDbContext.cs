using Microsoft.EntityFrameworkCore;
using PortfolioTracker.API.Models;

namespace PortfolioTracker.API.Data;

public class PortfolioTrackerDbContext(DbContextOptions<PortfolioTrackerDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();

    // Used only to enforce the approved Account deletion restriction.
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasKey(account => account.Id);
            entity.Property(account => account.Name).HasMaxLength(200).IsRequired();
            entity.Property(account => account.NormalizedName).HasMaxLength(200).IsRequired();
            entity.HasIndex(account => account.NormalizedName).IsUnique();
            entity.Property(account => account.AccountType).HasMaxLength(100).IsRequired();
            entity.Property(account => account.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions");
            entity.HasKey(transaction => transaction.Id);
            entity.HasOne(transaction => transaction.Account)
                .WithMany(account => account.Transactions)
                .HasForeignKey(transaction => transaction.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
