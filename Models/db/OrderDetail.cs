using System;
using System.Collections.Generic;

namespace _66076431_natalee.Models.db;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public string? ProductId { get; set; }

    public int? Quantity { get; set; }

    public int? UnitPrice { get; set; } //ราคาต่อชิ้นของสินค้า

    public int? TotalPrice { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
