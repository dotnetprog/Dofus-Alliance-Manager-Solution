using Microsoft.AspNetCore.Mvc;

namespace DAM.WebApp.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound404()
        {
            return View();
        }
        public IActionResult InternalError505()
        {
            return View();
        }
        [Route("/Error/HandleError/{code}")]
        public IActionResult HandleError(int code)
        {
            if (code == 500)
                return View("~/Views/Error/InternalError505.cshtml");
            return View("~/Views/Error/NotFound404.cshtml");
        }
    }
}
