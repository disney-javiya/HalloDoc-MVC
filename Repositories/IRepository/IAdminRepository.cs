using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IAdminRepository
    {
        AspNetUser ValidateUser(string email, string password);
        public IEnumerable<RequestandRequestClient> getRequestStateData(int type);
        public RequestClient getPatientInfo(int requestId);
        public string getConfirmationNumber(int requestId);
      
        public RequestNote getNotes(int requestId);
        public string getTranferNotes(int requestId);
        public void adminNotes(int requestId, RequestNote r, string email);

    }
}
