namespace Lab3_LeMinhTri_2231200125.Service {
    public class FileService : IFileService {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileService(IWebHostEnvironment webHostEnvironment) {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subDirectory) {
            if (file == null || file.Length == 0) {
                throw new ArgumentException("File is empty or null.");
            }

            var wwwRootPath = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrEmpty(wwwRootPath)) {
                throw new Exception("wwwroot folder not found.");
            }

            var directoryPath = Path.Combine(wwwRootPath, subDirectory);
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(directoryPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            // return the relative URL path
            // e.g "/carousel_images/a81dsf98-my-image.jpg"
            var relativePath = Path.Combine("/", subDirectory, uniqueFileName)
                                   .Replace("\\", "/");

            return relativePath;
        }

        public void DeleteFile(string? relativePath) {
            if (string.IsNullOrEmpty(relativePath)) {
                return; // No file to delete
            }

            // Get full path from the relative path
            // e.g "/carousel_images/img.jpg" -> "C:\project\wwwroot\carousel_images\img.jpg"
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath,
                                        relativePath.TrimStart('/'));

            if (File.Exists(fullPath)) {
                File.Delete(fullPath);
            }
        }
    }
}
