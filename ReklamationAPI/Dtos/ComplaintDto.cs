using ReklamationAPI.Models;
using ReklamationAPI.Validation;
using System.ComponentModel.DataAnnotations;

namespace ReklamationAPI.dto
{
    /// <summary>
    /// Data transfer object with validation for receiving create / update request.
    /// </summary>
    public class ComplaintDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a positive product id.")]
        public int ProductId { get; set; }
        public Customer Customer { get; set; }
        public DateOnly Date { get; set; }
        [StringLength(255, ErrorMessage = "Please enter a description up to 255 characters.")]
        public string Description { get; set; }
        [ValidComplaintStatus(ErrorMessage = "Complaint status is required, possible values: Open, InProgress, Rejected, Accepted, Canceled")]
        public string Status { get; set; }

        public ComplaintDto(int productId, Customer customer, DateOnly date, string description, string status)
        {
            ProductId = productId;
            Customer = customer;
            Date = date;
            Description = description;
            Status = status;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ComplaintDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public ComplaintDto(Complaint complaint)
        {
            this.ProductId = complaint.ProductId;
            this.Customer = complaint.Customer;
            this.Date = complaint.Date;
            this.Description = complaint.Description;
            this.Status = complaint.Status.ToString();
        }
        /// <summary>
        /// Utility for converting and objects string representation of the complaint status to an enum value.
        /// </summary>
        /// <returns>Enum value of somplaint status.</returns>
        public ComplaintStatus GetStatus()
        {
            if (Enum.TryParse<ComplaintStatus>(this.Status, out ComplaintStatus parsedStatus))
            {
                return parsedStatus;
            }

            throw new InvalidDataException($"Received invalid status value: {this.Status}");
        }
    }
}
