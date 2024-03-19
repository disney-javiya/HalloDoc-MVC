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
        public AspNetUser GetUserByEmail(string email);
        public RequestClient getPatientInfo(int requestId);
        public string getConfirmationNumber(int requestId);

        public List<RequestandRequestClient> getFilterByRegions(IEnumerable<RequestandRequestClient> r, int regionId);

        public List<RequestandRequestClient> getByRequesttypeId(IEnumerable<RequestandRequestClient> r, int requesttypeId);
        public List<RequestandRequestClient> getFilterByName(IEnumerable<RequestandRequestClient> r, string patient_name);

        public List<RequestandRequestClient> getFilterByRegionAndName(IEnumerable<RequestandRequestClient> r, string patient_name, int regionId);

        public List<RequestandRequestClient> getByRequesttypeIdRegionAndName(IEnumerable<RequestandRequestClient> r, int requesttypeId, int? regionId, string? patient_name);
        public void adminNotes(int requestId, viewNotes v, string email);
        //public void adminCancelNote([FromBody] viewNotes viewNoteData, string email);
        public void adminCancelNote(string requestId, string reason, string additionalNotes, string email);

        public string getName(string requestId);

        public string getConfirmationNumber(string requestId);
        public viewNotes getNotes(int requestId, string email);
        public List<Physician> GetPhysicians(int regionId);
        public void adminAssignNote(string requestId, string region, string physician, string additionalNotesAssign, string email);

        public void adminBlockNote(string requestId, string additionalNotesBlock, string email);

        public void adminTransferCase(string requestId, string physician, string additionalNotesTransfer, string email);

        public void patientCancelNote(string requestId, string additionalNotesPatient);
        public List<RequestWiseFile> GetDocumentsByRequestId(int requestId);

        public List<Region> getAllRegions();
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
        public void sendOrderDetails(int requestId, sendOrder s, string email);
        public List<HealthProfessionalType> GetAllHealthProfessionalType();
        public List<HealthProfessional> GetAllHealthProfessional();
        public List<string> GetNameConfirmation(int requestId);
        public List<HealthProfessional> GetHealthProfessional(int healthprofessionalId);
        public HealthProfessional GetProfessionInfo(int vendorId);

        public void adminClearCase(string requestId, string email);
        public List<string> adminSendAgreementGet(string requestId);

        public void closeCaseAdmin(int requestId, string email);

        public string adminTransferNotes(string requestId, string email);
        public Admin getAdminInfo(string email);
        public string adminCreateRequest(createAdminRequest RequestData, string email);
        public void passwordresetInsert(string Email, string id);
        public List<Region> getAdminRegions(string email);
    }
}
