# Create Solution
dotnet --version
dotnet new sln -n PortfolioTracker

# Create API project
dotnet new webapi -n PortfolioTracker.Api --framework net8.0
dotnet sln add PortfolioTracker.API/PortfolioTracker.API.csproj

# Add Packages
dotnet add package Microsoft.EntityFrameworkCore --version=8.0.3
dotnet add package Microsoft.EntityFrameworkCore.Design --version=8.0.3
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version=8.0.13
dotnet add package Pomelo.EntityFrameworkCore.MySql --version=8.0.2 

# Environment setup
export MYSQL_ROOT_PASSWORD_RAILWAY=your_password"
echo 'export MYSQL_ROOT_PASSWORD="your_password_here"' >> ~/.zshrc && source ~/.zshrc

# Build and Run Project
dotnet build
dotnet run

# Migrations
dotnet ef migrations add initmigration
dotnet ef database update 

# Add Tests
dotnet new xunit -n PortfolioTracker.Tests --framework net8.0
dotnet sln add PortfolioTracker.Tests/PortfolioTracker.Tests.csproj
dotnet add PortfolioTracker.Tests reference PortfolioTracker.API

dotnet test

# Publish
dotnet publish -c Release