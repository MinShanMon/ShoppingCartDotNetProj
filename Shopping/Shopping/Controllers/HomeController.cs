using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;

namespace Shopping.Controllers;

public class HomeController : Controller
{
    //private readonly ILogger<HomeController> _logger;

    //private ConnectDB db;



    //public HomeController(ILogger<HomeController> logger)
    //{
    //    _logger = logger;
    //    db = new ConnectDB(@"Server=localhost;Database=ShoppingCartDB;User ID=sa;Password=StrongPwd@19960831");
    //}
    private ConnectDB db;
    public HomeController(IConfiguration cfg)
    {
        db = new ConnectDB(cfg.GetConnectionString("connection"));
    }

    public IActionResult Index(string search)
    {
        //defalt homepage(when search box is empty)
        if (string.IsNullOrEmpty(search))
        {
            if (Request.Cookies["SessionId"] != null)
            {
                ViewData["username"] = db.GetUserBySession(Request.Cookies["SessionId"]).Username;
                ViewBag.Cookies = Request.Cookies["SessionId"];
            }
            ViewData["AllProduct"] = db.RetrieveProduct();
           
            
        }
        else
        {
            if (Request.Cookies["SessionId"] != null)
            {
                ViewData["username"] = db.GetUserBySession(Request.Cookies["SessionId"]).Username;
                ViewBag.Cookies = Request.Cookies["SessionId"];
            }
            //bind search result to view
            ViewData["AllProduct"] = db.SearchProduct(search);
            ViewBag.Cookies = Request.Cookies["SessionId"];
            //ViewData["username"] = db.GetUserBySession(Request.Cookies["SessionId"].ToString());
      
            ViewData["search"] = search;

        }
           
        return View();
    }

    //bind data by matching JSON keys against property names
    public IActionResult AddToCart(string sessionId,int productId)
    {
        User user = db.GetUserBySession(sessionId);
        int userId = user.UserId;
        //increment qty by 1 for each click action
        db.AddUserCart(userId, productId, 1);
        return Json(new {isOkay=true});
    }




    public IActionResult Cart(string productids)
    {
         if (Request.Cookies["SessionId"] != null)
            {
                ViewData["username"] = db.GetUserBySession(Request.Cookies["SessionId"]).Username;
                ViewBag.Cookies = Request.Cookies["SessionId"];
            }
        if (String.IsNullOrEmpty(productids))
        {
            if (Request.Cookies["items"] != null)
            {
                productids = Request.Cookies["items"];
            }
            else
            {
                return View();
            }
        }
        ViewData["products"] = db.RetrieveProduct(productids);
        return View();
    }

    public IActionResult PurchaseList()
    {
        int userId = db.GetUserBySession(Request.Cookies["SessionId"]).UserId;
        List<PurchasedList> products = db.RetrievePurchase(userId);
        ViewData["pruchaseList"] = products;
        List<PurchasedActivation> activities = db.RetrieveActivations(userId);
        ViewData["activities"] = activities;
        return View();
    }


    public IActionResult GetStar()
    {

        List<Reviews> nodes = db.GetStar(2);
        return Content(JsonSerializer.Serialize(nodes));
    }


    public string SetStarRating(int productId, int rating)
    {

        bool status = db.RatingIsExist(2, productId, rating);

        return status ? "success" : "fail";
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

