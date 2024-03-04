using HalloDoc.DataAccessLayer.DataModels;
using HalloDoc.DataAccessLayer.DataModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Repository.IRepository
{
    public interface IAdminRepository
    {
        AspNetUser ValidateUser(string email, string password);
        public IEnumerable<RequestandRequestClient> getRequestStateData(int type);
        
        public RequestClient getPatientInfo(int requestId);
        public string getConfirmationNumber(int requestId);

        //public RequestNote getNotes(int requestId);
        //public string getTranferNotes(int requestId);
        public void adminNotes(int requestId, viewNotes v, string email);
        //public void adminCancelNote([FromBody] viewNotes viewNoteData, string email);
        public void adminCancelNote(string requestId, string reason, string additionalNotes, string email);
        public viewNotes getNotes(int requestId, string email);
        public List<Physician> GetPhysicians(int regionId);
        public void adminAssignNote(string requestId, string region, string physician, string additionalNotesAssign, string email);

        public void adminBlockNote(string requestId, string additionalNotesBlock, string email);

        public List<RequestWiseFile> GetDocumentsByRequestId(int requestId);

        public void UploadFiles(int requestId, List<IFormFile> files, string email);
        public RequestWiseFile GetFileById(int fileId);
        public void DeleteFile(int fileId);
        public IEnumerable<RequestWiseFile> GetFilesByRequestId(int requestId);
        public IEnumerable<RequestWiseFile> GetFilesByIds(List<int> fileIds);

        public void GetFilesByIdsDelete(List<int> fileIds);
        public void GetFilesByRequestIdDelete(int requestId);
        public string GetPatientEmail(int requestId);
        public List<string> GetAllFiles(int requestId);

        public List<string> GetSelectedFiles(List<int> ids);


    }
}
