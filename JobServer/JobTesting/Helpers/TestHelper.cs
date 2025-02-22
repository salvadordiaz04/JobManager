using JobServer.Services;
using Microsoft.EntityFrameworkCore;

namespace JobTesting.Helpers
{
    public static class TestHelper
    {
        public static DbContextOptions<JobDbContext> CreateInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<JobDbContext>()
                .UseInMemoryDatabase(databaseName: "JobDatabase")
                .Options;
        }

        public static void ResetInMemoryDatabase(DbContextOptions<JobDbContext> options)
        {
            using (JobDbContext context = new JobDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
