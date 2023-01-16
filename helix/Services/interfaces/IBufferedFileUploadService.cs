using helix.Models;

namespace helix.Services.interfaces
{
    public interface IBufferedFileUploadService
    {
        Task<List<KeyValuePair<int,bool>>> UploadFile(string Id,IFormFile[] files);

        Task<List<KeyValuePair<int, ObservationSubmission>>> UploadFileFits(IFormFile[] files);

        Task<bool> DeleteFile(string Id, string fileName);
        
        Task<List<string>> GetFiles(string Id);

        Task<string> GetFITSAsImage(string Id, string Name);
    }
}
