using Microsoft.AspNetCore.Mvc;

public class ErrorsController : Controller
{
    [Route("Errors/403")]
    public IActionResult Error403()
    {
        return View("403");
    }

    [Route("Errors/404")]
    public IActionResult Error404()
    {
        return View("404");
    }

    [Route("Errors/500")]
    public IActionResult Error500()
    {
        return View("500");
    }
}
