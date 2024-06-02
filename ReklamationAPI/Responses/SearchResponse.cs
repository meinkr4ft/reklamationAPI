using ReklamationAPI.dto;

namespace ReklamationAPI.responses
{
    /// <summary>
    /// Response class containing results of the search operation.
    /// </summary>
    public class SearchResponse(ComplaintSearchFilterDto searchDto, IEnumerable<ComplaintResponse> complaints)
    {
        public ComplaintSearchFilterDto SearchDto { get; set; } = searchDto;
        public IEnumerable<ComplaintResponse> Complaints { get; set; } = complaints;
    }
}
