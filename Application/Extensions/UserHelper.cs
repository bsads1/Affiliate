using System.Security.Claims;

namespace Affiliate.Application.Extensions;

public class UserHelper(IHttpContextAccessor contextAccessor)
{
    public List<string> GetRoles()
    {
        var user = contextAccessor.HttpContext?.User;
        return user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value).ToList();
    }

    public bool IsInRole(string role)
    {
        var roles = GetRoles();
        return roles.Contains(role);
    }
}