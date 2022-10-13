using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shopping.Controllers
{
    public class CheckoutController : Controller
    {
        private ConnectDB db;
        private int userId;

        public CheckoutController(IConfiguration cfg)
        {
            db = new ConnectDB(cfg.GetConnectionString("connection"));
        }


        /*public IActionResult Index()
        {
            /*string sessionId = HttpContext.Session.GetString("sessionid");
            if (sessionId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }*/




        //binds local storage data to list of cart detail
        [HttpPost]
        public bool Index([FromBody] List<CartDetail> data)
        {

            //check if sessionid exists in cookies. If not, redirect to login page.
            /*string sessionId = HttpContext.Session.GetString("sessionid");
            if (String.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("Index", "Login");
            }*/

            if (Request.Cookies["SessionId"] == null)
            {
                return false;
            }

            //model bind localstorage[cart]
            //connect to db and insert into order table new record with userid
            //insert into orderdetails table the orders in the localstorage[cart]
            //clear localstorage[cart] when successful
            //redirect to my purchases page
            User user = db.GetUserBySession(Request.Cookies["SessionId"]);
            int userid = user.UserId;
            db.AddOrder(userid, data);

            return true;

         }

    }
}

