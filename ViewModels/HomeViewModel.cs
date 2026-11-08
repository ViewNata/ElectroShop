// namespace _66076431_natalee.ViewModels
// {
//     public class HomeViewModel
//     {
//         // หมวดสินค้า (distinct จาก DB)
//         public List<string> Categories { get; set; } = new();

//         // สินค้าแนะนำ (ทั้งหมดจาก DB)
//         public List<HomeProductItem> Products { get; set; } = new();

//     }

//     public class HomeProductItem
//     {
//         public string ProductId   { get; set; }
//         public string ProductName { get; set; }
//         public string Category    { get; set; }
//         public int    SellPrice   { get; set; }
//         public int    DiscountPrice { get; set; }
//         public string ImageUrl    { get; set; }
//     }
// }



namespace _66076431_natalee.ViewModels
{
    public class HomeViewModel
    {
        public List<string> Categories { get; set; } = new();
        public List<HomeProductItem> BestSellerProducts { get; set; } = new();
        public List<HomeProductItem> DiscountProducts { get; set; } = new();
    }

    public class HomeProductItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int SellPrice { get; set; }
        public int DiscountPrice { get; set; }
        public string ImageUrl { get; set; }
        public string Description   { get; set; }
    }
}