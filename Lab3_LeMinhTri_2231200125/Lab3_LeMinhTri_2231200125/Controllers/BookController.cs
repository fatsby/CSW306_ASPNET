using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.DTOs.BookDTOs;
using Lab3_LeMinhTri_2231200125.Models;
using Lab3_LeMinhTri_2231200125.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3_LeMinhTri_2231200125.Controllers {
    [Route("api/books")]
    [ApiController]
    public class BookController : ControllerBase {
        private readonly AppDbContext _dbContext;
        private readonly IFileService _fileService;

        public BookController(AppDbContext dbContext, IFileService fileService) {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] string? Title, [FromQuery] int? CategoryID, [FromQuery] int? AuthorID) {
            var query = _dbContext.Books.Include(b => b.Loans).AsQueryable();

            query = query.Where(b => b.IsActive); // Only active books

            if (!string.IsNullOrEmpty(Title)) {
                query = query.Where(b => b.Title.Contains(Title));
            }

            if (CategoryID.HasValue) {
                query = query.Where(b => b.CategoryId == CategoryID.Value);
            }
            if (AuthorID.HasValue) {
                query = query.Where(b => b.AuthorId == AuthorID.Value);
            }

            var books = await query.ToListAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        [ActionName("GetByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id) {
            var book = await _dbContext.Books.Include(b => b.Loans).FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null) {
                return NotFound(new { Message = "Book not found" });
            }
            return Ok(book);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateBookDTO request) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            //Foreign key validation
            var authorExists = await _dbContext.Authors.AnyAsync(a => a.AuthorId == request.AuthorId);
            var categoryExists = await _dbContext.Categories.AnyAsync(c => c.CategoryId == request.CategoryId);

            if (!authorExists) {
                return BadRequest(new { Message = $"Author with ID {request.AuthorId} not found." });
            }
            if (!categoryExists) {
                return BadRequest(new { Message = $"Category with ID {request.CategoryId} not found." });
            }

            if (request.AvailableCopies > request.TotalCopies) {
                return BadRequest(new { Message = "AvailableCopies cannot be greater than TotalCopies." });
            }

            //CoverImage and PDF validation and saving
            var coverImagePath = "";
            if (request.CoverImage != null && request.CoverImage.Length > 0) {
                coverImagePath = await _fileService.SaveFileAsync(request.CoverImage, "book_covers");
            }

            var PdfFileUrl = "";
            if (request.PdfFile != null && request.PdfFile.Length > 0) {
                PdfFileUrl = await _fileService.SaveFileAsync(request.PdfFile, "book_pdfs");
            }

            var newBook = new Book {
                Title = request.Title,
                Description = request.Description ?? "",
                BookCode = request.BookCode,
                Publisher = request.Publisher,
                PublishedYear = request.PublishedYear,
                CategoryId = request.CategoryId,
                AuthorId = request.AuthorId,
                CoverImageUrl = coverImagePath,
                PdfUrl = PdfFileUrl,
                CreatedDate = DateTime.Now,
                IsActive = true,
                TotalCopies = request.TotalCopies,
                AvailableCopies = request.AvailableCopies
            };

            _dbContext.Books.Add(newBook);
            await _dbContext.SaveChangesAsync();

            return Ok(newBook);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PutAsync(int id, UpdateBookDTO request) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            //validate book exists
            var bookToUpdate = await _dbContext.Books.FindAsync(id);
            if (bookToUpdate == null) {
                return NotFound(new { Message = $"Book with ID {id} not found" });
            }


            //validate foreign keys
            if (request.AuthorId.HasValue) {
                var authorExists = await _dbContext.Authors.AnyAsync(a => a.AuthorId == request.AuthorId.Value);
                if (!authorExists) {
                    return BadRequest(new { Message = $"Author with ID {request.AuthorId.Value} not found." });
                }
            }
            if (request.CategoryId.HasValue) {
                var categoryExists = await _dbContext.Categories.AnyAsync(c => c.CategoryId == request.CategoryId.Value);
                if (!categoryExists) {
                    return BadRequest(new { Message = $"Category with ID {request.CategoryId.Value} not found." });
                }
            }

            //validate unique BookCode
            if (!string.IsNullOrEmpty(request.BookCode)) {
                var existingBookWithSameCode = await _dbContext.Books
                    .AnyAsync(b => b.BookCode == request.BookCode && b.BookId != id);
                if (existingBookWithSameCode) {
                    return BadRequest(new { Message = $"Another book with BookCode {request.BookCode} already exists." });
                }
            }

            //validate files
            if (request.CoverImage != null) {
                _fileService.DeleteFile(bookToUpdate.CoverImageUrl); // Delete the old file
                bookToUpdate.CoverImageUrl = await _fileService.SaveFileAsync(request.CoverImage, "book_covers"); // Save the new file
            }

            if (request.PdfFile != null) {
                _fileService.DeleteFile(bookToUpdate.PdfUrl); // Delete the old file
                bookToUpdate.PdfUrl = await _fileService.SaveFileAsync(request.PdfFile, "book_pdfs"); // Save the new file
            }

            bookToUpdate.Title = request.Title ?? bookToUpdate.Title;
            bookToUpdate.Description = request.Description ?? bookToUpdate.Description;
            bookToUpdate.BookCode = request.BookCode ?? bookToUpdate.BookCode;
            bookToUpdate.Publisher = request.Publisher ?? bookToUpdate.Publisher;
            bookToUpdate.PublishedYear = request.PublishedYear ?? bookToUpdate.PublishedYear;
            bookToUpdate.CategoryId = request.CategoryId ?? bookToUpdate.CategoryId;
            bookToUpdate.AuthorId = request.AuthorId ?? bookToUpdate.AuthorId;
            bookToUpdate.TotalCopies = request.TotalCopies ?? bookToUpdate.TotalCopies;
            bookToUpdate.AvailableCopies = request.AvailableCopies ?? bookToUpdate.AvailableCopies;

            _dbContext.Books.Update(bookToUpdate);
            await _dbContext.SaveChangesAsync();

            return Ok(bookToUpdate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id) {
            var bookToDelete = await _dbContext.Books.FindAsync(id);
            if (bookToDelete == null) {
                return NotFound(new { Message = $"Book with ID {id} not found" });
            }
            //Delete associated files
            _fileService.DeleteFile(bookToDelete.CoverImageUrl);
            _fileService.DeleteFile(bookToDelete.PdfUrl);

            //soft delete
            bookToDelete.IsActive = false;

            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Book deleted successfully" });
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedBooksAsync() {
            var deletedBooks = await _dbContext.Books
                .Where(b => !b.IsActive)
                .ToListAsync();
            return Ok(deletedBooks);
        }

        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreBookAsync(int id) {
            var bookToRestore = await _dbContext.Books.FindAsync(id);
            if (bookToRestore == null) {
                return NotFound(new { Message = $"Book with ID {id} not found" });
            }
            if (bookToRestore.IsActive) {
                return BadRequest(new { Message = "Book is already active" });
            }
            bookToRestore.IsActive = true;
            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Book restored successfully" });
        }

        [HttpDelete("{id}/hard")]
        public async Task<IActionResult> HardDeleteAsync(int id) {
            var bookToDelete = await _dbContext.Books.FindAsync(id);
            if (bookToDelete == null) {
                return NotFound(new { Message = $"Book with ID {id} not found" });
            }
            //Delete associated files
            _fileService.DeleteFile(bookToDelete.CoverImageUrl);
            _fileService.DeleteFile(bookToDelete.PdfUrl);
            _dbContext.Books.Remove(bookToDelete);
            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Book permanently deleted successfully" });
        }

        [HttpGet("{id}/read")]
        public async Task<IActionResult> ReadBookAsync(int id) {
            var book = await _dbContext.Books.FindAsync(id);
            if (book == null) {
                return NotFound(new { Message = $"Book with ID {id} not found" });
            }

            if (string.IsNullOrEmpty(book.PdfUrl)) {
                return NotFound(new { Message = "PDF not found for this book." });
            }
            return Redirect(book.PdfUrl);
        }
    }
}
