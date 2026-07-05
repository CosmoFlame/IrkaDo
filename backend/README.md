# IrkaDo Backend

ASP.NET Core 10 API (Clean Architecture: `IrkaDo.Domain` / `IrkaDo.Application` / `IrkaDo.Infrastructure` / `IrkaDo.Api`).

## Local development

1. Start PostgreSQL (from the repo root):

   ```
   docker compose up -d postgres
   ```

2. Run the API:

   ```
   dotnet run --project src/IrkaDo.Api
   ```

   In `Development`, migrations are applied automatically and the database is seeded with realistic
   placeholder content (bio, travel guides, news articles, social links, collaborations) on startup —
   no manual step needed. The seed is idempotent: it's skipped on subsequent runs once data exists.

   For non-Development environments, apply migrations manually instead:

   ```
   dotnet ef database update --project src/IrkaDo.Infrastructure --startup-project src/IrkaDo.Api
   ```

3. Sanity check the API:

   ```
   curl http://localhost:5289/api/v1/home
   ```

   You should see hero/about/highlights/social links/collaborations/featured guides/latest news all
   populated with seed content. `http://localhost:5289/scalar` (or `/openapi/v1.json`) has the OpenAPI
   spec in Development.

## File storage

Images and downloadable guide PDFs are stored as files on disk under `src/IrkaDo.Api/wwwroot/uploads/`
and served directly via ASP.NET static files — never as binary blobs in the database, which only ever
stores the resulting path/URL. This lives behind the `IFileStorageService` abstraction
(`IrkaDo.Infrastructure/Storage/LocalFileStorageService.cs`), so a future cloud provider (S3, Azure
Blob Storage, Cloudflare R2) can be swapped in behind the same interface without touching business
logic.

## Tests

```
dotnet test
```
