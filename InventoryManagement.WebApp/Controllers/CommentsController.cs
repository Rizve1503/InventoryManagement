using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.WebApp.Controllers
{
    public class CommentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
