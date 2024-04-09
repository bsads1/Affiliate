using Affiliate.Application.Database;
using Affiliate.Application.Models;
using Affiliate.Application.Startup;
using Microsoft.EntityFrameworkCore;

namespace Affiliate.Application.Services.Auth;

public class RolesService
{
    private readonly DatabaseContext _db;

    public RolesService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<List<Role>> FindUserRolesAsync(Guid userId)
    {
        var roles = await _db.Roles.Where(role => role.UserRoles.Any(x => x.UserId == userId)).ToListAsync();
        return roles;
    }

    public async Task<bool> IsUserInRole(Guid userId, string roleName)
    {
        var userRolesQuery = from role in _db.Roles
            where role.Name == roleName
            from user in role.UserRoles
            where user.UserId == userId
            select role;
        var userRole = await userRolesQuery.FirstOrDefaultAsync();
        return userRole != null;
    }

    public async Task<List<User>> FindUsersInRoleAsync(string roleName)
    {
        var roleUserIdsQuery = from role in _db.Roles
            where role.Name == roleName
            from user in role.UserRoles
            select user.UserId;
        return await _db.Users.Where(user => roleUserIdsQuery.Contains(user.Guid))
            .ToListAsync();
    }
}