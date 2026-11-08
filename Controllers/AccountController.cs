using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _66076431_natalee.Models;
using _66076431_natalee.ViewModels;
using _66076431_natalee.Models.db;

namespace _66076431_natalee.Controllers;

public class AccountController : Controller
{
    

    public IActionResult Test1()
    {
        return View();
    }

    public IActionResult Lab4()
    {
        return View();
    }

    public IActionResult Lab5()
    {
        //อันนี้คือการทำแบบ 1:1
        // var user = new LabUserViewModel();
        // user.UserId = "PPP";
        // user.Name = "Abc";
        // user.LastName = "Def";
        // user.Address = "Some Address";
        // user.Age = 30;
        // user.Height = 175.5M;
        // user.Weight = 70.2M;

        //อันนี้คือการทำแบบ 1:N หรือ List
        var user = new List<LabUserViewModel>
        {
            new LabUserViewModel{ UserId = "PPP",Name = "Abc",LastName = "Def"},
            new LabUserViewModel{ UserId = "QQQ",Name = "Ghi",LastName = "Jkl"}

        };
        return View(user);
        // return View();
    }

    [HttpPost]
    public IActionResult Lab5(LabUserViewModel data)
    {
        string A,B,C;
        A = data.UserId;
        B = data.Name;
        C = data.LastName;
        ViewBag.UserId = A;
        ViewBag.Name = B;
        ViewBag.LastName = C;

        // return View();
        return RedirectToAction("Lab52","Account",new{UserId = data.UserId, Name = data.Name, LastName = data.LastName});
    }

    public IActionResult Lab52(string UserId, string Name, string LastName)
    {
        ViewBag.UserId = UserId;
        ViewBag.Name = Name;
        ViewBag.LastName = LastName;
        
        return View();
    }
    
    //ประกาศใช้ db
    public readonly Csi402dbContext _db;

    public AccountController(Csi402dbContext db)
    {
    
        _db = db;
    }
    public IActionResult Lab8()
    {
        var Customer = (from u in _db.Customers select u).ToList();
        return View(Customer);
    }
    public IActionResult Lab9()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Lab9(Lab9UserViewModel data)
    {   
        var u = new LabStudent();
        u.Stdld = data.UserId;
        u.StdldName = data.Name;
        u.StdldLastname = data.LastName;
        u.StdldPassword = data.Password;
        _db.Add(u);
        _db.SaveChanges();
        return RedirectToAction("Lab9List","Account");
    }

    public IActionResult Lab9List()
    {
        var user = (from u in _db.LabStudents select new Lab9UserViewModel
        {
            UserId = u.Stdld,
            Password = u.StdldPassword,
            Name = u.StdldName,
            LastName = u.StdldLastname
        }).ToList();
        return View(user);
    }

    public IActionResult Lab10(string UID)
    {
        var chenk = (from us in _db.LabStudents where us.Stdld == UID select new Lab9UserViewModel
        {
            UserId = us.Stdld,
            Password = us.StdldPassword,
            Name = us.StdldName,
            LastName = us.StdldLastname
        }).FirstOrDefault();
        return View(chenk);
    }

    [HttpPost]
    public IActionResult Lab10(Lab9UserViewModel data)
    {   
        var user = ( from u in  _db.LabStudents where u.Stdld == data.UserId select u).FirstOrDefault();
        user.StdldName = data.Name;
        user.StdldLastname = data.LastName;
        user.StdldPassword = data.Password;

        _db.Update(user);
        _db.SaveChanges();
        return RedirectToAction("Lab9List","Account");
    }

    public IActionResult Lab10D(string UID)
    {
        var user = (from u in _db.LabStudents where u.Stdld == UID select u).FirstOrDefault();

        _db.RemoveRange(user);
        _db.SaveChanges();
        return RedirectToAction("Lab9List","Account");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
