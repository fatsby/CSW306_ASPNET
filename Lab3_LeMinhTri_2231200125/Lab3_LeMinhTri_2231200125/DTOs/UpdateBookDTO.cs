using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.DTOs {
    public class UpdateBookDTO {
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        public string? Description { get; set; }
        public string? BookCode { get; set; }
        public string? Publisher { get; set; }

        public DateTime? PublishedYear { get; set; }

        public int? CategoryId { get; set; }

        public int? AuthorId { get; set; }

        public IFormFile? CoverImage { get; set; }
        public IFormFile? PdfFile { get; set; }
    }
}
