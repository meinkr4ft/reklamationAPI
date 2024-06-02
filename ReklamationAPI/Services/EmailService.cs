using ReklamationAPI.Models;

namespace ReklamationAPI.Services
{
    /// <summary>
    /// Mock emailservice writing messages to files instead of sending emails.
    /// </summary>
    public class EmailService(string emailDirectory)
    {
        private readonly string _emailDirectory = emailDirectory;

        public async Task SendEmailAsync(OutboxMessage message)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{timestamp}_{message.Subject}.txt";
            var filePath = Path.Combine(_emailDirectory, fileName);

            var emailContent = $"To: {message.To}\nSubject: {message.Subject}\n\n{message.Body}";

            Directory.CreateDirectory(_emailDirectory);
            await File.WriteAllTextAsync(filePath, emailContent);
        }
    }

}
