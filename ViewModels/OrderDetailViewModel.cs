namespace _66076431_natalee.ViewModels
{
    public class OrderDetailViewModel
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }

        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public string ShippingMethod { get; set; }
        public string PaymentMethod { get; set; }
        public int SubTotal { get; set; }
        public int Discount { get; set; }
        public string SlipImage { get; set; }
        public string? RejectNote { get; set; }
    }
}