# NZWalks API

ASP.NET Core 8 Web API with Entity Framework Core and SQL Server.

## Getting started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or a full instance)

### Configure the connection string

The connection string is **not** stored in `appsettings.json` — it holds credentials and must
stay out of source control. Set it locally with [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets set "ConnectionStrings:NZWalksConnectionString" "Server=YOUR_SERVER;Database=NZWalksDb;Trusted_Connection=True;TrustServerCertificate=True" --project NZWalks.API
```

`WebApplication.CreateBuilder` loads user-secrets automatically in the Development environment,
so nothing else needs to change. In other environments, supply the value via an environment
variable instead:

```bash
ConnectionStrings__NZWalksConnectionString="..."
```

### Apply migrations

```bash
dotnet ef database update --project NZWalks.API
```

### Run

```bash
dotnet run --project NZWalks.API
```

Swagger UI is available at `/swagger` when running in Development.
