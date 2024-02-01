using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Models;
using Affiliate.Application.Services.Auth;
using Affiliate.Pages.Profile;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Affiliate.Application.Services;

public class UserManageService(DatabaseContext db, UsersService usersService)
{
 
    
    public async Task<User?> CreateUserAsync(UserPostFormDto userPostFormDto)
    {
        try
        {
            
            var newUser = new User
            {
                Guid = Guid.NewGuid(),
                Name = userPostFormDto.Name,
                Points = 0,
                Email = userPostFormDto.Email,
                Password = usersService.GetSha256Hash(userPostFormDto.Password),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userPostFormDto.CreatedBy
            };

            db.Users.Add(newUser);
            if (userPostFormDto.Roles != null && userPostFormDto.Roles.Any())
            { 

                foreach (var role in userPostFormDto.Roles)
                {
                    db.UserRoles.Add(new UserRole
                    {
                        UserId = newUser.Guid,
                        RoleId = role
                    });
                }
            }
            
            await db.SaveChangesAsync();

            return newUser;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<User?> UpdateUserAsync(UserPostFormDto userPostForm)
    {
        try
        {
            var userToUpdate = await db.Users.FindAsync(userPostForm.Id);
            if (userToUpdate != null)
            {
                userToUpdate.Email = userPostForm.Email;
                userToUpdate.Points = userPostForm.Points ?? 0;
                userToUpdate.UpdatedAt = DateTime.UtcNow;
                userToUpdate.UpdatedBy = userPostForm.UpdatedBy;
                if (userPostForm.Roles != null && userPostForm.Roles.Any())
                {
                    var userRoles = await db.UserRoles.Where(x => x.UserId == userToUpdate.Guid).ToListAsync();
                    db.UserRoles.RemoveRange(userRoles);

                    foreach (var role in userPostForm.Roles)
                    {
                        db.UserRoles.Add(new UserRole
                        {
                            UserId = userToUpdate.Guid,
                            RoleId = role
                        });
                    }

                    await db.SaveChangesAsync();
                }
                else
                {
                    await db.SaveChangesAsync();
                }

                return userToUpdate;
            }

            return null;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<bool> CheckEmailExists(string email)
    {
        return await db.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<bool> CheckUsernameExists(string username)
    {
        return await db.Users.AnyAsync(x => x.Name == username);
    }
}