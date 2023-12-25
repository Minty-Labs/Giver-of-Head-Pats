using HeadPats.Configuration;
using Microsoft.EntityFrameworkCore;
using HeadPats.Data.Models;

namespace HeadPats.Data;

public sealed class Context : DbContext {
    public DbSet<Users> Users { get; set; }
    public DbSet<Guilds> Guilds { get; set; }
    public DbSet<DailyPats> DailyPats { get; set; }
    public DbSet<GlobalVariable> GlobalVariables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseNpgsql($"Host={Config.Base.Database.Host};Port={Config.Base.Database.Port};Username={Config.Base.Database.Username};Password={Config.Base.Database.Password};Database={Config.Base.Database.Database}");
        optionsBuilder.EnableSensitiveDataLogging();
    }
}