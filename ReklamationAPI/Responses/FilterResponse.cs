using ReklamationAPI.dto;

namespace ReklamationAPI.responses
{
    /// <summary>
    /// Response class containing results of the filter operation.
    /// </summary>
    public class FilterResponse(ComplaintSearchFilterDto searchDto, IEnumerable<ComplaintResponse> complaints)
    {
        public ComplaintSearchFilterDto SearchDto { get; set; } = searchDto;
        public IEnumerable<ComplaintResponse> Complaints { get; set; } = complaints;
    }
}
