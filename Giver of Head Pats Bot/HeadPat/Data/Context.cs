using Microsoft.EntityFrameworkCore;
using HeadPats.Data.Models;

namespace HeadPats.Data;

public class Context : DbContext {
    public Context() => Database.EnsureCreated();
    
    public DbSet<Users> Users { get; set; }
    public DbSet<Guilds> Guilds { get; set; }
    public DbSet<Overlord> Overall { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        const string path = "Data/data.db";
        optionsBuilder.UseSqlite($"Data Source={path}");
        optionsBuilder.EnableSensitiveDataLogging();

    }
}