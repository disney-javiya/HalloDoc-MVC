

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
            return _context.AspNetUsers.Where(x => x.Email == email && x.PasswordHash == password).FirstOrDefault();
        }
        //public IEnumerable<RequestandRequestClient> getRequestStateData(int type)
        //{


        //    var query = (from req in _context.Requests
        //                 join client in _context.RequestClients on req.RequestId equals client.RequestId
        //                 select new RequestandRequestClient 
        //                 {
        //                     requestId = client.RequestId,
        //                     patientName = client.FirstName + client.LastName,
        //                     patientDOB = client.IntDate + client.StrMonth + client.IntYear,
        //                     requestorName = req.FirstName + req.LastName,
        //                     resquestedDate = req.CreatedDate,
        //                     patientContact = client.PhoneNumber,
        //                     requestorContact = req.PhoneNumber,
        //                     patientAddress = client.Address
        //                 }).ToList();

        //    return query;
        //}

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
                             patientName = x.Client.FirstName + x.Client.LastName,
                             patientDOB = x.Client.IntDate + x.Client.StrMonth + x.Client.IntYear,
                             requestorName = x.Request.FirstName + x.Request.LastName,
                             requestedDate = x.Request.CreatedDate,
                             patientContact = x.Client.PhoneNumber,
                             requestorContact = x.Request.PhoneNumber,
                             patientAddress = x.Client.Address,
                             Status = x.Status,
                             RequestTypeId = x.Request.RequestTypeId
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
                query = query.Where(req => req.Status == 5 || req.Status == 6);
            }
            else if (type == 4)
            {
                //correction
                query = query.Where(req => req.Status == 2);
            }
            else if (type == 5)
            {
                query = query.Where(req => req.Status == 3 || req.Status == 8);
            }
            else if (type == 6)
            {
                query = query.Where(req => req.Status == 16);
            }
            else
            {
                Console.WriteLine("No");
            }
            
            return query.ToList();
        }



    }
}
