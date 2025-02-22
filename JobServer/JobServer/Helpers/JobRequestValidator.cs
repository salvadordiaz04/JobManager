using FluentValidation;
using JobServer.Models;

namespace JobServer.Helpers
{
    public class JobRequestValidator : AbstractValidator<JobRequest>
    {
        public JobRequestValidator()
        {
            RuleFor(x => x.JobType)
                .NotEmpty().WithMessage("JobType is required.");

            RuleFor(x => x.JobName)
                .NotEmpty().WithMessage("JobName is required.");
        }
    }
}
