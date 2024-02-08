
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
            User data = new User();
            data.FirstName = RequestData.FirstName;
            data.LastName = RequestData.LastName;
            data.Email = RequestData.Email;
            data.Mobile = RequestData.Mobile;
            data.Street = RequestData.Street;
            data.City = RequestData.City;
            data.State = RequestData.State;
            data.ZipCode = RequestData.ZipCode;
            data.Dateofbirth = RequestData.Dateofbirth;
            data.Symptoms = RequestData.Symptoms;
            data.Noofrooms = RequestData.Noofrooms;
            data.CreatedBy = RequestData.FirstName;
            data.CreatedDate = DateTime.Now;

            _context.Users.Add(data);
            _context.SaveChanges();

        }

    }

}
