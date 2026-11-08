namespace _66076431_natalee.ViewModels
{
    public class OrderTrackViewModel
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingMethod { get; set; }
        public int TotalPrice { get; set; }
        public string RejectNote { get; set; }
        public int Discount { get; set; }

        public List<OrderTrackDetailItem> Details { get; set; } = new();
    }

    public class OrderTrackDetailItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
}