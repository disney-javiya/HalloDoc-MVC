using Microsoft.AspNetCore.Mvc;
using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.Models;
using System.Diagnostics;
using Repository.IRepository;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;
using HalloDoc.DataAccessLayer.DataContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HalloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminRepository _adminRepository;
  



        public AdminController(ILogger<AdminController> logger, IAdminRepository adminRepository)
        {
            _logger = logger;
            _adminRepository = adminRepository;
 

        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AspNetUser user)
        {


            var data = _adminRepository.ValidateUser(user.Email, user.PasswordHash);
            if (data == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(user);
            }

            // Set session key only when user credentials are validated successfully
            HttpContext.Session.SetString("key", user.Email);

            return RedirectToAction("adminDashboard");
        }
        public IActionResult adminDashboard()
        {
           ViewBag.Data = HttpContext.Session.GetString("key");
            return View();
        }

        public IActionResult adminTableData(int type)
        {
          var res =  _adminRepository.getRequestStateData(type);
            if(type == 1)
            {
                return PartialView("_newState", res);
            }
            else if (type == 2)
            {
                return PartialView("_pendingState", res);
            }
            else if (type == 3)
            {
                return PartialView("_activeState", res);
            }
            else if (type == 4)
            {
                return PartialView("_concludeState", res);
            }
            else if (type == 5)
            {
                return PartialView("_tocloseState", res);
            }
            else if (type == 6)
            {
                return PartialView("_unpaidState", res);
            }

            return View();
        }

        public IActionResult logOut()
        {
            HttpContext.Session.Remove("key");
            return RedirectToAction("Index");
        }
    }
}
