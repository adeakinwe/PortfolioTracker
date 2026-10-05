using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.API.DTOs.Accounts;
using PortfolioTracker.API.Models;
using PortfolioTracker.API.Repositories;

namespace PortfolioTracker.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController(IAccountRepository accounts) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create(CreateAccountRequest request)
    {
        var name = request.Name!.Trim();
        var normalizedName = NormalizeName(name);

        if (await accounts.NameExistsAsync(normalizedName))
        {
            return Conflict(new { message = "An account with this name already exists." });
        }

        var account = new Account
        {
            Name = name,
            NormalizedName = normalizedName,
            AccountType = request.AccountType!.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await accounts.AddAsync(account);
        await accounts.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = account.Id }, ToResponse(account));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AccountResponse>>> GetAll() =>
        Ok((await accounts.GetAllAsync()).Select(ToResponse).ToList());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccountResponse>> GetById(int id)
    {
        var account = await accounts.GetByIdAsync(id);
        return account is null ? NotFound() : Ok(ToResponse(account));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AccountResponse>> Update(int id, UpdateAccountRequest request)
    {
        var account = await accounts.GetByIdAsync(id);
        if (account is null)
        {
            return NotFound();
        }

        var name = request.Name!.Trim();
        var normalizedName = NormalizeName(name);
        if (await accounts.NameExistsAsync(normalizedName, id))
        {
            return Conflict(new { message = "An account with this name already exists." });
        }

        account.Name = name;
        account.NormalizedName = normalizedName;
        account.AccountType = request.AccountType!.Trim();
        await accounts.SaveChangesAsync();

        return Ok(ToResponse(account));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var account = await accounts.GetByIdAsync(id);
        if (account is null)
        {
            return NotFound();
        }

        if (await accounts.HasTransactionsAsync(id))
        {
            return Conflict(new { message = "An account with recorded transactions cannot be deleted." });
        }

        accounts.Remove(account);
        await accounts.SaveChangesAsync();
        return NoContent();
    }

    private static string NormalizeName(string name) => name.Trim().ToUpperInvariant();

    private static AccountResponse ToResponse(Account account) =>
        new(account.Id, account.Name, account.AccountType, account.CreatedAt);
}
