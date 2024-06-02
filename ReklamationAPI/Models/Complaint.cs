using ReklamationAPI.dto;
using ReklamationAPI.Validation;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReklamationAPI.data;

namespace ReklamationAPI.Models

{
    /// <summary>
    /// Complaint model class for database interaction.
    /// </summary>
    public class Complaint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [SwaggerIgnore]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public Customer Customer { get; set; }
        [ForeignKey("Customer")]
        public string CustomerEmail { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; }
        [ValidComplaintStatus(ErrorMessage = "Complaint status is required, possible values: Open, InProgress, Rejected, Accepted, Canceled")]
        public ComplaintStatus Status { get; set; }

        public Complaint(int productId, Customer customer, string customerEmail, DateOnly date, string description, ComplaintStatus status)
        {
            Id = 0;
            ProductId = productId;
            Customer = customer;
            CustomerEmail = customerEmail;
            Date = date;
            Description = description;
            Status = status;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Complaint() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Loads the customer entity this object is referring to.
        /// This is because it is disabled by default for performance reasons.
        /// </summary>
        public async Task InitializeCustomerAsync()
        {
            if (this.Customer == null)
            {
                using var dbContext = new AppDbContext();
                var customerData = await dbContext.Customers.FindAsync(CustomerEmail);
                ArgumentNullException.ThrowIfNull(customerData);
                this.Customer = new Customer(CustomerEmail, customerData.Name);
            }
        }

        /// <summary>
        /// Loads all referenced customer entities for a given set of complaints.
        /// </summary>
        /// <param name="complaints">Complaints to have their customer entities loaded.</param>
        /// <returns>Complaints where all customer entities are loaded.</returns>
        public static async Task InitiliateCustomers(IEnumerable<Complaint> complaints)
        {
            foreach (var complaint in complaints)
            {
                await complaint.InitializeCustomerAsync();
            }
        }

        /// <summary>
        /// Copies the values of a dto to this object.
        /// </summary>
        /// <param name="dto">Dto to be copied from.</param>
        public void CopyValues(ComplaintDto dto)
        {
            this.ProductId = dto.ProductId;
            this.Customer.Email = dto.Customer.Email;
            this.Customer.Name = dto.Customer.Name;
            this.Description = dto.Description;
            this.Date = dto.Date;
            this.Status = Enum.Parse<ComplaintStatus>(dto.Status);
        }

        /// <summary>
        /// Creates a shallow copy of this object.
        /// </summary>
        /// <returns>Shallow copy.</returns>
        public Complaint ShallowCopy()
        {
            return (Complaint)this.MemberwiseClone();
        }

        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        /// <returns>Deep copy.</returns>
        public Complaint DeepCopy()
        {
            var other = (Complaint)this.MemberwiseClone();
            other.Customer = this.Customer.ShallowCopy();
            return other;
        }

        /// <summary>
        /// Custom implementation for Euqals.
        /// </summary>
        /// <param name="obj">Other.</param>
        /// <returns>True, if both objects represent identical complaints.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not Complaint other) return false;
            return this.ProductId == other.ProductId && this.Customer.Equals(other.Customer) && this.Date == other.Date
                && this.Description == other.Description && this.Status == other.Status;
        }

        /// <summary>
        /// Constructor for converting from a dto. 
        /// </summary>
        /// <param name="dto">A dto that encapsulates complaint data.</param>
        public Complaint(ComplaintDto dto) : this(dto.ProductId, dto.Customer, dto.Customer.Email,
            dto.Date, dto.Description, dto.GetStatus())
        { }

        /// <summary>
        /// Parent HashCode implementation.
        /// </summary>
        /// <returns>HashCode.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
