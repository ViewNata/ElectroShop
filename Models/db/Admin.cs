using System;
using System.Collections.Generic;

namespace _66076431_natalee.Models.db;

public partial class Admin
{
    public string AdminId { get; set; } = null!;

    public string? AdminPassword { get; set; }

    public string? AdminEmail { get; set; }

    public string? AdminName { get; set; }

    public string? AdminPhoneNumber { get; set; }

    public string? AdminRole { get; set; }
}
