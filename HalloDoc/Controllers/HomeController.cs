using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Repository.IRepository;
namespace HalloDoc.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPatientRepository _patientRepository;

        public HomeController(ILogger<HomeController> logger, IPatientRepository patientRepository)
        {
            _logger = logger;
            _patientRepository = patientRepository;
        }
  
      
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(AspNetUser user)
        {
            var data = _patientRepository.ValidateUser(user.Email, user.PasswordHash);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("createPatientRequest");
        }
 
        public IActionResult Forgotpassword()
        {
            return View();
        }
        public IActionResult createPatientAccount()
        {
            return View();
        }
    

        public IActionResult patientSite()
        {
            return View();
        }

        public IActionResult patientSubmitRequestScreen()
        {
            return View();
        }

        public IActionResult createPatientRequest()
        {
            return View();
        }

        public IActionResult familyCreateRequest()
        {
            return View();
        }


        public IActionResult conciergePatientRequest()
        {
            return View();
        }

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
            return View(new DataAccessLayer.DataModels.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}