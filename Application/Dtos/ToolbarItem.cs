using Affiliate.Application.Enums;
using Microsoft.AspNetCore.Components;

namespace Affiliate.Application.Dtos;

public class ToolbarItem
{
    public string? Text { get; set; }
    public string? Href { get; set; }
    public string? ClassList { get; set; }
}