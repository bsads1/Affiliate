using Affiliate.Application.Models;

namespace Affiliate.Application.Dtos;

public class OrderPointDto : OrderPoint
{
    public string UserName { get; set; }
    public string Email { get; set; }
}