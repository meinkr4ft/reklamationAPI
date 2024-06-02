using ReklamationAPI.Models;

namespace ReklamationAPI.responses
{
    /// <summary>
    /// Complain class containing its Id for sendingComplain Data as a response´.
    /// </summary>
    public class ComplaintResponse(int id, int productId, Customer customer, DateOnly date, string description, string status)
    {
        public int Id { get; set; } = id;
        public int ProductId { get; set; } = productId;
        public Customer Customer { get; set; } = customer;
        public DateOnly Date { get; set; } = date;
        public string Description { get; set; } = description;
        public string Status { get; set; } = status;

        /// <summary>
        /// Constructor for extracting data from an Complaint object.
        /// </summary>
        /// <param name="c">The complaint object to convert.</param>
        public ComplaintResponse(Complaint c) : this(c.Id, c.ProductId, c.Customer, c.Date, c.Description, c.Status.ToString()) { }
    }
}
