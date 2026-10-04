# PortfolioTracker MVP Requirements

## Overview

PortfolioTracker is a lightweight personal finance and investment portfolio tracking Web API.

The MVP provides four core areas:

1. Accounts
2. Assets
3. Transactions
4. Portfolio Summary

The purpose of the MVP is to allow a user to record where money is held, record investment activity, and calculate a basic portfolio position from recorded transactions.

The application is intentionally designed as a single-user MVP. Authentication and multi-user support are outside the scope of this version.

The API will follow the project architecture defined in `AGENTS.md`:

* Controllers coordinate HTTP requests and responses.
* Repositories handle data access and persistence.
* Services contain business logic and calculations where required.
* Controllers must not access `DbContext` directly.
* DTOs are used for API request and response models.

Simple CRUD operations do not require a service layer unless there is a genuine business-logic reason to introduce one.

---

# MVP Scope

## In Scope

* Account management
* Asset management
* Recording BUY, SELL, DEPOSIT, and WITHDRAWAL transactions
* Basic portfolio holdings calculation
* Basic invested amount calculation
* Basic current portfolio value calculation
* Basic profit/loss calculation
* Manual maintenance of asset current prices

## Out of Scope

The following are explicitly excluded from the MVP:

* Authentication
* Authorization
* Multi-user or multi-tenant support
* External market-price APIs
* Currency conversion APIs
* Notifications or alerts
* AI features
* Angular, React, or any other frontend
* Docker
* Cloud deployment
* Audit trails
* Soft deletion
* Report generation
* Advanced portfolio accounting
* Tax calculations
* Dividend tracking
* Fees and commissions
* Realized/unrealized gain separation
* FIFO/LIFO cost-basis accounting
* Advanced performance metrics

These may be considered in later versions.

---

# 1. Accounts

## Purpose

Accounts represent places where money is held or investments are managed.

Examples include:

* Brokerage account
* Savings account
* Retirement account
* Bank account

An account does not represent an investment itself. Investments are represented by Assets and connected to accounts through Transactions.

## User Stories

### US-ACC-001: Create an Account

As a user, I want to create an account so that I can track where my money is held.

### US-ACC-002: View Accounts

As a user, I want to view my accounts so that I can see where my money is held.

### US-ACC-003: View an Account

As a user, I want to retrieve a specific account so that I can view its details.

### US-ACC-004: Update an Account

As a user, I want to update an account's details so that its information remains accurate.

### US-ACC-005: Delete an Account

As a user, I want to delete an unused account so that obsolete records can be removed.

## Functional Requirements

* An account shall have:

  * Identifier
  * Name
  * Account type
  * Creation timestamp
* Account name shall be required and must not be empty or whitespace.
* Account type shall be required.
* Account type shall be represented as a simple value such as `Brokerage`, `Savings`, `Retirement`, or `Other`.
* Account names shall be unique within the MVP. Names shall be normalized by
  trimming surrounding whitespace and compared case-insensitively.
* The API shall support:

  * Creating an account
  * Listing accounts
  * Retrieving an account by identifier
  * Updating an account
  * Deleting an account
* An account with recorded transactions shall not be deletable.
* Account deletion shall return a clear client error when transactions exist.

## Acceptance Criteria

* A valid account can be created.
* An account with an empty or whitespace-only name is rejected.
* An account without an account type is rejected.
* Accounts can be listed.
* An existing account can be retrieved by identifier.
* A nonexistent account returns HTTP 404.
* An existing account can be updated.
* An account with no transactions can be deleted.
* An account with transactions cannot be deleted.
* Deleting an account with transactions returns a clear HTTP 400 or 409 response.
* Account names that are equal after trimming surrounding whitespace and
  comparing case-insensitively are rejected.

---

# 2. Assets

## Purpose

Assets represent investment instruments such as stocks, bonds, ETFs, mutual funds, or other investments.

An Asset is an investment instrument and is not owned by a specific account.

The relationship between an account and an asset is represented through transactions.

Each asset stores a manually maintained current unit price for the MVP. The application does not retrieve market prices from an external service.

## User Stories

### US-ASS-001: Create an Asset

As a user, I want to create an asset so that I can record transactions for an investment.

### US-ASS-002: View Assets

As a user, I want to view my assets so that I can see the investments I track.

### US-ASS-003: View an Asset

As a user, I want to retrieve a specific asset so that I can view its details.

### US-ASS-004: Update an Asset

As a user, I want to update an asset's details and current price so that my portfolio summary reflects the latest known price.

### US-ASS-005: Delete an Asset

As a user, I want to delete an unused asset so that obsolete investment records can be removed.

## Functional Requirements

* An asset shall have:

  * Identifier
  * Name
  * Asset type
  * Current unit price
  * Creation timestamp
* Asset name shall be required and must not be empty or whitespace.
* Asset type shall be required.
* Asset type shall be represented as a simple value such as `Stock`, `Bond`, `ETF`, `Fund`, or `Other`.
* Current unit price shall be zero or greater.
* Current unit price shall be manually entered.
* Current unit price is a monetary value and shall use two decimal places.
* The API shall support:

  * Creating an asset
  * Listing assets
  * Retrieving an asset by identifier
  * Updating an asset
  * Deleting an asset
* An asset with recorded BUY or SELL transactions shall not be deletable.
* Asset deletion shall return a clear client error when dependent transactions exist.

## Acceptance Criteria

* A valid asset can be created.
* An asset with an empty or whitespace-only name is rejected.
* An asset without an asset type is rejected.
* A negative current unit price is rejected.
* A current unit price with more than two decimal places is rejected.
* Assets can be listed.
* An existing asset can be retrieved by identifier.
* A nonexistent asset returns HTTP 404.
* An existing asset can be updated.
* The current unit price can be updated.
* An asset with no BUY or SELL transactions can be deleted.
* An asset with BUY or SELL transactions cannot be deleted.
* Deleting an asset with dependent transactions returns a clear HTTP 400 or 409 response.

---

# 3. Transactions

## Purpose

Transactions record money moving into or out of an account and investment purchases or sales.

The MVP supports exactly four transaction types:

* `BUY`
* `SELL`
* `DEPOSIT`
* `WITHDRAWAL`

Transactions are the source of the portfolio calculations.

## User Stories

### US-TRX-001: Record a Buy

As a user, I want to record an investment purchase so that I can track the quantity and purchase cost of an asset.

### US-TRX-002: Record a Sell

As a user, I want to record an investment sale so that I can track the quantity and proceeds from selling an asset.

### US-TRX-003: Record a Deposit

As a user, I want to record money deposited into an account so that I can track money added to the account.

### US-TRX-004: Record a Withdrawal

As a user, I want to record money withdrawn from an account so that I can track money removed from the account.

### US-TRX-005: View Transactions

As a user, I want to view my transactions so that I can review my financial activity.

### US-TRX-006: Delete a Transaction

As a user, I want to delete an incorrect transaction so that I can correct my records.

Updating transactions is outside the MVP. An incorrect transaction is corrected by deleting it and creating a new transaction.

## Functional Requirements

* A transaction shall have:

  * Identifier
  * Transaction type
  * Account identifier
  * Optional asset identifier
  * Transaction date
  * Amount calculated by the API for BUY and SELL transactions
  * Optional quantity
  * Optional unit price
  * Optional description
  * Creation timestamp

### Transaction Types

The supported transaction types shall be exactly:

* `BUY`
* `SELL`
* `DEPOSIT`
* `WITHDRAWAL`

Unsupported transaction types shall be rejected.

### BUY Transactions

A BUY transaction shall:

* Reference an existing account.
* Reference an existing asset.
* Have a positive quantity.
* Have a positive unit price.
* Have an amount calculated by the API as:

```text
Amount = Quantity × UnitPrice
```

* Not accept an amount supplied independently by the client.

### SELL Transactions

A SELL transaction shall:

* Reference an existing account.
* Reference an existing asset.
* Have a positive quantity.
* Have a positive unit price.
* Have an amount calculated by the API as:

```text
Amount = Quantity × UnitPrice
```

* Not result in a negative holding quantity for the relevant Account and Asset
  combination.
* Not accept an amount supplied independently by the client.

### DEPOSIT Transactions

A DEPOSIT transaction shall:

* Reference an existing account.
* Not reference an asset.
* Have a positive amount.
* Not contain quantity or unit price.

### WITHDRAWAL Transactions

A WITHDRAWAL transaction shall:

* Reference an existing account.
* Not reference an asset.
* Have a positive amount.
* Not contain quantity or unit price.

### Transaction Dates

* Transaction date shall be supplied by the user.
* Transaction dates shall not be in the future.
* Creation timestamp shall be generated by the application.
* For historical holding validation, transactions shall be ordered by
  `TransactionDate` ascending, then `CreatedAt` ascending, then `Id` ascending.

### Financial Precision and Currency

* Monetary values, including transaction amounts and unit prices, shall use
  decimal values with two decimal places.
* Unit quantities shall use decimal values with up to six decimal places.
* The MVP uses one implicit currency only. Multi-currency support and currency
  conversion are out of scope.

### Account Cash Balances

* Deposits and withdrawals are recorded as transactions only; they do not
  create or maintain an available-cash balance for an account in this MVP.
* BUY and SELL transactions shall not validate whether an account has
  sufficient cash.

### Deletion

* A transaction can be deleted.
* Deleting a transaction must not leave the transaction history in an invalid state.
* If removing a transaction would cause a remaining SELL transaction to exceed
  the available quantity for the same Account and Asset combination, based on
  the defined transaction order, deletion shall be rejected.
* After a successful deletion, portfolio calculations shall reflect the updated transaction history.

## Acceptance Criteria

* A valid BUY transaction can be created.
* A valid SELL transaction can be created.
* A valid DEPOSIT transaction can be created.
* A valid WITHDRAWAL transaction can be created.
* BUY and SELL require an existing account.
* BUY and SELL require an existing asset.
* DEPOSIT and WITHDRAWAL require an existing account.
* DEPOSIT and WITHDRAWAL cannot reference an asset.
* BUY and SELL require positive quantity and unit price.
* BUY and SELL amounts are calculated by the API; a client does not supply an
  independent amount.
* DEPOSIT and WITHDRAWAL require a positive amount.
* BUY and SELL amount equals quantity multiplied by unit price using the
  defined monetary precision.
* Unsupported transaction types are rejected.
* A nonexistent account is rejected.
* A nonexistent asset is rejected.
* A SELL transaction that exceeds the currently available quantity for the same
  Account and Asset combination is rejected.
* A transaction dated in the future is rejected.
* Historical holding validation uses `TransactionDate` ascending, then
  `CreatedAt` ascending, then `Id` ascending.
* Transactions can be listed.
* An existing transaction can be retrieved by identifier.
* A nonexistent transaction returns HTTP 404.
* A valid transaction can be deleted.
* Deleting a transaction updates subsequent portfolio calculations.
* Deleting a transaction that would make the remaining transaction history invalid is rejected.

---

# 4. Portfolio Summary

## Purpose

Portfolio Summary provides a simple view of the current investment position based solely on recorded BUY and SELL transactions and the manually maintained current prices of assets.

The summary is calculated at request time and is not persisted as a separate database entity in the MVP.

## User Stories

### US-SUM-001: View Portfolio Summary

As a user, I want to view my portfolio summary so that I can understand my current investment position.

### US-SUM-002: View Holdings

As a user, I want to view my current holdings so that I can see which assets I currently own and their values.

## Functional Requirements

* The API shall provide one portfolio summary covering all accounts and assets.
* Holdings shall be calculated per Account and Asset combination as:

```text
Holding Quantity =
    Total BUY Quantity - Total SELL Quantity
```

* Account and Asset positions with a holding quantity of zero shall not be
  included.
* Positions with a negative holding quantity shall never occur because SELL
  transactions that would create a negative holding are rejected.
* Net Invested Amount shall be calculated per currently held Account and Asset
  position as:

```text
Net Invested Amount =
    Sum of BUY Amounts - Sum of SELL Amounts
```

Only transactions belonging to a currently held Account and Asset position are
included. This is deliberately simplified accounting; it does not use FIFO,
LIFO, average cost, or realized/unrealized gain separation.

* The aggregate Net Invested Amount shall be the sum of Net Invested Amounts
  for all currently held positions.

* Current portfolio value shall be calculated as:

```text
Current Portfolio Value =
    Sum of (Holding Quantity × Current Asset Unit Price)
```

* Profit/loss shall be calculated as:

```text
Profit/Loss =
    Current Portfolio Value - Net Invested Amount
```

* Deposits and withdrawals shall not affect investment holdings or portfolio performance calculations in this MVP.
* Each holding shall include:

  * Account identifier
  * Account name
  * Asset identifier
  * Asset name
  * Holding quantity
  * Net invested amount
  * Current unit price
  * Current value
* Portfolio calculations shall be performed at request time.
* Portfolio Summary shall not be stored as a separate database table.

## Acceptance Criteria

* With no BUY or SELL transactions:

  * Net Invested Amount is zero.
  * Current portfolio value is zero.
  * Profit/loss is zero.
  * No holdings are returned.

* A BUY transaction increases the relevant Account and Asset holding quantity.

* A BUY transaction increases the relevant position's Net Invested Amount by
  the BUY amount.

* A valid SELL transaction decreases the relevant Account and Asset holding
  quantity.

* A valid SELL transaction decreases the relevant position's Net Invested
  Amount by the SELL amount.

* Changing an asset's current unit price changes current portfolio value.

* Changing an asset's current unit price does not change Net Invested Amount.

* Changing an asset's current unit price changes calculated profit/loss.

* Account and Asset positions with zero quantity are excluded.

* Profit/loss equals current portfolio value minus Net Invested Amount.

* DEPOSIT transactions do not change investment holdings or portfolio performance figures.

* WITHDRAWAL transactions do not change investment holdings or portfolio performance figures.

* Deleting a BUY or SELL transaction causes the summary to be recalculated using the remaining transactions.

* The summary returns aggregate Net Invested Amount, current portfolio value,
  and profit/loss across all current positions.

* The holdings endpoint returns the individual current Account and Asset
  positions used by the summary calculations.

---

# API Endpoints

The MVP shall expose the following endpoints.

## Accounts

```text
POST   /api/accounts
GET    /api/accounts
GET    /api/accounts/{id}
PUT    /api/accounts/{id}
DELETE /api/accounts/{id}
```

## Assets

```text
POST   /api/assets
GET    /api/assets
GET    /api/assets/{id}
PUT    /api/assets/{id}
DELETE /api/assets/{id}
```

## Transactions

```text
POST   /api/transactions
GET    /api/transactions
GET    /api/transactions/{id}
DELETE /api/transactions/{id}
```

Transaction update is intentionally excluded from the MVP.

## Portfolio

```text
GET /api/portfolio/summary
GET /api/portfolio/holdings
```

`GET /api/portfolio/summary` returns aggregate Net Invested Amount, current
portfolio value, and profit/loss. `GET /api/portfolio/holdings` returns the
individual current Account and Asset positions, including each position's
quantity, Net Invested Amount, current unit price, and current value.

---

# Data Relationships

The intended entity relationships are:

```text
Account (1)
    │
    └──────────< Transaction >────────── Asset (1)
```

More explicitly:

```text
Account
   │
   └──< Transaction >── Asset
```

An Account can have many Transactions.

An Asset can appear in many Transactions.

A Transaction belongs to exactly one Account.

A BUY or SELL Transaction references exactly one Asset.

A DEPOSIT or WITHDRAWAL Transaction does not reference an Asset.

An Asset does not directly belong to an Account.

This allows the same asset to be held across multiple accounts.

Example:

```text
Account: Bamboo
    └── BUY Apple × 10

Account: GTBank
    └── BUY Apple × 5

Asset: Apple
```

The summary aggregates all Account and Asset positions. The holdings endpoint
returns Apple in Bamboo and Apple in GTBank as separate positions.

---

# Validation Rules

The following validation rules apply to the MVP:

1. Account name must be non-empty.
2. Account type must be provided.
3. Account names must be unique after trimming surrounding whitespace and
   comparing case-insensitively.
4. Asset name must be non-empty.
5. Asset type must be provided.
6. Asset current unit price must be zero or greater and use two decimal places.
7. Transaction type must be one of:

   * BUY
   * SELL
   * DEPOSIT
   * WITHDRAWAL
8. Every transaction must reference an existing account.
9. BUY and SELL transactions must reference an existing asset.
10. DEPOSIT and WITHDRAWAL transactions must not reference an asset.
11. BUY and SELL quantities must be positive.
12. BUY and SELL unit prices must be positive.
13. DEPOSIT and WITHDRAWAL amounts must be positive.
14. BUY and SELL amount is calculated by the API as quantity multiplied by unit
    price; the client must not independently provide the amount.
15. Transaction dates cannot be in the future.
16. A SELL cannot exceed the currently held quantity for the same Account and
    Asset combination, evaluated in transaction order: `TransactionDate`
    ascending, then `CreatedAt` ascending, then `Id` ascending.
17. An account with transactions cannot be deleted.
18. An asset with BUY or SELL transactions cannot be deleted.
19. Deleting a transaction must not leave subsequent transaction history
    invalid for an Account and Asset combination when evaluated in transaction
    order.
20. Monetary values use decimal values with two decimal places; unit quantities
    use decimal values with up to six decimal places.
21. The MVP uses one implicit currency only; multi-currency support and
    currency conversion are out of scope.
22. Deposits and withdrawals do not create or maintain an available-cash
    balance, and BUY and SELL do not validate sufficient account cash.

---

# HTTP Response Expectations

The API should use conventional HTTP status codes.

### Successful creation

```text
201 Created
```

### Successful retrieval

```text
200 OK
```

### Successful update

```text
200 OK
```

### Successful deletion

```text
204 No Content
```

### Resource not found

```text
404 Not Found
```

### Invalid request

```text
400 Bad Request
```

### Operation conflicts with existing data

```text
409 Conflict
```

The API should use standard ASP.NET Core error handling conventions such as `ProblemDetails` rather than introducing a custom response envelope unless a later requirement explicitly calls for one.

---

# Non-Functional Expectations

The MVP should:

* Use .NET 8.
* Use ASP.NET Core Web API.
* Use Entity Framework Core.
* Use MySQL.
* Use repository interfaces and concrete repository implementations.
* Use DTOs for API request and response models.
* Use dependency injection.
* Keep controllers thin.
* Keep persistence logic inside repositories.
* Place genuine business calculations and validation that spans multiple entities in services where appropriate.
* Avoid unnecessary architectural patterns.
* Include automated tests for important business logic and API behavior.
* Keep the implementation straightforward and maintainable.

The MVP should not introduce:

* CQRS
* MediatR
* Unit of Work
* Generic repositories
* Event sourcing
* Microservices
* Unnecessary abstractions

unless explicitly requested later.

---

# Implementation Order

Implementation should proceed in the following order:

1. Accounts
2. Assets
3. Transactions
4. Portfolio Summary
5. Tests and QA refinement

Each feature should be implemented and verified before moving to the next feature.

The Developer should not implement features outside the current approved scope.

The QA agent should validate each completed feature against this document before the next feature is considered complete.
