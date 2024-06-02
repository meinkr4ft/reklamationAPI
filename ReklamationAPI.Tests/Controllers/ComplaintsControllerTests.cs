using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ReklamationAPI.data;
using ReklamationAPI.dto;
using ReklamationAPI.Models;
using ReklamationAPI.responses;
using ReklamationAPI.Tests;
using ReklamationAPI.Validation;

namespace ReklamationAPI.Controllers.Tests
{
    [TestClass]
    public class ComplaintsControllerTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private AppDbContext _context;
        private ComplaintsController _controller;
        private List<Complaint> complaintSeedData;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new AppDbContext(options);
            _controller = new ComplaintsController(_context);

            // Seed the in-memory database with test data
            SeedDatabase();

            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedDatabase()
        {
            // Generate and add 10 customers
            var customers = new List<Customer>
            {
                new ("john.doe@example.com", "John Doe"),
                new ("jane.smith@example.com", "Jane Smith"),
                new ("alice.jones@example.com", "Alice Jones"),
                new ("bob.brown@example.com", "Bob Brown"),
                new ("charlie.black@example.com", "Charlie Black"),
                new ("david.white@example.com", "David White"),
                new ("eva.green@example.com", "Eva Green"),
                new ("frank.lee@example.com", "Frank Lee"),
                new ("george.king@example.com", "George King"),
                new ("helen.gray@example.com", "Helen Gray")

            };

            _context.Customers.AddRange(customers);

            // Generate and add 15 complaints
            var complaints = new List<Complaint>
            {
     /*Id: 1 */ new(1, customers[0], customers[0].Email, new DateOnly(2023, 1, 15), "Defektes Produkt.", ComplaintStatus.Open),
     /*Id: 2 */ new(2, customers[1], customers[1].Email, new DateOnly(2023, 2, 20), "Verspätete Lieferung.", ComplaintStatus.InProgress),
     /*Id: 3 */ new(3, customers[2], customers[2].Email, new DateOnly(2023, 3, 25), "Falscher Artikel.", ComplaintStatus.Rejected),
     /*Id: 4 */ new(4, customers[3], customers[3].Email, new DateOnly(2023, 4, 10), "Schlechte Qualität.", ComplaintStatus.Accepted),
     /*Id: 5 */ new(3, customers[4], customers[4].Email, new DateOnly(2023, 5, 5), "Nicht wie beschrieben.", ComplaintStatus.Canceled),
     /*Id: 6 */ new(6, customers[5], customers[5].Email, new DateOnly(2023, 6, 15), "Fehlende Teile.", ComplaintStatus.Open),
     /*Id: 7 */ new(7, customers[6], customers[6].Email, new DateOnly(2023, 7, 25), "Beschädigter Artikel.", ComplaintStatus.InProgress),
     /*Id: 8 */ new(2, customers[7], customers[7].Email, new DateOnly(2023, 8, 30), "Paket nie angekommen.", ComplaintStatus.Rejected),
     /*Id: 9 */ new(9, customers[8], customers[8].Email, new DateOnly(2023, 9, 15), "Kundendienst Problem.", ComplaintStatus.Accepted),
     /*Id: 10*/ new(10, customers[9], customers[9].Email, new DateOnly(2023, 10, 10), "Falsche Farbe.", ComplaintStatus.Canceled),
     /*Id: 11*/ new(11, customers[0], customers[0].Email, new DateOnly(2023, 11, 20), "Wieder defektes Produkt.", ComplaintStatus.Open),
     /*Id: 12*/ new(6, customers[1], customers[1].Email, new DateOnly(2023, 12, 5), "Paket verloren.", ComplaintStatus.InProgress),
     /*Id: 13*/ new(13, customers[2], customers[2].Email, new DateOnly(2023, 1, 30), "Zu viel berechnet.", ComplaintStatus.Rejected),
     /*Id: 14*/ new(2, customers[3], customers[3].Email, new DateOnly(2023, 2, 15), "Falsche Größe.", ComplaintStatus.Accepted),
     /*Id: 15*/ new(15, customers[4], customers[4].Email, new DateOnly(2023, 3, 10), "Nicht wie auf dem Bild gezeigt.", ComplaintStatus.Canceled)
            };

            complaintSeedData = complaints;
            _context.Complaints.AddRange(complaints);

        }

        [TestMethod]
        public async Task GetComplaint_ReturnsComplaintList()
        {
            // Act
            var result = await _controller.GetComplaint();

            // Assert
            Assert.IsInstanceOfType<ActionResult<IEnumerable<ComplaintResponse>>>(result);
            var value = result.Value;
            Assert.IsInstanceOfType<IEnumerable<ComplaintResponse>>(value);
            Assert.AreEqual(15, value.Count());
            var ids = value.Select(v => v.Id);
            Assert.AreEqual(1, ids.Min());
            Assert.AreEqual(15, ids.Max());
        }

        [TestMethod]
        public async Task GetComplaint_ReturnsEmptyList()
        {
            // Arange
            await ClearComplaintsAsync();

            // Act
            var result = await _controller.GetComplaint();

            // Assert
            Assert.IsInstanceOfType<ActionResult<IEnumerable<ComplaintResponse>>>(result);
            var value = result.Value;
            Assert.IsInstanceOfType<IEnumerable<ComplaintResponse>>(value);
            Assert.AreEqual(0, value.Count());
        }

        [TestMethod]
        public async Task GetComplaint_ValidId_ReturnsComplaint()
        {
            for (var id = 1; id <= 15; id++)
            {
                // Act
                var result = await _controller.GetComplaint(id);

                // Assert
                Assert.IsInstanceOfType<ActionResult<ComplaintResponse>>(result);
                var value = result.Value;
                Assert.IsInstanceOfType<ComplaintResponse>(value);
                Assert.AreEqual(complaintSeedData[id - 1].Id, value.Id);
                Assert.AreEqual(complaintSeedData[id - 1].ProductId, value.ProductId);
                Assert.AreEqual(complaintSeedData[id - 1].Customer.Email, value.Customer.Email);
                Assert.AreEqual(complaintSeedData[id - 1].Customer.Name, value.Customer.Name);
                Assert.AreEqual(complaintSeedData[id - 1].Date, value.Date);
                Assert.AreEqual(complaintSeedData[id - 1].Description, value.Description);
                Assert.AreEqual(complaintSeedData[id - 1].Status.ToString(), value.Status);
            }

        }

        [TestMethod]
        public async Task GetComplaint_InvalidId_ReturnsNotFound()
        {
            foreach (var id in new int[] { -1, 16 })
            {
                // Act
                var actionResult = await _controller.GetComplaint(id);
                var result = actionResult.Result;

                // Assert
                Assert.IsInstanceOfType<NotFoundObjectResult>(result);
                Assert.IsNull(actionResult.Value);
                var resultValue = result.GetValue<Object?>("Value");
                Assert.IsNotNull(resultValue);
                var message = resultValue.GetValue<string>("message");
                Assert.AreEqual("A complaint with the specified id doesn't exist.", message);
                var resultId = resultValue.GetValue<int>("id");
                Assert.AreEqual(id, resultId);

            }
        }

        [TestMethod()]
        public async Task PutComplaint_Open_CorrectlyUpdatesValue()
        {
            // Arange
            var id = 1;

            // Act
            var newComplaint = complaintSeedData[id - 1].DeepCopy();
            newComplaint.ProductId = 100;
            newComplaint.Customer = new Customer("maxi.schmidt@gmail.com", "Maximilian Schmidt");
            newComplaint.Date = new DateOnly(2023, 2, 20);
            newComplaint.Description = "Falsches Produkt.";
            newComplaint.Status = ComplaintStatus.InProgress;
            var actionResult = await _controller.PutComplaint(id, new ComplaintDto(newComplaint));

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(actionResult);
            var changedComplaint = await _context.Complaints.FindAsync(id);
            Assert.AreEqual(newComplaint, changedComplaint);
        }

        [TestMethod()]
        public async Task PutComplaint_InProgress_CorrectlyUpdatesValue()
        {
            // Arange
            var id = 2;

            // Act
            var newComplaint = complaintSeedData[id - 1].DeepCopy();
            newComplaint.ProductId = 100;
            newComplaint.Customer = new Customer("maxi.schmidt@gmail.com", "Maximilian Schmidt");
            newComplaint.Date = new DateOnly(2023, 2, 20);
            newComplaint.Description = "Falsches Produkt.";
            newComplaint.Status = ComplaintStatus.Open;
            var actionResult = await _controller.PutComplaint(id, new ComplaintDto(newComplaint));

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(actionResult);
            var changedComplaint = await _context.Complaints.FindAsync(id);
            Assert.AreEqual(newComplaint, changedComplaint);
        }

        [TestMethod()]
        public async Task PutComplaint_Rejected_DoesntUpdatesValue()
        {
            // Arange
            var id = 3;

            // Act
            var complaintBefore = await _context.Complaints.FindAsync(id) ?? throw new Exception("Unexpected null value for Complaint.");
            complaintBefore = complaintBefore.DeepCopy();
            var newComplaint = complaintSeedData[id - 1].ShallowCopy();
            newComplaint.ProductId = 100;
            newComplaint.Customer = new Customer("maxi.schmidt@gmail.com", "Maximilian Schmidt");
            newComplaint.Date = new DateOnly(2023, 2, 20);
            newComplaint.Description = "Falsches Produkt.";
            var oldStatus = newComplaint.Status;
            newComplaint.Status = ComplaintStatus.Open;
            var actionResult = await _controller.PutComplaint(id, new ComplaintDto(newComplaint));

            // Assert
            Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult);
            var value = actionResult.GetValue<Object>("Value");
            Assert.IsNotNull(value);
            var message = value.GetValue<string>("message");
            Assert.AreEqual("Can only update complaints with Status \"InProgress\" or \"Open\"", message);
            var status = value.GetValue<string>("currentStatus");
            Assert.AreEqual(oldStatus.ToString(), status);
            var changedComplaint = await _context.Complaints.FindAsync(id);
            Assert.AreEqual(complaintBefore, changedComplaint);
        }


        [TestMethod()]
        public async Task PutComplaint_Accepted_DoesntUpdatesValue()
        {
            // Arange
            var id = 4;

            // Act
            var complaintBefore = await _context.Complaints.FindAsync(id) ?? throw new Exception("Unexpected null value for Complaint.");
            complaintBefore = complaintBefore.DeepCopy();
            var newComplaint = complaintSeedData[id - 1].ShallowCopy();
            newComplaint.ProductId = 100;
            newComplaint.Customer = new Customer ("maxi.schmidt@gmail.com", "Maximilian Schmidt");
            newComplaint.Date = new DateOnly(2023, 2, 20);
            newComplaint.Description = "Falsches Produkt.";
            var oldStatus = newComplaint.Status;
            newComplaint.Status = ComplaintStatus.Open;
            var actionResult = await _controller.PutComplaint(id, new ComplaintDto(newComplaint));

            // Assert
            Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult);
            var value = actionResult.GetValue<Object>("Value");
            Assert.IsNotNull(value);
            var message = value.GetValue<string>("message");
            Assert.AreEqual("Can only update complaints with Status \"InProgress\" or \"Open\"", message);
            var status = value.GetValue<string>("currentStatus");
            Assert.AreEqual(oldStatus.ToString(), status);
            var changedComplaint = await _context.Complaints.FindAsync(id);
            Assert.AreEqual(complaintBefore, changedComplaint);
        }


        [TestMethod()]
        public async Task PutComplaint_Canceled_DoesntUpdatesValue()
        {
            // Arange
            var id = 5;

            // Act
            var complaintBefore = await _context.Complaints.FindAsync(id) ?? throw new Exception("Unexpected null value for Complaint.");
            complaintBefore = complaintBefore.DeepCopy();
            var newComplaint = complaintSeedData[id - 1].ShallowCopy();
            newComplaint.ProductId = 100;
            newComplaint.Customer = new Customer ("maxi.schmidt@gmail.com", "Maximilian Schmidt");
            newComplaint.Date = new DateOnly(2023, 2, 20);
            newComplaint.Description = "Falsches Produkt.";
            var oldStatus = newComplaint.Status;
            newComplaint.Status = ComplaintStatus.Open;
            var actionResult = await _controller.PutComplaint(id, new ComplaintDto(newComplaint));

            // Assert
            Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult);
            var value = actionResult.GetValue<Object>("Value");
            Assert.IsNotNull(value);
            var message = value.GetValue<string>("message");
            Assert.AreEqual("Can only update complaints with Status \"InProgress\" or \"Open\"", message);
            var status = value.GetValue<string>("currentStatus");
            Assert.AreEqual(oldStatus.ToString(), status);
            var changedComplaint = await _context.Complaints.FindAsync(id);
            Assert.AreEqual(complaintBefore, changedComplaint);
        }

        [TestMethod()]
        public async Task PutComplaint_NewCustomerName_ImplicitCustomerTableUpdate()
        {
            // Arange
            var id = 1;

            // Act
            var customerBefore = await _context.Customers.FindAsync(complaintSeedData[id - 1].Customer.Email) ?? throw new Exception("Unexpected null value for customer.");
            customerBefore = customerBefore.ShallowCopy();
            var newComplaint = complaintSeedData[id - 1].ShallowCopy();
            newComplaint.Customer = new Customer(customerBefore.Email, "Maximilian Schmidt");
            var actionResult = await _controller.PutComplaint(id, new ComplaintDto(newComplaint));

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(actionResult);
            var changedComplaint = await _context.Complaints.FindAsync(id);
            Assert.AreEqual(newComplaint, changedComplaint);
            var changedCustomer = await _context.Customers.FindAsync(customerBefore.Email);
            Assert.IsNotNull(changedCustomer);
            Assert.AreEqual(newComplaint.Customer.Name, changedCustomer.Name);
            Assert.AreNotEqual(customerBefore.Name, changedCustomer.Name);
        }


        [TestMethod()]
        public async Task PutComplaint_NewCustomer_ImplicitCustomerTableInsert()
        {
            // Arange
            var id = 1;
            var newEmail = "maximilian.schmidt@gmail.com";

            // Act
            var customerBefore = await _context.Customers.FindAsync(newEmail);
            if (customerBefore != null)
            {
                throw new Exception("Unexpected not-null value for customer.");
            }


            var newComplaint = complaintSeedData[id - 1].ShallowCopy();
            newComplaint.Customer = new Customer (newEmail, "Maximilian Schmidt");
            var actionResult = await _controller.PutComplaint(id, new ComplaintDto(newComplaint));

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(actionResult);
            var newCustomer = await _context.Customers.FindAsync(newEmail);
            Assert.IsNotNull(newCustomer);
            Assert.AreEqual(newComplaint.Customer.Name, newCustomer.Name);
        }

        [TestMethod()]
        public async Task PutComplaint_InvalidId_ReturnsNotFound()
        {

            foreach (var id in new int[] { -1, 16 })
            {
                // Act
                var dto = new ComplaintDto(complaintSeedData[0]);
                var actionResult = await _controller.PutComplaint(id, dto);

                // Assert
                Assert.IsInstanceOfType<NotFoundObjectResult>(actionResult);
                var notFound = (NotFoundObjectResult)actionResult;
                Assert.IsNotNull(notFound);
                var value = notFound.GetValue<Object?>("Value");
                Assert.IsNotNull(value);
                var message = value.GetValue<string>("message");
                Assert.AreEqual("A complaint with the specified id doesn't exist.", message);
                var resultId = value.GetValue<int>("id");
                Assert.AreEqual(id, resultId);

            }
        }

        [TestMethod()]
        public async Task PutComplaint_ChangeStatus_OutboxMessageCreated()
        {
            // Arange
            var id = 1;

            // Act
            var newComplaint = complaintSeedData[id - 1].DeepCopy();
            newComplaint.Status = ComplaintStatus.InProgress;
            await _controller.PutComplaint(id, new ComplaintDto(newComplaint));

            // Assert
            Assert.AreEqual(1, _context.OutboxMessages.Count());
            var outboxMessage = await _context.OutboxMessages.FindAsync(1);
            Assert.IsNotNull(outboxMessage);
            Assert.IsTrue((DateTime.Now - outboxMessage.Date).TotalSeconds <= 15);
            Assert.AreEqual(newComplaint.Customer.Email, outboxMessage.To);
            Assert.IsTrue(outboxMessage.Subject.StartsWith("Ihre Reklamation zu Produkt "));
            Assert.IsTrue(outboxMessage.Body.StartsWith("Guten Tag"));
            Assert.IsFalse(outboxMessage.Processed);
        }

        [TestMethod()]
        public async Task PutComplaint_NoChangeStatus_OutboxMessageNotCreated()
        {
            // Arange
            var id = 1;

            // Act
            var newComplaint = complaintSeedData[id - 1].DeepCopy();
            await _controller.PutComplaint(id, new ComplaintDto(newComplaint));

            newComplaint.Status = ComplaintStatus.Accepted;
            await _controller.PutComplaint(-1, new ComplaintDto(newComplaint));

            // Assert
            Assert.AreEqual(0, _context.OutboxMessages.Count());
        }



        [TestMethod()]
        public async Task PostComplaint_CorrectlyCreatesComplaint()
        {
            // Act
            var newComplaintDto = new ComplaintDto(complaintSeedData[0]);
            var actionResult = await _controller.PostComplaint(newComplaintDto);
            var createdResult = actionResult.Result;

            // Assert
            Assert.IsInstanceOfType<CreatedAtActionResult>(createdResult);
            var value = createdResult.GetValue<ComplaintResponse?>("Value");
            Assert.IsNotNull(value);
            Assert.AreEqual(newComplaintDto.ProductId, value.ProductId);
            Assert.AreEqual(newComplaintDto.Customer, value.Customer);
            Assert.AreEqual(newComplaintDto.Date, value.Date);
            Assert.AreEqual(newComplaintDto.Description, value.Description);
            Assert.AreEqual(newComplaintDto.Status, value.Status);
            var actionName = createdResult.GetValue<string>("ActionName");
            Assert.IsNotNull(actionName);
            Assert.AreEqual(actionName, "GetComplaint");
            var routeValues = createdResult.GetValue<RouteValueDictionary>("RouteValues");
            Assert.IsNotNull(routeValues);
            var id = routeValues["id"];
            Assert.IsNotNull(id);
            Assert.IsInstanceOfType<int>(id);
            Assert.AreEqual(complaintSeedData.Count + 1, (int)id);

            var createdComplaint = await _context.Complaints.FindAsync(id);
            Assert.IsNotNull(createdComplaint);
            Assert.AreEqual(complaintSeedData[0], createdComplaint);
        }

        /// <summary>
        /// Tests if the Customers table is correctly updated if a new name is provided for the same email adress.
        /// </summary>
        [TestMethod()]
        public async Task PostComplaint_NewCustomerName_ImplicitCustomerTableUpdate()
        {
            // Arange
            var id = 1;

            // Act
            var customerBefore = await _context.Customers.FindAsync(complaintSeedData[id - 1].Customer.Email) ?? throw new Exception("Unexpected null value for customer.");
            customerBefore = customerBefore.ShallowCopy();
            var newComplaint = complaintSeedData[id - 1].ShallowCopy();
            newComplaint.Customer = new Customer (customerBefore.Email, "Maximilian Schmidt");
            var actionResult = await _controller.PostComplaint(new ComplaintDto(newComplaint));
            var createdResult = actionResult.Result;

            // Assert
            Assert.IsInstanceOfType<CreatedAtActionResult>(createdResult);
            var changedCustomer = await _context.Customers.FindAsync(customerBefore.Email);
            Assert.IsNotNull(changedCustomer);
            Assert.AreEqual(newComplaint.Customer.Name, changedCustomer.Name);
            Assert.AreNotEqual(customerBefore.Name, changedCustomer.Name);
        }

        /// <summary>
        /// Tests if a customer created when an unknown email adress is provided.
        /// </summary>
        [TestMethod()]
        public async Task PostComplaint_NewCustomer_ImplicitCustomerTableInsert()
        {
            // Arange
            var id = 1;
            var newEmail = "maximilian.schmidt@gmail.com";

            // Act
            var customerBefore = await _context.Customers.FindAsync(newEmail);
            if (customerBefore != null)
            {
                throw new Exception("Unexpected not-null value for customer.");
            }


            var newComplaint = complaintSeedData[id - 1].ShallowCopy();
            newComplaint.Customer = new Customer (newEmail, "Maximilian Schmidt");
            var actionResult = await _controller.PostComplaint(new ComplaintDto(newComplaint));
            var createdResult = actionResult.Result;

            // Assert
            Assert.IsInstanceOfType<CreatedAtActionResult>(createdResult);
            var newCustomer = await _context.Customers.FindAsync(newEmail);
            Assert.IsNotNull(newCustomer);
            Assert.AreEqual(newComplaint.Customer.Name, newCustomer.Name);
        }

        [TestMethod()]
        public async Task PostComplaint_OutboxMessageCreated()
        {
            // Act
            var newComplaintDto = new ComplaintDto(complaintSeedData[0]);
            await _controller.PostComplaint(newComplaintDto);

            // Assert
            Assert.AreEqual(1, _context.OutboxMessages.Count());
            var outboxMessage = await _context.OutboxMessages.FindAsync(1);
            Assert.IsNotNull(outboxMessage);
            Assert.IsTrue((DateTime.Now - outboxMessage.Date).TotalSeconds <= 15);
            Assert.AreEqual(newComplaintDto.Customer.Email, outboxMessage.To);
            Assert.IsTrue(outboxMessage.Subject.StartsWith("Ihre Reklamation zu Produkt "));
            Assert.IsTrue(outboxMessage.Body.StartsWith("Guten Tag"));
            Assert.IsFalse(outboxMessage.Processed);
        }


        [TestMethod]
        public async Task DeleteComplaint_ValidId_ReturnsComplaint()
        {
            // Arange
            var id = 1;
            var toDelete = complaintSeedData[id - 1];
            toDelete = toDelete.DeepCopy();
            if (toDelete.Status == ComplaintStatus.Canceled)
            {
                throw new Exception("Unexpected Canceled value for complaint status.");

            }
            // Act
            var actionResult = await _controller.DeleteComplaint(id);

            // Assert

            Assert.IsInstanceOfType<NoContentResult>(actionResult);
            var deleted = await _context.Complaints.FindAsync(id);
            Assert.IsNotNull(deleted);
            Assert.AreEqual(toDelete.ProductId, deleted.ProductId);
            Assert.AreEqual(toDelete.Customer.Email, deleted.Customer.Email);
            Assert.AreEqual(toDelete.Customer.Name, deleted.Customer.Name);
            Assert.AreEqual(toDelete.Date, deleted.Date);
            Assert.AreEqual(toDelete.Description, deleted.Description);
            Assert.AreEqual(ComplaintStatus.Canceled, deleted.Status);

        }

        [TestMethod]
        public async Task DeleteComplaint_CanceledComplaint_ReturnsBadRequest()
        {
            // Arange
            var id = 5;
            var toDelete = complaintSeedData[id - 1];
            toDelete = toDelete.DeepCopy();
            if (toDelete.Status != ComplaintStatus.Canceled)
            {
                throw new Exception("Unexpected not-Canceled value for complaint status.");

            }

            // Act
            var actionResult = await _controller.DeleteComplaint(id);

            // Assert
            Assert.IsInstanceOfType<BadRequestObjectResult>(actionResult);
            var value = actionResult.GetValue<Object>("Value");
            Assert.IsNotNull(value);
            var message = value.GetValue<string>("message");
            Assert.AreEqual("The complaint is already canceled.", message);
            var resultId = value.GetValue<int>("id");


            var deleted = await _context.Complaints.FindAsync(id);
            Assert.IsNotNull(deleted);
            Assert.AreEqual(toDelete.Id, resultId);
            Assert.AreEqual(toDelete.ProductId, deleted.ProductId);
            Assert.AreEqual(toDelete.Customer.Email, deleted.Customer.Email);
            Assert.AreEqual(toDelete.Customer.Name, deleted.Customer.Name);
            Assert.AreEqual(toDelete.Date, deleted.Date);
            Assert.AreEqual(toDelete.Description, deleted.Description);
            Assert.AreEqual(toDelete.Status, deleted.Status);
        }


        [TestMethod]
        public async Task DeleteComplaint_InvalidId_ReturnsNotFound()
        {
            foreach (var id in new int[] { -1, 16 })
            {
                // Act
                var actionResult = await _controller.DeleteComplaint(id);

                // Assert
                Assert.IsInstanceOfType<NotFoundObjectResult>(actionResult);
                //Assert.IsNull(actionResult.);
                var resultValue = actionResult.GetValue<Object?>("Value");
                Assert.IsNotNull(resultValue);
                var message = resultValue.GetValue<string>("message");
                Assert.AreEqual("A complaint with the specified id doesn't exist.", message);
                var resultId = resultValue.GetValue<int>("id");
                Assert.AreEqual(id, resultId);

            }
        }

        [TestMethod()]
        public async Task DeleteComplaint_ChangeStatus_OutboxMessageCreated()
        {
            // Arange
            var id = 1;

            // Act
            var complaint = await _context.Complaints.FindAsync(id) ?? throw new Exception("Unexpected null value for complaint");
            var email = complaint.Customer.Email;
            await _controller.DeleteComplaint(id);

            // Assert
            Assert.AreEqual(1, _context.OutboxMessages.Count());
            var outboxMessage = await _context.OutboxMessages.FindAsync(1);
            Assert.IsNotNull(outboxMessage);
            Assert.IsTrue((DateTime.Now - outboxMessage.Date).TotalSeconds <= 15);
            Assert.AreEqual(email, outboxMessage.To);
            Assert.IsTrue(outboxMessage.Subject.StartsWith("Ihre Reklamation zu Produkt "));
            Assert.IsTrue(outboxMessage.Body.StartsWith("Guten Tag"));
            Assert.IsFalse(outboxMessage.Processed);
        }

        [TestMethod()]
        public async Task DeleteComplaint_NoChangeStatus_OutboxMessageNotCreated()
        {
            // Arange
            var id = 5;

            // Act
            await _controller.DeleteComplaint(id);
            await _controller.DeleteComplaint(-1);

            // Assert
            Assert.AreEqual(0, _context.OutboxMessages.Count());
        }

        [TestMethod]
        public async Task SearchComplaints_NoParameters_ReturnsAllComplaints()
        {
            // Act
            var expected = new ComplaintSearchFilterDto(true);
            var result = await _controller.Search(null, null, null, null, null, null);

            // Assert
            Assert.IsInstanceOfType<ActionResult<SearchResponse>>(result);
            var value = result.Value;
            Assert.IsNotNull(value);

            Assert.AreEqual(expected, value.SearchDto);
            var complaints = value.Complaints;
            Assert.IsInstanceOfType<IEnumerable<ComplaintResponse>>(complaints);
            Assert.AreEqual(15, complaints.Count());
            var ids = complaints.Select(v => v.Id);
            Assert.AreEqual(1, ids.Min());
            Assert.AreEqual(15, ids.Max());
        }

        [TestMethod]
        public async Task SearchComplaints_StringParameterCaseInsensitive_IgnoresCase()
        {
            // Arange
            var expected = new ComplaintSearchFilterDto(true)
            {
                Description = "PRODUKT"
            };

            var manualExpectedIds = new int[] { 1, 11 };
            var expectedIds = complaintSeedData.Where(complaint =>
            {
                return complaint.Description.Contains(expected.Description, StringComparison.CurrentCultureIgnoreCase);
            })
                .Select(complaint => complaint.Id);

            if (!DistinctListsArePermutation(manualExpectedIds, expectedIds))
            {
                throw new Exception($"Test values are different than what is expected\nExpected: {String.Join(",", manualExpectedIds)}\n" +
                    $"Found: {String.Join(",", expectedIds)}");
            }

            // Act
            var result = await _controller.Search(null, null, null, null, expected.Description, null);

            // Assert
            Assert.IsInstanceOfType<ActionResult<SearchResponse>>(result);
            var value = result.Value;
            Assert.IsNotNull(value);

            Assert.AreEqual(expected, value.SearchDto);
            var complaints = value.Complaints;
            Assert.IsInstanceOfType<IEnumerable<ComplaintResponse>>(complaints);

            Assert.AreEqual(expectedIds.Count(), complaints.Count());

            var resultIds = complaints.Select(complaint => complaint.Id);
            foreach (var id in expectedIds)
            {
                Assert.IsTrue(resultIds.Contains(id));
            }
        }

        [TestMethod]
        public async Task SearchComplaints_StringParameterCasesensitive_NoResults()
        {
            // Arange
            var expected = new ComplaintSearchFilterDto(true)
            {
                IgnoreCase = false,
                Description = "PRODUKT"
            };

            var manualExpectedIds = Array.Empty<int>();
            var expectedIds = complaintSeedData.Where(complaint =>
            {
                return complaint.Description.Contains(expected.Description);
            })
                .Select(complaint => complaint.Id);

            if (!DistinctListsArePermutation(manualExpectedIds, expectedIds))
            {
                throw new Exception($"Test values are different than what is expected\nExpected: {String.Join(",", manualExpectedIds)}\n" +
                    $"Found: {String.Join(",", expectedIds)}");
            }

            // Act
            var result = await _controller.Search(null, null, null, null, expected.Description, null, false);

            // Assert
            Assert.IsInstanceOfType<ActionResult<SearchResponse>>(result);
            var notFound = result.Result;
            Assert.IsNotNull(notFound);
            Assert.IsInstanceOfType<NotFoundObjectResult>(notFound);
            var resultValue = notFound.GetValue<Object?>("Value");
            Assert.IsNotNull(resultValue);
            var message = resultValue.GetValue<string>("message");
            Assert.AreEqual("There are no complaints matching the search criteria.", message);
            var searchDto = resultValue.GetValue<ComplaintSearchFilterDto>("searchDto");
            Assert.AreEqual(expected, searchDto);
        }

        [TestMethod]
        public async Task SearchComplaints_MultipleParameter_ParameterAreInclusive()
        {
            // Arange
            var expected = new ComplaintSearchFilterDto(true);
            var prodId = 7;
            expected.ProductId = prodId.ToString();
            expected.Status = "Canceled";

            var manualExpectedIds = new int[] { 5, 7, 10, 15 };
            var expectedIds = complaintSeedData.Where(complaint =>
            {
                return complaint.ProductId.ToString() == expected.ProductId
                    || complaint.Status.ToString().Contains(expected.Status, StringComparison.CurrentCultureIgnoreCase);
            })
                .Select(complaint => complaint.Id);

            if (!DistinctListsArePermutation(manualExpectedIds, expectedIds))
            {
                throw new Exception($"Test values are different than what is expected\nExpected: {String.Join(",", manualExpectedIds)}\n" +
                    $"Found: {String.Join(",", expectedIds)}");
            }

            // Act
            var result = await _controller.Search(prodId, null, null, null, null, expected.Status);

            // Assert
            Assert.IsInstanceOfType<ActionResult<SearchResponse>>(result);
            var value = result.Value;
            Assert.IsNotNull(value);

            Assert.AreEqual(expected, value.SearchDto);
            var complaints = value.Complaints;
            Assert.IsInstanceOfType<IEnumerable<ComplaintResponse>>(complaints);

            Assert.AreEqual(expectedIds.Count(), complaints.Count());

            var resultIds = complaints.Select(complaint => complaint.Id);
            foreach (var id in expectedIds)
            {
                Assert.IsTrue(resultIds.Contains(id));
            }
        }

        [TestMethod]
        public async Task FilterComplaints_NoParameters_ReturnsAllComplaints()
        {
            // Act
            var expected = new ComplaintSearchFilterDto(false);
            var result = await _controller.Filter(null, null, null, null, null, null);

            // Assert
            Assert.IsInstanceOfType<ActionResult<FilterResponse>>(result);
            var value = result.Value;
            Assert.IsNotNull(value);

            Assert.AreEqual(expected, value.SearchDto);
            var complaints = value.Complaints;
            Assert.IsInstanceOfType<IEnumerable<ComplaintResponse>>(complaints);
            Assert.AreEqual(15, complaints.Count());
            var ids = complaints.Select(v => v.Id);
            Assert.AreEqual(1, ids.Min());
            Assert.AreEqual(15, ids.Max());
        }

        [TestMethod]
        public async Task FilterComplaints_StringParameterCaseInsensitive_IgnoresCase()
        {
            // Arange
            ComplaintSearchFilterDto expected = new(false)
            {
                Description = "PRODUKT"
            };

            var manualExpectedIds = new int[] { 1, 11 };
            var expectedIds = complaintSeedData.Where(complaint =>
            {
                return complaint.Description.Contains(expected.Description, StringComparison.CurrentCultureIgnoreCase);
            })
                .Select(complaint => complaint.Id);

            if (!DistinctListsArePermutation(manualExpectedIds, expectedIds))
            {
                throw new Exception($"Test values are different than what is expected\nExpected: {String.Join(",", manualExpectedIds)}\n" +
                    $"Found: {String.Join(",", expectedIds)}");
            }

            // Act
            var result = await _controller.Filter(null, null, null, null, expected.Description, null);

            // Assert
            Assert.IsInstanceOfType<ActionResult<FilterResponse>>(result);
            var value = result.Value;
            Assert.IsNotNull(value);

            Assert.AreEqual(expected, value.SearchDto);
            var complaints = value.Complaints;
            Assert.IsInstanceOfType<IEnumerable<ComplaintResponse>>(complaints);

            Assert.AreEqual(expectedIds.Count(), complaints.Count());

            var resultIds = complaints.Select(complaint => complaint.Id);
            foreach (var id in expectedIds)
            {
                Assert.IsTrue(resultIds.Contains(id));
            }
        }

        [TestMethod]
        public async Task FilterComplaints_StringParameterCasesensitive_NoResults()
        {
            // Arange
            var expected = new ComplaintSearchFilterDto(false)
            {
                IgnoreCase = false,
                Description = "PRODUKT"
            };

            var manualExpectedIds = Array.Empty<int>();
            var expectedIds = complaintSeedData.Where(complaint =>
            {
                return complaint.Description.Contains(expected.Description);
            })
                .Select(complaint => complaint.Id);

            if (!DistinctListsArePermutation(manualExpectedIds, expectedIds))
            {
                throw new Exception($"Test values are different than what is expected\nExpected: {String.Join(",", manualExpectedIds)}\n" +
                    $"Found: {String.Join(",", expectedIds)}");
            }

            // Act
            var result = await _controller.Filter(null, null, null, null, expected.Description, null, false);

            // Assert
            Assert.IsInstanceOfType<ActionResult<FilterResponse>>(result);
            var notFound = result.Result;
            Assert.IsNotNull(notFound);
            Assert.IsInstanceOfType<NotFoundObjectResult>(notFound);
            var resultValue = notFound.GetValue<Object?>("Value");
            Assert.IsNotNull(resultValue);
            var message = resultValue.GetValue<string>("message");
            Assert.AreEqual("There are no complaints matching the filter criteria.", message);
            var searchDto = resultValue.GetValue<ComplaintSearchFilterDto>("searchDto");
            Assert.AreEqual(expected, searchDto);
        }

        [TestMethod]
        public async Task FilterComplaints_MultipleParameter_ParameterAreExclusive()
        {
            // Arange
            var expected = new ComplaintSearchFilterDto(false);
            var prodId = 7;
            expected.ProductId = prodId.ToString();
            expected.Status = "InProgress";

            var manualExpectedIds = new int[] { 7 };
            var expectedIds = complaintSeedData.Where(complaint =>
            {
                return complaint.ProductId.ToString() == expected.ProductId
                    && complaint.Status.ToString().Contains(expected.Status, StringComparison.CurrentCultureIgnoreCase);
            })
                .Select(complaint => complaint.Id);

            if (!DistinctListsArePermutation(manualExpectedIds, expectedIds))
            {
                throw new Exception($"Test values are different than what is expected\nExpected: {String.Join(",", manualExpectedIds)}\n" +
                    $"Found: {String.Join(",", expectedIds)}");
            }

            // Arange
            var result = await _controller.Filter(prodId, null, null, null, null, expected.Status);

            // Assert
            Assert.IsInstanceOfType<ActionResult<FilterResponse>>(result);
            var value = result.Value;
            Assert.IsNotNull(value);

            Assert.AreEqual(expected, value.SearchDto);
            var complaints = value.Complaints;
            Assert.IsInstanceOfType<IEnumerable<ComplaintResponse>>(complaints);

            Assert.AreEqual(expectedIds.Count(), complaints.Count());

            var resultIds = complaints.Select(complaint => complaint.Id);
            foreach (var id in expectedIds)
            {
                Assert.IsTrue(resultIds.Contains(id));
            }
        }


        /// <summary>
        /// Empties db table complaints.
        /// </summary>
        /// <returns></returns>
        private async Task ClearComplaintsAsync()
        {
            var complaints = await _context.Complaints.ToListAsync();
            _context.Complaints.RemoveRange(complaints);
            await _context.SaveChangesAsync();

        }

        private static bool DistinctListsArePermutation<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            if (!IsDistinctList(list1) || !IsDistinctList(list2)) throw new ArgumentException("Arguments must be distinct lists");
            if (list1.Count() != list2.Count()) return false;
            foreach (var el in list1)
            {
                if (!list2.Contains(el))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsDistinctList<T>(IEnumerable<T> list)
        {
            return list.Distinct().Count() == list.Count();
        }

    }
}