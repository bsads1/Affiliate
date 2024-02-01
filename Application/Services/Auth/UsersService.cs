using Affiliate.Application.Database;
using Affiliate.Application.Models;
using Affiliate.Application.Startup;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Affiliate.Application.Services.Auth;

public class UsersService
{
    private readonly DatabaseContext _db;

    public UsersService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<User?> FindUserAsync(int userId)
    {
        return await _db.Users.FindAsync(userId);
    }

    public async Task<User?> FindUserByCredsAsync(string email, string password)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
    }

    public async Task<User?> FindUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var addedUser = await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        var role = _db.Roles.FirstOrDefault(p=>p.Name == CustomRoles.User);
        if (role == null)
        {
            role = new Role
            {
                Name = CustomRoles.User
            };
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
        }
        
        await _db.UserRoles.AddAsync(new UserRole
        {
            RoleId = role.Guid, User = user
        });
        await _db.SaveChangesAsync();
        return addedUser.Entity;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Users
                .ToListAsync();
    }

    public string GetSha256Hash(string input)
    {
        using (var hashAlgorithm = SHA256.Create())
        {
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }
    }
}
