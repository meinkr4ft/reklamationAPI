using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamationAPI.Models
{
    /// <summary>
    /// Customer model class for database interaction.
    /// </summary>
    public class Customer(string email, string name)
    {
        [Key]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "Please enter an email up to 100 characters.")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; } = email;
        [StringLength(40, ErrorMessage = "Please enter an email up to 40 characters.")]
        public string Name { get; set; } = name;

        public override bool Equals(object? obj)
        {
            if (obj is not Customer other) return false;
            return this.Email == other.Email && this.Name == other.Name;
        }

        /// <summary>
        /// Creates a shallow copy of this object.
        /// </summary>
        /// <returns>Shallow copy.</returns>
        public Customer ShallowCopy()
        {
            return (Customer)this.MemberwiseClone();
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
