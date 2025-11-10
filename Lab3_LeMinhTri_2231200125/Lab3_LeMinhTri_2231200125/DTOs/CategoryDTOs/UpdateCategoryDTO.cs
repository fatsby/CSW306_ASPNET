using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.DTOs.CategoryDTOs {
    public class UpdateCategoryDTO {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }
}