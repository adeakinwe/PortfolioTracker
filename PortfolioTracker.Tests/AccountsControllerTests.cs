using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.API.Controllers;
using PortfolioTracker.API.Data;
using PortfolioTracker.API.DTOs.Accounts;
using PortfolioTracker.API.Models;
using PortfolioTracker.API.Repositories;

namespace PortfolioTracker.Tests;

public class AccountsControllerTests
{
    [Fact]
    public async Task Create_WithValidRequest_CreatesTrimmedAccount()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Create(new CreateAccountRequest
        {
            Name = "  Main Brokerage  ",
            AccountType = "  Brokerage  "
        });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<AccountResponse>(created.Value);
        Assert.Equal("Main Brokerage", response.Name);
        Assert.Equal("Brokerage", response.AccountType);
        Assert.NotEqual(default, response.CreatedAt);
        Assert.Single(context.Accounts);
    }

    [Theory]
    [InlineData(null, "Brokerage")]
    [InlineData("   ", "Brokerage")]
    [InlineData("Main", null)]
    [InlineData("Main", "   ")]
    public void Create_WithMissingRequiredValue_IsInvalid(string? name, string? accountType)
    {
        var request = new CreateAccountRequest { Name = name, AccountType = accountType };
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        var valid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            request,
            new System.ComponentModel.DataAnnotations.ValidationContext(request),
            validationResults,
            validateAllProperties: true);

        Assert.False(valid);
    }

    [Fact]
    public async Task Create_WithNameMatchingTrimmedCaseInsensitiveName_ReturnsConflict()
    {
        await using var context = CreateContext();
        context.Accounts.Add(new Account
        {
            Name = "Main",
            NormalizedName = "MAIN",
            AccountType = "Brokerage",
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.Create(new CreateAccountRequest
        {
            Name = "  main  ",
            AccountType = "Savings"
        });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_AndGetById_ReturnAccounts_AndMissingAccountReturnsNotFound()
    {
        await using var context = CreateContext();
        var account = await AddAccount(context, "Main", "Brokerage");
        var controller = CreateController(context);

        var all = await controller.GetAll();
        var allResult = Assert.IsType<OkObjectResult>(all.Result);
        var accounts = Assert.IsAssignableFrom<IReadOnlyList<AccountResponse>>(allResult.Value);
        Assert.Single(accounts);

        var found = await controller.GetById(account.Id);
        var foundResult = Assert.IsType<OkObjectResult>(found.Result);
        Assert.Equal(account.Id, Assert.IsType<AccountResponse>(foundResult.Value).Id);

        var missing = await controller.GetById(999);
        Assert.IsType<NotFoundResult>(missing.Result);
    }

    [Fact]
    public async Task Update_ChangesAccount_AndRejectsMissingAccount()
    {
        await using var context = CreateContext();
        var account = await AddAccount(context, "Main", "Brokerage");
        var controller = CreateController(context);

        var updated = await controller.Update(account.Id, new UpdateAccountRequest
        {
            Name = "  Savings  ",
            AccountType = "  Savings "
        });

        var result = Assert.IsType<OkObjectResult>(updated.Result);
        var response = Assert.IsType<AccountResponse>(result.Value);
        Assert.Equal("Savings", response.Name);
        Assert.Equal("Savings", response.AccountType);
        Assert.Equal("SAVINGS", (await context.Accounts.FindAsync(account.Id))!.NormalizedName);

        var missing = await controller.Update(999, new UpdateAccountRequest { Name = "Other", AccountType = "Other" });
        Assert.IsType<NotFoundResult>(missing.Result);
    }

    [Fact]
    public async Task Delete_RemovesAccountWithoutTransactions_AndReturnsNotFoundForMissingAccount()
    {
        await using var context = CreateContext();
        var account = await AddAccount(context, "Main", "Brokerage");
        var controller = CreateController(context);

        var deleted = await controller.Delete(account.Id);
        Assert.IsType<NoContentResult>(deleted);
        Assert.Empty(context.Accounts);

        var missing = await controller.Delete(account.Id);
        Assert.IsType<NotFoundResult>(missing);
    }

    [Fact]
    public async Task Delete_WithRecordedTransaction_ReturnsConflict()
    {
        await using var context = CreateContext();
        var account = await AddAccount(context, "Main", "Brokerage");
        context.Transactions.Add(new Transaction { AccountId = account.Id });
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.Delete(account.Id);

        Assert.IsType<ConflictObjectResult>(result);
        Assert.NotNull(await context.Accounts.FindAsync(account.Id));
    }

    private static PortfolioTrackerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PortfolioTrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PortfolioTrackerDbContext(options);
    }

    private static AccountsController CreateController(PortfolioTrackerDbContext context) =>
        new(new AccountRepository(context));

    private static async Task<Account> AddAccount(PortfolioTrackerDbContext context, string name, string accountType)
    {
        var account = new Account
        {
            Name = name,
            NormalizedName = name.ToUpperInvariant(),
            AccountType = accountType,
            CreatedAt = DateTime.UtcNow
        };
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }
}
