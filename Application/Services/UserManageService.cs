using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Extensions;
using Affiliate.Application.Models;
using Affiliate.Application.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
                Name = userPostFormDto.Name ?? string.Empty,
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
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config page: {Exception}", exception);
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
                userToUpdate.UpdatedAt = DateTime.UtcNow;
                userToUpdate.UpdatedBy = userPostForm.UpdatedBy;
                if (string.IsNullOrEmpty(userToUpdate.Uid))
                {
                    userToUpdate.Uid = GuidExtension.TaoUid();
                }
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
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config page: {Exception}", exception);
            return null;
        }
    }

    public async Task<User?> DeleteUserAsync(int id, Guid updateBy)
    {
        try
        {
            var user = await db.Users.FindAsync(id);
            if (user != null)
            {
                user.IsDelete = true;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = updateBy;
                await db.SaveChangesAsync();
                return user;
            }

            return null;
        }
        catch (Exception e)
        {
            var exception = e.InnerException?.Message ?? e.Message;
            Log.Error("Error updating config page: {Exception}", exception);
            return null;
        }
    }

    public async Task<bool> CheckEmailExists(string email)
    {
        return await db.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<bool> CheckEmailExists(string email, int userId)
    {
        return await db.Users.AnyAsync(x => x.Email == email && x.Id != userId);
    }

    public async Task<bool> CheckUsernameExists(string username)
    {
        return await db.Users.AnyAsync(x => x.Name == username);
    }
}