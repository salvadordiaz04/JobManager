namespace JobClient.Helpers
{
    public static class JobHelper
    {
        public static string GetValidJobInput(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Error: The field cannot be empty.");
                }

            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public static int GetValidJobId()
        {
            int jobId;

            do
            {
                Console.Write("Enter JobId (must be a number greater than 0): ");
                string input = Console.ReadLine() ?? string.Empty;

                // Validate if the input is an integer greater than 0
                if (int.TryParse(input, out jobId) && jobId > 0)
                {
                    return jobId;
                }
                else
                {
                    Console.WriteLine("Error: JobId must be a valid number greater than 0.");
                }

            } while (true);
        }

        public static bool GetJobExecutedFirst(int? currentJobId)
        {
            if (currentJobId == null)
            {
                Console.WriteLine("Error: No JobId available. You must start a Job first.");

                return false;
            }

            return true;
        }
    }
}
