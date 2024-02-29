using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Extensions;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class AccountApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapPost("/api/account/edit",
                async (UserManageService userManageService, HttpContext context) =>
                {
                    try
                    {
                        var id = int.TryParse(context.Request.Form["Id"][0], out var i) ? i : 0;
                        var email = context.Request.Form["Email"][0];
                        var roles = context.Request.Form["Roles"];
                        var roleGuids = new List<Guid>();
                        if (roles.Any())
                        {
                            roleGuids = roles.Select(p => p.ToGuid()).ToList();
                        }

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            return Results.Problem("Email cannot be empty");
                        }

                        //check email
                        if (!new EmailAddressAttribute().IsValid(email))
                        {
                            return Results.Problem("Invalid email address");
                        }

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            return Results.Problem("Email cannot be empty");
                        }

                        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
                        {
                            return Results.Problem("Invalid email address");
                        }

                        //check email exists
                        var existEmail = await userManageService.CheckEmailExists(email, id);
                        if (existEmail)
                        {
                            return Results.Problem("Email already exists");
                        }

                        var result = await userManageService.UpdateUserAsync(new UserPostFormDto()
                        {
                            Id = id,
                            Email = email,
                            Roles = roleGuids,
                            UpdatedBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ??
                                                   "0")
                        });
                        if (result != null)
                            return Results.Ok(new
                            {
                                Ok = true,
                                Id = result.Id,
                                Points = result.Points
                            });
                        return Results.Ok(new
                        {
                            Ok = false
                        });
                    }
                    catch (Exception e)
                    {
                        return Results.Problem(e.Message);
                    }
                })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);

        app.MapPost("/api/account/create",
                async (IFormCollection request, UserManageService userManageService, HttpContext context) =>
                {
                    try
                    {
                        var email = request["Email"][0];
                        var password = request["Password"][0];
                        var username = request["Name"][0];
                        var repeatPassword = request["RepeatPassword"][0];
                        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(repeatPassword))
                        {
                            return Results.Problem("Password cannot be empty");
                        }

                        if (password != repeatPassword)
                        {
                            return Results.Problem("Password does not match");
                        }

                        //check if email is valid
                        if (!new EmailAddressAttribute().IsValid(email))
                        {
                            return Results.Problem("Invalid email address");
                        }

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            return Results.Problem("Email cannot be empty");
                        }

                        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
                        {
                            return Results.Problem("Invalid email address");
                        }

                        //check if username is valid
                        if (string.IsNullOrWhiteSpace(username))
                        {
                            return Results.Problem("Username cannot be empty");
                        }

                        var minLength = 3;
                        var maxLength = 30;
                        if (username.Length < minLength || username.Length > maxLength)
                        {
                            return Results.Problem(
                                $"Username must be between {minLength} and {maxLength} characters long");
                        }

                        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
                        {
                            return Results.Problem("Username can only contain alphanumeric characters");
                        }

                        //check email exists
                        var existEmail = await userManageService.CheckEmailExists(email.ToString());
                        if (existEmail)
                        {
                            return Results.Problem("Email already exists");
                        }

                        var existUsername = await userManageService.CheckUsernameExists(username.ToString());
                        if (existUsername)
                        {
                            return Results.Problem("Username already exists");
                        }

                        var roles = context.Request.Form["Roles"];
                        var roleGuids = new List<Guid>();
                        if (roles.Any())
                        {
                            roleGuids = roles.Select(p => p.ToGuid()).ToList();
                        }

                        var result = await userManageService.CreateUserAsync(new UserPostFormDto()
                        {
                            Email = email,
                            Name = username,
                            Password = password,
                            Roles = roleGuids,
                            CreatedBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ??
                                                   "0")
                        });

                        if (result != null)
                            return Results.Ok(new
                            {
                                Ok = true,
                                Id = result.Id,
                                Points = result.Points
                            });
                        return Results.Ok(new
                        {
                            Ok = false
                        });
                    }
                    catch (Exception e)
                    {
                        return Results.Problem(e.Message);
                    }
                })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);

        //delete
        app.MapDelete("/api/account", async (UserManageService userManageService, HttpContext context) =>
            {
                try
                {
                    var isParsed = int.TryParse(context.Request.Form["Id"][0], out var id);
                    if (isParsed)
                    {
                        var createBy =
                            Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");
                        var result = await userManageService.DeleteUserAsync(id, createBy);
                        if (result != null)
                        {
                            return Results.Ok(new
                            {
                                Ok = true
                            });
                        }
                    }

                    return Results.Ok(new
                    {
                        Ok = false
                    });
                }
                catch (Exception e)
                {
                    return Results.Problem(e.Message);
                }
            })
            .DisableAntiforgery()
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);
    }
}