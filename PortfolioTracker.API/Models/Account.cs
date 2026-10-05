namespace PortfolioTracker.API.Models;

public class Account
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string AccountType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
