# Strava Minimal API (.NET)

Kleine .NET Minimal API zum Authentifizieren über Strava (OAuth2), Synchronisieren von Aktivitäten per Strava API und Speichern in einer lokalen SQLite-Datenbank. Zusätzlich gibt es einen CSV-Export aller gespeicherten Aktivitäten.

## Features

- OAuth2-Login mit Strava
- Synchronisation von Aktivitäten des authentifizierten Athleten und Speicherung in SQLite (EF Core)
- CSV-Export aller gespeicherten Aktivitäten
- Minimal API (kein MVC)

## Voraussetzungen

- .NET 7 (oder kompatible .NET-Version)
- Strava Developer Account (ClientId & ClientSecret)
- Git (optional)
- Visual Studio / VS Code / JetBrains Rider oder Terminal

## Benötigte NuGet-Pakete

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
    "ClientId": "DEINE_CLIENT_ID",
    "ClientSecret": "DEIN_CLIENT_SECRET"
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