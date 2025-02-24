using JobClient.Services;
using JobClient.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using JobClient.Interfaces;

namespace JobClient
{
    class Program
    {
        static async Task Main()
        {
            ServiceProvider serviceProvider = ConfigureServices();
            IJobService jobService = serviceProvider.GetRequiredService<IJobService>();
            JobApp app = new JobApp(jobService);
            await app.RunAsync();
        }

        private static ServiceProvider ConfigureServices()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddHttpClient<IJobService, JobService>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiBaseUrl"] ?? "http://localhost:5000");
            });

            services.AddTransient<JobApp>();

            return services.BuildServiceProvider();
        }
    }

    public class JobApp
    {
        private readonly IJobService _jobService;
        private int? _currentJobId = null;

        public JobApp(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. Start Job");
                Console.WriteLine("2. View Job Status");
                Console.WriteLine("3. Cancel Job");
                Console.WriteLine("4. Clear Screen and Show Menu");
                Console.WriteLine("5. Exit");

                string? option = Console.ReadLine();

                if (option == "5") break;

                switch (option)
                {
                    case "1":
                        await StartJob();
                        break;
                    case "2":
                        if (JobHelper.GetJobExecutedFirst(_currentJobId))
                        {
                            await GetJobStatus();
                        }
                        break;
                    case "3":
                        if (JobHelper.GetJobExecutedFirst(_currentJobId))
                        {
                            await CancelJob();
                        }
                        break;
                    case "4":
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Invalid option, please choose an option from 1 to 5.");
                        break;
                }
            }
        }

        private async Task StartJob()
        {
            string jobType = JobHelper.GetValidJobInput("Enter JobType: ");
            string jobName = JobHelper.GetValidJobInput("Enter JobName: ");

            Models.JobResponse? jobResponse = await _jobService.StartJobAsync(jobType, jobName);

            if (jobResponse != null)
            {
                _currentJobId = jobResponse.JobId;
                Console.WriteLine($"Job started successfully with JobId: {_currentJobId}");
            }
        }

        private async Task GetJobStatus()
        {
            int jobId = JobHelper.GetValidJobId();
            string? status = await _jobService.GetJobStatusAsync(jobId);

            if (status != null)
            {
                Console.WriteLine($"Status: {status}");
            }
        }

        private async Task CancelJob()
        {
            int jobId = JobHelper.GetValidJobId();
            await _jobService.CancelJobAsync(jobId);
        }
    }
}
