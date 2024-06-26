﻿using Affiliate.Application.Dtos;
using Affiliate.Application.Models;

namespace Affiliate.Pages.Components;

public class PageState
{
    public User User { get; set; } = new();
    public string AppName { get; set; }
    public ConfigPostFormDto? Config { get; set; }
}