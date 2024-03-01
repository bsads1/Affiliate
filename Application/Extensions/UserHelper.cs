using System.Security.Claims;

namespace Affiliate.Application.Extensions;

public class UserHelper
{
    private IHttpContextAccessor _contextAccessor;
    public UserHelper(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public List<string> GetRoles()
    {
        var user = _contextAccessor.HttpContext?.User;
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