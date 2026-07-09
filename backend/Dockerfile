FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/IrkaDo.Api/IrkaDo.Api.csproj src/IrkaDo.Api/
COPY src/IrkaDo.Application/IrkaDo.Application.csproj src/IrkaDo.Application/
COPY src/IrkaDo.Domain/IrkaDo.Domain.csproj src/IrkaDo.Domain/
COPY src/IrkaDo.Infrastructure/IrkaDo.Infrastructure.csproj src/IrkaDo.Infrastructure/
RUN dotnet restore src/IrkaDo.Api/IrkaDo.Api.csproj

COPY src/ src/
RUN dotnet publish src/IrkaDo.Api/IrkaDo.Api.csproj -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Npgsql loads Kerberos GSSAPI at connection time; the slim .NET runtime image doesn't ship it,
# which otherwise fails with "Cannot load library libgssapi_krb5.so.2".
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out ./

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet IrkaDo.Api.dll"]
