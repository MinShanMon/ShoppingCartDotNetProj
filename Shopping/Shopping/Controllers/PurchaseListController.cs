using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;

namespace Shopping.Controllers;

public class PurchaseListController : Controller
{
    private ConnectDB db;
    public PurchaseListController(IConfiguration cfg)
    {
        db = new ConnectDB(cfg.GetConnectionString("connection"));
    }

    public IActionResult Index()
    {
        if (Request.Cookies["SessionId"] == null)
        {
            return RedirectToAction("Index", "Login");
        }
        User user = db.GetUserBySession(Request.Cookies["SessionId"]);
        int userId = user.UserId;

    
            List<PurchasedList> products = db.RetrievePurchase(userId);
            ViewData["pruchaseList"] = products;
            List<PurchasedActivation> activities = db.RetrieveActivations(userId);
            ViewData["activities"] = activities;
            ViewData["username"] = user.Username;
       
        return View();
    }


    public IActionResult GetStar()
    {
        User user = db.GetUserBySession(Request.Cookies["SessionId"]);
        int userId = user.UserId;
        List<Reviews> nodes = db.GetStar(userId);
        return Content(JsonSerializer.Serialize(nodes));
    }


    public string SetStarRating(int productId, int rating)
    {
        User user = db.GetUserBySession(Request.Cookies["SessionId"]);
        int userId = user.UserId;
        List<Reviews> nodes = db.GetStar(userId);
        bool status = db.RatingIsExist(userId, productId, rating);

        return status ? "success" : "fail";
    }
  
}

