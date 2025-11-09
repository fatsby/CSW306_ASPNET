using Azure.Core;
using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.DTOs.CarouselDTOs;
using Lab3_LeMinhTri_2231200125.Models;
using Lab3_LeMinhTri_2231200125.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3_LeMinhTri_2231200125.Controllers {
    [Route("api/carousel")]
    [ApiController]
    public class CarouselController : ControllerBase {
        private readonly AppDbContext _dbContext;
        private readonly IFileService _fileService;

        public CarouselController(AppDbContext dbContext, IFileService fileService) {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync() {
            //Find all active carousels
            var carousels = await _dbContext.Carousel
                                .Where(c => c.IsActive)
                                .ToListAsync();
            return Ok(carousels);
        }

        [HttpGet("{id}")]
        [ActionName("GetByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id) {
            var carousel = await _dbContext.Carousel.FindAsync(id);
            if (carousel == null) {
                return NotFound(new { Message = "Carousel not found" });
            }
            return Ok(carousel);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateCarouselDTO carouselRequest) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var existingOrder = await _dbContext.Carousel
                                        .AnyAsync(c => c.Order == carouselRequest.Order);
            if (existingOrder) {
                return BadRequest(new { Message = "Order already exists" });
            }

            var imageUrl = await _fileService.SaveFileAsync(carouselRequest.ImageFile, "carousel_images");

            var newCarousel = new Carousel {
                ImageUrl = imageUrl,
                Title = carouselRequest.Title,
                Description = carouselRequest.Description ?? "",
                LinkUrl = carouselRequest.LinkUrl ?? "",
                Order = carouselRequest.Order,
                IsActive = true,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _dbContext.Carousel.Add(newCarousel);
            await _dbContext.SaveChangesAsync();
            return Ok(newCarousel);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateCarouselDTO request) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            //find the carousel to update
            var carouselToUpdate = await _dbContext.Carousel.FindAsync(id);
            if (carouselToUpdate == null) {
                return NotFound(new { Message = "Carousel not found" });
            }

            //check if the order already exists for another carousel
            var orderExists = await _dbContext.Carousel
        .AnyAsync(c => c.Order == request.Order && c.CarouselId != id);

            if (orderExists) {
                return BadRequest(new { Message = "Order already exists for another Carousel" });
            }

            //handle image
            if (request.ImageFile != null && request.ImageFile.Length > 0) {
                //delete old image
                _fileService.DeleteFile(carouselToUpdate.ImageUrl);
                //save new image
                var newImageUrl = await _fileService.SaveFileAsync(request.ImageFile, "carousel_images");
                carouselToUpdate.ImageUrl = newImageUrl;
            }

            carouselToUpdate.Title = request.Title;
            carouselToUpdate.Description = request.Description ?? carouselToUpdate.Description;
            carouselToUpdate.LinkUrl = request.LinkUrl ?? carouselToUpdate.LinkUrl;
            carouselToUpdate.Order = request.Order;
            carouselToUpdate.IsActive = request.IsActive;
            carouselToUpdate.UpdatedDate = DateTime.Now;

            _dbContext.Carousel.Update(carouselToUpdate);

            await _dbContext.SaveChangesAsync();
            return Ok(carouselToUpdate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id) {
            var carouselToDelete = await _dbContext.Carousel.FindAsync(id);
            if (carouselToDelete == null) {
                return NotFound(new { Message = "Carousel not found" });
            }
            //delete image file
            _fileService.DeleteFile(carouselToDelete.ImageUrl);


            _dbContext.Carousel.Remove(carouselToDelete);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
