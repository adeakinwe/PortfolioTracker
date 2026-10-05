using System.ComponentModel.DataAnnotations;

namespace PortfolioTracker.API.DTOs.Accounts;

public class UpdateAccountRequest
{
    [Required]
    public string? Name { get; init; }

    [Required]
    public string? AccountType { get; init; }
}
