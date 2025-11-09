using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.DTOs.AuthorDTOs
{
    public class CreateAuthorDTO
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Biography { get; set; }
        [StringLength(100)]
        public string? Nationality { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        [StringLength(100)]
        [Url]
        public string? Website { get; set; }

        [Required]
        public IFormFile AvatarFile { get; set; }
    }
}
