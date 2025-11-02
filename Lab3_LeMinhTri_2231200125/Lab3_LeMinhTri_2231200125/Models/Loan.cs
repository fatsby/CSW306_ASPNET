using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3_LeMinhTri_2231200125.Models
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }

        // Foreign Key for User
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Foreign Key for Book
        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [Required]
        public DateTime LoanDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required]
        public int Status { get; set; } // 0: Active, 1: Returned, 2: Overdue

        public Loan()
        {
            LoanDate = DateTime.Now;
            Status = 0;
        }
    }
}
