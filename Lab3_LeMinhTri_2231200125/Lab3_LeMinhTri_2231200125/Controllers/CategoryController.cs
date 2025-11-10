using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.DTOs.CategoryDTOs;
using Lab3_LeMinhTri_2231200125.Models;
using Lab3_LeMinhTri_2231200125.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3_LeMinhTri_2231200125.Controllers {
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase {
        private readonly AppDbContext _dbContext;
        private readonly IFileService _fileService;

        public CategoryController(AppDbContext dbContext, IFileService fileService) {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] string? CategoryName) {
            var query = _dbContext.Categories.Include(c => c.Books).AsQueryable();

            query = query.Where(c => c.IsActive); // active categories

            if (!string.IsNullOrEmpty(CategoryName)) {
                query = query.Where(c => c.Name.Contains(CategoryName));
            }
            var categories = await query.ToListAsync();

            return Ok(categories);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateCategoryDTO categoryRequest) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var avatarUrlPath = "";
            if (categoryRequest.AvatarFile != null) {
                avatarUrlPath = await _fileService.SaveFileAsync(categoryRequest.AvatarFile, "category_avatars");
            }

            var newCategory = new Category {
                Name = categoryRequest.Name,
                Description = categoryRequest.Description ?? "",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                IsActive = true,
            };

            _dbContext.Categories.Add(newCategory);
            await _dbContext.SaveChangesAsync();
            return Ok(newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateCategoryDTO categoryRequest) {
            var existingCategory = await _dbContext.Categories.FindAsync(id);
            if (existingCategory == null || !existingCategory.IsActive) {
                return NotFound($"Category with ID {id} not found.");
            }
            if (!string.IsNullOrEmpty(categoryRequest.Name)) {
                existingCategory.Name = categoryRequest.Name;
            }
            if (!string.IsNullOrEmpty(categoryRequest.Description)) {
                existingCategory.Description = categoryRequest.Description;
            }
            if (categoryRequest.AvatarFile != null) {
                var avatarUrlPath = await _fileService.SaveFileAsync(categoryRequest.AvatarFile, "category_avatars");
                existingCategory.Avatar = avatarUrlPath;
            }
            existingCategory.UpdatedDate = DateTime.Now;
            _dbContext.Categories.Update(existingCategory);
            await _dbContext.SaveChangesAsync();
            return Ok(existingCategory);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateAsync(int id) {
            var existingCategory = await _dbContext.Categories.FindAsync(id);
            if (existingCategory == null || !existingCategory.IsActive) {
                return NotFound($"Category with ID {id} not found.");
            }
            existingCategory.IsActive = false;
            existingCategory.UpdatedDate = DateTime.Now;
            _dbContext.Categories.Update(existingCategory);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
