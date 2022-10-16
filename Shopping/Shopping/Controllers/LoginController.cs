using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shopping;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shopping.Controllers
{
    public class LoginController : Controller
    {

        private ConnectDB db;

        public LoginController(IConfiguration cfg)
        {
            db = new ConnectDB(cfg.GetConnectionString("connection"));
        }


        public IActionResult isLogin(string returnURL)
        {
            if (Request.Cookies["SessionId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Login", Action = "Index", returnURL = returnURL }));

        }


        //[HttpPost]
        public IActionResult Index(string username, string password, string returnURL)
        {
            User user = db.GetUserByUsername(username);
            if (user != null)
            {

                if (user.Password == password)
                {

                    string sessionId = db.AddSession(user.UserId);

                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Append("SessionId", sessionId, options);

                    if (!string.IsNullOrEmpty(returnURL))
                    {
                        return Redirect(returnURL);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    ViewBag.loginErr = "Your Username or Password is incorrect. Please try again.";
                }
            }

            if (username != null)
            {
                ViewBag.loginErr = "Your Username or Password is incorrect. Please try again.";
            }


            ViewBag.param = returnURL;
            return View();
        }


    }
}

