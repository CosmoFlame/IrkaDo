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
COPY --from=build /app/out ./

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet IrkaDo.Api.dll"]
