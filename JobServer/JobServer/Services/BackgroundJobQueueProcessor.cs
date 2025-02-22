using JobServer.Interfaces;

namespace JobServer.Services
{
    public class BackgroundJobQueueProcessor : BackgroundService
    {
        private readonly IBackgroundJobQueue _jobQueue;

        public BackgroundJobQueueProcessor(IBackgroundJobQueue jobQueue)
        {
            _jobQueue = jobQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // A loop that processes the jobs in the queue.
            while (!stoppingToken.IsCancellationRequested)
            {
                await _jobQueue.ProcessJobs(stoppingToken);  // Processes the jobs in the queue
            }
        }
    }
}
