using PortfolioTracker.API.Models;

namespace PortfolioTracker.API.Repositories;

public interface IAccountRepository
{
    Task<IReadOnlyList<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string normalizedName, int? excludingAccountId = null);
    Task AddAsync(Account account);
    Task SaveChangesAsync();
    Task<bool> HasTransactionsAsync(int accountId);
    void Remove(Account account);
}
