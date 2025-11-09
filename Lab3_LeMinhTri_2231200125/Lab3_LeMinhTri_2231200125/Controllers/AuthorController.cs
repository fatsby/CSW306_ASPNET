using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.Service;
using Lab3_LeMinhTri_2231200125.DTOs.AuthorDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Lab3_LeMinhTri_2231200125.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private IFileService _fileService;

        public AuthorController(AppDbContext dbContext, IFileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] string? name)
        {
            var query = _dbContext.Authors.AsQueryable();

            query = query.Where(a => a.IsActive); // Only active authors

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(a => (a.FirstName + " " + a.LastName).Contains(name));
            }

            var authors = await query.ToListAsync();

            return Ok(authors);
        }

        [HttpGet("{id}")]
        [ActionName("GetByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var author = await _dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }
            return Ok(author);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateAuthorDTO authorRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var emailExists = await _dbContext.Authors
                                    .AnyAsync(a => a.Email == authorRequest.Email);
            if (emailExists)
            {
                return BadRequest(new { Message = "Email already exists" });
            }

            var avatarUrl = await _fileService.SaveFileAsync(authorRequest.AvatarFile, "author_avatars");

            var newAuthor = new Models.Author
            {
                FirstName = authorRequest.FirstName,
                LastName = authorRequest.LastName,
                DateOfBirth = authorRequest.DateOfBirth ?? new DateTime(1999, 1, 1),
                Biography = authorRequest.Biography ?? "",
                Nationality = authorRequest.Nationality ?? "",
                Email = authorRequest.Email ?? "",
                Website = authorRequest.Website ?? "",
                Avatar = avatarUrl,
            };

            _dbContext.Authors.Add(newAuthor);
            await _dbContext.SaveChangesAsync();
            return Ok(newAuthor);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateAuthorDTO authorRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var author = await _dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }
            if (!string.IsNullOrEmpty(authorRequest.Email) && authorRequest.Email != author.Email)
            {
                var emailExists = await _dbContext.Authors
                                        .AnyAsync(a => a.Email == authorRequest.Email && a.AuthorId != id);
                if (emailExists)
                {
                    return BadRequest(new { Message = "Email already exists" });
                }
            }

            author.FirstName = authorRequest.FirstName ?? author.FirstName;
            author.LastName = authorRequest.LastName ?? author.LastName;
            author.DateOfBirth = authorRequest.DateOfBirth ?? author.DateOfBirth;
            author.Biography = authorRequest.Biography ?? author.Biography;
            author.Nationality = authorRequest.Nationality ?? author.Nationality;
            author.Email = authorRequest.Email ?? author.Email;
            author.Website = authorRequest.Website ?? author.Website;

            var avatarUrl = "";
            if (authorRequest.AvatarFile != null)
            {
                avatarUrl = await _fileService.SaveFileAsync(authorRequest.AvatarFile, "author_avatars");
            }

            author.Avatar = avatarUrl ?? author.Avatar;

            _dbContext.Authors.Update(author);
            await _dbContext.SaveChangesAsync();

            return Ok(author);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var author = await _dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }
            author.IsActive = false;
            _dbContext.Authors.Update(author);
            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Author deleted successfully" });
        }
    }
}
