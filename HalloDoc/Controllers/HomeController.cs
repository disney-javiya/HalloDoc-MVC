using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HalloDoc.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
  
        [Route("/index.html")]
      
        public IActionResult Index()
        {
            return View();
        }
        [Route("/forgot-password.html")]
        public IActionResult Forgotpassword()
        {
            return View();
        }
        public IActionResult createPatientAccount()
        {
            return View();
        }
        [Route("~/")]
        [Route("/patient-site.html")]
        public IActionResult patientSite()
        {
            return View();
        }
        [Route("/patient-submit-request-page.html")]
        public IActionResult patientSubmitRequestScreen()
        {
            return View();
        }
        [Route("/create-patient-request.html")]
        public IActionResult createPatientRequest()
        {
            return View();
        }
        [Route("/family-create-request.html")]
        public IActionResult familyCreateRequest()
        {
            return View();
        }

        [Route("/concierge-patient-request.html")]
        public IActionResult conciergePatientRequest()
        {
            return View();
        }
        [Route("/business-patient-request.html")]
        public IActionResult businessPatientRequest()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}