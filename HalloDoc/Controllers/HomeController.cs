﻿using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Repository.IRepository;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using NETCore.MailKit.Core;
using System.Collections;
using System.Net.Mail;
using System.Net;
using HalloDoc.DataAccessLayer.DataContext;

namespace HalloDoc.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPatientRepository _patientRepository;
        private readonly ApplicationDbContext _context;



        public HomeController(ILogger<HomeController> logger, IPatientRepository patientRepository, ApplicationDbContext context)
        {
            _logger = logger;
            _patientRepository = patientRepository;
            _context = context;
     
        }


 /*-----------------------------------Index--------------------------------------------------*/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AspNetUser user)
        {
          

            var data = _patientRepository.ValidateUser(user.Email, user.PasswordHash);
            if (data == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(user);
            }

            HttpContext.Session.SetString("key", user.Email);

            return RedirectToAction("patientDashboard");
        }


        /*-----------------------------------Forgot Password--------------------------------------------------*/
        public IActionResult Forgotpassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Reset(PatientLoginVM obj)
        {
            AspNetUser aspNetUser = _patientRepository.GetUserByEmail(obj.Email);
            if (aspNetUser == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                return RedirectToAction("Index", obj);
            }
            else
            {
                string senderEmail = "tatva.dotnet.disneyjaviya@outlook.com";

                string senderPassword = "Disney@20";
                string Token = Guid.NewGuid().ToString();
                string resetLink = $"{Request.Scheme}://{Request.Host}/Home/Resetpassword?token={Token}";

                Passwordreset temp = _context.Passwordresets.Where(x => x.Email == obj.Email).FirstOrDefault();



                if (temp != null)
                {
                    temp.Token = Token;
                    temp.Createddate = DateTime.Now;
                    temp.Isupdated = new BitArray(1);
                }
                else
                {
                   
                    Passwordreset passwordReset = new Passwordreset();
                    passwordReset.Token = Token;
                    passwordReset.Email = obj.Email;
                    passwordReset.Isupdated = new BitArray(1);
                    passwordReset.Createddate = DateTime.Now;
                    _context.Passwordresets.Add(passwordReset);
                }
                _context.SaveChanges();


                SmtpClient client = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "HalloDoc"),
                    Subject = "Set up your Account",
                    IsBodyHtml = true,
                    Body = $"Please click the following link to reset your password: <a href='{resetLink}'>{resetLink}</a>"
                };

                mailMessage.To.Add(obj.Email);

                client.SendMailAsync(mailMessage);

                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        //[Route("Patient/ResetPassword/{token}")]
        public IActionResult ResetPassword(string token)
        {
            var passwordReset = _context.Passwordresets.Where(u => u.Token == token).FirstOrDefault();

            ResetPasswordVM resetPasswordVM = new ResetPasswordVM
            {
                Email = passwordReset.Email,
                Token = token
            };

            TimeSpan difference = (TimeSpan)(DateTime.Now - passwordReset.Createddate);

            double hours = difference.TotalHours;

            if (hours > 24)
            {
                return NotFound();
            }

            if (passwordReset.Isupdated == new BitArray(1))
            {
                ModelState.AddModelError("Email", "You can only update one time using this link");
                return View(resetPasswordVM);
            }
            TempData["success"] = "Enter Password";
            return View(resetPasswordVM); ;

        }


        [HttpPost]
        //[Route("/ResetPassword/{token}")]

        public IActionResult ResetPassword(ResetPasswordVM obj)
        {
            var passwordReset = _context.Passwordresets.Where(u => u.Token == obj.Token).FirstOrDefault();

            AspNetUser aspNetUser = _patientRepository.GetUserByEmail(obj.Email);

            if (aspNetUser != null)
            {
                aspNetUser.PasswordHash = obj.Password;
                passwordReset.Isupdated = new BitArray(1, true);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }


        /*-----------------------------------Create Patient Account--------------------------------------------------*/
        public IActionResult createPatientAccount()
        {
            return View();
        }

     


        [HttpPost]
        public IActionResult CheckEmail(string email)
        {
            bool emailExists;
            var data = _patientRepository.GetUserByEmail(email);
            if (data == null)
            {
                emailExists = false;
            }
            else
            {
                emailExists = true;
            }


            return new OkObjectResult(new { exists = emailExists });
        }

        /*-----------------------------------Patient Dashboard--------------------------------------------------*/
        public IActionResult patientDashboard()
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            var res = _patientRepository.GetbyEmail(ViewBag.Data);
            return View(res);
        }
        /*-----------------------------------Request Me Form --------------------------------------------------*/
        public IActionResult requestMe()
        {
            return View();
        }

        [HttpPost]
        public IActionResult requestMe(createPatientRequest RequestData)
        {
            _patientRepository.createPatientRequestMe(RequestData);
            return RedirectToAction(nameof(requestMe));
        }


        /*-----------------------------------request Someone Else Form--------------------------------------------------*/
        public IActionResult requestSomeoneElse()
        {

            return View();
        }

        [HttpPost]
        public IActionResult requestSomeoneElse(requestSomeoneElse r)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _patientRepository.createPatientRequestSomeoneElse(ViewBag.Data ,r);
            return RedirectToAction(nameof(requestSomeoneElse));
        }


        /*-----------------------------------patient site--------------------------------------------------*/
        public IActionResult patientSite()
        {
            return View();
        }
        /*-----------------------------------Patient Submit Screen--------------------------------------------------*/
        public IActionResult patientSubmitRequestScreen()
        {
            return View();
        }
        /*-----------------------------------create patient request--------------------------------------------------*/
        public IActionResult createPatientRequest()
        {
            return View();
        }



        [HttpPost]
        public IActionResult createPatientRequest(createPatientRequest RequestData)
        {

            _patientRepository.CreateRequest(RequestData);
            return RedirectToAction(nameof(createPatientRequest));
        }

        /*-----------------------------------create family request--------------------------------------------------*/
        public IActionResult familyCreateRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult familyCreateRequest(familyCreateRequest RequestData)
        {
            var row = _context.AspNetUsers.Where(x => x.Email == RequestData.Email).FirstOrDefault();
            var res = _patientRepository.CreateFamilyRequest(RequestData);
            if(row==null)
            {
                SendEmailUser(RequestData.Email, res);
            }
            
            return RedirectToAction(nameof(familyCreateRequest));
        }
        /*-----------------------------------create concierge request--------------------------------------------------*/
        public IActionResult conciergePatientRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult conciergePatientRequest(conciergeCreateRequest RequestData)
        {
            var row = _context.AspNetUsers.Where(x => x.Email == RequestData.Email).FirstOrDefault();
            var res = _patientRepository.CreateConciergeRequest(RequestData);
            if (row == null)
            {
                SendEmailUser(RequestData.Email, res);
            }

            return RedirectToAction(nameof(conciergePatientRequest));
        }
        /*-----------------------------------create business request--------------------------------------------------*/
        public IActionResult businessPatientRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult businessPatientRequest(businessCreateRequest RequestData)
        {
            var row = _context.AspNetUsers.Where(x => x.Email == RequestData.Email).FirstOrDefault();
            var res = _patientRepository.CreateBusinessRequest(RequestData);
            if (row == null)
            {
                SendEmailUser(RequestData.Email, res);
            }
            return RedirectToAction(nameof(businessPatientRequest));
        }



        public Action SendEmailUser(String Email, string id)
        {
            // Existing code...
            AspNetUser aspNetUser = _patientRepository.GetUserByEmail(Email);


            string senderEmail = "tatva.dotnet.disneyjaviya@outlook.com";
            string senderPassword = "Disney@20";
            string resetLink = $"{Request.Scheme}://{Request.Host}/Home/createPatientAccount?id={id}";

            // Existing code...
            Passwordreset temp = _context.Passwordresets.Where(x => x.Email == Email).FirstOrDefault();


            /*if (temp.Isupdated.Get(0))
            {
                TempData["error"] = "You can only reset once";
                RedirectToAction("Patientlogin");

            }*/

            if (temp != null)
            {
                temp.Token = id.ToString();
                temp.Createddate = DateTime.Now;
                temp.Isupdated = new BitArray(1);
            }
            else
            {
                Passwordreset passwordReset = new Passwordreset();
                passwordReset.Token = id.ToString();
                passwordReset.Email = Email;
                passwordReset.Isupdated = new BitArray(1);
                passwordReset.Createddate = DateTime.Now;
                _context.Passwordresets.Add(passwordReset);
            }
            _context.SaveChanges();


            SmtpClient client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, "HalloDoc"),
                Subject = "Set up your Account",
                IsBodyHtml = true,
                Body = $"Please click the following link to reset your password: <a href='{resetLink}'>{resetLink}</a>"
            };

            mailMessage.To.Add(Email)
;

            client.SendMailAsync(mailMessage);
            return null;
        }


        [HttpGet]
        public IActionResult createPatientAccount(string id)
        {
            var passwordReset = _context.Passwordresets.Where(u => u.Token == id).FirstOrDefault();

            if (passwordReset == null)
            {
                return NotFound(); // No password reset request found with the given id
            }
            ResetPasswordVM resetPasswordVM = new ResetPasswordVM
            {
                Email = passwordReset.Email,
                Token = id
            };

            TimeSpan difference = (TimeSpan)(DateTime.Now - passwordReset.Createddate);

            if (difference.TotalHours > 24)
            {
                return NotFound(); // Password reset request expired
            }

            if (passwordReset.Isupdated.Get(0))
            {
                ModelState.AddModelError("Email", "You can only update once using this link");
                return View(new ResetPasswordVM()); // Display error message
            }

            TempData["success"] = "Enter Password";
            return View(new ResetPasswordVM { Token = id }); // Pass the token to the view
        }

        [HttpPost]
        public IActionResult createPatientAccount(ResetPasswordVM obj)
        {
            var passwordReset = _context.Passwordresets.Where(u => u.Token == obj.Token).FirstOrDefault();

            AspNetUser aspNetUser = _context.AspNetUsers.Where(x => x.Id == obj.Token).FirstOrDefault();

            if (aspNetUser != null)
            {
                aspNetUser.PasswordHash = obj.Password;
                passwordReset.Isupdated = new BitArray(1, true);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }



        /*-----------------------------------View Documents--------------------------------------------------*/
        public IActionResult ViewDocuments(int requestId)
        {
            var document = _patientRepository.GetDocumentsByRequestId(requestId);

            if (document == null)
            {
                return NotFound(); 
            }

            return View(document);
        }

        public IActionResult DownloadFile(int fileId)
        {
            var file = _patientRepository.GetFileById(fileId);
            if (file == null)
            {
                return NotFound(); 
            }

            var filePath = Path.Combine("wwwroot/Files", file.FileName);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        /*-----------------------------------Patient Profile--------------------------------------------------*/
        public IActionResult patientProfile()
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            var res = _patientRepository.GetPatientData(ViewBag.Data);
            return View(res);
        }
        [HttpPost]
        public IActionResult patientProfile(User u)
        {

            ViewBag.Data = HttpContext.Session.GetString("key");
            _patientRepository.updateProfile(ViewBag.Data, u);
            return View();
        }

        /*-----------------------------------Upload Files--------------------------------------------------*/

        public IActionResult UploadFiles(int requestId, List<IFormFile> files)
        {
            _patientRepository.UploadFiles(requestId, files);
            return RedirectToAction("ViewDocuments", new { requestId = requestId });
        }


        public IActionResult DownloadFiles(List<int> fileIds)
        {
            IEnumerable<RequestWiseFile> files;
            if (fileIds != null && fileIds.Any())
            {
                files = _patientRepository.GetFilesByIds(fileIds);
            }
            else
            {
                files = _patientRepository.GetAllFiles();
            }

            var zipMemoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var filePath = Path.Combine("wwwroot/Files", file.FileName);
                    var entry = zipArchive.CreateEntry(file.FileName);
                    using (var entryStream = entry.Open())
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }
            }

            zipMemoryStream.Seek(0, SeekOrigin.Begin);
            return File(zipMemoryStream, "application/zip", "DownloadedFiles.zip");
        }

        /*-----------------------------------Review Agreement--------------------------------------------------*/
        public IActionResult reviewAgreement()
        {
            return View();
        }

        /*-----------------------------------Logout--------------------------------------------------*/
        public IActionResult logOut()
        {
            HttpContext.Session.Remove("key");
            return RedirectToAction("Index");
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