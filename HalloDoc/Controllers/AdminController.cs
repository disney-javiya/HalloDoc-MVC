using Microsoft.AspNetCore.Mvc;
using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.Models;
using System.Diagnostics;
using Repository.IRepository;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;
using HalloDoc.DataAccessLayer.DataContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.EntityFrameworkCore;
using System.Security;


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
        public int getCountNumber(int type)
        {
            var res = _adminRepository.getRequestStateData(type);

            return res.Count();
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

        public IActionResult adminViewCase(int requestId)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");

            var requestClient = _adminRepository.getPatientInfo(requestId);
            if (requestClient != null)
            {
                var confirmationNumber = _adminRepository.getConfirmationNumber(requestId);
                ViewBag.ConfirmationNumber = confirmationNumber;
            }

            return View(requestClient);
            
        }
        [HttpGet]
        public IActionResult adminViewNotes(int requestId)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            var res = _adminRepository.getNotes(requestId, ViewBag.Data);
           

            return View(res);

        }

        [HttpPost]
        public IActionResult adminViewNotes(int requestId, viewNotes v)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.adminNotes(requestId, v, ViewBag.Data);
            return RedirectToAction("adminDashboard");

        }

        [HttpPost]
        public IActionResult adminCancelNote(string requestId, string reason, string additionalNotes)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.adminCancelNote(requestId,reason,additionalNotes, ViewBag.Data);
            return RedirectToAction("adminDashboard");

        }
        [HttpGet]
        public List<Physician> GetPhysicians(int regionId)
        {
            var res = _adminRepository.GetPhysicians(regionId);
            return res;
        }
        [HttpPost]
        public IActionResult adminAssignNote(string requestId, string region, string physician, string additionalNotesAssign)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.adminAssignNote(requestId, region, physician, additionalNotesAssign, ViewBag.Data);
            return RedirectToAction("adminDashboard");

        }

        //[HttpGet]
        //public IActionResult adminViewNotes(int requestId)
        //{
        //    ViewBag.Data = HttpContext.Session.GetString("key");
        //    var res = _adminRepository.getNotes(requestId);
        //    ViewBag.TransferNotes = _adminRepository.getTranferNotes(requestId);

        //    return View(res);

        //}
        //[HttpPost]
        //public IActionResult adminViewNotes(int requestId, RequestNote r)
        //{
        //    ViewBag.Data = HttpContext.Session.GetString("key");
        //    _adminRepository.adminNotes(requestId, r, ViewBag.Data);
        //    return RedirectToAction("adminDashboard");

        //}


        public IActionResult logOut()
        {
            HttpContext.Session.Remove("key");
            return RedirectToAction("Index");
        }
    }
}
