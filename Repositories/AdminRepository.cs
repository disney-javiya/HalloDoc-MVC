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

        
        public RequestNote getNotes(int requestId)
        {
            var res = _context.RequestNotes.FirstOrDefault(x => x.RequestId == requestId);
            if (res == null)
            {
                RequestNote r = new RequestNote();
                r.RequestId = requestId;
                r.PhysicianNotes = "";
                r.AdminNotes = "";
                r.CreatedBy = "";
    
                _context.RequestNotes.Add(r);
                _context.SaveChanges();
                res = _context.RequestNotes.FirstOrDefault(x => x.RequestId == requestId);
            }
            return res;
        }

        public string getTranferNotes(int requestId)
        {
           
           
            var res = _context.RequestStatusLogs.Where(x => x.RequestId == requestId).Select(x=>x.Notes).FirstOrDefault();
            if (res == null)
            {
                return res = "";
            }
            return res;
        }
        public void adminNotes(int requestId , RequestNote r, string email)
        {
            RequestNote rn = new RequestNote();
            var res = _context.RequestNotes.Where(x => x.RequestId == requestId).FirstOrDefault();
            if (res == null)
            {
                rn.RequestId = requestId;
                rn.AdminNotes = r.AdminNotes;
                rn.PhysicianNotes = null;
                rn.CreatedBy = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.UserName).FirstOrDefault();
                rn.CreatedDate = DateTime.Now;
                _context.RequestNotes.Add(rn);
                _context.SaveChanges();
            }
            else { 
            
                res = _context.RequestNotes.Where(u=>u.RequestNotesId == res.RequestNotesId).FirstOrDefault();
                res.RequestNotesId = res.RequestNotesId;
                res.AdminNotes = r.AdminNotes;
                res.PhysicianNotes = null;
                if(res.CreatedBy == "")
                {
                    res.CreatedBy = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.UserName).FirstOrDefault();
                    res.CreatedDate = DateTime.Now;
                }
                else
                {
                    res.ModifiedBy = _context.AspNetUsers.Where(x => x.Email == email).Select(u => u.UserName).FirstOrDefault();
                    res.ModifiedDate = DateTime.Now;
                    
                }
                
               
                _context.SaveChanges();

            }
            

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
                             CaseTag = _context.CaseTags.ToList()
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


       
    }
}
