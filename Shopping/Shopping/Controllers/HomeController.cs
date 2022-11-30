using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;

namespace Shopping.Controllers;

public class HomeController : Controller
{

    private ConnectDB db;
    public HomeController(IConfiguration cfg)
    {
        db = new ConnectDB(cfg.GetConnectionString("connection"));
    }

    public IActionResult Index(string search)
    {
        try
        {
            if (Request.Cookies["SessionId"] != null)
            {
                ViewData["username"] = db.GetUserBySession(Request.Cookies["SessionId"]).Username;
                ViewBag.Cookies = Request.Cookies["SessionId"];
            }
        }
        catch (Exception e)
        {

        }
        finally
        {
            //defalt homepage(when search box is empty)
            if (string.IsNullOrEmpty(search))
            {
                ViewData["AllProduct"] = db.RetrieveProduct();
            }
            else
            {
                //bind search result to view
                ViewData["AllProduct"] = db.SearchProduct(search);
                ViewData["search"] = search;
            }
        }
        return View();
    }
}

