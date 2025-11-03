namespace Lab3_LeMinhTri_2231200125.Service {
    public interface IFileService {
        Task<string> SaveFileAsync(IFormFile file, string subDirectory);
        void DeleteFile(string? relativePath);
    }
}
