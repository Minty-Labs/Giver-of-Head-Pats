using Microsoft.EntityFrameworkCore;
using HeadPats.Data.Models;
using HeadPats.Handlers;

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

/*public class ModerationModuleContext : DbContext {
    public ModerationModuleContext() => Database.EnsureCreated();
    public DbSet<Moderation> Moderation { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        const string path = "Data/moderation.db";
        optionsBuilder.UseSqlite($"Data Source={path}");
        optionsBuilder.EnableSensitiveDataLogging();
    }
}*/