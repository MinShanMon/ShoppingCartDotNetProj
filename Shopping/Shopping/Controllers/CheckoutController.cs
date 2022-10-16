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


        //binds local storage data to list of cart detail

        public IActionResult Index([FromBody] List<CartDetail> data)
        {
            try
            {
                //check if sessionid exists in cookies.
                if (Request.Cookies["SessionId"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                //model bind localstorage[cart]
                //connect to db and insert into order table new record with userid
                //insert into orderdetails table the orders in the localstorage[cart]
                //clear localstorage[cart] when successful
                //redirect to my purchases page
                User user = db.GetUserBySession(Request.Cookies["SessionId"]);
                int userid = user.UserId;
                db.AddOrder(userid, data);
                return Json(new { isOkay = true });
            }
            catch (Exception e)
            {
                return Json(new { isOkay = false });
            }
         
        }
    }
}

