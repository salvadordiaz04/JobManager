using JobServer.Interfaces;
using System.Collections.Concurrent;

namespace JobServer.Services
{
    public class BackgroundJobQueue : IBackgroundJobQueue
    {
        private readonly ConcurrentQueue<Func<Task>> _jobs = new();
        private readonly SemaphoreSlim _signal = new(0);

        // Enqueue background jobs
        public void EnqueueBackgroundJob(Func<Task> job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            _jobs.Enqueue(job);
            _signal.Release();
        }

        // Process the jobs in the queue
        public async Task ProcessJobs(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _signal.WaitAsync(cancellationToken);

                if (_jobs.TryDequeue(out Func<Task>? job))
                {
                    try
                    {
                        await job();
                    }
                    catch (Exception ex)
                    {
                        // Exception handling
                        Console.WriteLine($"Error executing the job: {ex.Message}");
                    }
                    finally
                    {
                        _signal.Release();  // Release the semaphore after the job has finished
                    }
                }
            }
        }
    }
}
