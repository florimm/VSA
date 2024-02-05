using Npgsql;
using Template.Api.Features.Users.Models;

namespace Template.Api.Infrastructure;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    static DatabaseContext() => NpgsqlConnection.GlobalTypeMapper.MapEnum<UserRole>("userhub.UserRole");

    public DbSet<UserModel> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<UserRole>("userhub", "UserRole");
        modelBuilder
            .Entity<UserModel>()
            .ToTable("User")
            .HasKey(t => t.Id);

        var properties = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties());

        // Model fields are PascalCase which are converted to camelCase for db fields.
        foreach (var pb in properties)
        {
            pb.SetColumnName(ToCamelCase(pb.Name));
        }
    }

    private static string ToCamelCase(string str) =>
        string.IsNullOrEmpty(str) || str.Length < 2
            ? str.ToLowerInvariant()
            : char.ToLowerInvariant(str[0]) + str[1..];
}
