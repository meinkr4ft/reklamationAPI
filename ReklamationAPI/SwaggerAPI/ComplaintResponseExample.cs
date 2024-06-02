using ReklamationAPI.Models;
using ReklamationAPI.responses;
using Swashbuckle.AspNetCore.Filters;

namespace ReklamationAPI.Swagger
{
    /// <summary>
    /// class providing an example complaint response dto's for swagger response documentation.
    /// </summary>
    public class ComplaintResponseExample : IExamplesProvider<ComplaintResponse>
    {
        public ComplaintResponse GetExamples()
        {
            var complaint = new ComplaintResponse(1, 101, new Customer("john.doe@example.com", "John Doe"), new DateOnly(2023, 5, 28), 
                "Das Produkt funktioniert nicht wie erwartet.", "Open");
            return complaint;
        }
    }

    /// <summary>
    /// Class providing an example array of complaint responses for swagger response documentation.
    /// </summary>
    public class ComplaintResponseArrayExample : IExamplesProvider<ComplaintResponse[]>
    {
        public ComplaintResponse[] GetExamples()
        {
            var complaint = new ComplaintResponse(1, 101, new Customer("john.doe@example.com", "John Doe"),
                new DateOnly(2023, 5, 28), "Das Produkt funktioniert nicht wie erwartet.", "Open");

            var complaint2 = new ComplaintResponse(2, 54, new Customer("max.mustermann@example.com", "Max Mustermann"),
                new DateOnly(2023, 4, 21), "Die Lieferung kam nicht an.", "Accepted");

            var complaints = new ComplaintResponse[] { complaint, complaint2 };
            return complaints;
        }
    }
}
