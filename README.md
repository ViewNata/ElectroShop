# CSI402 Electrical Shop

ASP.NET Core MVC application for the CSI402 project.

## Requirements

- .NET 10 SDK
- SQL Server with a database named `CSI402DB`

The source project does not include the database backup. `SQLQuery_3.sql` only contains a partial table script, so use the complete class database or a compatible schema before running database-backed pages.

## Run locally

1. Set a local connection string with .NET User Secrets (do not commit credentials):

   ```bash
   dotnet user-secrets init --project 66076431_natalee.csproj
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=CSI402DB;User Id=sa;Password=YOUR_SQL_PASSWORD;TrustServerCertificate=True" --project 66076431_natalee.csproj
   ```

2. Restore and run:

   ```bash
   dotnet restore 66076431_natalee.sln
   dotnet run --project 66076431_natalee.csproj
   ```

3. Open the local URL printed by `dotnet run` (the launch profile uses `http://localhost:5036`).

Uploaded payment slip files and QR images are intentionally excluded. Add test-only files locally if needed; never commit real customer payment records or credentials.
