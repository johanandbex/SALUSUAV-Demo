using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SALUSUAV_Demo.Models;

namespace SALUSUAV_Demo.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

      
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
