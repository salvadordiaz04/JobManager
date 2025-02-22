using Microsoft.AspNetCore.Mvc;

namespace JobServer.Services
{
    public class JobValidator
    {
        public static IActionResult? ValidateJobId(int jobId, JobDbContext dbContext)
        {
            bool jobExists = dbContext.Jobs.Any(j => j.JobId == jobId);

            // Validation: jobId must be an integer greater than 0
            if (jobId <= 0)
            {
                return new BadRequestObjectResult(new ProblemDetails
                {
                    Title = "Invalid JobId",
                    Detail = "JobId must be an integer greater than 0.",
                    Status = 400
                });
            }

            if (!jobExists)
            {
                // Create a ProblemDetails object when the JobId does not exist
                ProblemDetails problemDetails = new ProblemDetails
                {
                    Title = "Job Not Found",
                    Detail = $"JobId {jobId} not found.",
                    Status = 404
                };

                return new NotFoundObjectResult(problemDetails); // Returns 404 with the ProblemDetails object
            }

            return null; // Returns null if validation is successful
        }
    }
}
