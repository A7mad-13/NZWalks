# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

Single project solution — everything is under `NZWalks.API/`.

```bash
# Build
dotnet build

# Run (Swagger UI at /swagger in Development)
dotnet run --project NZWalks.API
```

There is no test project in this solution.

### Connection strings (required before running)

`appsettings.json` currently has literal `ConnectionStrings` and a `Jwt:Key` checked in, but the
README's documented workflow is user-secrets — don't add real credentials to `appsettings.json`:

```bash
dotnet user-secrets set "ConnectionStrings:NZWalksConnectionString" "Server=YOUR_SERVER;Database=NZWalksDb;Trusted_Connection=True;TrustServerCertificate=True" --project NZWalks.API
```

In non-Development environments, supply it as an env var instead: `ConnectionStrings__NZWalksConnectionString`.

### EF Core migrations — two separate DbContexts

The app has **two independent databases/migration histories**, so `--context` is required for
anything touching auth:

```bash
# NZWalksDbContext (domain data: Regions/Walks/Difficulties/Images) — default context, migrations in Migrations/
dotnet ef migrations add "<Name>" --project NZWalks.API
dotnet ef database update --project NZWalks.API

# NZWalksAuthDbContext (ASP.NET Core Identity: users/roles) — migrations in Migrations/NZWalksAuthDb/
dotnet ef migrations add "<Name>" --project NZWalks.API --context NZWalksAuthDbContext --output-dir Migrations/NZWalksAuthDb
dotnet ef database update --project NZWalks.API --context NZWalksAuthDbContext
```

## Architecture

ASP.NET Core 8 Web API, layered as `Controller → Repository interface → SQL repository → DbContext`.
Controllers never touch `DbContext` directly — every data access goes through a repository interface
injected via DI (registered in `Program.cs`): `IRegionRespository`, `IWalkRepository`,
`ITokenRepository`, `IImageRepository`.

- **Two DbContexts, two databases**: `NZWalksDbContext` (domain: `Difficulties`, `Regions`, `Walks`,
  `Images`, seeded via `HasData` in `OnModelCreating`) and `NZWalksAuthDbContext` (an `IdentityDbContext`
  for users/roles, seeding fixed-GUID `Reader`/`Writer`/`Admin` roles in its own `OnModelCreating`).
  Both are registered against different connection strings in `Program.cs`.
- **Domain ↔ DTO mapping** is centralized in `Mappings/AutoMapperProfiles.cs` (AutoMapper). Any new
  domain model or DTO needs a `CreateMap<>` added there — controllers only map, never hand-construct DTOs.
- **API versioning** (`Asp.Versioning`) is only wired up on `RegionsController`: `[ApiVersion(1.0)]` /
  `[ApiVersion(2.0)]` with `[MapToApiVersion]` per action, and separate DTOs per version (`RegionDtoV1`
  vs `RegionDtoV2`, the latter adding a computed `HasImage`). `ConfigureSwaggerOptions.cs` generates one
  Swagger doc per discovered API version. Other controllers (`WalksController`, `ImagesController`,
  `AuthController`) are unversioned.
- **Auth**: JWT bearer auth backed by ASP.NET Core Identity (`NZWalksAuthDbContext`). `AuthController`
  handles Register/Login; `TokenRepository` issues the JWT (claims: email + one claim per role) signed
  with `Jwt:Key`/`Jwt:Issuer`/`Jwt:Audience` from configuration. Endpoints are locked down with
  `[Authorize(Roles = "...")]` using the `Reader`/`Writer`/`Admin` roles seeded in `NZWalksAuthDbContext`.
- **Validation**: `CustomActionFilter/ValidateModelAttribute.cs` is an `ActionFilterAttribute`
  (`[ValidateModel]`) that short-circuits with `BadRequest` when `ModelState` is invalid — the
  established convention for POST/PUT actions, used instead of manual `ModelState.IsValid` checks
  (though `ImagesController` and some `RegionsController` actions still check `ModelState.IsValid`
  manually rather than using the attribute).
- **Error handling**: `Middlewares/ExceptionHandlerMiddleware.cs` is a global exception-catching
  middleware registered first in the `Program.cs` pipeline — logs via Serilog and returns a generic
  500 JSON body. Note there are two folders, `Middleware/` (empty) and `Middlewares/` (contains the
  real middleware) — new middleware belongs in `Middlewares/`.
- **Logging**: Serilog, configured directly in `Program.cs` (not via `appsettings.json`), writing to
  console and a daily rolling file under `Logs/`.
- **Image upload**: `LocalImageRepository` saves uploaded files to the `Images/` folder on disk
  (served back out via `UseStaticFiles` at the `/Images` request path in `Program.cs`) and records
  metadata in the `Images` table on `NZWalksDbContext`.
- **Pagination/filtering/sorting**: implemented ad hoc inside `SQLWalkRepository.GetAllAsync`
  (query params `filterOn`/`filterQuery`/`sortBy`/`isAscending`/`pageNumber`/`pageSize`), currently
  only supporting filtering by `Name` and sorting by `Name` or `Length`; result shape is
  `WalkPageResult`/`WalkPageDtoResult` under `Models/Pagination Result/`. `RegionsController` has no
  equivalent paging.
