using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StravaDataApi.DatabaseContext;

var builder = WebApplication.CreateBuilder(args);

// --- Strava Config aus appsettings.json laden ---
var stravaConfig = builder.Configuration.GetSection("Strava");

var clientId = stravaConfig["ClientId"];
var clientSecret = stravaConfig["ClientSecret"];

if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
    throw new InvalidOperationException("Bitte Strava ClientId und ClientSecret in appsettings.json setzen!");


// --- OAuth 2.0 Setup ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Strava";
})
.AddCookie()
.AddOAuth("Strava", options =>
{
    options.ClientId = clientId!;
    options.ClientSecret = clientSecret!;
    options.CallbackPath = "/strava/callback";

    options.AuthorizationEndpoint = "https://www.strava.com/oauth/authorize";
    options.TokenEndpoint = "https://www.strava.com/oauth/token";
    options.Scope.Add("activity:read_all");
    options.SaveTokens = true;

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = context => Task.CompletedTask
    };
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // nur lokal testen
});

builder.Services.AddDbContext<StravaDbContext>();

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StravaDbContext>();
    db.Database.EnsureCreated();  // <-- Tabelle wird automatisch erstellt
}

app.UseAuthentication();
app.UseAuthorization();

// --- Login ---
app.MapGet("/login", async (HttpContext context) =>
{
    await context.ChallengeAsync("Strava", new AuthenticationProperties
    {
        RedirectUri = "/activities/sync"
    });
});

// --- Aktivitäten synchronisieren und in SQLite speichern ---
app.MapGet("/activities/sync", async (HttpContext context, StravaDbContext db) =>
{
    if (!context.User.Identity?.IsAuthenticated ?? true)
        return Results.Redirect("/login");

    var accessToken = await context.GetTokenAsync("access_token");
    if (accessToken == null)
        return Results.Unauthorized();

    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    int page = 1, perPage = 50;
    var newActivities = 0;

    while (true)
    {
        var response = await httpClient.GetAsync($"https://www.strava.com/api/v3/athlete/activities?per_page={perPage}&page={page}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var pageActivities = JsonSerializer.Deserialize<List<StravaActivity>>(json);

        if (pageActivities == null || pageActivities.Count == 0)
            break;

        foreach (var act in pageActivities)
        {
            if (!await db.Activities.AnyAsync(a => a.Id == act.Id))
            {
                db.Activities.Add(act);
                newActivities++;
            }
        }

        page++;
    }

    await db.SaveChangesAsync();
    return Results.Text($"Synchronisiert: {newActivities} neue Aktivitäten in SQLite gespeichert.");
});

// --- Alle Aktivitäten als CSV exportieren ---
app.MapGet("/activities/csv", async (StravaDbContext db) =>
{
    var activities = await db.Activities.ToListAsync();

    using var writer = new StringWriter();
    using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
    csv.WriteRecords(activities);

    return Results.Text(writer.ToString(), "text/csv");
});

await  app.RunAsync();
