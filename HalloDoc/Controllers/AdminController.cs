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
using Repository;
using System.IO.Compression;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Net.Mail;
using System.Net;
using Elfie.Serialization;
using HalloDoc.AuthMiddleware;
using System.Text;


namespace HalloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly IAuthenticateRepository _authenticate;



        public AdminController(ILogger<AdminController> logger, IAdminRepository adminRepository, IAuthenticateRepository authenticate)
        {
            _logger = logger;
            _adminRepository = adminRepository;
            _authenticate = authenticate;

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
            AspNetUser loginuser = new()
            {
                Email = data.Email,
                UserName = data.UserName,
                Id = data.Id
            };

            var jwttoken = _authenticate.GenerateJwtToken(loginuser, "Admin");
            Response.Cookies.Append("jwt", jwttoken);
            ViewBag.LoginSuccess = true;
            // Set session key only when user credentials are validated successfully
            HttpContext.Session.SetString("key", user.Email);
            ViewBag.Data = HttpContext.Session.GetString("key");

            List<Region> r = new List<Region>();
            r = _adminRepository.getAllRegions();
            return View("adminDashboard", r);
        }

        [CustomeAuthorize("Admin")]
        public IActionResult adminDashboard()
        {
           ViewBag.Data = HttpContext.Session.GetString("key");
            List<Region> r = new List<Region>();
            r = _adminRepository.getAllRegions();
            return View(r);
        }
        public List<int> getCountNumber()
        {
            List<int> result = new List<int>();
            IEnumerable<RequestandRequestClient> res;
            for (int i = 1; i<=6; i++)
            {
               res = _adminRepository.getRequestStateData(i);
               result.Add(res.Count());
            }


            return result;
        }
        [CustomeAuthorize("Admin")]
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

        [HttpPost]
        public FileResult Export(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "Data.xls");
        }   


        [CustomeAuthorize("Admin")]
        public IActionResult getFilterByRegions(int regionId, int type)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            IEnumerable<RequestandRequestClient> r = _adminRepository.getRequestStateData(type);
            List<RequestandRequestClient> byregion = _adminRepository.getFilterByRegions(r, regionId);

            if (type == 1)
            {
                return PartialView("_newState", byregion);
            }
            else if (type == 2)
            {
                return PartialView("_pendingState", byregion);
            }
            else if (type == 3)
            {
                return PartialView("_activeState", byregion);
            }
            else if (type == 4)
            {
                return PartialView("_concludeState", byregion);
            }
            else if (type == 5)
            {
                return PartialView("_tocloseState", byregion);
            }
            else if (type == 6)
            {
                return PartialView("_unpaidState", byregion);
            }

            return View();
           

        }

        public IActionResult getByRequesttypeId(string typeid, int type)
        {
            int reqId = int.Parse(typeid);
            ViewBag.Data = HttpContext.Session.GetString("key");
            IEnumerable<RequestandRequestClient> r = _adminRepository.getRequestStateData(type);
            List<RequestandRequestClient> bytypeid = _adminRepository.getByRequesttypeId(r, reqId);

            if (type == 1)
            {
                return PartialView("_newState", bytypeid);
            }
            else if (type == 2)
            {
                return PartialView("_pendingState", bytypeid);
            }
            else if (type == 3)
            {
                return PartialView("_activeState", bytypeid);
            }
            else if (type == 4)
            {
                return PartialView("_concludeState", bytypeid);
            }
            else if (type == 5)
            {
                return PartialView("_tocloseState", bytypeid);
            }
            else if (type == 6)
            {
                return PartialView("_unpaidState", bytypeid);
            }

            return View();


        }

        public IActionResult getByRequesttypeIdRegionAndName(string typeid, int type, int regionid, string patient_name)
        {
            int reqId = int.Parse(typeid);
            ViewBag.Data = HttpContext.Session.GetString("key");
            IEnumerable<RequestandRequestClient> r = _adminRepository.getRequestStateData(type);
           
           List<RequestandRequestClient> bytypeid = _adminRepository.getByRequesttypeIdRegionAndName(r, reqId, regionid, patient_name);
            if (type == 1)
            {
                return PartialView("_newState", bytypeid);
            }
            else if (type == 2)
            {
                return PartialView("_pendingState", bytypeid);
            }
            else if (type == 3)
            {
                return PartialView("_activeState", bytypeid);
            }
            else if (type == 4)
            {
                return PartialView("_concludeState", bytypeid);
            }
            else if (type == 5)
            {
                return PartialView("_tocloseState", bytypeid);
            }
            else if (type == 6)
            {
                return PartialView("_unpaidState", bytypeid);
            }

            return View();

        }
            public IActionResult getFilterByName(string patient_name, int type)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            IEnumerable<RequestandRequestClient> r = _adminRepository.getRequestStateData(type);
            List<RequestandRequestClient> byregion = _adminRepository.getFilterByName(r, patient_name);

            if (type == 1)
            {
                return PartialView("_newState", byregion);
            }
            else if (type == 2)
            {
                return PartialView("_pendingState", byregion);
            }
            else if (type == 3)
            {
                return PartialView("_activeState", byregion);
            }
            else if (type == 4)
            {
                return PartialView("_concludeState", byregion);
            }
            else if (type == 5)
            {
                return PartialView("_tocloseState", byregion);
            }
            else if (type == 6)
            {
                return PartialView("_unpaidState", byregion);
            }

            return View();

        }

        public IActionResult getFilterByRegionAndName(string patient_name, int type, int regionId)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            IEnumerable<RequestandRequestClient> r = _adminRepository.getRequestStateData(type);
            List<RequestandRequestClient> byregionandname = _adminRepository.getFilterByRegionAndName(r, patient_name, regionId);
            if (type == 1)
            {
                return PartialView("_newState", byregionandname);
            }
            else if (type == 2)
            {
                return PartialView("_pendingState", byregionandname);
            }
            else if (type == 3)
            {
                return PartialView("_activeState", byregionandname);
            }
            else if (type == 4)
            {
                return PartialView("_concludeState", byregionandname);
            }
            else if (type == 5)
            {
                return PartialView("_tocloseState", byregionandname);
            }
            else if (type == 6)
            {
                return PartialView("_unpaidState", byregionandname);
            }

            return View();

        }



        [CustomeAuthorize("Admin")]
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
        [CustomeAuthorize("Admin")]
        [HttpGet]
        public IActionResult adminViewNotes(int requestId)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            var res = _adminRepository.getNotes(requestId, ViewBag.Data);
           

            return View(res);

        }
        [CustomeAuthorize("Admin")]
        [HttpPost]
        public IActionResult adminViewNotes(int requestId, viewNotes v)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.adminNotes(requestId, v, ViewBag.Data);
            return RedirectToAction("adminDashboard");

        }
        [CustomeAuthorize("Admin")]
        [HttpGet]
        public string adminCancelNote(string requestId)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            string pname = _adminRepository.getName(requestId);
            return pname;

        }
        [CustomeAuthorize("Admin")]
        [HttpPost]
        public IActionResult adminCancelNote(string requestId, string reason, string additionalNotes)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            ViewBag.CancelNote = true;
            _adminRepository.adminCancelNote(requestId,reason,additionalNotes, ViewBag.Data);
            TempData["CancelNote"] = true;
            return RedirectToAction("adminDashboard");

        }
        [CustomeAuthorize("Admin")]
        [HttpGet]
        public List<Physician> GetPhysicians(int regionId)
        {
            var res = _adminRepository.GetPhysicians(regionId);
            return res;
        }
        [CustomeAuthorize("Admin")]
        [HttpPost]
        public IActionResult adminAssignNote(string requestId, string region, string physician, string additionalNotesAssign)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.adminAssignNote(requestId, region, physician, additionalNotesAssign, ViewBag.Data);
            
           TempData["AssignNote"] = true;
            return RedirectToAction("adminDashboard");

        }

        [CustomeAuthorize("Admin")]
        [HttpGet]
        public string adminBlockNote(string requestId)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            string pname = _adminRepository.getName(requestId);
            return pname;

        }
        [CustomeAuthorize("Admin")]
        [HttpPost]
        public IActionResult adminBlockNote(string requestId , string additionalNotesBlock)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.adminBlockNote(requestId, additionalNotesBlock, ViewBag.Data);
            TempData["BlockNote"] = true;
            return RedirectToAction("adminDashboard");

        }
        [CustomeAuthorize("Admin")]
        [HttpPost]
        public IActionResult adminTransferCase(string requestId, string physician, string additionalNotesTransfer)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.adminTransferCase(requestId, physician, additionalNotesTransfer, ViewBag.Data);
            return RedirectToAction("adminDashboard");

        }


        [CustomeAuthorize("Admin")]
        public IActionResult adminViewUploads(int requestId)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            //if (ViewBag.Data == null)
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            var document = _adminRepository.GetDocumentsByRequestId(requestId);
            ViewBag.pname = _adminRepository.getName(requestId.ToString());
            ViewBag.num = _adminRepository.getConfirmationNumber(requestId.ToString());
            if (document == null)
            {
                return NotFound();
            }

            return View(document);

        }
        public List<string> GetNameConfirmation(string requestId)
        {
           int  r = int.Parse(requestId);
            var res = _adminRepository.GetNameConfirmation(r);
            return res;
        }
        /*-----------------------------------Upload Files--------------------------------------------------*/

        public IActionResult UploadFiles(int requestId, List<IFormFile> files)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.UploadFiles(requestId, files, ViewBag.Data);
            return RedirectToAction("adminViewUploads", new { requestId = requestId });
        }


        public IActionResult DownloadFile(int fileId)
        {
            var file = _adminRepository.GetFileById(fileId);
            if (file == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine("wwwroot/Files", file.FileName);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        public IActionResult DeleteFile(int requestId, int fileId)
        {
            _adminRepository.DeleteFile(fileId);

            return RedirectToAction("adminViewUploads", new { requestId = requestId });
        }


        public IActionResult DownloadFiles(string fileIds, int? requestId)
        {
            IEnumerable<RequestWiseFile> files;
            
            if (!fileIds.IsNullOrEmpty())
            {
                var ids = fileIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();

                files = _adminRepository.GetFilesByIds(ids);
            }
            else if (requestId != null)
            {
                files = _adminRepository.GetFilesByRequestId(requestId.Value);
            }
            else
            {

                return BadRequest("No files selected or invalid request.");
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


        public IActionResult DeleteFiles(string fileIds, int? requestId)
        {
            IEnumerable<RequestWiseFile> files;

            if (!fileIds.IsNullOrEmpty())
            {
                var ids = fileIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();

                _adminRepository.GetFilesByIdsDelete(ids);
            }
            else if (requestId != null)
            {
               _adminRepository.GetFilesByRequestIdDelete(requestId.Value);
            }
            else
            {

                return BadRequest("No files selected or invalid request.");
            }

            return RedirectToAction("adminViewUploads", new { requestId = requestId });

        }


        public async Task<ActionResult> SendEmailDocument(string fileIds, int requestId)
        {
            var patientEmail = _adminRepository.GetPatientEmail(requestId);

            string senderEmail = "tatva.dotnet.disneyjaviya@outlook.com";
            string senderPassword = "Disney@20";
            string filesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files");

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
                From = new MailAddress(senderEmail, "HalloDoc-Documents"),
                Subject = "Documents",
                IsBodyHtml = true,
                Body = "Please review the following documents:"
            };

            if (string.IsNullOrWhiteSpace(patientEmail))
            {
                return BadRequest("Patient email not found or invalid.");
            }

           

            if (!fileIds.IsNullOrEmpty())
            {
                var ids = fileIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();

               var selectedfiles = _adminRepository.GetSelectedFiles(ids);
                var filesInFolder = Directory.GetFiles(filesFolder);

                foreach (var fileName in selectedfiles)
                {
                    if (filesInFolder.Contains(Path.Combine(filesFolder, fileName)))
                    {
                        mailMessage.Attachments.Add(new Attachment(Path.Combine(filesFolder, fileName)));
                    }
                }
            }
            else
            {
                var fileNames = _adminRepository.GetAllFiles(requestId);

                var filesInFolder = Directory.GetFiles(filesFolder);

                foreach (var fileName in fileNames)
                {
                    if (filesInFolder.Contains(Path.Combine(filesFolder, fileName)))
                    {
                        mailMessage.Attachments.Add(new Attachment(Path.Combine(filesFolder, fileName)));
                    }
                }

            }





            mailMessage.To.Add(patientEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
                return RedirectToAction("adminViewUploads", new { requestId = requestId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }

        [HttpGet]
        [CustomeAuthorize("Admin")]
        public IActionResult sendOrder()
        {
           sendOrder s = new sendOrder();
            s.HealthProfessionalType = _adminRepository.GetAllHealthProfessionalType();
            s.HealthProfessional = _adminRepository.GetAllHealthProfessional();
            return View(s);
        }
        [HttpGet]
        public List<HealthProfessional> GetHealthProfessional(int healthprofessionalId)
        {
            var res = _adminRepository.GetHealthProfessional(healthprofessionalId);
            return res;
        }
        [HttpGet]
        public HealthProfessional GetProfessionInfo(int vendorId)
        {
            var res = _adminRepository.GetProfessionInfo(vendorId);
            return res;
        }
        [HttpPost]
        [CustomeAuthorize("Admin")]
        public IActionResult sendOrder(int requestId, sendOrder s)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.sendOrderDetails(requestId, s, ViewBag.Data);
            return RedirectToAction("sendOrder", new { requestId = requestId });
        }
        [HttpPost]
        public IActionResult adminClearCase(string requestId)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.adminClearCase(requestId, ViewBag.Data);
            TempData["ClearCase"] = true;
            return RedirectToAction("adminDashboard");

        }

        [HttpGet]
        public List<string> adminSendAgreement(string requestId)
        {
            List<string> res = new List<string>();
            ViewBag.Data = HttpContext.Session.GetString("key");
           res  =  _adminRepository.adminSendAgreementGet(requestId);
            return res;

        }

        [HttpPost]
        public IActionResult adminSendAgreement(string requestId, string email, string mobile)
        {
           
            ViewBag.Data = HttpContext.Session.GetString("key");
            AspNetUser aspNetUser = _adminRepository.GetUserByEmail(email);
            if (aspNetUser == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                return RedirectToAction("Index");
            }
            else
            {
                string senderEmail = "tatva.dotnet.disneyjaviya@outlook.com";

                string senderPassword = "Disney@20";
                string req = requestId;
                string agreementLink = $"{Request.Scheme}://{Request.Host}/Home/reviewAgreement?requestId={req}";

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
                    Subject = "Review the Agreement",
                    IsBodyHtml = true,
                    Body = $"Please review the agreement by clicking the following link. Further treatment will be carried out only after you agreee to the conditions.: <a href='{agreementLink}'>{agreementLink}</a>"
                };

                mailMessage.To.Add(email);

                client.SendMailAsync(mailMessage);

                return RedirectToAction("adminDashboard");
            }
            

        }


        [CustomeAuthorize("Admin")]
        public IActionResult closeCase(int requestId)
        {
            var document = _adminRepository.GetDocumentsByRequestId(requestId);
            ViewBag.pname = _adminRepository.getName(requestId.ToString());
            ViewBag.num = _adminRepository.getConfirmationNumber(requestId.ToString());
           
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }


        [CustomeAuthorize("Admin")]
        public IActionResult closeCaseAdmin(int requestId)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");

            _adminRepository.closeCaseAdmin(requestId, ViewBag.Data);
            return RedirectToAction("adminDashboard");
        }
        public RequestClient getPatientInfo(int requestId)
        {
            RequestClient r = new RequestClient();
             r = _adminRepository.getPatientInfo(requestId);
            return r;
        }

       
        [HttpPost]
        public IActionResult patientCancelNote(string requestId, string additionalNotesPatient)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            _adminRepository.patientCancelNote(requestId, additionalNotesPatient);
            return RedirectToAction("adminDashboard");

        }


        public string adminTransferNotes(string requestId)
        {
           ViewBag.Data = HttpContext.Session.GetString("key");
           
            string res = _adminRepository.adminTransferNotes(requestId, ViewBag.Data);
            return res;

        }

        [CustomeAuthorize("Admin")]
        [HttpGet]
        public IActionResult adminProfile()
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            Admin a = new Admin();
            a = _adminRepository.getAdminInfo(ViewBag.Data);
            return View(a);

        }
        [CustomeAuthorize("Admin")]

        public IActionResult adminCreateRequest()
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
           
            //if (row == null)
            //{
            //    SendEmailUser(RequestData.Email, res);
            //}
            return View();

        }
        [CustomeAuthorize("Admin")]
        [HttpPost]
        public IActionResult adminCreateRequest(createAdminRequest RequestData)
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            string email = RequestData.Email;
            var row = _adminRepository.GetUserByEmail(email);
            var res = _adminRepository.adminCreateRequest(RequestData, ViewBag.Data);

            if (row == null)
            {
                SendEmailUser(RequestData.Email, res);
            }
            return RedirectToAction("adminDashboard");

        }

        public Action SendEmailUser(System.String Email, string id)
        {

            AspNetUser aspNetUser = _adminRepository.GetUserByEmail(Email);


            string senderEmail = "tatva.dotnet.disneyjaviya@outlook.com";
            string senderPassword = "Disney@20";
            string resetLink = $"{Request.Scheme}://{Request.Host}/Home/createPatientAccount?id={id}";

           _adminRepository.passwordresetInsert(Email,id);

           



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
                Body = $"Please create password for your account: <a href='{resetLink}'>{resetLink}</a>"
            };

            mailMessage.To.Add(Email)
;

            client.SendMailAsync(mailMessage);
            return null;
        }

        //[CustomeAuthorize("Admin")]
        public IActionResult encounterForm()
        {
            ViewBag.Data = HttpContext.Session.GetString("key");

           
            return View();
        }
        [HttpGet]
        public List<Region> getAdminRegions()
        {
            ViewBag.Data = HttpContext.Session.GetString("key");
            //List<Region> res = new List<Region>();
         var res =  _adminRepository.getAdminRegions(ViewBag.Data);
            return res;
        }
        public IActionResult logOut()
        {
            Response.Cookies.Delete("jwt");   
            HttpContext.Session.Remove("key");
            return RedirectToAction("Index");
        }
    }
}
