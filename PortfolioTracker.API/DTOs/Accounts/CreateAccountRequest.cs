using System.ComponentModel.DataAnnotations;

namespace PortfolioTracker.API.DTOs.Accounts;

public class CreateAccountRequest
{
    [Required]
    public string? Name { get; init; }

    [Required]
    public string? AccountType { get; init; }
}
