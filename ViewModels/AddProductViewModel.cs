// namespace _66076431_natalee.ViewModels
// {
//   public class AddProductViewModel
//   {
//     public string ProductName { get; set; }     // ชื่อสินค้า
//     public string Category { get; set; }        // หมวดสินค้า

//     public int Quantity { get; set; }           // จำนวน
//     public string Unit { get; set; }            // หน่วย

//     public decimal CostPrice { get; set; }      // ต้นทุน
//     public decimal SellPrice { get; set; }      // ราคาขาย

//     public string Description { get; set; }     // รายละเอียดสินค้า
//   }
// }

namespace _66076431_natalee.ViewModels
{
    public class AddProductViewModel
    {
        public string ProductId     { get; set; }
        public string ProductName   { get; set; }
        public string Brand         { get; set; }
        public string Category      { get; set; }
        public int    Quantity      { get; set; }
        public string Unit          { get; set; }
        public int    CostPrice     { get; set; }
        public int    SellPrice     { get; set; }
        public int    DiscountPrice { get; set; }
        public string Warranty      { get; set; }
        public string Description   { get; set; }
        public string  ImageUrl   { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
