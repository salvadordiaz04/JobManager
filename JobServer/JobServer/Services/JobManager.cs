using JobServer.Interfaces;
using JobServer.Models;

namespace JobServer.Services
{
    public class JobManager
    {
        private readonly JobDbContext _dbContext;
        private readonly IBackgroundJobQueue _jobQueue;  // Service for executing background jobs
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public JobManager(JobDbContext dbContext, IBackgroundJobQueue jobQueue, IServiceScopeFactory serviceScopeFactory)
        {
            _dbContext = dbContext;
            _jobQueue = jobQueue;
            _serviceScopeFactory = serviceScopeFactory; // Injecting the factory
        }

        public async Task<int> StartJobAsync(string jobType, string jobName)
        {
            // Limit the number of concurrent jobs of the same type to 5
            List<Job> activeJobsOfType = _dbContext.Jobs
                .Where(job => job.JobType == jobType && job.IsRunning)
            .ToList();

            // Mark as Not Running jobs that have been running for too long
            foreach (Job activeJob in activeJobsOfType)
            {
                if (activeJob.IsRunning && activeJob.CreatedAt < DateTime.Now.AddSeconds(-15))
                {
                    activeJob.IsRunning = false;
                }
            }

            await _dbContext.SaveChangesAsync();

            if (activeJobsOfType.Count >= 5)
            {
                throw new InvalidOperationException($"Cannot start more than 5 jobs of the same type concurrently.");
            }

            // Start the new job
            Job job = new Job { JobType = jobType, JobName = jobName, IsRunning = true, CreatedAt = DateTime.Now };

            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync();

            // Enqueue the background job
            _jobQueue.EnqueueBackgroundJob(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(15)); // Simulate job execution

                    // Create a new service scope to get a new DbContext
                    using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                    {
                        JobDbContext scopedDbContext = scope.ServiceProvider.GetRequiredService<JobDbContext>();
                        Job? jobToUpdate = await scopedDbContext.Jobs.FindAsync(job.JobId);

                        if (jobToUpdate != null)
                        {
                            jobToUpdate.IsRunning = false;
                            await scopedDbContext.SaveChangesAsync(); // Mark the job as finished
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Create a new service scope to handle the error with a new DbContext
                    using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                    {
                        JobDbContext scopedDbContext = scope.ServiceProvider.GetRequiredService<JobDbContext>();
                        Job? jobToUpdate = await scopedDbContext.Jobs.FindAsync(job.JobId);

                        if (jobToUpdate != null)
                        {
                            jobToUpdate.IsRunning = false;
                            await scopedDbContext.SaveChangesAsync();
                        }
                    }

                    Console.WriteLine($"Error executing the job: {ex.Message}");
                }
            });

            return job.JobId;
        }

        public async Task<string> GetJobStatusAsync(int jobId)
        {
            Job? job = await _dbContext.Jobs.FindAsync(jobId);
            return job == null ? "JobId Not Found" : (job.IsRunning ? "Running" : "Not Running");
        }

        public async Task<bool> CancelJobAsync(int jobId)
        {
            Job? job = await _dbContext.Jobs.FindAsync(jobId);
            if (job == null) return false;

            job.IsRunning = false;
            _dbContext.Jobs.Remove(job);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
