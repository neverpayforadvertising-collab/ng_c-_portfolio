# ---------- Restore ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore

WORKDIR /src

COPY backend/src/LedgerFlow.Domain/LedgerFlow.Domain.csproj \
     backend/src/LedgerFlow.Domain/

COPY backend/src/LedgerFlow.Application/LedgerFlow.Application.csproj \
     backend/src/LedgerFlow.Application/

COPY backend/src/LedgerFlow.Infrastructure/LedgerFlow.Infrastructure.csproj \
     backend/src/LedgerFlow.Infrastructure/

COPY backend/src/LedgerFlow.Api/LedgerFlow.Api.csproj \
     backend/src/LedgerFlow.Api/

RUN dotnet restore \
    backend/src/LedgerFlow.Api/LedgerFlow.Api.csproj


# ---------- Build / Publish ----------
FROM restore AS publish

COPY backend/ backend/

RUN dotnet publish \
    backend/src/LedgerFlow.Api/LedgerFlow.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false


# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

USER app

ENTRYPOINT ["dotnet", "LedgerFlow.Api.dll"]