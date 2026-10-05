using Microsoft.EntityFrameworkCore;
using PortfolioTracker.API.Data;
using PortfolioTracker.API.Models;

namespace PortfolioTracker.API.Repositories;

public class AccountRepository(PortfolioTrackerDbContext context) : IAccountRepository
{
    public async Task<IReadOnlyList<Account>> GetAllAsync() =>
        await context.Accounts.AsNoTracking().OrderBy(account => account.Id).ToListAsync();

    public Task<Account?> GetByIdAsync(int id) => context.Accounts.FindAsync(id).AsTask();

    public Task<bool> NameExistsAsync(string normalizedName, int? excludingAccountId = null) =>
        context.Accounts.AnyAsync(account =>
            account.NormalizedName == normalizedName &&
            (!excludingAccountId.HasValue || account.Id != excludingAccountId.Value));

    public Task AddAsync(Account account) => context.Accounts.AddAsync(account).AsTask();

    public Task SaveChangesAsync() => context.SaveChangesAsync();

    public Task<bool> HasTransactionsAsync(int accountId) =>
        context.Transactions.AnyAsync(transaction => transaction.AccountId == accountId);

    public void Remove(Account account) => context.Accounts.Remove(account);
}
