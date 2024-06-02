using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ReklamationAPI.data;
using ReklamationAPI.Models;
using System.Globalization;

namespace ReklamationAPI.Services.Tests
{
    [TestClass()]
    public class EmailServiceTests
    {
        private readonly string emailPath = "emails";
        private readonly string emailDateFormat = "yyyyMMdd_HHmmss";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private AppDbContext _context;
        private EmailService emailService;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private readonly OutboxMessage outboxMessage = new(DateTime.Now, "Test To", "Test Subject", "Test Body", false);


        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new AppDbContext(options);

            _context.Add(outboxMessage);

            _context.SaveChanges();

            emailService = new EmailService(emailPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod()]
        public async Task SendEmailAsyncTest()
        {
            // Arange
            var currentCount = !Directory.Exists(emailPath) ? 0 : Directory.GetFiles(emailPath).Length;

            // Act
            await emailService.SendEmailAsync(outboxMessage);
            Thread.Sleep(1000);

            // Assert
            Assert.IsTrue(Directory.Exists(emailPath));
            string[] filePaths = Directory.GetFiles(emailPath);
            Assert.AreEqual(currentCount + 1, filePaths.Length);
            var fileName = Path.GetFileName(filePaths[^1]);
            Assert.IsTrue(emailDateFormat.Length <= fileName.Length);
            var dateString = fileName[..emailDateFormat.Length];
            var dateTime = DateTime.ParseExact(dateString, emailDateFormat, CultureInfo.CurrentCulture);
            Assert.IsTrue((DateTime.Now - dateTime).TotalSeconds <= 5);
        }
    }
}