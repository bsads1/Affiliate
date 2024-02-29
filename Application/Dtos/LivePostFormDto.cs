using Affiliate.Application.Models;

namespace Affiliate.Application.Dtos;

public class LivePostFormDto : Livestream
{
    public string? StartTimeStr { get; set; }
    public string? CloseTimeStr { get; set; }
    public string? AvailableTimeStartStr { get; set; }
    public string? AvailableTimeEndStr { get; set; }
    public IFormFile File { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool HasImage { get; set; }
}