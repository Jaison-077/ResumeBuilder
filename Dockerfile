# Multi-stage build for .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution file
COPY backend/ResumeBuilder.sln .

# Copy project files
COPY backend/src/Domain/Domain.csproj ./src/Domain/
COPY backend/src/Application/Application.csproj ./src/Application/
COPY backend/src/Infrastructure/Infrastructure.csproj ./src/Infrastructure/
COPY backend/src/Api/Api.csproj ./src/Api/

# Copy source code
COPY backend/src/Domain/ ./src/Domain/
COPY backend/src/Application/ ./src/Application/
COPY backend/src/Infrastructure/ ./src/Infrastructure/
COPY backend/src/Api/ ./src/Api/

# Restore dependencies
RUN dotnet restore

# Build and publish
RUN dotnet publish -c Release -o /app src/Api/Api.csproj

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy published artifacts
COPY --from=build /app .

# Port configuration
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ResumeBuilder.Api.dll"]
