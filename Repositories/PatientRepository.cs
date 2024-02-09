
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.DataAccessLayer.DataContext;
using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;

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
            
            Request req = new Request();                    
            req.RequestTypeId = 2;       
            req.UserId = data.UserId;
            req.FirstName = RequestData.FirstName;
            req.LastName = RequestData.LastName;
            req.PhoneNumber = RequestData.Mobile;
            req.Email = RequestData.Email;
            req.Status = 1;
            var c =_context.Users.Count(x => x.CreatedDate == DateTime.Now);
            req.ConfirmationNumber = RequestData.State.Substring(0, 2) + DateTime.Now.ToString().Substring(0,4) + RequestData.LastName.Substring(0,2) + RequestData.FirstName.Substring(0,2) + c;
            req.CreatedDate = DateTime.Now;
           

            _context.AspNetUsers.Add(asp);
            _context.SaveChanges();
            _context.Users.Add(data);
            _context.SaveChanges();
            _context.Requests.Add(req);
            _context.SaveChanges();

        }

    }

}
