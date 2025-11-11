using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.DTOs.AuthDTOs {
    public class LoginDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
