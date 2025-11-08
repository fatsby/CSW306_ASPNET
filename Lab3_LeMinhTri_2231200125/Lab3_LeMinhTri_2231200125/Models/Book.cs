using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3_LeMinhTri_2231200125.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public string BookCode { get; set; }

        [Required]
        public string Publisher { get; set; }

        [Required]
        public DateTime PublishedYear { get; set; }

        // Foreign Key for Category
        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        // Foreign Key for Author
        [Required]
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Total copies must be a positive number.")]
        public int TotalCopies { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Available copies must be a positive number.")]
        public int AvailableCopies { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? CoverImageUrl { get; set; } // Cover image path

        public string? PdfUrl { get; set; } // PDF file path

        public bool IsActive { get; set; } = true;

        // nav properties

        // A Book can be part of many Loans
        public virtual ICollection<Loan> Loans { get; set; }

        // Constructor to set default values
        public Book()
        {
            CreatedDate = DateTime.Now;
            Loans = new List<Loan>();
        }
    }
}
