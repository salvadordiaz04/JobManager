using FluentValidation.TestHelper;
using JobServer.Helpers;
using JobServer.Models;

public class JobRequestValidatorTests
{
    private readonly JobRequestValidator _validator;

    public JobRequestValidatorTests()
    {
        _validator = new JobRequestValidator();
    }

    [Fact]
    public void Should_NotHaveAnyValidationErrors_When_JobTypeAndJobNameAreProvided()
    {
        // Arrange
        JobRequest model = new JobRequest
        {
            JobType = "Type1",
            JobName = "Job1"
        };

        // Act
        TestValidationResult<JobRequest> result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveValidationError_When_JobTypeIsMissing()
    {
        // Arrange
        JobRequest model = new JobRequest
        {
            JobType = string.Empty,
            JobName = "Job1"
        };

        // Act
        TestValidationResult<JobRequest> result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.JobType)
              .WithErrorMessage("JobType is required.");
    }

    [Fact]
    public void Should_HaveValidationError_When_JobNameIsMissing()
    {
        // Arrange
        JobRequest model = new JobRequest
        {
            JobType = "Type1",
            JobName = string.Empty
        };

        // Act
        TestValidationResult<JobRequest> result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.JobName)
              .WithErrorMessage("JobName is required.");
    }

    [Fact]
    public void Should_HaveValidationErrors_When_JobTypeAndJobNameAreMissing()
    {
        // Arrange
        JobRequest model = new JobRequest
        {
            JobType = string.Empty,
            JobName = string.Empty
        };

        // Act
        TestValidationResult<JobRequest> result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.JobType)
              .WithErrorMessage("JobType is required.");

        result.ShouldHaveValidationErrorFor(x => x.JobName)
              .WithErrorMessage("JobName is required.");
    }
}