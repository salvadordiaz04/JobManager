using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using JobServer.Interfaces;
using JobServer.Services;
using JobServer.Helpers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
string? databaseProvider = builder.Configuration["DatabaseProvider"];

// Load configuration from appsettings.json and environment variables
builder.Configuration.AddEnvironmentVariables();

// Configure Kestrel to accept connections from any address
builder.WebHost.UseKestrel()
               .UseUrls("http://0.0.0.0:5000"); // Listen on all network interfaces

// Add services
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<JobRequestValidator>();
builder.Services.AddDbContext<JobDbContext>(options =>
{
    if (databaseProvider == "SqlServer")
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        // Default to using SQLite
        options.UseSqlite(connectionString);
    }
});

// Register the background service and the job queue
builder.Services.AddSingleton<IBackgroundJobQueue, BackgroundJobQueue>();

// This is the service that will process background jobs
builder.Services.AddHostedService<BackgroundJobQueueProcessor>();

// Register JobManager as a service
builder.Services.AddScoped<JobManager>();

WebApplication app = builder.Build();

// Enable Controllers
app.MapControllers();

app.Run();
