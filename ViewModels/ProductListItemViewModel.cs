// namespace _66076431_natalee.ViewModels
// {
//   public class ProductListItemViewModel
//   {
//     public int ProductId { get; set; }

//     public string ProductName { get; set; }
//     public int Quantity { get; set; }
//     public string Unit { get; set; }
//     public int StockLeft { get; set; }
//   }
// }


namespace _66076431_natalee.ViewModels
{
    public class ProductListItemViewModel
    {
        public string ProductId   { get; set; }
        public string ProductName { get; set; }
        public string Brand       { get; set; }
        public string Category    { get; set; }
        public int    SellPrice   { get; set; }
        public int    Quantity    { get; set; }
        public string Unit        { get; set; }
        public string ImageUrl    { get; set; }
    }
}
