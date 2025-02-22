using System.Net.Http.Json;
using Newtonsoft.Json;
using JobClient.Models;
using JobClient.Interfaces;

namespace JobClient.Services
{
    public class JobService : IJobService
    {
        private readonly HttpClient _httpClient;

        public JobService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JobResponse?> StartJobAsync(string jobType, string jobName)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/job/start", new { jobType, jobName });

            if (response.IsSuccessStatusCode)
            {
                string? result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<JobResponse>(result);
            }

            ErrorResponse? errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            Console.WriteLine($"Error: {errorResponse?.Title ?? "Unknown"} - {errorResponse?.Detail ?? "No details provided."}");
            
            return null;
        }

        public async Task<string?> GetJobStatusAsync(int jobId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"/job/status/{jobId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Error: JobId does not exist.");

                    return null;
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error getting the status: {e.Message}");

                return null;
            }
        }

        public async Task<bool> CancelJobAsync(int jobId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync($"/job/cancel/{jobId}", null);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Error: JobId does not exist.");

                    return false;
                }

                response.EnsureSuccessStatusCode();
                Console.WriteLine("Job successfully Canceled and Removed.");

                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error Canceling the job: {e.Message}");

                return false;
            }
        }
    }
}
