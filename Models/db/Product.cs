using System;
using System.Collections.Generic;

namespace _66076431_natalee.Models.db;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string? ProductName { get; set; }

    public string? Brand { get; set; }

    public string? Category { get; set; }

    public int? CostPrice { get; set; }

    public int? SellPrice { get; set; }

    public int? DiscountPrice { get; set; }

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? Warranty { get; set; }

    public string? DescriptionPd { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
