using ReklamationAPI.dto;
using ReklamationAPI.Models;
using ReklamationAPI.responses;
using Swashbuckle.AspNetCore.Filters;


namespace ReklamationAPI.Swagger
{
    /// <summary>
    /// class providing an example search response for swagger response documentation.
    /// </summary>
    public class SearchResponseExample : IExamplesProvider<SearchResponse>
    {
        public SearchResponse GetExamples()
        {
            var searchDto = new ComplaintSearchFilterDto(null, "Max", null, null, null, "Open", true, true);

            var customer1 = new Customer("max.schmidt@gmail.com", "Max Schmidt");
            var complaint1 = new ComplaintResponse(1, 101, customer1, new DateOnly(2023, 5, 28), "Das Produkt funktioniert nicht wie erwartet.", "Open");

            var customer2 = new Customer("maximilian.weber@web.de", "Maximilian Weber");
            var complaint2 = new ComplaintResponse(2, 54, customer2, new DateOnly(2023, 4, 21), "Die Lieferung kam nicht an.", "Rejected");

            var customer3 = new Customer("Sabine.Wagner@yahoo.de", "Sabine Wagner");
            var complaint3 = new ComplaintResponse(3, 20, customer3, new DateOnly(2023, 3, 12), "Falsches Produkt wurde geliefert.", "Open");


            var complaints = new ComplaintResponse[] { complaint1, complaint2, complaint3 };
            var result = new SearchResponse(searchDto, complaints);
            return result;
        }
    }
}
