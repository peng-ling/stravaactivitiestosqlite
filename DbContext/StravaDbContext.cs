using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


namespace StravaDataApi.DatabaseContext;

public class StravaActivity
{
    [JsonPropertyName("id")]
    public long Id { get; set; } 

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("distance")]
    public float Distance { get; set; }       // Meter

    [JsonPropertyName("moving_time")]
    public float MovingTime { get; set; }     // Sekunden

    [JsonPropertyName("elapsed_time")]
    public float ElapsedTime { get; set; }    // Sekunden

    [JsonPropertyName("total_elevation_gain")]
    public float TotalElevationGain { get; set; }
}

public class StravaDbContext : DbContext
{
    public DbSet<StravaActivity> Activities { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=strava.db");
    }

  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StravaActivity>().HasKey(a => a.Id);
    }


}
