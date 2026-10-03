# POFTracker

## Project

PortfolioTracker is a lightweight personal finance and investment
portfolio tracking Web API.

## Technology

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- xUnit
- Repository Pattern
- DTOs
- Dependency Injection

## Architecture

The application should follow this either of this request flow depending on the implementation:
For CRUD operations =>
Controller
    ↓
Interface
    ↓
Repository
    ↓
DbContext
    ↓
MySQL

For Business Operations =>
Controller
    ↓
Service
    ↓
Interface -> Other Services
    ↓
Repository
    ↓
DbContext
    ↓
MySQL

Controllers must not access the DbContext directly.

Services contain business logic.

Repositories are responsible for data access.

DTOs are used for API request and response models.

## Development principles

- Keep the initial implementation simple.
- Do not introduce unnecessary architectural patterns.
- Prefer small, focused changes.
- Write tests for business logic.
- Do not introduce CQRS, MediatR, Unit of Work, or generic repositories unless explicitly requested.
- Do not implement features that have not been specified.
- Do not modify requirements to make implementation easier.

## Agent roles

### Product Manager

Defines requirements, user stories and acceptance criteria.

Does not write application code.

### Developer

Implements approved requirements.

Writes tests and documentation where appropriate.

Does not change requirements.

### QA

Reviews implementations against requirements.

Runs tests and identifies defects.

Does not modify application code unless explicitly asked.