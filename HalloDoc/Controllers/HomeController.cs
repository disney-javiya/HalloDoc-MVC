﻿using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Repository.IRepository;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace HalloDoc.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPatientRepository _patientRepository;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnv;

        public HomeController(ILogger<HomeController> logger, IPatientRepository patientRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _logger = logger;
            _patientRepository = patientRepository;

            this.hostingEnv = env;
        }



        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(AspNetUser user)
        {
            HttpContext.Session.SetString("key", user.Email);
      

            var data = _patientRepository.ValidateUser(user.Email, user.PasswordHash);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("patientDashboard");
        }
 
        public IActionResult Forgotpassword()
        {
            return View();
        }
        public IActionResult createPatientAccount()
        {
            return View();
        }

        public IActionResult logOut()
        {
            HttpContext.Session.Remove("key");
            return RedirectToAction("Index");         
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

        public IActionResult patientDashboard()
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            var res = _patientRepository.GetbyEmail(ViewBag.Data);
            return View(res);
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



        [HttpPost]
        public IActionResult createPatientRequest(createPatientRequest RequestData)
        {

            _patientRepository.CreateRequest(RequestData);
            return RedirectToAction(nameof(createPatientRequest));
        }

        public IActionResult familyCreateRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult familyCreateRequest(familyCreateRequest RequestData)
        {
            _patientRepository.CreateFamilyRequest(RequestData);
            return RedirectToAction(nameof(familyCreateRequest));
        }
        public IActionResult conciergePatientRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult conciergePatientRequest(conciergeCreateRequest RequestData)
        {
            _patientRepository.CreateConciergeRequest(RequestData);
            return RedirectToAction(nameof(conciergePatientRequest));
        }

        public IActionResult businessPatientRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult businessPatientRequest(businessCreateRequest RequestData)
        {
            _patientRepository.CreateBusinessRequest(RequestData);
            return RedirectToAction(nameof(businessPatientRequest));
        }
        public IActionResult ViewDocuments(int requestId)
        {
            var document = _patientRepository.GetDocumentsByRequestId(requestId);

            if (document == null)
            {
                return NotFound(); // Handle the case where the document is not found
            }

            return View(document);
        }

        public IActionResult DownloadFile(int fileId)
        {
            var file = _patientRepository.GetFileById(fileId);
            if (file == null)
            {
                return NotFound(); // Handle the case where the file is not found
            }

            var filePath = Path.Combine("wwwroot/Files", file.FileName);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        public IActionResult patientProfile()
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            var res = _patientRepository.GetPatientData(ViewBag.Data);
            return View(res);
        }
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