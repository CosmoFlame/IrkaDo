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

   Pending EF migrations are applied automatically on startup in **every** environment, so a deploy
   self-updates the schema (the app runs single-instance). In `Development` the database is also
   seeded with realistic placeholder content (bio, travel guides, news articles, social links,
   collaborations); the seed is idempotent and skipped once data exists. Seeding never runs outside
   Development.

   To apply migrations manually against a database (e.g. to inspect before deploying):

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

File storage lives behind the `IFileStorageService` abstraction and the backend picks an
implementation from `Storage:Provider`:

- **`Local`** (default, development) — images and guide PDFs are written to disk under
  `src/IrkaDo.Api/wwwroot/uploads/` (images, served via ASP.NET static files) and a sibling
  `GuideFiles/` directory (private PDFs, only reachable through a signed download link).
  `IrkaDo.Infrastructure/Storage/LocalFileStorageService.cs`.
- **`R2`** (production) — Cloudflare R2 (S3-compatible) object storage. Required on hosts with an
  ephemeral filesystem (e.g. Railway): local disk would silently lose uploads and purchased guide
  files on every redeploy. `IrkaDo.Infrastructure/Storage/R2FileStorageService.cs`. In non-Development
  environments the app refuses to start unless `Storage:Provider=R2` and all `R2:*` values are set.

Either way, only the resulting path/URL is stored in the database — never binary blobs.

### Configuring R2 for production

1. Create two R2 buckets: one **public** (images) exposed via an r2.dev or custom CDN domain, and one
   **private** (guide PDFs, no public access).
2. Create an R2 API token (Object Read & Write) to get an Access Key ID / Secret Access Key.
3. Set these environment variables on the deployment (double-underscore maps to config sections):

   ```
   Storage__Provider=R2
   R2__ServiceUrl=https://<account-id>.r2.cloudflarestorage.com
   R2__AccessKeyId=<access-key-id>
   R2__SecretAccessKey=<secret-access-key>
   R2__PublicBucket=<public-bucket-name>
   R2__PrivateBucket=<private-bucket-name>
   R2__PublicBaseUrl=https://<cdn-or-r2dev-domain-for-public-bucket>
   R2__ApiBaseUrl=https://<this-backend-public-url>
   ```

   `R2__ApiBaseUrl` is the backend's own public URL — signed guide-download links point back at
   `/api/v1/downloads/{token}` so the HMAC token, rate limiting, and download logging still gate
   every fetch. Also override `Storage__SigningKey` and `Admin__TokenSigningKey` with real secrets.

## Tests

```
dotnet test
```
