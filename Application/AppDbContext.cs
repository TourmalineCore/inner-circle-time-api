using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace Application;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }

    public AppDbContext()
    {
    }

    public virtual DbSet<WorkEntry> WorkEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            // this is needed to avoid renaming of __EFMigrationsHistory internal table with its columns
            // as part of transition to camel_case naming to be compatible with the default PostgreSQL naming convention
            // to avoid queries like this SELECT * FROM public."Items"
            // in favor of less quotes like that SELECT * FROM public.items
            .ReplaceService<IHistoryRepository, CamelCaseHistoryContext>()
            .UseSnakeCaseNamingConvention();

        base.OnConfiguring(optionsBuilder);
    }

#pragma warning disable EF1001 // Internal EF Core API usage.
    public class CamelCaseHistoryContext : NpgsqlHistoryRepository
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
        public CamelCaseHistoryContext(HistoryRepositoryDependencies dependencies)
#pragma warning disable EF1001 // Internal EF Core API usage.
            : base(dependencies)
#pragma warning restore EF1001 // Internal EF Core API usage.
        {
        }

        protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);

            history
                .Property(x => x.MigrationId)
                .HasColumnName("MigrationId");
            history
                .Property(x => x.ProductVersion)
                .HasColumnName("ProductVersion");
        }
    }
}
