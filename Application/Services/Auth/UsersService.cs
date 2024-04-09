using Affiliate.Application.Database;
using Affiliate.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Affiliate.Application.Services.Auth;

public class UsersService(DatabaseContext db)
{
    public async Task<User?> FindUserAsync(int userId)
    {
        return await db.Users.FindAsync(userId);
    }

    public async Task<User?> FindUserByCredsAsync(string email, string password)
    {
        return await db.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
    }

    public async Task<User?> FindUserByEmailAsync(string email)
    {
        return await db.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var addedUser = await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        var role = db.Roles.FirstOrDefault(p => p.Name == CustomRoles.User);
        if (role == null)
        {
            role = new Role
            {
                Name = CustomRoles.User
            };
            db.Roles.Add(role);
            await db.SaveChangesAsync();
        }

        await db.UserRoles.AddAsync(new UserRole
        {
            RoleId = role.Guid, User = user
        });
        await db.SaveChangesAsync();
        return addedUser.Entity;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await db.Users
            .ToListAsync();
    }

    public string GetSha256Hash(string input)
    {
        using var hashAlgorithm = SHA256.Create();
        var byteValue = Encoding.UTF8.GetBytes(input);
        var byteHash = hashAlgorithm.ComputeHash(byteValue);
        return Convert.ToBase64String(byteHash);
    }
}