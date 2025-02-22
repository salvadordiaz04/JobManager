using JobClient.Models;

namespace JobClient.Interfaces
{
    public interface IJobService
    {
        Task<JobResponse?> StartJobAsync(string jobType, string jobName);
        Task<string?> GetJobStatusAsync(int jobId);
        Task<bool> CancelJobAsync(int jobId);
    }
}
