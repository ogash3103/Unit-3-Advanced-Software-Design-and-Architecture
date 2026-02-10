# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy repo root content into image
COPY . .

# Restore + publish with correct paths
RUN dotnet restore AgriProcurementPlatform/Procurement.Api/Procurement.Api.csproj
RUN dotnet publish AgriProcurementPlatform/Procurement.Api/Procurement.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Procurement.Api.dll"]
