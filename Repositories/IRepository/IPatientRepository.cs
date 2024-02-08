using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IPatientRepository
    {
        AspNetUser ValidateUser(string email, string password);
        AspNetUser GetUserByEmail(string email);
        public void CreateRequest(createPatientRequest RequestData);
    }
}
