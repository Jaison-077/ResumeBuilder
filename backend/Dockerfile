# Use Multi-stage build for .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /build

# Copy solution and project files
COPY backend/ .

# Restore and publish
RUN dotnet restore
RUN dotnet publish -c Release -o /app src/Api/Api.csproj

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy published files from build stage
COPY --from=build /app .

# Configure ASP.NET Core to listen on port 8080 (required by Render)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Api.dll"]
