using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.Models {
    public class Carousel {
        [Key]
        public int CarouselId { get; set; }

        [Required(ErrorMessage = "ImageURL is required for Carousel")]
        public required string ImageUrl { get; set; }

        [Required(ErrorMessage = "Title is required for Carousel")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public required string Title { get; set; }

        public string? Description { get; set; }
        public string? LinkUrl { get; set; }

        [Required(ErrorMessage = "Order is required for Carousel")]
        public int Order { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public Carousel() {
            CreatedDate = DateTime.Now;
            UpdatedDate = DateTime.Now;
            IsActive = true;
        }
    }
}
