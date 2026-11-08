namespace _66076431_natalee.ViewModels
{
    public class CartItemViewModel
    {
        public string ProductId   { get; set; }
        public string ProductName { get; set; }
        public int    Price       { get; set; }
        public int    Quantity    { get; set; }
        public int    Total       => Price * Quantity;
    }
}
