using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(200, ErrorMessage = "Full name cannot exceed 200 characters.")]
        public string Fullname { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        public string UserCode { get; set; }

        public bool IsLocked { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public string? ActiveCode { get; set; }

        public string? Avatar { get; set; }

        [Required]
        public string Role { get; set; } = "User";

        public bool CanManageCategories { get; set; } = false;

        public bool EmailConfirmed { get; set; } = false;

        // nav properties
        // 1 User can have many Loans
        public virtual ICollection<Loan> Loans { get; set; }

        public User()
        {
            CreatedDate = DateTime.Now;
            IsLocked = false;
            IsDeleted = false;
            IsActive = false;
            Loans = new List<Loan>();
            Status = 0;
        }
    }
}
