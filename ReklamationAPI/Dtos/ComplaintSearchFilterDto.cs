using ReklamationAPI.Models;

namespace ReklamationAPI.dto
{
    /// <summary>
    /// Class whose objects represent the parameter of one search / filter operation.
    /// </summary>
    public class ComplaintSearchFilterDto
    {
        /// <summary>
        /// search or filter operation
        /// </summary>
        public string Operation { get; set; }
        public string? ProductId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? Date { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        private bool HasCriteria { get; set; }
        private bool IsSearch { get; set; }
        public bool IgnoreCase { get; set; }
        public ComplaintSearchFilterDto(int? productId, string? customerName, string? customerEmail, string? date, string? description, string? status, bool isSearch, bool ignoreCase)
        {
            Operation = isSearch ? "search" : "filter";
            ProductId = productId?.ToString();
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            Date = date;
            Description = description;
            Status = status;
            IsSearch = isSearch;
            IgnoreCase = ignoreCase;
            HasCriteria = ProductId != null || CustomerName != null || CustomerEmail != null || Date != null | Description != null || Status != null;
        }

        public ComplaintSearchFilterDto(bool search) : this(null, null, null, null, null, null, search, true) { }

        public ComplaintSearchFilterDto(Complaint complaint, bool isSearch, bool ignoreCase) : this(complaint.ProductId, complaint.Customer.Name, complaint.Customer.Email,
            complaint.Date.ToString(), complaint.Description, complaint.Status.ToString(), isSearch, ignoreCase)
        { }

        /// <summary>
        /// Evaluates if a given complaing matches this search object.
        /// Only non-null parameter values are checked.
        /// </summary>
        /// <param name="complaint">Complaint to check.</param>
        /// <returns>True, if the complaint matches all the search parameter.</returns>
        private bool Search(Complaint complaint)
        {
            return (ProductId != null && Contains(complaint.ProductId.ToString(), ProductId)) ||
                   (CustomerName != null && Contains(complaint.Customer.Name, CustomerName)) ||
                   (CustomerEmail != null && Contains(complaint.Customer.Email, CustomerEmail)) ||
                   (Date != null && Contains(complaint.Date.ToString(), Date)) ||
                   (Description != null && Contains(complaint.Description, Description)) ||
                   (Status != null && Contains(complaint.Status.ToString(), Status));
        }

        /// <summary>
        /// Evaluates if a given complaing matches this filter object.
        /// Only non-null parameter values are checked.
        /// </summary>
        /// <param name="complaint">Complaint to check.</param>
        /// <returns>True, if the complaint matches all the filter parameter.</returns>
        private bool Filter(Complaint complaint)
        {
            return (ProductId == null || Contains(complaint.ProductId.ToString(), ProductId)) &&
                   (CustomerName == null || Contains(complaint.Customer.Name, CustomerName)) &&
                   (CustomerEmail == null || Contains(complaint.Customer.Email, CustomerEmail)) &&
                   (Date == null || Contains(complaint.Date.ToString(), Date)) &&
                   (Description == null || Contains(complaint.Description, Description)) &&
                   (Status == null || Contains(complaint.Status.ToString(), Status));
        }

        /// <summary>
        /// Helper method for insensitive string compare. 
        /// </summary>
        /// <param name="str">Base string.</param>
        /// <param name="searchStr">Serch string.</param>
        /// <returns>True, if base string contains search string (case insensitive).</returns>
        private bool Contains(string str, string searchStr)
        {
            return IgnoreCase ? str.Contains(searchStr, StringComparison.OrdinalIgnoreCase) : str.Contains(searchStr);
        }

        /// <summary>
        /// Applies this search/filter object on a given set of complaints.
        /// </summary>
        /// <param name="complaints">Collection of complaints to be checked.</param>
        /// <returns>A collection of complaints who match this objects criteria.</returns>
        public IEnumerable<Complaint> SearchComplaint(IEnumerable<Complaint> complaints)
        {
            return complaints.Where(complaint =>
            {
                if (HasCriteria)
                {
                    return IsSearch ? Search(complaint) : Filter(complaint);
                }
                else
                {
                    return true;
                }
            });
        }

        /// <summary>
        /// Custom Equals implementation.
        /// </summary>
        /// <param name="obj">Other.</param>
        /// <returns>True, if both objects represent the same search / filter operation.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not ComplaintSearchFilterDto other) return false;
            return this.ProductId == other.ProductId && this.CustomerName == other.CustomerName && this.CustomerEmail == other.CustomerEmail
                && this.Date == other.Date && this.Description == other.Description && this.Status == other.Status && this.IsSearch == other.IsSearch
                && this.IgnoreCase == other.IgnoreCase;
        }

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
