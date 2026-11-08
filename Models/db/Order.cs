using System;
using System.Collections.Generic;

namespace _66076431_natalee.Models.db;

public partial class Order
{
    public int OrderId { get; set; }

    public string? OrderNo { get; set; }

    public string? CusId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? ShippingMethod { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public string? OrderStatus { get; set; }

    public int? SubTotal { get; set; }

    public int? Discount { get; set; }

    public int? ShippingFee { get; set; }

    public int? TotalPrice { get; set; }

    public string? ShipAddress { get; set; }

    public virtual Customer? Cus { get; set; }
    public string? SlipImage { get; set; }
    public string? RejectNote { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
