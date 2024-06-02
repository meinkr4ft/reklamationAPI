using ReklamationAPI.dto;
using ReklamationAPI.Models;
using ReklamationAPI.responses;
using Swashbuckle.AspNetCore.Filters;


namespace ReklamationAPI.Swagger
{
    /// <summary>
    /// class providing an example filter response for swagger response documentation.
    /// </summary>
    public class FilterResponseExample : IExamplesProvider<FilterResponse>
    {
        public FilterResponse GetExamples()
        {
            var searchDto = new ComplaintSearchFilterDto(null, "Alex", null, null, "liefer", null, false, true);

            var customer1 = new Customer("alex.schmidt@gmail.com", "Alex Schmidt");
            var complaint1 = new ComplaintResponse(1, 101, customer1, new DateOnly(2023, 5, 28), "Falsches Produkt wurde geliefert.", "Open");

            var customer2 = new Customer("alexander.weber@web.de", "Alexander Weber");
            var complaint2 = new ComplaintResponse(2, 54, customer2, new DateOnly(2023, 4, 21), "Die Lieferung kam nicht an.", "Rejected");

            var customer3 = new Customer("Alexandra.Wagner@yahoo.de", "Alexandra Wagner");
            var complaint3 = new ComplaintResponse(3, 20, customer3, new DateOnly(2023, 3, 18), "Das Produkt wurde zwei Mal geliefert.", "Open");


            var complaints = new ComplaintResponse[] { complaint1, complaint2, complaint3 };
            var result = new FilterResponse(searchDto, complaints);
            return result;
        }
    }
}
