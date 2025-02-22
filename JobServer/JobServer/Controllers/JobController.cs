using JobServer.Models;
using JobServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobServer.Controllers
{
    [ApiController]
    [Route("job")]
    public class JobController : ControllerBase
    {
        private readonly JobManager _jobManager;
        private readonly JobDbContext _dbContext;

        public JobController(JobManager jobManager, JobDbContext dbContext)
        {
            _jobManager = jobManager;
            _dbContext = dbContext;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartJob([FromBody] JobRequest jobRequest)
        {
            try
            {
                // Attempt to start the job and get its ID
                int jobId = await _jobManager.StartJobAsync(jobRequest.JobType, jobRequest.JobName);

                return Ok(new { JobId = jobId });
            }
            catch (InvalidOperationException ex)
            {
                // If concurrent jobs exceed the limit, an exception is thrown
                return BadRequest(new ProblemDetails
                {
                    Title = "Too Many Concurrent Jobs",
                    Detail = ex.Message,
                    Status = 400
                });
            }
        }

        [HttpGet("status/{jobId}")]
        public async Task<IActionResult> GetJobStatus(int jobId)
        {
            IActionResult? validationResult = JobValidator.ValidateJobId(jobId, _dbContext);

            if (validationResult != null)
            {
                return validationResult;
            }

            string status = await _jobManager.GetJobStatusAsync(jobId);

            return Ok(status);
        }

        [HttpPost("cancel/{jobId}")]
        public async Task<IActionResult> CancelJob(int jobId)
        {
            IActionResult? validationResult = JobValidator.ValidateJobId(jobId, _dbContext);

            if (validationResult != null)
            {
                return validationResult;
            }

            await _jobManager.CancelJobAsync(jobId);

            return Ok("Job successfully Canceled and Removed.");
        }
    }
}
