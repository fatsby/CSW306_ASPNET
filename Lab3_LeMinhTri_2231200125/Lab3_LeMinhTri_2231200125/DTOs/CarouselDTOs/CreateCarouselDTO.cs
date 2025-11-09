using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.DTOs.CarouselDTOs {
    public class CreateCarouselDTO {
        [Required]
        public required IFormFile ImageFile { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public required string Title { get; set; }

        public string? Description { get; set; }
        public string? LinkUrl { get; set; }
        [Required]
        public int Order { get; set; }

    }
}
