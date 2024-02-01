using Microsoft.Extensions.Primitives;

namespace Affiliate.Application.Dtos;

public class UserPostFormDto
{
    public int? Id { get; set; } = default!;
    public string? Name { get; set; } = default!;
    public long? Points { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
    public List<Guid>? Roles { get; set; } = default!;

    public IFormFile? File { get; set; } = default!;
    public string? RolesString { get; set; } = default!;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}