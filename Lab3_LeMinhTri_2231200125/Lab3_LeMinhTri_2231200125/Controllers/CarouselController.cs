using Azure.Core;
using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.DTOs;
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
                Description = carouselRequest.Description,
                LinkUrl = carouselRequest.LinkUrl,
                Order = carouselRequest.Order,
                IsActive = true,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _dbContext.Carousel.Add(newCarousel);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByIdAsync), new { id = newCarousel.CarouselId }, newCarousel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Carousel updatedCarousel) {
            var carousel = await _dbContext.Carousel.FindAsync(id);
            if (carousel == null) {
                return NotFound(new { Message = "Carousel not found" });
            }
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            carousel.ImageUrl = updatedCarousel.ImageUrl;
            carousel.Title = updatedCarousel.Title;
            carousel.Description = updatedCarousel.Description;
            carousel.LinkUrl = updatedCarousel.LinkUrl;
            carousel.Order = updatedCarousel.Order;
            carousel.IsActive = updatedCarousel.IsActive;
            carousel.UpdatedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
