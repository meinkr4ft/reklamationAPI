using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReklamationAPI.data;
using ReklamationAPI.dto;
using ReklamationAPI.Models;
using ReklamationAPI.responses;
using ReklamationAPI.Swagger;
using ReklamationAPI.Validation;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ReklamationAPI.Controllers
{
    /// <summary>
    /// Controller for complaint API.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/Complaints
        /// <summary>
        /// Retrieves a list of complaints.
        /// </summary>
        /// <returns>A ilst of complaint response objects.</returns>
        /// <response code="200">OK if the request is successful.</response>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<ComplaintResponse>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ComplaintResponseArrayExample))]
        public async Task<ActionResult<IEnumerable<ComplaintResponse>>> GetComplaint()
        {
            var complaints = await _context.Complaints.ToListAsync();

            await Complaint.InitiliateCustomers(complaints);

            // Convert Model Object to Response Object
            var responses = complaints.Select(c => new ComplaintResponse(c)).ToList();
            return responses;
        }

        // GET: api/Complaints/5
        /// <summary>
        /// Retrieves a specific complaint by ID.
        /// </summary>
        /// <param name="id">The ID of the complaint to retrieve.</param>
        /// <returns>A complaint response object.</returns>
        /// <response code="200">OK if the request is successful.</response>
        /// <response code="404">Not found if the complaint with the specified ID does not exist.</response>
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ComplaintResponse))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ComplaintResponseExample))]
        public async Task<ActionResult<ComplaintResponse>> GetComplaint(int id)
        {
            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
            {
                return NotFound(new
                {
                    message = "A complaint with the specified id doesn't exist.",
                    id
                });
            }

            await complaint.InitializeCustomerAsync();

            return new ComplaintResponse(complaint);
        }

        // PUT: api/Complaints/5        
        /// <summary>
        /// Updates a specific complaint by ID.
        /// Warning: Complaints with status "Rejected", "Accepted" or "Canceled" can't be edited anymore.
        /// </summary>
        /// <param name="id">The ID of the complaint to update.</param>
        /// <param name="dto">The updated complaint object.</param>
        /// <returns>A response indicating the result of the update operation.</returns>
        /// <response code="204">No content if the update was successful.</response>
        /// <response code="400">Bad request if the complaint is already Rejected, Accepted or Canceled.</response>
        /// <response code="401">Unauthorized if no authentication token is provided.</response>
        /// <response code="403">Forbidden if the provided token does not belong to an admin user.</response>
        /// <response code="404">Not found if the complaint with the specified ID does not exist.</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> PutComplaint(int id, ComplaintDto dto)
        {
            if (!ComplaintExists(id))
            {
                return NotFound(new
                {
                    message = "A complaint with the specified id doesn't exist.",
                    id
                });
            }
            var existingComplaint = await _context.Complaints.FindAsync(id) ?? throw new Exception("Internal Server Error");
            // Only Complaints with Status "Open" and "InProgress" are eligible for Updating.
            if (existingComplaint.Status != ComplaintStatus.Open && existingComplaint.Status != ComplaintStatus.InProgress)
            {
                return BadRequest(new
                {
                    message = "Can only update complaints with Status \"InProgress\" or \"Open\"",
                    currentStatus = existingComplaint.Status.ToString()
                });
            }

            var statusChanged = existingComplaint.Status.ToString() != dto.Status;

            await existingComplaint.InitializeCustomerAsync();
            existingComplaint.CopyValues(dto);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Entry(existingComplaint).State = EntityState.Modified;

                    await ApplyCustomerChanges(existingComplaint);

                    if (statusChanged)
                    {
                        var outboxMessage = new OutboxMessage(existingComplaint, "Der Status Ihrer Reklamation hat sich geändert.");

                        _context.OutboxMessages.Add(outboxMessage);

                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Complaints
        /// <summary>
        /// Creates a new complaint.
        /// </summary>
        /// <param name="dto">The complaint data transfer object containing the details of the complaint to create.</param>
        /// <returns>The created complaint response object.</returns>
        /// <response code="201">Created if the complaint was successfully created.</response>
        /// <response code="401">Unauthorized if no authentication token is provided.</response>
        /// <response code="403">Forbidden if the provided token does not belong to an admin user.</response>
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        [SwaggerResponseExample(201, typeof(ComplaintResponse))]
        public async Task<ActionResult<ComplaintResponse>> PostComplaint(ComplaintDto dto)
        {
            Complaint complaint = new(dto);
            await complaint.InitializeCustomerAsync();


            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Complaints.Add(complaint);

                    await ApplyCustomerChanges(complaint);

                    var outboxMessage = new OutboxMessage(complaint, "Es wurde eine Reklamation unter Ihrere Email Adresse eröffnet.");

                    _context.Add(outboxMessage);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            var response = new ComplaintResponse(complaint);

            return CreatedAtAction("GetComplaint", new { id = complaint.Id }, response);
        }

        // DELETE: api/Complaints/5
        /// <summary>
        /// Sets the complaint status of a specified complaint to Canceled.
        /// </summary>
        /// <param name="id">The ID of the complaint to cancel.</param>
        /// <returns>A response indicating the result of the cancel operation.</returns>
        /// <response code="204">No content if the cancellation was successful.</response>
        /// <response code="400">Bad request if the complaint has already been canceled.</response>
        /// <response code="401">Unauthorized if no authentication token is provided.</response>
        /// <response code="403">Forbidden if the provided token does not belong to an admin user.</response>
        /// <response code="404">Not found if the complaint with the specified ID does not exist.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
            {
                return NotFound(new
                {
                    message = "A complaint with the specified id doesn't exist.",
                    id
                });
            }

            if (complaint.Status == ComplaintStatus.Canceled)
            {
                return BadRequest(new
                {
                    message = "The complaint is already canceled.",
                    id
                });
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    complaint.Status = ComplaintStatus.Canceled;
                    _context.Entry(complaint).State = EntityState.Modified;

                    await complaint.InitializeCustomerAsync();

                    var outboxMessage = new OutboxMessage(complaint, "Der Status Ihrer Reklamation wurde auf 'Canceled' geändert.");

                    _context.Add(outboxMessage);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Complaints/search?name=john&status=open
        /// <summary>
        /// Searches through complaints and returns a list of complains having at least one parameter contain the respective provided search parameter.
        /// </summary>
        /// <returns>List of complaints as the result of the search operation.</returns>
        /// <response code="200">OK if the request is successful.</response>
        /// <response code="404">Not found there are no search results.</response>
        [Route("search")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(SearchResponse))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchResponseExample))]
        public async Task<ActionResult<SearchResponse>> Search([SwaggerParameter(Description = "Product id")][FromQuery] int? productId,
            [SwaggerParameter(Description = "Customer full name")][FromQuery] string? customerName,
            [SwaggerParameter(Description = "Customer email adress")][FromQuery] string? customerEmail,
            [SwaggerParameter(Description = "Date of complaint")][FromQuery] string? date,
            [SwaggerParameter(Description = "Complaint description")][FromQuery] string? description,
            [SwaggerParameter(Description = "Complaint status")][FromQuery] string? status,
            [SwaggerParameter(Description = "Perform case-insensitive search?")][FromQuery] bool ignoreCase = true)
        {
            var searchDto = new ComplaintSearchFilterDto(productId, customerName, customerEmail, date, description, status, true, ignoreCase);

            var complaints = await _context.Complaints.ToListAsync();

            await Complaint.InitiliateCustomers(complaints);

            var complaintsResponse = searchDto.SearchComplaint(complaints).Select(c => new ComplaintResponse(c)).ToList();
            var searchResponse = new SearchResponse(searchDto, complaintsResponse);

            if (!searchResponse.Complaints.Any())
            {
                return NotFound(new
                {
                    message = "There are no complaints matching the search criteria.",
                    searchDto
                });
            }

            return searchResponse;
        }

        // GET: api/Complaints/filter?name=john&status=open
        /// <summary>
        /// Searches through complaints and returns a list of complains where EVERY parameter value is contained by the respective property value.
        /// </summary>
        /// <returns>List of complaints as the result of the search operation.</returns>
        /// <response code="200">OK if the request is successful.</response>
        /// <response code="404">Not found there are no filter results.</response>
        [Route("filter")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(FilterResponse))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FilterResponseExample))]
        public async Task<ActionResult<FilterResponse>> Filter([SwaggerParameter(Description = "Product id")][FromQuery] int? productId,
            [SwaggerParameter(Description = "Customer full name")][FromQuery] string? customerName,
            [SwaggerParameter(Description = "Customer email adress")][FromQuery] string? customerEmail,
            [SwaggerParameter(Description = "Date of complaint")][FromQuery] string? date,
            [SwaggerParameter(Description = "Complaint description")][FromQuery] string? description,
            [SwaggerParameter(Description = "Complaint status")][FromQuery] string? status,
            [SwaggerParameter(Description = "Perform case-insensitive filter?")][FromQuery] bool ignoreCase = true)
        {
            var searchDto = new ComplaintSearchFilterDto(productId, customerName, customerEmail, date, description, status, false, ignoreCase);

            var complaints = await _context.Complaints.ToListAsync();

            await Complaint.InitiliateCustomers(complaints);

            var complaintsResponse = searchDto.SearchComplaint(complaints).Select(c => new ComplaintResponse(c)).ToList();
            var filterResponse = new FilterResponse(searchDto, complaintsResponse);

            if (!filterResponse.Complaints.Any())
            {
                return NotFound(new
                {
                    message = "There are no complaints matching the filter criteria.",
                    searchDto
                });
            }
            return filterResponse;
        }


        private async Task ApplyCustomerChanges(Complaint complaint)
        {
            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(customer => customer.Email == complaint.Customer.Email);
            if (existingCustomer != null)
            {
                if (existingCustomer.Email == complaint.Customer.Email && existingCustomer.Name != complaint.Customer.Name)
                {
                    existingCustomer.Name = complaint.Customer.Name;
                    _context.Entry(existingCustomer).State = EntityState.Modified;
                    complaint.Customer = existingCustomer;
                }
            }
            else
            {
                await _context.Customers.AddAsync(complaint.Customer);
            }
        }

        /// <summary>
        /// Checks if a complaint with the specified ID exists.
        /// </summary>
        /// <param name="id">ID of the complaint.</param>
        /// <returns>True if the complaint exists, false otherwise.</returns>
        private bool ComplaintExists(int id)
        {
            return _context.Complaints.Any(e => e.Id == id);
        }
    }
}
