using Microsoft.EntityFrameworkCore;
using HeadPats.Data.Models;

namespace HeadPats.Data;

public sealed class Context : DbContext {
    /*public Context(DbSet<Users> users, DbSet<Guilds> guilds, DbSet<Overlord> overall) {
        Users = users;
        Guilds = guilds;
        Overall = overall;
        Database.EnsureCreated();
    }*/
    
    public Context() => Database.EnsureCreated();

    public DbSet<Users> Users { get; set; }
    public DbSet<Guilds> Guilds { get; set; }
    public DbSet<Overlord> Overall { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        var path = Path.Combine(Environment.CurrentDirectory, "Data", "data.db");
        if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Data")))
            Directory.CreateDirectory("Data");
        optionsBuilder.UseSqlite($"Data Source={path}");
        optionsBuilder.EnableSensitiveDataLogging();
    }
}