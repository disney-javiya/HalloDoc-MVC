
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

namespace Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;
       
        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public AspNetUser ValidateUser(string email, string password)
        {
            return _context.AspNetUsers.Where(x => x.Email == email && x.PasswordHash == password).FirstOrDefault();
        }

        public AspNetUser GetUserByEmail(string email)
        {
            return _context.AspNetUsers.Where(x => x.Email == email).FirstOrDefault();
        }


        public void CreateRequest(createPatientRequest RequestData)
        {


            AspNetUser asp = new AspNetUser();
            asp.Id = Guid.NewGuid().ToString();
            asp.UserName = RequestData.FirstName + RequestData.LastName;
            asp.PasswordHash = RequestData.PasswordHash;
            asp.Email = RequestData.Email;
            asp.PhoneNumber = RequestData.Mobile;
            asp.CreatedDate = DateTime.Now;

            _context.AspNetUsers.Add(asp);
            _context.SaveChanges();

            User data = new User();
            data.AspNetUserId = asp.Id;
            data.FirstName = RequestData.FirstName;
            data.LastName = RequestData.LastName;
            data.Email = RequestData.Email;
            data.Mobile = RequestData.Mobile;
            data.Street = RequestData.Street;
            data.City = RequestData.City;
            data.State = RequestData.State;
            data.ZipCode = RequestData.ZipCode;
            data.CreatedBy = RequestData.FirstName;
            data.CreatedDate = DateTime.Now;

            String sDate = RequestData.DateOfBirth.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = datevalue.Day;
            String mn = datevalue.Month.ToString();
            int yy = datevalue.Year;

            data.IntYear = yy;
            data.StrMonth = mn;
            data.IntDate = dy;
            data.Status = 1;

            _context.Users.Add(data);
            _context.SaveChanges();


            Request req = new Request();
            req.RequestTypeId = 2;
            req.UserId = data.UserId;
            req.FirstName = RequestData.FirstName;
            req.LastName = RequestData.LastName;
            req.PhoneNumber = RequestData.Mobile;
            req.Email = RequestData.Email;
            req.Status = 1;
            var c = _context.Users.Count(x => x.CreatedDate == DateTime.Now);
            req.ConfirmationNumber = RequestData.State.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4) + RequestData.LastName.Substring(0, 2) + RequestData.FirstName.Substring(0, 2) + c;
            req.CreatedDate = DateTime.Now;

            foreach (var file in RequestData.MultipleFiles)
            {

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files");

                //create folder if not exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


                string fileNameWithPath = Path.Combine(path, file.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            _context.Requests.Add(req);
            _context.SaveChanges();

            //RequestWiseFile rf = new RequestWiseFile();
            //rf.RequestId = req.RequestId;

            
                
            


                RequestClient rc = new RequestClient();
                rc.RequestId = req.RequestId;
                rc.FirstName = RequestData.FirstName;
                rc.LastName = RequestData.LastName;
                rc.PhoneNumber = RequestData.Mobile;
                rc.Location = RequestData.State;
                rc.Address = RequestData.Street + "," + RequestData.City + "," + RequestData.State + " ," + RequestData.ZipCode;
                rc.Notes = RequestData.Symptoms;
                rc.Email = RequestData.Email;
                rc.StrMonth = mn;
                rc.IntDate = dy;
                rc.IntYear = yy;
                rc.Street = RequestData.Street;
                rc.City = RequestData.City;
                rc.State = RequestData.State;
                rc.ZipCode = RequestData.ZipCode;




                _context.RequestClients.Add(rc);
                _context.SaveChanges();


        }
        
        public void CreateFamilyRequest(familyCreateRequest RequestData)
        {
            AspNetUser asp = new AspNetUser();
            var row = _context.AspNetUsers.Where(x=> x.Email==RequestData.Email).FirstOrDefault();
            User data = new User();
            data.AspNetUserId = row.Id;
            data.Email = RequestData.Email;
            data.FirstName = RequestData.FirstName;
            data.LastName = RequestData.LastName;      
            data.Mobile = RequestData.PhoneNumber;
            data.Street = RequestData.Street;
            data.City = RequestData.City;
            data.State = RequestData.State;
            data.ZipCode = RequestData.ZipCode;
            String sDate = RequestData.DateOfBirth.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = datevalue.Day;
            String mn = datevalue.Month.ToString();
            int yy = datevalue.Year;

            data.IntYear = yy;
            data.StrMonth = mn;
            data.IntDate = dy;
            data.CreatedBy = RequestData.FamilyFirstName;
            data.CreatedDate = DateTime.Now;
            data.Status = 1;
            _context.Users.Add(data);
            _context.SaveChanges();


            Request req = new Request();
            req.RequestTypeId = 3;
            req.UserId = data.UserId;
            req.FirstName = RequestData.FamilyFirstName;
            req.LastName = RequestData.FamilyLastName;
            req.PhoneNumber = RequestData.FamilyPhoneNumber;
            req.Email   = RequestData.FamilyEmail;
            req.Status = 1;
            var c = _context.Users.Count(x => x.CreatedDate == DateTime.Now);
            req.ConfirmationNumber = RequestData.State.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4) + RequestData.LastName.Substring(0, 2) + RequestData.FirstName.Substring(0, 2) + c;
            req.CreatedDate = DateTime.Now;
            req.RelationName = RequestData.RelationName;

            _context.Requests.Add(req);
            _context.SaveChanges();


            RequestClient rc = new RequestClient();
            rc.RequestId = req.RequestId;
            rc.FirstName = RequestData.FirstName;
            rc.LastName = RequestData.LastName;
            rc.PhoneNumber = RequestData.PhoneNumber;
            rc.Location = RequestData.State;
            rc.Address = RequestData.Street + "," + RequestData.City + "," + RequestData.State + " ," + RequestData.ZipCode;
            rc.Notes = RequestData.Notes;
            rc.Email = RequestData.Email;
            rc.StrMonth = mn;
            rc.IntDate = dy;
            rc.IntYear = yy;
            rc.Street = RequestData.Street;
            rc.City = RequestData.City;
            rc.State = RequestData.State;
            rc.ZipCode = RequestData.ZipCode;

            _context.RequestClients.Add(rc);
            _context.SaveChanges();

        }

        public void CreateConciergeRequest(conciergeCreateRequest RequestData)
        {
            AspNetUser asp = new AspNetUser();
            var row = _context.AspNetUsers.Where(x => x.Email == RequestData.Email).FirstOrDefault();
            User data = new User();
            data.AspNetUserId = row.Id;
            data.FirstName = RequestData.FirstName;
            data.LastName = RequestData.LastName;
            data.Email = RequestData.Email;
            data.Mobile = RequestData.PhoneNumber;;
            String sDate = RequestData.DateOfBirth.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = datevalue.Day;
            String mn = datevalue.Month.ToString();
            int yy = datevalue.Year;

            data.IntYear = yy;
            data.StrMonth = mn;
            data.IntDate = dy;
            data.CreatedBy = RequestData.ConciergeFirstName;
            data.CreatedDate = DateTime.Now;
            data.Status = 1;
            _context.Users.Add(data);
            _context.SaveChanges();



            Request req = new Request();
            req.RequestTypeId = 4;
            req.UserId = data.UserId;
            req.FirstName = RequestData.ConciergeFirstName;
            req.LastName = RequestData.ConciergeLastName;
            req.PhoneNumber = RequestData.ConciergePhoneNumber;
            req.Email = RequestData.ConciergeEmail;
            req.Status = 1;
            var c = _context.Users.Count(x => x.CreatedDate == DateTime.Now);
            req.ConfirmationNumber = RequestData.ConciergeState.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4) + RequestData.LastName.Substring(0, 2) + RequestData.FirstName.Substring(0, 2) + c;
            req.CreatedDate = DateTime.Now;
            _context.Requests.Add(req);
            _context.SaveChanges();


            RequestClient rc = new RequestClient();
            rc.RequestId = req.RequestId;
            rc.FirstName = RequestData.FirstName;
            rc.LastName = RequestData.LastName;
            rc.PhoneNumber = RequestData.PhoneNumber;
            rc.Location = RequestData.ConciergeState;
            rc.Notes = RequestData.Notes;
            rc.Email = RequestData.Email;
            rc.StrMonth = mn;
            rc.IntDate = dy;
            rc.IntYear = yy;
            _context.RequestClients.Add(rc);
            _context.SaveChanges();


            Concierge concierge = new Concierge();
            concierge.ConciergeName = RequestData.ConciergeFirstName + "  " + RequestData.ConciergeLastName;
            concierge.Address = RequestData.ConciergeStreet + " " + RequestData.ConciergeCity + " " + RequestData.ConciergeState + " " + RequestData.ConciergeZipCode;
            concierge.Street = RequestData.ConciergeStreet;
            concierge.City = RequestData.ConciergeCity;
            concierge.State = RequestData.ConciergeState;
            concierge.ZipCode = RequestData.ConciergeZipCode;
            concierge.CreatedDate = DateTime.Now;
            concierge.RegionId = 1;
            concierge.Propertyname = RequestData.ConciergePropertyName;
            _context.Concierges.Add(concierge);
            _context.SaveChanges();

            RequestConcierge requestConcierge = new RequestConcierge();
            requestConcierge.RequestId = req.RequestId;
            requestConcierge.ConciergeId = concierge.ConciergeId;
            _context.RequestConcierges.Add(requestConcierge);
            _context.SaveChanges();
        }



        public void CreateBusinessRequest(businessCreateRequest RequestData)
        {
            AspNetUser asp = new AspNetUser();
            var row = _context.AspNetUsers.Where(x => x.Email == RequestData.Email).FirstOrDefault();
            User data = new User();
            data.AspNetUserId = row.Id;
            data.FirstName = RequestData.FirstName;
            data.LastName = RequestData.LastName;
            data.Email = RequestData.Email;
            data.Mobile = RequestData.PhoneNumber; 
            data.Street = RequestData.Street;
            data.City = RequestData.City;
            data.State = RequestData.State;
            data.ZipCode = RequestData.ZipCode;
            String sDate = RequestData.DateOfBirth.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = datevalue.Day;
            String mn = datevalue.Month.ToString();
            int yy = datevalue.Year;

            data.IntYear = yy;
            data.StrMonth = mn;
            data.IntDate = dy;
            data.CreatedBy = RequestData.BusinessFirstName;
            data.CreatedDate = DateTime.Now;
            data.Status = 1;
            _context.Users.Add(data);
            _context.SaveChanges();



            Request req = new Request();
            req.RequestTypeId = 1;
            req.UserId = data.UserId;
            req.FirstName = RequestData.BusinessFirstName;
            req.LastName = RequestData.BusinessLastName;
            req.PhoneNumber = RequestData.BusinessPhoneNumber;
            req.Email = RequestData.BusinessEmail;
            req.Status = 1;
            var c = _context.Users.Count(x => x.CreatedDate == DateTime.Now);
            req.ConfirmationNumber = RequestData.State.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4) + RequestData.LastName.Substring(0, 2) + RequestData.FirstName.Substring(0, 2) + c;
            req.CreatedDate = DateTime.Now;
            _context.Requests.Add(req);
            _context.SaveChanges();


            RequestClient rc = new RequestClient();
            rc.RequestId = req.RequestId;
            rc.FirstName = RequestData.FirstName;
            rc.LastName = RequestData.LastName;
            rc.PhoneNumber = RequestData.PhoneNumber;
            rc.Location = RequestData.State;
            rc.Address = RequestData.Street + "," + RequestData.City + "," + RequestData.State + " ," + RequestData.ZipCode;
            rc.Notes = RequestData.Notes;
            rc.Email = RequestData.Email;
            rc.StrMonth = mn;
            rc.IntDate = dy;
            rc.IntYear = yy;
            rc.Street = RequestData.Street;
            rc.City = RequestData.City;
            rc.State = RequestData.State;
            rc.ZipCode = RequestData.ZipCode;

            _context.RequestClients.Add(rc);
            _context.SaveChanges();
        }

        public List<Request> GetbyEmail(string email)
        {
            return _context.Requests.Where(x => x.Email == email).ToList();
        }

        

    }

   

}
