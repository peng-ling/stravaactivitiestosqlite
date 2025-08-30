# Strava Minimal API (.NET)

Small .NET Minimal API for authenticating with Strava (OAuth2), synchronizing activities via the Strava API, and storing them in a local SQLite database. Additionally, it provides a CSV export of all stored activities.

## Features

- OAuth2 login with Strava  
- Synchronization of activities for the authenticated athlete and storage in SQLite (EF Core)  
- CSV export of all stored activities  
- Minimal API (no MVC)

## Requirements

- .NET 7 (or a compatible .NET version)  
- Strava Developer Account (ClientId & ClientSecret)  
- Git (optional)  
- Visual Studio / VS Code / JetBrains Rider or terminal

## Required NuGet packages

```ps
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.Cookies
dotnet add package Microsoft.AspNetCore.Authentication.OAuth
dotnet add package CsvHelper
```

## appsettings.json

```json
{
  "Strava": {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=strava.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
