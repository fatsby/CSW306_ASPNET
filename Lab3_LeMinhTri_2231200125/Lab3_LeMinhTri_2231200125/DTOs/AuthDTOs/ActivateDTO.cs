using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.DTOs.AuthDTOs {
    public class ActivateDTO {
        [Required]
        public string Code { get; set; }
    }
}
