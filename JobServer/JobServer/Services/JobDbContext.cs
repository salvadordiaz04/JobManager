using JobServer.Models;
using Microsoft.EntityFrameworkCore;

namespace JobServer.Services
{
    public class JobDbContext(DbContextOptions<JobDbContext> options) : DbContext(options)
    {
        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the CreatedAt property
            Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<DateTime> createdAtProperty = modelBuilder.Entity<Job>()
                .Property(j => j.CreatedAt);

            // Configuration specific to SQLite
            if (Database.ProviderName != null && Database.ProviderName.Contains("Sqlite"))
            {
                createdAtProperty
                    .HasConversion(
                        v => v.ToString("yyyy-MM-dd HH:mm:ss"), // Convert to string for SQLite
                        v => DateTime.Parse(v)) // Convert from string to DateTime
                    .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value for SQLite
            }

            // Configuration specific to SQL Server
            if (Database.ProviderName != null && Database.ProviderName.Contains("SqlServer"))
            {
                createdAtProperty
                    .HasColumnType("datetime2") // Column type for SQL Server
                    .HasDefaultValueSql("GETUTCDATE()"); // Default value for SQL Server
            }
        }
    }
}
