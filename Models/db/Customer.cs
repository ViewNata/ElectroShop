using System;
using System.Collections.Generic;

namespace _66076431_natalee.Models.db;

public partial class Customer
{
    public string CusId { get; set; } = null!;

    public string? CusPassword { get; set; }

    public string? CusrEmail { get; set; }

    public string? CusName { get; set; }

    public string? CusPhoneNumber { get; set; }

    public string? CusAddress { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
