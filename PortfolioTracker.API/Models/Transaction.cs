namespace PortfolioTracker.API.Models;

// This minimal persistence relationship supports the Account deletion rule.
// Transaction behavior and API endpoints are intentionally not implemented yet.
public class Transaction
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public Account Account { get; set; } = null!;
}
