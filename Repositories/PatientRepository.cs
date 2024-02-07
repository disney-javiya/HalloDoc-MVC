
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.DataAccessLayer.DataContext;
using HalloDoc.DataAccessLayer.DataModels;

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
    }

}
