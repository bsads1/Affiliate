using System.Text.Json;
using System.Text.Json.Serialization;
using Affiliate.Application.Database;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Affiliate.Application.Services.Hubs;

public class BetHub : Hub
{
    private readonly IHttpContextAccessor _accessor;
    private readonly BetService _betService;
    private readonly LivestreamService _livestreamService;

    public BetHub(IHttpContextAccessor accessor, BetService betService, LivestreamService livestreamService)
    {
        _accessor = accessor;
        _betService = betService;
        _livestreamService = livestreamService;
    }

    public async Task SendDataUpdate(string data)
    {
        try
        {
            var createBy = Guid.Empty;
            var userGuidClaim = _accessor.HttpContext.User.Claims.FirstOrDefault(p => p.Type == "Guid")?.Value;
            if (!string.IsNullOrEmpty(userGuidClaim))
                createBy = Guid.Parse(userGuidClaim);

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            var form = JsonSerializer.Deserialize<PostData>(data, options);
            if (form != null)
            {
                form.CreateBy = createBy;
                var live = await _livestreamService.GetLiveAsync(form.Live);
                form.LiveGuid = live?.Guid ?? Guid.Empty;
            }
            else
            {
                Log.Error("Form data is null after deserialization.");
            }

            var result = await _betService.BetAsync(form);
            if (result.Success)
                await Clients.All.SendAsync("ReceiveDataUpdate", result.Data);
            else
            {
                Log.Error("Bet operation failed.");
            }
        }
        catch (Exception ex)
        {
            Log.Error("An error occurred in SendDataUpdate method.", ex); 
        }
    }

}

public sealed class PostData
{
    public int Live { get; set; }
    public int Bet { get; set; }
    public double RatioBet { get; set; }
    public double RatioWon { get; set; }
    public long PointBet { get; set; }
    public long PointGet { get; set; }
    public Guid CreateBy { get; set; }
    public Guid LiveGuid { get; set; }
}