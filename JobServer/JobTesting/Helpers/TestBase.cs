using JobServer.Services;
using Microsoft.EntityFrameworkCore;

namespace JobTesting.Helpers
{
    public abstract class TestBase
    {
        protected static DbContextOptions<JobDbContext> CreateInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<JobDbContext>()
                .UseInMemoryDatabase(databaseName: "JobDatabase")
                .Options;
        }

        protected static void ResetInMemoryDatabase(DbContextOptions<JobDbContext> options)
        {
            using JobDbContext context = new JobDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}