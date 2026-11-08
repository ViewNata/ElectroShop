namespace _66076431_natalee.ViewModels
{
  public class DashboardViewModel
  {
    // === สรุปยอดขาย ===
    public decimal TotalSaleToday { get; set; }       // ยอดขายวันนี้
    public decimal TotalSaleThisMonth { get; set; }   // ยอดขายเดือนนี้
    public int TotalOrderToday { get; set; }           // จำนวนออเดอร์วันนี้
    public int TotalOrderPending { get; set; }         // ออเดอร์รอดำเนินการ

    // === สต็อกสินค้า ===
    public int TotalProduct { get; set; }              // สินค้าทั้งหมด (รายการ)
    public int LowStockCount { get; set; }             // สินค้าใกล้หมด (< 5 ชิ้น)
    public int OutOfStockCount { get; set; }           // สินค้าหมดสต็อก

    // === ลูกค้า / ผู้ใช้งาน ===
    public int TotalCustomer { get; set; }             // ลูกค้าทั้งหมด
    public int NewCustomerThisMonth { get; set; }      // ลูกค้าใหม่เดือนนี้

    // === ออเดอร์ล่าสุด ===
    public List<RecentOrderItem> RecentOrders { get; set; } = new List<RecentOrderItem>();

    // === สินค้าขายดี ===
    public List<TopProductItem> TopProducts { get; set; } = new List<TopProductItem>();

    // === สินค้าใกล้หมด ===
    public List<LowStockItem> LowStockProducts { get; set; } = new List<LowStockItem>();
  }

  // ออเดอร์ล่าสุดสำหรับแสดงในตาราง
  public class RecentOrderItem
  {
    public string OrderNo { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string OrderStatus { get; set; }   // รอดำเนินการ / กำลังจัดส่ง / จัดส่งแล้ว
    public string PaymentStatus { get; set; } // จ่ายแล้ว / ยังไม่จ่าย
  }

  // สินค้าขายดีสำหรับแสดงในตาราง
  public class TopProductItem
  {
    public string ProductName { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public int SoldQty { get; set; }          // จำนวนที่ขายได้
    public decimal Revenue { get; set; }      // รายได้จากสินค้านี้
  }

  // สินค้าใกล้หมดสต็อก
  public class LowStockItem
  {
    public string ProductName { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public int StockLeft { get; set; }        // จำนวนคงเหลือ
    public string Unit { get; set; }          // หน่วย เช่น เครื่อง
  }
}