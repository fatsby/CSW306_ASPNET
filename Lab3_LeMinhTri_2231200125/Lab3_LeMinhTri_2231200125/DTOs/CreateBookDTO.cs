using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.DTOs {
    public class CreateBookDTO {
        [Required(ErrorMessage = "Title is reqiured.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public required string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public required string BookCode { get; set; }

        [Required]
        public required string Publisher { get; set; }

        [Required]
        public DateTime PublishedYear { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public IFormFile? CoverImage { get; set; }

        public IFormFile? PdfFile { get; set; }
    }
}
