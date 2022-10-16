
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;

namespace Shopping.Controllers;

public class CartController : Controller
{
   
    private ConnectDB db;
    public CartController(IConfiguration cfg)
    {
        db = new ConnectDB(cfg.GetConnectionString("connection"));
    }


    public IActionResult Index()
    {
        //check if sessionid exists in cookies.
        if (Request.Cookies["SessionId"] != null)
        {
            ViewData["username"] = db.GetUserBySession(Request.Cookies["SessionId"]).Username;
            ViewBag.Cookies = Request.Cookies["SessionId"];
        }

        string productids = "";


        if (Request.Cookies["items"] != null)
        {
            productids = Request.Cookies["items"];
        }

        ViewData["products"] = db.RetrieveProduct(productids);
        return View();
    }

}


