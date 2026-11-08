using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _66076431_natalee.Models;

namespace _66076431_natalee.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // int No1,No2,No3 = 5,No4;
        // No1 = 1;
        // No2 = 3;
        // No4 = No1 + No2 + No3;
        // char T1 = 'A', T2 = 'B';
        // string tx1;
        // tx1 = Convert.ToString(T1) + Convert.ToString(T2);
        // ViewBag.R1 = tx1;

        string fullname = "นาตาลี นาคทรัพย์";
        string studentId = "66076431";
        string room = "5-904";
        string wicha = "CSI402-L001";
        int year = 3;
        string language = "html, css";
        
        int score_1, score_2, score_3 , score_4, score_5 , score_6 , score_7 , score_8 , score_9 , score_10;
        score_1 = 10;
        score_2 = 3;
        score_3 = 7;       
        score_4 = 8;
        score_5 = 9;
        score_6 = 7;
        score_7 = 6;
        score_8 = 8;
        score_9 = 2;
        score_10 = 7;

        //scores[i];
        // int[] scores = new int[10];
        // scores[0] = 10;
        // scores[1] = 3;
        // scores[2] = 7;       
        // scores[3] = 8;
        // scores[4] = 9;
        // scores[5] = 7;
        // scores[6] = 4;
        // scores[7] = 8;
        // scores[8] = 2;
        // scores[9] = 9;

        
        
        // int  x = scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9];
        int  x = score_1 + score_2 + score_3 + score_4 + score_5 + score_6 + score_7 + score_8 + score_9 + score_10;
        if (x >= 80)
        {
            ViewBag.grade = "A";
        }
        else if (x >= 76)
        {
            ViewBag.grade = "B+";
        }
        else if (x >= 70)
        {
            ViewBag.grade = "B";
        }
        else if (x >= 66)
        {
            ViewBag.grade = "C+";
        }
        else if (x >= 60)
        {
            ViewBag.grade = "C";
        }
        else if (x >= 56)
        {
            ViewBag.grade = "D+";
        }
        else if (x >= 50)
        {
            ViewBag.grade = "D";
        }
        else if (x < 50)
        {
            ViewBag.grade = "F";
        }

        // for (int i = 0; i < 10; i++)
        // {
        //     int score = scores[i];

        //     if (score <5)
        //         ViewBag.SP = "ชิ้นงาน" + i;
        // }

        // string[] sp = new string[10];
        // for (int i = 0; i < 10; i++)
        // {
        //     if (scores[i] < 5)
        //         sp[i] = "ชิ้นงาน " + (i+1) + " ";
        //     else 
        //         sp[i]  = "";
        // }
        // ViewBag.SP0 = sp[0];
        // ViewBag.SP1 = sp[1];
        // ViewBag.SP2 = sp[2];
        // ViewBag.SP3 = sp[3];
        // ViewBag.SP4 = sp[4];
        // ViewBag.SP5 = sp[5];
        // ViewBag.SP6 = sp[6];
        // ViewBag.SP7 = sp[7];
        // ViewBag.SP8 = sp[8];
        // ViewBag.SP9 = sp[9];


        ViewBag.Name = fullname;
        ViewBag.StudentId = studentId;
        ViewBag.Room = room;
        ViewBag.Wicha = wicha;
        ViewBag.Year = year;
        ViewBag.Language = language;
        ViewBag.Score_1 = score_1;
        ViewBag.Score_2 = score_2;
        ViewBag.Score_3 = score_3;
        ViewBag.Score_4 = score_4;      
        ViewBag.Score_5 = score_5;
        ViewBag.Score_6 = score_6;
        ViewBag.Score_7 = score_7;
        ViewBag.Score_8 = score_8;
        ViewBag.Score_9 = score_9;
        ViewBag.Score_10 = score_10;
        ViewBag.TotalScore = score_1 + score_2 + score_3 + score_4 + score_5 + score_6 + score_7 + score_8 + score_9 + score_10;
        // ViewBag.Score1 = scores[0];
        // ViewBag.Score2 = scores[1];
        // ViewBag.Score3 = scores[2];
        // ViewBag.Score4 = scores[3];       
        // ViewBag.Score5 = scores[4];
        // ViewBag.Score6 = scores[5];
        // ViewBag.Score7 = scores[6];
        // ViewBag.Score8 = scores[7];
        // ViewBag.Score9 = scores[8];
        // ViewBag.Score10 = scores[9];
        // ViewBag.TotalScore = scores[0] + scores[1] + scores[2] + scores[3] + scores[4] + scores[5] + scores[6] + scores[7] + scores[8] + scores[9];

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
