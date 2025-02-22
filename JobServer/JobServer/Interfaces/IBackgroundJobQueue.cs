namespace JobServer.Interfaces
{
    public interface IBackgroundJobQueue
    {
        void EnqueueBackgroundJob(Func<Task> job);
        Task ProcessJobs(CancellationToken cancellationToken);
    }
}
