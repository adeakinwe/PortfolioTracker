namespace PortfolioTracker.API.DTOs.Accounts;

public record AccountResponse(int Id, string Name, string AccountType, DateTime CreatedAt);
