using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.DataAccessLayer.DataContext;
using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using System.Web.Mvc;
using System.Web.Http;
using System.IO.Compression;

namespace Repository
{
    public class AdminRepository: IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public AspNetUser ValidateUser(string email, string password)
        {
            var passwordhash = "";
            if (password != null)
            {
                var plainText = Encoding.UTF8.GetBytes(password);
                passwordhash = Convert.ToBase64String(plainText);
            }
            return _context.AspNetUsers.Where(x => x.Email == email && x.PasswordHash == passwordhash).FirstOrDefault();
        }



        public RequestClient getPatientInfo(int requestId)
        {
            return _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId);
        }

        public string getConfirmationNumber(int requestId)
        {
            return _context.Requests.Where(r => r.RequestId == requestId).Select(r => r.ConfirmationNumber).FirstOrDefault();
        }




        public viewNotes getNotes(int requestId , string email)
        {
            var res = _context.RequestNotes.FirstOrDefault(x => x.RequestId == requestId);
            var res1 = _context.RequestStatusLogs.Where(x => x.RequestId == requestId).ToList();
            viewNotes v = new viewNotes();
            if (res == null)
            {
               
                v.RequestId = requestId;
                v.AdditionalNote = "";
                _context.SaveChanges();
            }
            else
            {
                v.RequestId = res.RequestId;
                v.AdminNote = res.AdminNotes;
                _context.SaveChanges();
            }
            foreach (var row in res1)
            {
                if (row.Status == 3)
                {
                    v.AdminCancellationNotes = _context.Requests.Where(x=>x.RequestId == requestId && x.Status == 3).Select(u=>u.CaseTag).FirstOrDefault() + row.Notes;
                }
                else if(row.Status == 2)
                {
                    //admin tranfered to some physician
                    if(row.TransToAdmin == null)
                    {
                        var adminusername = _context.AspNetUsers.Where(x=>x.Email == email).Select(u=>u.UserName).FirstOrDefault();
                        v.TransferNote = adminusername + " " + "transfered to " + row.TransToPhysicianId + " : " + row.Notes;
                    }
                    //physician transfered to admin
                    if(row.TransToPhysicianId == null)
                    {
                        var physicianusername = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.UserName).FirstOrDefault();
                        v.TransferNote = physicianusername + "tranfered to" + row.AdminId + ":" + row.Notes;
                    }
                }
            }
            
            
            return v;
        }

        public void adminNotes(int requestId, viewNotes v, string email)
        {
            RequestNote rn = new RequestNote();
            var res = _context.RequestNotes.Where(x => x.RequestId == requestId).FirstOrDefault();
            if (res == null)
            {
                rn.RequestId = requestId;
                rn.AdminNotes = v.AdditionalNote;
                rn.PhysicianNotes = null;
                rn.CreatedBy = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.UserName).FirstOrDefault();
                rn.CreatedDate = DateTime.Now;
                _context.RequestNotes.Add(rn);
                _context.SaveChanges();
            }
            else
            {

              var  r = _context.RequestNotes.Where(u => u.RequestNotesId == res.RequestNotesId).FirstOrDefault();
                r.RequestNotesId = r.RequestNotesId;
                r.AdminNotes = v.AdditionalNote;
                r.PhysicianNotes = null;
                if (r.CreatedBy == "")
                {
                    r.CreatedBy = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.UserName).FirstOrDefault();
                    r.CreatedDate = DateTime.Now;
                }
                else
                {
                    r.ModifiedBy = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.UserName).FirstOrDefault();
                    r.ModifiedDate = DateTime.Now;

                }

               
                _context.SaveChanges();

            }


        }

        public void adminCancelNote(string requestId, string reason, string additionalNotes, string email)
        {
            RequestStatusLog rs = new RequestStatusLog();
            Request r = new Request();
            int reqId = int.Parse(requestId);
            //int CaseTagid = int.Parse(reason);
            var res = _context.Requests.Where(x => x.RequestId == reqId).FirstOrDefault();
            if (reqId != null)
            {
                res.Status = 3;
                //var caseName = _context.CaseTags.Where(x => x.CaseTagId == CaseTagid).Select(u => u.Name).ToString();
                res.CaseTag = reason;
                res.DeclinedBy = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.UserName).FirstOrDefault();
                _context.SaveChanges();
                rs.RequestId = reqId;
                rs.Status = 3;
                var aspId = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.Id).FirstOrDefault();
                var id = _context.Admins.Where(x => x.AspNetUserId == aspId).Select(u => u.AdminId).FirstOrDefault();
                rs.AdminId = id;
                rs.Notes = additionalNotes;
                rs.CreatedDate = DateTime.Now;
            }
            _context.RequestStatusLogs.Add(rs);
            _context.SaveChanges();
        }

        public List<Physician> GetPhysicians(int regionId)
        {
          return   _context.Physicians.Where(x=>x.RegionId ==  regionId).ToList();
        }


        public void adminAssignNote(string requestId, string region,  string physician, string additionalNotesAssign, string email)
        {
            RequestStatusLog rs = new RequestStatusLog();
            Request r = new Request();
            int reqId = int.Parse(requestId);
            var res = _context.Requests.Where(x => x.RequestId == reqId).FirstOrDefault();
            if (reqId != null)
            {
                res.Status = 2;
                res.PhysicianId = int.Parse(physician);
            
                _context.SaveChanges();
                rs.RequestId = reqId;
                rs.Status = 2;
                var aspId = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.Id).FirstOrDefault();
                var id = _context.Admins.Where(x => x.AspNetUserId == aspId).Select(u => u.AdminId).FirstOrDefault();
                rs.AdminId = id;
                rs.TransToPhysicianId = int.Parse(physician);
                rs.Notes = additionalNotesAssign;
                rs.CreatedDate = DateTime.Now;
            }
            _context.RequestStatusLogs.Add(rs);
            _context.SaveChanges();
        }

        public void adminBlockNote(string requestId,  string additionalNotesBlock, string email)
        {
            RequestStatusLog rs = new RequestStatusLog();
            Request r = new Request();
            BlockRequest b = new BlockRequest();
            int reqId = int.Parse(requestId);
            var res = _context.Requests.Where(x => x.RequestId == reqId).FirstOrDefault();
            if(res != null)
            {
                rs.RequestId = reqId;
                res.Status = 11;
                _context.SaveChanges();
                rs.RequestId = reqId;
                rs.Status = 11;
                var aspId = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.Id).FirstOrDefault();
                var id = _context.Admins.Where(x => x.AspNetUserId == aspId).Select(u => u.AdminId).FirstOrDefault();
                rs.AdminId = id;
                rs.Notes = additionalNotesBlock;
                rs.CreatedDate = DateTime.Now;

                b.PhoneNumber = res.PhoneNumber;
                b.Email = res.Email;
                b.Reason = additionalNotesBlock;
                b.RequestId = reqId.ToString();
                b.CreatedDate = DateTime.Now;


            }
            _context.RequestStatusLogs.Add(rs);
            _context.SaveChanges();
        }

        public List<RequestWiseFile> GetDocumentsByRequestId(int requestId)
        {
            
             
            return _context.RequestWiseFiles.Where(d => d.RequestId == requestId && d.IsDeleted == new BitArray(new bool [] {false})).ToList();
        }


        public List<string> GetNameConfirmation(int requestId)
        {
            List<string> result = new List<string>();
           string name = _context.RequestClients.Where(x=>x.RequestId == requestId).Select(x=>x.FirstName).ToString();
            result.Add(name);
            string Cnum = _context.Requests.Where(x=>x.RequestId == requestId).Select(x=>x.ConfirmationNumber).ToString();
            result.Add(Cnum);
            return result;
        }

        public IEnumerable<RequestandRequestClient> getRequestStateData(int type)
        {
            var query = (from req in _context.Requests
                         join client in _context.RequestClients on req.RequestId equals client.RequestId
                         select new
                         {
                             Request = req,
                             Client = client,
                             Status = req.Status
                         })
                         .Select(x => new RequestandRequestClient
                         {
                             requestId = x.Client.RequestId,
                             patientName = x.Client.FirstName + " " + x.Client.LastName,
                             patientDOB = x.Client.IntDate + "/" + x.Client.StrMonth + "/" + x.Client.IntYear,
                             requestorName = x.Request.FirstName + " " + x.Request.LastName,
                             requestedDate = x.Request.CreatedDate,
                             patientContact = x.Client.PhoneNumber,
                             requestorContact = x.Request.PhoneNumber,
                             patientAddress = x.Client.Address,
                             Status = x.Status,
                             RequestTypeId = x.Request.RequestTypeId,
                             patientEmail = x.Client.Email,
                             CaseTag = _context.CaseTags.ToList(),
                             Region = _context.Regions.ToList(),
                             Physician = _context.Physicians.ToList()
                         });

            if (type == 1)
            {
                query = query.Where(req => req.Status == 1);
            }
            else if (type == 2)
            {
                query = query.Where(req => req.Status == 2);
            }
            else if (type == 3)
            {
                query = query.Where(req => req.Status == 4 || req.Status == 5);
            }
            else if (type == 4)
            {
              
                query = query.Where(req => req.Status == 6);
            }
            else if (type == 5)
            {
                query = query.Where(req => req.Status == 3 || req.Status == 7 || req.Status == 8);
            }
            else if (type == 6)
            {
                query = query.Where(req => req.Status == 9);
            }

            
            return query.ToList();
        }

        public void UploadFiles(int requestId, List<IFormFile> files, string email)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine("wwwroot/Files", fileName);
                    var aspId = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.Id).FirstOrDefault();
                    var id = _context.Admins.Where(x => x.AspNetUserId == aspId).Select(u => u.AdminId).FirstOrDefault();
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    RequestWiseFile newFile = new RequestWiseFile
                    {
                        FileName = fileName,
                        RequestId = requestId,
                        AdminId = id,
                        IsDeleted = new BitArray(new bool[] { false }),
                        CreatedDate = DateTime.Now
                    };

                    _context.RequestWiseFiles.Add(newFile);
                }
            }

            _context.SaveChanges();
        }

        public RequestWiseFile GetFileById(int fileId)
        {
            return _context.RequestWiseFiles.FirstOrDefault(f => f.RequestWiseFileId == fileId);
        }

        public void DeleteFile(int fileId)
        {
          var file =  _context.RequestWiseFiles.Where(f=> f.RequestWiseFileId == fileId).ExecuteUpdate(setters => setters.SetProperty(b => b.IsDeleted, new BitArray(new bool[] {true}) ));
          
            _context.SaveChanges();
        }

        public IEnumerable<RequestWiseFile> GetFilesByRequestId(int requestId)
        {
            return _context.RequestWiseFiles.Where(f => f.RequestId == requestId && f.IsDeleted == new BitArray(new bool[] { false })).ToList();
        }

        public IEnumerable<RequestWiseFile> GetFilesByIds(List<int> fileIds)
        {
            return _context.RequestWiseFiles.Where(f => fileIds.Contains(f.RequestWiseFileId) && f.IsDeleted == new BitArray(new bool[] {false})).ToList();
        }


        public void GetFilesByIdsDelete(List<int> fileIds)
        {
           
            foreach (var id in fileIds)
            {
              var file =  _context.RequestWiseFiles.Where(f => f.RequestWiseFileId == id);
                file.ExecuteUpdate(setters => setters.SetProperty(b => b.IsDeleted, new BitArray(new bool[] { true })));
                _context.SaveChanges();
            }
           
           
        }

        public void GetFilesByRequestIdDelete(int requestId)
        {
            List<RequestWiseFile> files = _context.RequestWiseFiles.Where(f => f.RequestId == requestId).ToList();
            foreach (var file in files)
            {
                file.IsDeleted = new BitArray(new bool[] { true });
                _context.SaveChanges();
            }
          
           
        }
        public string GetPatientEmail(int requestId)
        {
          string email =  _context.RequestClients.Where(x=>x.RequestId == requestId).Select(f=> f.Email).FirstOrDefault().ToString();
            return email;
        }

        public List<string> GetAllFiles(int requestId)
        {
            return _context.RequestWiseFiles.Where(x => x.RequestId == requestId && x.IsDeleted == new BitArray(new bool[] { false })).Select(f => f.FileName).ToList();
            
        }
       

        public List<string> GetSelectedFiles(List<int> ids)
        {
            List<string> selectedfilenames = new List<string>();
            foreach (var id in ids)
            {
                var name = _context.RequestWiseFiles
                                  .Where(x => x.RequestWiseFileId == id && x.IsDeleted == new BitArray(new bool[] { false }))
                                  .Select(x => x.FileName)
                                  .FirstOrDefault(); 
                if (name != null)
                {
                    selectedfilenames.Add(name);
                }
            }

            return selectedfilenames;
        }

    }
}
