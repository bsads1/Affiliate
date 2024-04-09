using System.Globalization;
using Affiliate.Application.Database;
using Affiliate.Application.Dtos;
using Affiliate.Application.Enums;
using Affiliate.Application.Extensions;
using Affiliate.Application.Models;
using Affiliate.Application.Services;
using Microsoft.EntityFrameworkCore;
using Spark.Library.Routing;

namespace Affiliate.Pages.Api;

public class PointApi : IRoute
{
    public void Map(WebApplication app)
    {
        app.MapPut("/api/point",
                async (HttpContext context, TransactionPointService transactionPointService) =>
                {
                    var id = int.TryParse(context.Request.Form["Id"][0], out var i) ? i : 0;
                    var points = long.TryParse(context.Request.Form["Points"][0]?.Replace(".", ""), out var p) ? p : 0;
                    var description = context.Request.Form["Description"][0] ?? "";
                    var money = long.TryParse(context.Request.Form["Money"][0]?.Replace(".", ""), out var m) ? m : 0;
                    var datetime = DateTime.TryParseExact(context.Request.Form["Datetime"][0], "dd/MM/yyyy HH:mm",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                        ? DateTime.SpecifyKind(dt, DateTimeKind.Utc)
                        : DateTime.Now.ToUniversalTime();
                    var byUser = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");

                    var result = points switch
                    {
                        < 0 => await transactionPointService.RemovePointAsync(id, points, money, description, datetime,
                            byUser),
                        > 0 => await transactionPointService.AddPointAsync(id, points, money, description, datetime,
                            byUser),
                        _ => false
                    };

                    var response = new
                    {
                        ok = result,
                        message = result ? "Success" : "Failed"
                    };
                    return Results.Json(response);
                })
            .RequireAuthorization(CustomPolicies.MasterAdminAccess);

        app.MapPost("/api/point/confirm",
            async (OrderPoint form, HttpContext context, IDbContextFactory<DatabaseContext> factory) =>
            {
                try
                {
                    var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");

                    await using var db = await factory.CreateDbContextAsync();
                    await using var trans = await db.Database.BeginTransactionAsync();

                    var orderPoint = db.OrderPoints.FirstOrDefault(p => p.Id == form.Id);
                    if (orderPoint == null)
                    {
                        await trans.DisposeAsync();
                        await db.DisposeAsync();
                        return Results.NotFound(new { ok = false, message = "Order point not found" });
                    }

                    var user = await db.Users.FirstOrDefaultAsync(x => x.Guid == orderPoint.UserGuid);
                    if (user == null)
                    {
                        await trans.DisposeAsync();
                        await db.DisposeAsync();
                        return Results.NotFound(new { ok = false, message = "User not found" });
                    }

                    orderPoint.Status = OrderPointStatus.Completed.ToString();
                    orderPoint.UpdatedAt = DateTime.Now.ToUniversalTime();
                    orderPoint.UpdatedBy = createBy;

                    user.Points += orderPoint.Points;

                    var transactionPoint = new TransactionPoint
                    {
                        UserId = user.Guid,
                        TransactionTimestamp = DateTime.Now.ToUniversalTime(),
                        PointsChanged = orderPoint.Points,
                        TransactionType = TransactionType.Purchase.ToString(),
                        TransactionDescription = orderPoint.OrderUid,
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        UpdatedAt = DateTime.Now.ToUniversalTime(),
                        Guid = Guid.NewGuid(),
                        CreatedBy = createBy,
                        UpdatedBy = createBy,
                        AccountBalance = user.Points,
                        AmountChanged = orderPoint.Amount
                    };
                    db.TransactionPoints.Add(transactionPoint);
                    await db.SaveChangesAsync();
                    await trans.CommitAsync();

                    return Results.Ok(new
                    {
                        ok = true
                    });
                }
                catch (Exception e)
                {
                    return Results.Ok(new
                    {
                        ok = false,
                        message = e.Message
                    });
                }
            }).RequireAuthorization(CustomPolicies.MasterAdminAccess);
        
        app.MapPost("/api/point/refuse",
           async (OrderPoint form, HttpContext context, IDbContextFactory<DatabaseContext> factory) =>
            {
                try
                {
                    var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");

                    await using var db = await factory.CreateDbContextAsync();

                    var orderPoint = db.OrderPoints.FirstOrDefault(p => p.Id == form.Id);
                    if (orderPoint == null)
                    {
                        await db.DisposeAsync();
                        return Results.NotFound(new { ok = false, message = "Order point not found" });
                    }

                    orderPoint.Status = OrderPointStatus.Rejected.ToString();
                    orderPoint.UpdatedAt = DateTime.Now.ToUniversalTime();
                    orderPoint.UpdatedBy = createBy;
                    await db.SaveChangesAsync();

                    return Results.Ok(new
                    {
                        ok = true
                    });
                }
                catch (Exception e)
                {
                    return Results.Ok(new
                    {
                        ok = false,
                        message = e.Message
                    });
                }
            }).RequireAuthorization(CustomPolicies.MasterAdminAccess);
        
        app.MapPost("/api/point/confirm-withdraw",
            async (WithdrawDto form, HttpContext context, IDbContextFactory<DatabaseContext> factory) =>
            {
                try
                {
                    var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");

                    await using var db = await factory.CreateDbContextAsync();
                    await using var trans = await db.Database.BeginTransactionAsync();

                    var withdraw = db.Withdraws.FirstOrDefault(p => p.Id == form.Id);
                    if (withdraw == null)
                    {
                        await trans.DisposeAsync();
                        await db.DisposeAsync();
                        return Results.NotFound(new { ok = false, message = "Withdraw not found" });
                    }

                    var user = await db.Users.FirstOrDefaultAsync(x => x.Guid == withdraw.UserGuid);
                    if (user == null)
                    {
                        await trans.DisposeAsync();
                        await db.DisposeAsync();
                        return Results.NotFound(new { ok = false, message = "User not found" });
                    }

                    var text = "Rút " + form.Amount.AddDecimalSeparator() + " VND tương đương " + form.Points.AddDecimalSeparator() + " điểm về tài khoản " + withdraw.AccountNumber + " - " + withdraw.BankName;
                    withdraw.Status = OrderPointStatus.Completed.ToString();
                    withdraw.Note = form.Note != null ? form.Note + " \n " + text : text;
                    withdraw.Points = form.Points;
                    withdraw.UpdatedAt = DateTime.Now.ToUniversalTime();
                    withdraw.UpdatedBy = createBy;

                    user.Points -= form.Points;

                    var transactionPoint = new TransactionPoint
                    {
                        UserId = user.Guid,
                        TransactionTimestamp = DateTime.Now.ToUniversalTime(),
                        PointsChanged = form.Points,
                        TransactionType = TransactionType.Withdrawal.ToString(),
                        TransactionDescription = form.Note != null ? form.Note + " \n " + text : text,
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        UpdatedAt = DateTime.Now.ToUniversalTime(),
                        Guid = Guid.NewGuid(),
                        CreatedBy = createBy,
                        UpdatedBy = createBy,
                        AccountBalance = user.Points,
                        AmountChanged = form.Amount
                    };
                    db.TransactionPoints.Add(transactionPoint);
                    await db.SaveChangesAsync();
                    await trans.CommitAsync();
                    await db.DisposeAsync();
                    return Results.Ok(new
                    {
                        ok = true
                    });
                }
                catch (Exception e)
                {
                    return Results.Ok(new
                    {
                        ok = false,
                        message = e.Message
                    });
                }
            }).RequireAuthorization(CustomPolicies.MasterAdminAccess);

        app.MapPost("/api/point/refuse-withdraw",
            async (WithdrawDto form, HttpContext context, IDbContextFactory<DatabaseContext> factory) =>
            {
                try
                {
                    var createBy = Guid.Parse(context.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value ?? "0");

                    await using var db = await factory.CreateDbContextAsync();

                    var withdraw = db.Withdraws.FirstOrDefault(p => p.Id == form.Id);
                    if (withdraw == null)
                    {
                        await db.DisposeAsync();
                        return Results.NotFound(new { ok = false, message = "Withdraw not found" });
                    }

                    withdraw.Status = OrderPointStatus.Rejected.ToString();
                    withdraw.Note = form.Note ?? "Từ chối do thông tin không chính xác";
                    withdraw.UpdatedAt = DateTime.Now.ToUniversalTime();
                    withdraw.UpdatedBy = createBy;
                    await db.SaveChangesAsync();

                    await db.DisposeAsync();
                    return Results.Ok(new
                    {
                        ok = true
                    });
                }
                catch (Exception e)
                {
                    return Results.Ok(new
                    {
                        ok = false,
                        message = e.Message
                    });
                }
            }).RequireAuthorization(CustomPolicies.MasterAdminAccess);
    }
}