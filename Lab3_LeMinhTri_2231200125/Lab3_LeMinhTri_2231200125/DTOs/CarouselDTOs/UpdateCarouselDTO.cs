using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.DTOs.CarouselDTOs {
    public class UpdateCarouselDTO {
        [Required]
        public required string Title { get; set; }

        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Description { get; set; }

        public string? LinkUrl { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
