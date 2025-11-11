using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.DTOs.AuthDTOs {
    public class RegisterDTO {
        [Required]
        [StringLength(200)]
        public string Fullname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
