using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shopping;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShoppingWebsite.Controllers
{
    public class LogoutController : Controller
    {
        private ConnectDB db;

        public LogoutController(IConfiguration cfg)
        {
            db = new ConnectDB(cfg.GetConnectionString("connection"));
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            if (Request.Cookies["SessionId"] != null)
            {
                // remove our entry from database
                db.RemoveSession(Request.Cookies["SessionId"]);

                // ask client to remove its cookie so that it won't sent
                // it back next time
                Response.Cookies.Delete("SessionId");
                Response.Cookies.Delete("items");

            }

            return RedirectToAction("Index", "Login");
        }
    }
}

