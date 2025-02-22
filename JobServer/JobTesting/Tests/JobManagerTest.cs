using Moq;
using JobServer.Services;
using Microsoft.Extensions.DependencyInjection;
using JobServer.Interfaces;
using JobTesting.Helpers;

namespace JobTesting.Services
{
    public class JobManagerTest : TestBase
    {
        [Fact]
        public async Task StartJobAsync_ShouldReturnJobId_WhenJobStartsSuccessfully()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();

            var mockJobQueue = new Mock<IBackgroundJobQueue>();
            var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

            // Reset the in-memory database
            ResetInMemoryDatabase(options);

            using var context = new JobDbContext(options);
            var jobManager = new JobManager(context, mockJobQueue.Object, mockServiceScopeFactory.Object);

            // Act
            var jobId = await jobManager.StartJobAsync("Type1", "TestJob");

            // Assert
            Assert.Equal(1, jobId);
        }
    }
}
