using System.ComponentModel.DataAnnotations;

namespace Lab3_LeMinhTri_2231200125.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; } // Nullable

        public bool IsActive { get; set; }

        public string? Avatar { get; set; }

        // nav

        // Category can have many Books
        public virtual ICollection<Book> Books { get; set; }

        public Category()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            Books = new List<Book>();
        }
    }
}
