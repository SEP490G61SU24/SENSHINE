using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ReceptionistController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;


        public ReceptionistController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger) : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        

        public async Task<IActionResult> Dashboard()
        {
            UserDTO userProfile = ViewData["UserProfile"] as UserDTO;
            if (userProfile == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }
    }
}
