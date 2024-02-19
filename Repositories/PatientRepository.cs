
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

            System.String sDate = RequestData.DateOfBirth.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = datevalue.Day;
            System.String mn = datevalue.Month.ToString();
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
            if(RequestData.MultipleFiles != null)
            {
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
            }
          

            _context.Requests.Add(req);
            _context.SaveChanges();
            if (RequestData.MultipleFiles != null)
            {
                foreach (var file in RequestData.MultipleFiles)
                {
                    RequestWiseFile rf = new RequestWiseFile();
                    rf.RequestId = req.RequestId;
                    rf.FileName = file.FileName;
                    rf.CreatedDate = DateTime.Now;
                    _context.RequestWiseFiles.Add(rf);
                    _context.SaveChanges();

                }
            }
               
            

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
            System.String sDate = RequestData.DateOfBirth.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = datevalue.Day;
            System.String mn = datevalue.Month.ToString();
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


            if (RequestData.MultipleFiles != null)
            {
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
            }

            _context.Requests.Add(req);
            _context.SaveChanges();

            if (RequestData.MultipleFiles != null)
            {

                foreach (var file in RequestData.MultipleFiles)
                {
                    RequestWiseFile rf = new RequestWiseFile();
                    rf.RequestId = req.RequestId;
                    rf.FileName = file.FileName;
                    rf.CreatedDate = DateTime.Now;
                    _context.RequestWiseFiles.Add(rf);
                    _context.SaveChanges();
                }
            }

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
            System.String sDate = RequestData.DateOfBirth.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = datevalue.Day;
            System.String mn = datevalue.Month.ToString();
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
            System.String sDate = RequestData.DateOfBirth.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = datevalue.Day;
            System.String mn = datevalue.Month.ToString();
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
        
           var userIds = _context.Users.Where(x => x.Email == email).Select(u => u.UserId).ToList();
            var userData = new List<Request>();
            foreach (var userId in userIds)
            {
                 userData.AddRange( _context.Requests.Where(ud => ud.UserId == userId).ToList());
            }
            return userData;
        }

        public List<RequestWiseFile> GetDocumentsByRequestId(int requestId)
        {
            return _context.RequestWiseFiles.Where(d => d.RequestId == requestId).ToList();
        }

        public void UploadFiles(int requestId, List<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine("wwwroot/Files", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    RequestWiseFile newFile = new RequestWiseFile
                    {
                        FileName = fileName,
                        RequestId = requestId,
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


        public User GetPatientData(string email)
        {

            var data = _context.Users.Where(x => x.Email == email).ToList().First();
            
            return data;
        }

        public void updateProfile(string email, User u)
        {

            var aspId = _context.AspNetUsers.Where(x => x.Email == email).Select(a=>a.Id).First();

           var listUsers = _context.Users.Where(x=>x.AspNetUserId==aspId).ToList();
            if(listUsers!=null)
            {
                foreach (var user in listUsers)
                {

                    user.FirstName = u.FirstName;
                    user.LastName = u.LastName;
                    user.Email = u.Email;
                    user.Street = u.Street;
                    user.City = u.City;
                    user.State = u.State;
                    user.ZipCode = u.ZipCode;
                    _context.SaveChanges();


                    var req = _context.Requests.Where(x => x.Email == user.Email).ToList();
                    foreach (var r in req)
                    {
                        r.FirstName = u.FirstName;
                        r.LastName = u.LastName;
                        r.Email = u.Email;
                        var c = _context.Users.Count(x => x.CreatedDate == DateTime.Now);
                        req.First().ConfirmationNumber = u.State.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4) + u.LastName.Substring(0, 2) + u.FirstName.Substring(0, 2) + c;
                        _context.SaveChanges();
                    }

                    var reqc = _context.RequestClients.Where(x => x.Email == user.Email).ToList();
                    foreach (var rc in reqc)
                    {
                        rc.FirstName = u.FirstName;
                        rc.LastName = u.LastName;
                        rc.PhoneNumber = u.Mobile;
                        rc.Location = u.State;
                        rc.Address = u.Street + "," + u.City + "," + u.State + " ," + u.ZipCode;
                        rc.Email = u.Email;



                        rc.Street = u.Street;
                        rc.City = u.City;
                        rc.State = u.State;
                        rc.ZipCode = u.ZipCode;
                        _context.SaveChanges();

                    }
                    var asp = _context.AspNetUsers.Where(x=>x.Id== aspId).ToList().First();
                    asp.Email = u.Email;
                    asp.UserName =  u.FirstName + u.LastName;
                    asp.PhoneNumber = u.Mobile;
                    

                }
            }




        }

       



        public IEnumerable<RequestWiseFile> GetAllFiles()
        {
            return _context.RequestWiseFiles.ToList();
        }

        public IEnumerable<RequestWiseFile> GetFilesByIds(List<int> fileIds)
        {
            return _context.RequestWiseFiles.Where(f => fileIds.Contains(f.RequestWiseFileId)).ToList();
        }

        public void createPatientRequestMe(createPatientRequest RequestData)
        {

      
            User data = new User();
            var d = _context.AspNetUsers.FirstOrDefault(x => x.Email == RequestData.Email);
          
            data.AspNetUserId = d.Id;
            
         
            
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

            System.String sDate = RequestData.DateOfBirth.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            int dy = datevalue.Day;
            System.String mn = datevalue.Month.ToString();
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
            if (RequestData.MultipleFiles != null)
            {
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
            }


            _context.Requests.Add(req);
            _context.SaveChanges();
            if (RequestData.MultipleFiles != null)
            {
                foreach (var file in RequestData.MultipleFiles)
                {
                    RequestWiseFile rf = new RequestWiseFile();
                    rf.RequestId = req.RequestId;
                    rf.FileName = file.FileName;
                    rf.CreatedDate = DateTime.Now;
                    _context.RequestWiseFiles.Add(rf);
                    _context.SaveChanges();

                }
            }



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

        public void createPatientRequestSomeoneElse(string email ,requestSomeoneElse r)
        {
       
            var existUser =  _context.AspNetUsers.FirstOrDefault(x => x.Email == r.Email);
            if(existUser!=null)
            {
                /*User already exists*/
                User data = new User();
                data.AspNetUserId = existUser.Id;
                data.FirstName = r.FirstName;
                data.LastName = r.LastName;
                data.Email = r.Email;
                data.Mobile = r.Mobile;
                data.Street = r.Street;
                data.City = r.City;
                data.State = r.State;
                data.ZipCode = r.ZipCode;
                data.CreatedBy = r.FirstName;
                data.CreatedDate = DateTime.Now;
                System.String sDate = r.DateOfBirth.ToString();
                DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                int dy = datevalue.Day;
                System.String mn = datevalue.Month.ToString();
                int yy = datevalue.Year;

                data.IntYear = yy;
                data.StrMonth = mn;
                data.IntDate = dy;
                data.Status = 1;

                _context.Users.Add(data);
                _context.SaveChanges();
              
                var emailUser = _context.Users.FirstOrDefault(x => x.Email == email);
                Request req = new Request();
                req.RequestTypeId = 2;
                req.UserId = data.UserId;
                req.FirstName = emailUser.FirstName;
                req.LastName = emailUser.LastName;
                req.PhoneNumber = emailUser.Mobile;
                req.Email = emailUser.Email;
                req.Status = 1;
                var c = _context.Users.Count(x => x.CreatedDate == DateTime.Now);
                req.ConfirmationNumber = r.State.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4) + r.LastName.Substring(0, 2) + r.FirstName.Substring(0, 2) + c;
                req.CreatedDate = DateTime.Now;
                if (r.MultipleFiles != null)
                {
                    foreach (var file in r.MultipleFiles)
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
                }

                _context.Requests.Add(req);
                _context.SaveChanges();
                if (r.MultipleFiles != null)
                {
                    foreach (var file in r.MultipleFiles)
                    {
                        RequestWiseFile rf = new RequestWiseFile();
                        rf.RequestId = req.RequestId;
                        rf.FileName = file.FileName;
                        rf.CreatedDate = DateTime.Now;
                        _context.RequestWiseFiles.Add(rf);
                        _context.SaveChanges();

                    }
                }

                RequestClient rc = new RequestClient();
                rc.RequestId = req.RequestId;
                rc.FirstName = r.FirstName;
                rc.LastName = r.LastName;
                rc.PhoneNumber = r.Mobile;
                rc.Location = r.State;
                rc.Address = r.Street + "," + r.City + "," + r.State + " ," + r.ZipCode;
                rc.Notes = r.Symptoms;
                rc.Email = r.Email;
                rc.StrMonth = mn;
                rc.IntDate = dy;
                rc.IntYear = yy;
                rc.Street = r.Street;
                rc.City = r.City;
                rc.State = r.State;
                rc.ZipCode = r.ZipCode;




                _context.RequestClients.Add(rc);
                _context.SaveChanges();

            }
            else
            {
                /*User does not exists*/
                AspNetUser asp = new AspNetUser();
                asp.Id = Guid.NewGuid().ToString();
                asp.UserName = r.FirstName + r.LastName;
                asp.Email = r.Email;
                asp.PhoneNumber = r.Mobile;
                asp.CreatedDate = DateTime.Now;

                _context.AspNetUsers.Add(asp);
                _context.SaveChanges();

                User data = new User();
                data.AspNetUserId = asp.Id;
                data.FirstName = r.FirstName;
                data.LastName = r.LastName;
                data.Email = r.Email;
                data.Mobile = r.Mobile;
                data.Street = r.Street;
                data.City = r.City;
                data.State = r.State;
                data.ZipCode = r.ZipCode;
                data.CreatedBy = r.FirstName;
                data.CreatedDate = DateTime.Now;

                System.String sDate = r.DateOfBirth.ToString();
                DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                int dy = datevalue.Day;
                System.String mn = datevalue.Month.ToString();
                int yy = datevalue.Year;

                data.IntYear = yy;
                data.StrMonth = mn;
                data.IntDate = dy;
                data.Status = 1;

                _context.Users.Add(data);
                _context.SaveChanges();


                var emailUser = _context.Users.FirstOrDefault(x => x.Email == email);
                Request req = new Request();
                req.RequestTypeId = 2;
                req.UserId = data.UserId;
                req.FirstName = emailUser.FirstName;
                req.LastName = emailUser.LastName;
                req.PhoneNumber = emailUser.Mobile;
                req.Email = emailUser.Email;
                req.Status = 1;
                var c = _context.Users.Count(x => x.CreatedDate == DateTime.Now);
                req.ConfirmationNumber = r.State.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4) + r.LastName.Substring(0, 2) + r.FirstName.Substring(0, 2) + c;
                req.CreatedDate = DateTime.Now;
                if (r.MultipleFiles != null)
                {
                    foreach (var file in r.MultipleFiles)
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
                }

                _context.Requests.Add(req);
                _context.SaveChanges();
                if (r.MultipleFiles != null)
                {
                    foreach (var file in r.MultipleFiles)
                    {
                        RequestWiseFile rf = new RequestWiseFile();
                        rf.RequestId = req.RequestId;
                        rf.FileName = file.FileName;
                        rf.CreatedDate = DateTime.Now;
                        _context.RequestWiseFiles.Add(rf);
                        _context.SaveChanges();

                    }
                }



                RequestClient rc = new RequestClient();
                rc.RequestId = req.RequestId;
                rc.FirstName = r.FirstName;
                rc.LastName = r.LastName;
                rc.PhoneNumber = r.Mobile;
                rc.Location = r.State;
                rc.Address = r.Street + "," + r.City + "," + r.State + " ," + r.ZipCode;
                rc.Notes = r.Symptoms;
                rc.Email = r.Email;
                rc.StrMonth = mn;
                rc.IntDate = dy;
                rc.IntYear = yy;
                rc.Street = r.Street;
                rc.City = r.City;
                rc.State = r.State;
                rc.ZipCode = r.ZipCode;




                _context.RequestClients.Add(rc);
                _context.SaveChanges();


            }
        }






    }



}
