# Multi-stage build for .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy backend directory entirely
COPY backend/ ./

# Restore dependencies
RUN dotnet restore ResumeBuilder.sln

# Build and publish
RUN dotnet publish -c Release -o /publish src/Api/Api.csproj

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy published artifacts
COPY --from=build /publish .

# Port configuration
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ResumeBuilder.Api.dll"]
