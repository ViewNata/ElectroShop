using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _66076431_natalee.Models;
using _66076431_natalee.ViewModels;
using _66076431_natalee.Models.db;

namespace _66076431_natalee.Controllers;

public class ElectricalController : Controller
{
    //ประกาศใช้ db
    public readonly Csi402dbContext _db;

    public ElectricalController(Csi402dbContext db)
    {
        _db = db;
    }

    // ===== Login / Register / Logout =====

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel data)
    {
        // เช็ค Admin (พนักงาน) ก่อน
        var admin = (from a in _db.Admins where a.AdminEmail == data.Email && a.AdminPassword == data.Password select a).FirstOrDefault();

        if (admin != null)
        {
            HttpContext.Session.SetString("AdminId", admin.AdminId);
            HttpContext.Session.SetString("Role", "Admin");
            HttpContext.Session.SetString("AdminRole", admin.AdminRole);
            return RedirectToAction("Dashboard", "Electrical");
        }

        // เช็ค Customer
        var customer = (from c in _db.Customers where c.CusrEmail == data.Email && c.CusPassword == data.Password select c).FirstOrDefault();

        if (customer != null)
        {
            HttpContext.Session.SetString("CusId", customer.CusId);
            HttpContext.Session.SetString("Role", "Customer");
            return RedirectToAction("Home", "Electrical");
        }

        // ไม่พบ
        ViewBag.Error = "อีเมลหรือรหัสผ่านไม่ถูกต้อง";
        return View(data);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel data)
    {
        // เช็คกรอกครบ
        if (string.IsNullOrEmpty(data.RegisName) ||
            string.IsNullOrEmpty(data.RegisEmail) ||
            string.IsNullOrEmpty(data.RegisPhoneNumber) ||
            string.IsNullOrEmpty(data.RegisPassword))
        {
            ViewBag.ErrorMessage = "กรอกข้อมูลให้ครบ";
            return View(data);
        }

        // เช็ค email ซ้ำ
        if (_db.Customers.Any(c => c.CusrEmail == data.RegisEmail))
        {
            ViewBag.ErrorMessage = "อีเมลนี้ถูกใช้งานแล้ว";
            return View(data);
        }

        if (_db.Customers.Any(c => c.CusPhoneNumber == data.RegisPhoneNumber))
        {
            ViewBag.ErrorMessage = "เบอร์โทรนี้ถูกใช้งานแล้ว";
            return View(data);
        }

        // เช็ค password
        string pass = data.RegisPassword;

        if (pass.Length < 6)
        {
            ViewBag.ErrorMessage = "รหัสผ่านต้องอย่างน้อย 6 ตัว";
            return View(data);
        }

        if (!pass.Any(char.IsLetter) || !pass.Any(char.IsDigit))
        {
            ViewBag.ErrorMessage = "ต้องมีทั้งตัวอักษรและตัวเลข";
            return View(data);
        }

        // สร้าง ID 
        int count = _db.Customers.Count() + 1;
        string newId = "C" + count.ToString("D3");

        // สร้างข้อมูล
        var c = new Customer
        {
            CusId = newId,
            CusrEmail = data.RegisEmail,
            CusName = data.RegisName,
            CusPhoneNumber = data.RegisPhoneNumber,
            CusPassword = data.RegisPassword
        };

        _db.Customers.Add(c);
        _db.SaveChanges();

        return RedirectToAction("Login", "Electrical");
    }
    

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Electrical");
    }


    public IActionResult Home()
    {
        // หมวดหมู่สินค้า
        var categories = (from p in _db.Products where p.Category != null select p.Category).Distinct().ToList();

        // สินค้าขายดี
        var bestSellerProducts = (from d in _db.OrderDetails join p in _db.Products on d.ProductId equals p.ProductId
                                    group d by new
                                    {
                                        p.ProductId,
                                        p.ProductName,
                                        p.Category,
                                        p.SellPrice,
                                        p.DiscountPrice,
                                        p.ImageUrl
                                    } into g
                                    orderby g.Sum(x => x.Quantity ?? 0) descending 
                                    select new HomeProductItem
                                    {
                                        ProductId = g.Key.ProductId,
                                        ProductName = g.Key.ProductName,
                                        Category = g.Key.Category,
                                        SellPrice = g.Key.SellPrice ?? 0,
                                        DiscountPrice = g.Key.DiscountPrice ?? 0,
                                        ImageUrl = g.Key.ImageUrl
                                    }).Take(8).ToList();

        // ถ้ายังไม่มีข้อมูลขาย ใช้สินค้าล่าสุดแทน
        if (bestSellerProducts.Count == 0)
        {
            bestSellerProducts = (from p in _db.Products orderby p.ProductId descending select new HomeProductItem
                                    {
                                        ProductId = p.ProductId,
                                        ProductName = p.ProductName,
                                        Category = p.Category,
                                        SellPrice = p.SellPrice ?? 0,
                                        DiscountPrice = p.DiscountPrice ?? 0,
                                        ImageUrl = p.ImageUrl
                                    }).Take(8).ToList();
        }

        // สินค้าลดราคา
        var discountProducts = (from p in _db.Products where p.DiscountPrice != null && p.DiscountPrice > 0 && p.DiscountPrice < p.SellPrice
                                orderby p.ProductId descending
                                select new HomeProductItem
                                {
                                    ProductId = p.ProductId,
                                    ProductName = p.ProductName,
                                    Category = p.Category,
                                    SellPrice = p.SellPrice ?? 0,
                                    DiscountPrice = p.DiscountPrice ?? 0,
                                    ImageUrl = p.ImageUrl
                                }).Take(4).ToList();

        var vm = new HomeViewModel();
        vm.Categories = categories;
        vm.BestSellerProducts = bestSellerProducts;
        vm.DiscountProducts = discountProducts;

        return View(vm);
    }



    public IActionResult ProductListUser(string category)
    {
    var categories = (from p in _db.Products where p.Category != null select p.Category).Distinct().ToList();

    var query = from p in _db.Products select p;

    if (!string.IsNullOrEmpty(category))
    {
        query = from p in query where p.Category == category select p;
    }

    var products = (from p in query select new HomeProductItem
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Category = p.Category,
                        SellPrice = p.SellPrice ?? 0,
                        DiscountPrice = p.DiscountPrice ?? 0,
                        ImageUrl = p.ImageUrl
                    }).ToList();

        ViewBag.AllCategories = categories;
        ViewBag.Category = string.IsNullOrEmpty(category) ? "ทั้งหมด" : category;
        ViewBag.TotalProducts = products.Count;

        return View(products);
    }

    public IActionResult ProductDetailUser(string PID)
    {
        var product = (from p in _db.Products where p.ProductId == PID select new HomeProductItem
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Category = p.Category,
                            SellPrice = p.SellPrice ?? 0,
                            DiscountPrice = p.DiscountPrice ?? 0,
                            ImageUrl = p.ImageUrl,
                            Description   = p.DescriptionPd
                        }).FirstOrDefault();
        return View(product);
    }

    // ===== ตะกร้าสินค้า =====

    public IActionResult AddToCart(string PID, int Qty = 1)
    {
        var product = (from p in _db.Products where p.ProductId == PID select p).FirstOrDefault();
        if (product == null) return RedirectToAction("ProductListUser");

        var cusId = HttpContext.Session.GetString("CusId");

        // หา order สถานะ "ตะกร้า" ของ user คนนี้
        var order = (from o in _db.Orders where o.OrderStatus == "ตะกร้า" && o.CusId == cusId
                    select o).FirstOrDefault();

        if (order == null)
        {
            // สร้าง Order ใหม่
            var lastId = (from o in _db.Orders select (int?)o.OrderId).Max() ?? 0;
            int newId = lastId + 1;

            order = new Order();
            order.OrderId = newId;
            order.OrderNo = "ORD" + newId.ToString("D5");
            order.OrderDate = DateTime.Now;
            order.OrderStatus = "ตะกร้า";
            order.CusId = cusId;
            order.SubTotal = 0;
            order.TotalPrice = 0;
            _db.Orders.Add(order);
            _db.SaveChanges();
        }

        // เช็คว่าสินค้านี้อยู่ในตะกร้าแล้วหรือยัง
        var detail = (from d in _db.OrderDetails
                        where d.OrderId == order.OrderId && d.ProductId == PID
                        select d).FirstOrDefault();

        if (detail != null)
        {
            // มีแล้ว เพิ่มจำนวน
            detail.Quantity = (detail.Quantity ?? 0) + Qty;
            detail.TotalPrice = detail.Quantity * (detail.UnitPrice ?? 0);
            _db.Update(detail);
        }
        else
        {
            // ยังไม่มี เพิ่มรายการใหม่
            var lastDid = (from d in _db.OrderDetails select (int?)d.OrderDetailId).Max() ?? 0;
            int newDid = lastDid + 1;

            var od = new OrderDetail();
            od.OrderDetailId = newDid;
            od.OrderId = order.OrderId;
            od.ProductId = PID;
            od.Quantity = Qty;
            od.UnitPrice = product.DiscountPrice ?? product.SellPrice ?? 0;
            od.TotalPrice = (product.DiscountPrice ?? product.SellPrice ?? 0) * Qty;
            _db.OrderDetails.Add(od);
        }

        _db.SaveChanges();
        return RedirectToAction("CartUser");
    }

    public IActionResult BuyNow(string PID, int Qty = 1)
    {
        var product = (from p in _db.Products where p.ProductId == PID select p).FirstOrDefault();

        if (product == null) return RedirectToAction("ProductListUser");

        var cusId = HttpContext.Session.GetString("CusId");
        if (string.IsNullOrEmpty(cusId))
        {
            return RedirectToAction("Login");
        }

        var order = (from o in _db.Orders
                    where o.OrderStatus == "ตะกร้า" && o.CusId == cusId
                    select o).FirstOrDefault();

        if (order == null)
        {
            var lastId = (from o in _db.Orders select (int?)o.OrderId).Max() ?? 0;
            int newId = lastId + 1;

            order = new Order();
            order.OrderId = newId;
            order.OrderNo = "ORD" + newId.ToString("D5");
            order.OrderDate = DateTime.Now;
            order.OrderStatus = "ตะกร้า";
            order.CusId = cusId;
            order.SubTotal = 0;
            order.TotalPrice = 0;

            _db.Orders.Add(order);
            _db.SaveChanges();
        }

        var detail = (from d in _db.OrderDetails
                    where d.OrderId == order.OrderId && d.ProductId == PID
                    select d).FirstOrDefault();

        if (detail != null)
        {
            detail.Quantity = Qty; // ซื้อเลย = ใช้จำนวนที่เลือกตอนนี้
            detail.UnitPrice = product.DiscountPrice ?? product.SellPrice ?? 0;
            detail.TotalPrice = (detail.UnitPrice ?? 0) * (detail.Quantity ?? 0);
            _db.Update(detail);
        }
        else
        {
            var lastDid = (from d in _db.OrderDetails select (int?)d.OrderDetailId).Max() ?? 0;
            int newDid = lastDid + 1;

            detail = new OrderDetail();
            detail.OrderDetailId = newDid;
            detail.OrderId = order.OrderId;
            detail.ProductId = PID;
            detail.Quantity = Qty;
            detail.UnitPrice = product.DiscountPrice ?? product.SellPrice ?? 0;
            detail.TotalPrice = (detail.UnitPrice ?? 0) * Qty;

            _db.OrderDetails.Add(detail);
        }

    _db.SaveChanges();

    return RedirectToAction("CheckoutUser", new
    {
        orderId = order.OrderId,
        selectedIds = detail.OrderDetailId.ToString()
    });
}

    public IActionResult RemoveFromCart(int OdId)
    {
        var detail = (from d in _db.OrderDetails where d.OrderDetailId == OdId select d).FirstOrDefault();
        _db.RemoveRange(detail);
        _db.SaveChanges();
        return RedirectToAction("CartUser");
    }

    public IActionResult UpdateQty(int OdId, int Change)
    {
        var detail = (from d in _db.OrderDetails where d.OrderDetailId == OdId select d).FirstOrDefault();
        if (detail != null)
        {
            detail.Quantity = (detail.Quantity ?? 0) + Change;
            if (detail.Quantity <= 0)
            {
                _db.RemoveRange(detail);
            }
            else
            {
                detail.TotalPrice = detail.Quantity * (detail.UnitPrice ?? 0);
                _db.Update(detail);
            }
            _db.SaveChanges();
        }
        return RedirectToAction("CartUser");
    }

    public IActionResult CartUser()
    {
        var cusId = HttpContext.Session.GetString("CusId");

        var order = (from o in _db.Orders where o.OrderStatus == "ตะกร้า" && o.CusId == cusId
                        select o).FirstOrDefault();

        if (order == null) return View(new List<OrderDetailViewModel>());

        var details = (from d in _db.OrderDetails join p in _db.Products on d.ProductId equals p.ProductId where d.OrderId == order.OrderId
                select new OrderDetailViewModel
                        {
                            OrderDetailId = d.OrderDetailId,
                            OrderId = d.OrderId ?? 0,
                            ProductName = p.ProductName,
                            Quantity = d.Quantity ?? 0,
                            UnitPrice = d.UnitPrice ?? 0,
                            TotalPrice = d.TotalPrice ?? 0
                        }).ToList();

        ViewBag.OrderId = order.OrderId;
        ViewBag.SubTotal = details.Sum(d => d.TotalPrice);
        return View(details);
    }

    // ===== Checkout / ชำระเงิน =====

    public IActionResult CheckoutUser(int orderId, string selectedIds)
    {
        if (string.IsNullOrEmpty(selectedIds))
        {
            return RedirectToAction("CartUser");
        }

        var idList = selectedIds.Split(',')
                                .Select(int.Parse)
                                .ToList();

        var details = (from d in _db.OrderDetails
                    join p in _db.Products on d.ProductId equals p.ProductId
                    where d.OrderId == orderId && idList.Contains(d.OrderDetailId)
                    select new OrderDetailViewModel
                    {
                        OrderDetailId = d.OrderDetailId,
                        OrderId = d.OrderId ?? 0,
                        ProductName = p.ProductName,
                        Quantity = d.Quantity ?? 0,
                        UnitPrice = d.UnitPrice ?? 0,
                        TotalPrice = d.TotalPrice ?? 0
                    }).ToList();

        ViewBag.OrderId = orderId;
        ViewBag.SubTotal = details.Sum(x => x.TotalPrice);

        var cusId = HttpContext.Session.GetString("CusId");
        int orderCount = (from o in _db.Orders
                        where o.CusId == cusId && o.OrderStatus != "ตะกร้า"
                        select o).Count();

        //ลูกค้าใหม่ไหม?
        ViewBag.IsFirstOrder = (orderCount == 0);

        return View(details);
    }

    [HttpPost]
    public IActionResult ConfirmOrder(int OrderId, string FullName, string Phone, string Address, string ShippingMethod, string PaymentMethod, int Discount)
    {
        var order = (from o in _db.Orders where o.OrderId == OrderId select o).FirstOrDefault();
        var details = (from d in _db.OrderDetails where d.OrderId == OrderId select d).ToList();

        int subTotal = details.Sum(d => d.TotalPrice ?? 0);

        order.OrderStatus = "รอชำระ";
        order.SubTotal = subTotal;
        order.Discount = Discount;
        order.TotalPrice = subTotal - Discount;
        order.ShipAddress = FullName + " | " + Phone + " | " + Address;
        order.ShippingMethod = ShippingMethod;
        order.PaymentMethod = PaymentMethod;
        order.PaymentStatus = "ยังไม่จ่าย";
        _db.Update(order);

        // ลดสต็อกสินค้า
        foreach (var d in details)
        {
            var p = (from x in _db.Products where x.ProductId == d.ProductId select x).FirstOrDefault();
            if (p != null)
            {
                p.Quantity = (p.Quantity ?? 0) - (d.Quantity ?? 0);
                _db.Update(p);
            }
        }

        _db.SaveChanges();
        return RedirectToAction("Payment", new { orderId = OrderId });
    }

    public IActionResult Payment(int orderId)
    {
        var order = (from o in _db.Orders where o.OrderId == orderId
                        select new OrderListItemViewModel
                        {
                            OrderId = o.OrderId,
                            OrderNo = o.OrderNo,
                            CustomerName = "",
                            ItemCount = 0,
                            PaymentStatus = o.PaymentStatus,
                            TotalPrice = o.TotalPrice ?? 0,
                            SlipImage = o.SlipImage
                        }).FirstOrDefault();
        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UploadSlip(int orderId, IFormFile slipFile)
    {
        if (slipFile != null && slipFile.Length > 0)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "slips");
            Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(slipFile.FileName);
            string path = Path.Combine(folder, fileName);

            //บันทึกไฟล์ลงโฟลเดอร์
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await slipFile.CopyToAsync(stream);
            }

            var order = (from o in _db.Orders where o.OrderId == orderId select o).FirstOrDefault();
            if (order != null)
            {
                order.SlipImage = fileName;
                order.PaymentStatus = "รอตรวจสอบ";
                order.OrderStatus = "รอตรวจสอบ";
                _db.Update(order);
                _db.SaveChanges();
            }
        }
        return RedirectToAction("OrderSuccessUser", new { orderId = orderId });
    }

    public IActionResult OrderSuccessUser(int orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }

    // ===== ติดตาม / โปรไฟล์ (ฝั่ง User) =====

    public IActionResult OrderUser()
    {
        var cusId = HttpContext.Session.GetString("CusId");

        var orders = (from o in _db.Orders
                    where o.CusId == cusId
                            && o.OrderStatus != "ตะกร้า"
                            && (o.OrderStatus == "ส่งสำเร็จ" || o.OrderStatus == "ยกเลิก")
                    orderby o.OrderDate descending
                    select new OrderTrackViewModel
                    {
                        OrderId = o.OrderId,
                        OrderNo = o.OrderNo,
                        OrderStatus = o.OrderStatus,
                        OrderDate = o.OrderDate ?? DateTime.Now,
                        RejectNote = o.RejectNote,
                        ShippingMethod = o.ShippingMethod,
                        TotalPrice = o.TotalPrice ?? 0,
                        Discount = o.Discount ?? 0,
                        Details = (from d in _db.OrderDetails join p in _db.Products on d.ProductId equals p.ProductId into dp
                                    from p in dp.DefaultIfEmpty() where d.OrderId == o.OrderId
                                    select new OrderTrackDetailItem
                                    {
                                        ProductId = d.ProductId,
                                        ProductName = p != null ? p.ProductName : d.ProductId,
                                        Quantity = d.Quantity ?? 0,
                                        TotalPrice = d.TotalPrice ?? 0
                                    }).ToList()
                    }).ToList();

        return View(orders);
    }

public IActionResult OrderTrackUser()
{
    var cusId = HttpContext.Session.GetString("CusId");

        var orders = (from o in _db.Orders
                    where o.CusId == cusId
                            && o.OrderStatus != "ตะกร้า"
                            && o.OrderStatus != "ส่งสำเร็จ"
                            && o.OrderStatus != "ยกเลิก"
                    orderby o.OrderDate descending
                    select new OrderTrackViewModel
                    {
                        OrderId = o.OrderId,
                        OrderNo = o.OrderNo,
                        OrderStatus = o.OrderStatus,
                        OrderDate = o.OrderDate ?? DateTime.Now,
                        RejectNote = o.RejectNote,
                        ShippingMethod = o.ShippingMethod,
                        TotalPrice = o.TotalPrice ?? 0,
                        Discount = o.Discount ?? 0,
                        Details = (from d in _db.OrderDetails
                                    join p in _db.Products on d.ProductId equals p.ProductId into dp
                                    from p in dp.DefaultIfEmpty()
                                    where d.OrderId == o.OrderId
                                    select new OrderTrackDetailItem
                                    {
                                        ProductId = d.ProductId,
                                        ProductName = p != null ? p.ProductName : d.ProductId,
                                        Quantity = d.Quantity ?? 0,
                                        TotalPrice = d.TotalPrice ?? 0
                                    }).ToList()
                    }).ToList();

        return View(orders);
    }

    public IActionResult ProfileUser()
    {
        var cusId = HttpContext.Session.GetString("CusId");
        var customer = (from c in _db.Customers where c.CusId == cusId select c).FirstOrDefault();

        // นับ order แต่ละสถานะ
        ViewBag.CountPending = (from o in _db.Orders where o.CusId == cusId && o.OrderStatus == "รอจัดส่ง" select o).Count();
        ViewBag.CountShipping = (from o in _db.Orders where o.CusId == cusId && o.OrderStatus == "กำลังจัดส่ง" select o).Count();
        ViewBag.CountSuccess = (from o in _db.Orders where o.CusId == cusId && o.OrderStatus == "ส่งสำเร็จ" select o).Count();
        ViewBag.CountCancel = (from o in _db.Orders where o.CusId == cusId && o.OrderStatus == "ยกเลิก" select o).Count();

        return View(customer);
    }

    [HttpPost]
    public IActionResult ProfileUser(string CusName, string CusPhoneNumber, string CusAddress)
    {
        var cusId = HttpContext.Session.GetString("CusId");
        var c = (from x in _db.Customers where x.CusId == cusId select x).FirstOrDefault();
        c.CusName = CusName;
        c.CusPhoneNumber = CusPhoneNumber;
        c.CusAddress = CusAddress;
        _db.Update(c);
        _db.SaveChanges();
        return RedirectToAction("ProfileUser", "Electrical");
    }

    // ===== Admin: จัดการพนักงาน =====

    public IActionResult AdminList()
    {
        var admins = (from a in _db.Admins
                        select new AddAdminViewModel
                        {
                            AdminId = a.AdminId,
                            AdminName = a.AdminName,
                            AdminEmail = a.AdminEmail,
                            AdminPhone = a.AdminPhoneNumber,
                            AdminRole = a.AdminRole
                        }).ToList();
        return View(admins);
    }

    public IActionResult AddUser()
    {
        return View(new AddAdminViewModel());
    }

    [HttpPost]
    public IActionResult AddUser(AddAdminViewModel data)
    {
        // Auto Gen AdminId แบบ A001, A002...
        var allIds = (from a in _db.Admins where a.AdminId.StartsWith("A")
                        select a.AdminId).ToList();

        int maxNum = 0;
        foreach (var id in allIds)
            if (int.TryParse(id.Substring(1), out int n) && n > maxNum) maxNum = n;

        var admin = new Admin();
        admin.AdminId = "A" + (maxNum + 1).ToString("D3");
        admin.AdminName = data.AdminName;
        admin.AdminEmail = data.AdminEmail;
        admin.AdminPassword = data.AdminPassword;
        admin.AdminPhoneNumber = data.AdminPhone;
        admin.AdminRole = data.AdminRole;
        _db.Admins.Add(admin);
        _db.SaveChanges();
        return RedirectToAction("AdminList", "Electrical");
    }

    public IActionResult EditAdmin(string AID)
    {
        var admin = (from a in _db.Admins
                        where a.AdminId == AID
                        select new AddAdminViewModel
                        {
                            AdminId = a.AdminId,
                            AdminName = a.AdminName,
                            AdminEmail = a.AdminEmail,
                            AdminPassword = a.AdminPassword,
                            AdminPhone = a.AdminPhoneNumber,
                            AdminRole = a.AdminRole
                        }).FirstOrDefault();
        return View(admin);
    }

    [HttpPost]
    public IActionResult EditAdmin(AddAdminViewModel data)
    {
        var admin = (from a in _db.Admins where a.AdminId == data.AdminId select a).FirstOrDefault();
        admin.AdminName = data.AdminName;
        admin.AdminEmail = data.AdminEmail;
        admin.AdminPassword = data.AdminPassword;
        admin.AdminPhoneNumber = data.AdminPhone;
        admin.AdminRole = data.AdminRole;
        _db.Update(admin);
        _db.SaveChanges();
        return RedirectToAction("AdminList", "Electrical");
    }

    public IActionResult DeleteAdmin(string AID)
    {
        var admin = (from a in _db.Admins where a.AdminId == AID select a).FirstOrDefault();
        _db.RemoveRange(admin);
        _db.SaveChanges();
        return RedirectToAction("AdminList", "Electrical");
    }



    // ===== Admin: จัดการลูกค้า =====

    public IActionResult UserList()
    {
        var user = (from c in _db.Customers
                    select new UserListItemViewModel
                    {
                        ListId = c.CusId,
                        ListEmail = c.CusrEmail,
                        ListName = c.CusName,
                        ListPhoneNumber = c.CusPhoneNumber
                    }).ToList();
        return View(user);
    }

    public IActionResult EditUser(string UID)
    {
        var chenk = (from eu in _db.Customers
                        where eu.CusId == UID
                        select new EditUserViewModel
                        {
                            UserId = eu.CusId,
                            FullName = eu.CusName,
                            Phone = eu.CusPhoneNumber,
                            Email = eu.CusrEmail,
                            password = eu.CusPassword
                        }).FirstOrDefault();
        return View(chenk);
    }

    [HttpPost]
    public IActionResult EditUser(EditUserViewModel data)
    {
        var user = (from c in _db.Customers where c.CusId == data.UserId select c).FirstOrDefault();
        user.CusName = data.FullName;
        user.CusPhoneNumber = data.Phone;
        user.CusrEmail = data.Email;
        user.CusPassword = data.password;
        _db.Update(user);
        _db.SaveChanges();
        return RedirectToAction("UserList", "Electrical");
    }

    public IActionResult DeleteUser(string UID)
    {
        var user = (from c in _db.Customers where c.CusId == UID select c).FirstOrDefault();
        _db.RemoveRange(user);
        _db.SaveChanges();
        return RedirectToAction("UserList", "Electrical");
    }

    // ===== Admin: จัดการสินค้า =====

    public IActionResult ProductList()
    {
        var products = (from p in _db.Products
                        select new ProductListItemViewModel
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Brand = p.Brand,
                            Category = p.Category,
                            SellPrice = p.SellPrice ?? 0,
                            Quantity = p.Quantity ?? 0,
                            Unit = p.Unit,
                            ImageUrl     = p.ImageUrl 
                        }).ToList();
        return View(products);
    }

    public IActionResult AddProduct()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(AddProductViewModel data)
    {
        var allPIds = (from p in _db.Products where p.ProductId.StartsWith("P")  select p.ProductId).ToList();
        int maxNum = 0;
        foreach (var pid in allPIds)
            if (int.TryParse(pid.Substring(1), out int num) && num > maxNum)
                maxNum = num;

        string newId = "P" + (maxNum + 1).ToString("D3");
        string imageUrl = data.ImageUrl;

        if (data.ImageFile != null && data.ImageFile.Length > 0)
        {
            var ext = Path.GetExtension(data.ImageFile.FileName);
            var fileName = Guid.NewGuid().ToString() + ext;
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await data.ImageFile.CopyToAsync(stream);
            }

            imageUrl = "/images/" + fileName;
        }
        var p2 = new Product();
        p2.ProductId = newId;
        p2.ProductName = data.ProductName;
        p2.Brand = data.Brand;
        p2.Category = data.Category;
        p2.Quantity = data.Quantity;
        p2.Unit = data.Unit;
        p2.CostPrice = data.CostPrice;
        p2.SellPrice = data.SellPrice;
        p2.DiscountPrice = data.DiscountPrice;
        p2.Warranty = data.Warranty;
        p2.DescriptionPd = data.Description;
        p2.ImageUrl = imageUrl;

        _db.Add(p2);
        _db.SaveChanges();

        return RedirectToAction("ProductList", "Electrical");
    }

    public IActionResult EditProduct(string PID)
    {
        var chenk = (from p in _db.Products where p.ProductId == PID
                        select new AddProductViewModel
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Brand = p.Brand,
                            Category = p.Category,
                            Quantity = p.Quantity ?? 0,
                            Unit = p.Unit,
                            CostPrice = p.CostPrice ?? 0,
                            SellPrice = p.SellPrice ?? 0,
                            DiscountPrice = p.DiscountPrice ?? 0,
                            Warranty = p.Warranty,
                            Description = p.DescriptionPd
                        }).FirstOrDefault();
        return View(chenk);
    }
    [HttpPost]
    public async Task<IActionResult> EditProduct(AddProductViewModel data)
    {
        var p = (from x in _db.Products where x.ProductId == data.ProductId
                select x).FirstOrDefault();

        if (p == null)
            return RedirectToAction("ProductList", "Electrical");

        p.ProductName = data.ProductName;
        p.Brand = data.Brand;
        p.Category = data.Category;
        p.Quantity = data.Quantity;
        p.Unit = data.Unit;
        p.CostPrice = data.CostPrice;
        p.SellPrice = data.SellPrice;
        p.DiscountPrice = data.DiscountPrice;
        p.Warranty = data.Warranty;
        p.DescriptionPd = data.Description;

        if (data.ImageFile != null && data.ImageFile.Length > 0)
        {
            var ext = Path.GetExtension(data.ImageFile.FileName);
            var fileName = Guid.NewGuid().ToString() + ext;
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await data.ImageFile.CopyToAsync(stream);
            }

            p.ImageUrl = "/images/" + fileName;
        }

        _db.Update(p);
        _db.SaveChanges();

        return RedirectToAction("ProductList", "Electrical");
    }

    public IActionResult DeleteProduct(string PID)
    {
        var p = (from x in _db.Products where x.ProductId == PID select x).FirstOrDefault();
        _db.RemoveRange(p);
        _db.SaveChanges();
        return RedirectToAction("ProductList", "Electrical");
    }

    // ===== Admin: จัดการออเดอร์ =====

    public IActionResult OrderList()
    {
        var orders = (from o in _db.Orders join c in _db.Customers on o.CusId equals c.CusId into oc
                    from c in oc.DefaultIfEmpty()
                    where o.PaymentStatus == "รอตรวจสอบ"
                            || o.OrderStatus == "ยกเลิก"
                    orderby o.OrderDate descending
                    select new OrderListItemViewModel
                    {
                        OrderId = o.OrderId,
                        OrderNo = o.OrderNo,
                        CustomerName = c != null ? c.CusName : "",
                        ItemCount = (from d in _db.OrderDetails
                                    where d.OrderId == o.OrderId
                                    select d).Count(),
                        PaymentStatus = o.PaymentStatus,
                        TotalPrice = o.TotalPrice ?? 0,
                        SlipImage = o.SlipImage,
                        RejectNote = o.RejectNote,
                        OrderStatus = o.OrderStatus,

                        ProductNames = string.Join(", ",
                            (from d in _db.OrderDetails
                            join p in _db.Products on d.ProductId equals p.ProductId into dp
                            from p in dp.DefaultIfEmpty()
                            where d.OrderId == o.OrderId
                            select p != null ? p.ProductName : d.ProductId).ToList()
                        )
                    }).ToList();

        return View(orders);
    }

    public IActionResult OrderDetail(int orderId)
    {
        var order = (from o in _db.Orders where o.OrderId == orderId select o).FirstOrDefault();
        if (order == null) return RedirectToAction("OrderList", "Electrical");

        string cusName = "";
        string phone = "";
        string address = "";

        var customer = (from c in _db.Customers where c.CusId == order.CusId select c).FirstOrDefault();
        if (customer != null)
        {
            cusName = customer.CusName;
            phone = customer.CusPhoneNumber;
            address = customer.CusAddress;
        }

        if (!string.IsNullOrEmpty(order.ShipAddress))
        {
            var arr = order.ShipAddress.Split(" | ");
            if (arr.Length >= 3)
            {
                cusName = arr[0];
                phone = arr[1];
                address = arr[2];
            }
        }

        var details = (from d in _db.OrderDetails
                join p in _db.Products on d.ProductId equals p.ProductId into dp
                from p in dp.DefaultIfEmpty()
                where d.OrderId == orderId
                select new OrderDetailViewModel
                {
                    OrderDetailId = d.OrderDetailId,
                    OrderId = d.OrderId ?? 0,
                    ProductId = d.ProductId,
                    ProductName = p != null ? p.ProductName : d.ProductId,
                    Quantity = d.Quantity ?? 0,
                    UnitPrice = d.UnitPrice ?? 0,
                    TotalPrice = d.TotalPrice ?? 0,
                    OrderNo = order.OrderNo,
                    OrderDate = order.OrderDate ?? DateTime.Now,
                    CustomerName = cusName,
                    Phone = phone,
                    Address = address,
                    PaymentStatus = order.PaymentStatus,
                    OrderStatus = order.OrderStatus,
                    ShippingMethod = order.ShippingMethod,
                    PaymentMethod = order.PaymentMethod,
                    SubTotal = order.SubTotal ?? 0,
                    Discount = order.Discount ?? 0,
                    SlipImage = order.SlipImage,
                    RejectNote = order.RejectNote
                }).ToList();
        return View(details);
    }

    public IActionResult ApproveOrder(int orderId)
    {
        var order = (from o in _db.Orders where o.OrderId == orderId select o).FirstOrDefault();
        if (order != null)
        {
            order.PaymentStatus = "จ่ายแล้ว";
            order.OrderStatus = "รอจัดส่ง";
            _db.Update(order);
            _db.SaveChanges();
        }
        return RedirectToAction("OrderList", "Electrical");
    }

    public IActionResult ProductOrderDetail()
    {
        var orders = (from o in _db.Orders
                        join c in _db.Customers on o.CusId equals c.CusId into oc
                        from c in oc.DefaultIfEmpty()
                        where o.PaymentStatus == "จ่ายแล้ว"
                        orderby o.OrderDate descending
                        select new ProductOrderListViewModel
                    {
                        OrderId = o.OrderId,
                        OrderNo = o.OrderNo,
                        CustomerName = c != null ? c.CusName : "",
                        OrderDate = o.OrderDate ?? DateTime.Now,
                        Status = o.OrderStatus
                    }).ToList();
        return View(orders);
    }

    public IActionResult StartShipping(int orderId)
    {
        var order = (from o in _db.Orders where o.OrderId == orderId select o).FirstOrDefault();
        if (order != null)
        {
            order.OrderStatus = "กำลังจัดส่ง";
            _db.Update(order);
            _db.SaveChanges();
        }
        return RedirectToAction("ProductOrderDetail", "Electrical");
    }

    public IActionResult CompleteShipping(int orderId)
    {
        var order = (from o in _db.Orders where o.OrderId == orderId select o).FirstOrDefault();
        if (order != null)
        {
            order.OrderStatus = "ส่งสำเร็จ";
            _db.Update(order);
            _db.SaveChanges();
        }
        return RedirectToAction("ProductOrderDetail", "Electrical");
    }

    // ลูกค้ากดยืนยันรับสินค้าแล้ว
    public IActionResult ConfirmReceived(int orderId)
    {
        var order = (from o in _db.Orders
                    where o.OrderId == orderId
                    select o).FirstOrDefault();

        if (order != null)
        {
            order.OrderStatus = "ส่งสำเร็จ";
            _db.Update(order);
            _db.SaveChanges();
        }

        return RedirectToAction("OrderUser", "Electrical");
    }

    // ===== Dashboard =====

    public IActionResult Dashboard()
    {
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        // ยอดขายวันนี้
        var totalSaleToday = (from o in _db.Orders
                                where o.OrderDate.Value.Date == today
                                && o.PaymentStatus == "จ่ายแล้ว"
                                select (int?)o.TotalPrice).Sum() ?? 0;

        // ยอดขายเดือนนี้
        var totalSaleThisMonth = (from o in _db.Orders
                                    where o.OrderDate >= firstDayOfMonth
                                    && o.PaymentStatus == "จ่ายแล้ว"
                                    select (int?)o.TotalPrice).Sum() ?? 0;

        // จำนวนออเดอร์วันนี้
        var totalOrderToday = (from o in _db.Orders
                                where o.OrderDate.Value.Date == today
                                && o.OrderStatus != "ตะกร้า"
                                select o).Count();

        // ออเดอร์รอดำเนินการ
        var totalOrderPending = (from o in _db.Orders
                                    where o.OrderStatus == "รอตรวจสอบ"
                                    select o).Count();

        // สินค้าทั้งหมด
        var totalProduct = (from p in _db.Products select p).Count();

        // สินค้าใกล้หมด (> 0 และ < 5)
        var lowStockCount = (from p in _db.Products
                                where p.Quantity > 0 && p.Quantity < 5
                                select p).Count();

        // สินค้าหมดสต็อก
        var outOfStockCount = (from p in _db.Products
                                where p.Quantity == 0
                                select p).Count();

        // ลูกค้าทั้งหมด
        var totalCustomer = (from c in _db.Customers select c).Count();

        // ลูกค้าใหม่เดือนนี้
        var newCustomerThisMonth = (from o in _db.Orders
                                    where o.OrderDate >= firstDayOfMonth
                                    && o.OrderStatus != "ตะกร้า"
                                    select o.CusId).Distinct().Count();

        // ออเดอร์ล่าสุด 5 รายการ
        var recentOrders = (from o in _db.Orders
                            join c in _db.Customers on o.CusId equals c.CusId into oc
                            from c in oc.DefaultIfEmpty()
                            where o.OrderStatus != "ตะกร้า"
                            orderby o.OrderDate descending
                            select new RecentOrderItem
                            {
                                OrderNo = o.OrderNo,
                                CustomerName = c != null ? c.CusName : "-",
                                OrderDate = o.OrderDate ?? DateTime.Today,
                                TotalPrice = o.TotalPrice ?? 0,
                                OrderStatus = o.OrderStatus ?? "รอดำเนินการ",
                                PaymentStatus = o.PaymentStatus ?? "ยังไม่จ่าย"
                            }).Take(5).ToList();

        // สินค้าขายดี Top 5
        var topProducts = (from d in _db.OrderDetails
                            join o in _db.Orders on d.OrderId equals o.OrderId
                            join p in _db.Products on d.ProductId equals p.ProductId
                            where o.PaymentStatus == "จ่ายแล้ว"
                            group new { d, p } by new { p.ProductId, p.ProductName, p.Brand, p.Category } into g
                            orderby g.Sum(x => x.d.Quantity) descending
                            select new TopProductItem
                            {
                                ProductName = g.Key.ProductName,
                                Brand = g.Key.Brand,
                                Category = g.Key.Category,
                                SoldQty = g.Sum(x => x.d.Quantity) ?? 0,
                                Revenue = g.Sum(x => x.d.TotalPrice) ?? 0
                            }).Take(5).ToList();

        // สินค้าใกล้หมดสต็อก
        var lowStockProducts = (from p in _db.Products
                                where p.Quantity > 0 && p.Quantity < 5
                                orderby p.Quantity ascending
                                select new LowStockItem
                                {
                                    ProductName = p.ProductName,
                                    Brand = p.Brand,
                                    Category = p.Category,
                                    StockLeft = p.Quantity ?? 0,
                                    Unit = p.Unit
                                }).ToList();

        var vm = new DashboardViewModel();
        vm.TotalSaleToday = totalSaleToday;
        vm.TotalSaleThisMonth = totalSaleThisMonth;
        vm.TotalOrderToday = totalOrderToday;
        vm.TotalOrderPending = totalOrderPending;
        vm.TotalProduct = totalProduct;
        vm.LowStockCount = lowStockCount;
        vm.OutOfStockCount = outOfStockCount;
        vm.TotalCustomer = totalCustomer;
        vm.NewCustomerThisMonth = newCustomerThisMonth;
        vm.RecentOrders = recentOrders;
        vm.TopProducts = topProducts;
        vm.LowStockProducts = lowStockProducts;
        return View(vm);
    }


    public IActionResult ReplyOrder()
    {
        return View();
    }
    [HttpPost]
    public IActionResult ReplyOrder(int orderId, string note, string status)
    {
        var order = _db.Orders.FirstOrDefault(o => o.OrderId == orderId);

        if (order != null)
        {
            order.RejectNote = note;
            order.OrderStatus = status;

            if (status == "ยกเลิก")
            {
                order.PaymentStatus = "ไม่ผ่านการตรวจสอบ";
            }
            else if (status == "รอตรวจสอบ")
            {
                order.PaymentStatus = "รอตรวจสอบ";
            }

            _db.SaveChanges();
        }

        return RedirectToAction("OrderList", "Electrical");
    }
}
