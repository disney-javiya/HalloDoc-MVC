using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;
using Microsoft.AspNetCore.Http;
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
        public void CreateFamilyRequest(familyCreateRequest RequestData);
        public void CreateConciergeRequest(conciergeCreateRequest RequestData);
        public void CreateBusinessRequest(businessCreateRequest RequestData);
        public List<Request> GetbyEmail(string email);
  
        public List<RequestWiseFile> GetDocumentsByRequestId(int requestId);
        public void UploadFiles(int requestId, List<IFormFile> files);
        public RequestWiseFile GetFileById(int fileId);
        public User GetPatientData(string email);
        public IEnumerable<RequestWiseFile> GetAllFiles();
        public IEnumerable<RequestWiseFile> GetFilesByIds(List<int> fileIds);
        public void createPatientRequestMe(createPatientRequest RequestData);

        public void createPatientRequestSomeoneElse(string email,requestSomeoneElse r);
        public void updateProfile(string email,User u);
       



    }
}
