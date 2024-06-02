using ReklamationAPI.dto;
using ReklamationAPI.Models;
using Swashbuckle.AspNetCore.Filters;

namespace ReklamationAPI.Swagger
{
    /// <summary>
    /// Class providing an example complaint dto for swagger request documentation.
    /// </summary>
    public class ComplaintDtoExmaple : IExamplesProvider<ComplaintDto>
    {

        public ComplaintDto GetExamples()
        {
            var customer = new Customer("john.doe@example.com", "John Doe");

            var complaint = new ComplaintDto(101, customer, new DateOnly(2023, 5, 28), "Das Produkt funktioniert nicht wie erwartet.", "Open");
            return complaint;
        }
    }
}
