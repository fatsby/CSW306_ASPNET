using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

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

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        public string? Avatar { get; set; }

        // nav

        // Author can have many Books
        public virtual ICollection<Book> Books { get; set; }

        public Author()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            Books = new List<Book>();
        }
    }
}
