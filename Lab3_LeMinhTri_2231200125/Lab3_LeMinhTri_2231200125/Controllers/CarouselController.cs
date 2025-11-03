using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.Models;
using Lab3_LeMinhTri_2231200125.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3_LeMinhTri_2231200125.Controllers {
    [Route("api/carousel")]
    [ApiController]
    public class CarouselController : ControllerBase {
        private readonly AppDbContext _dbContext;

        public CarouselController(AppDbContext dbContext) {
            _dbContext = dbContext;
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
        public async Task<IActionResult> CreateAsync([FromBody] CreateCarouselDTO carouselRequest) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var existingOrder = await _dbContext.Carousel
                                        .AnyAsync(c => c.Order == carouselRequest.Order);
            if (existingOrder) {
                return BadRequest(new { Message = "Order already exists" });
            }

            var newCarousel = new Carousel {
                ImageUrl = carouselRequest.ImageUrl,
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
            //if (id != updatedCarousel.CarouselId) {
            //    return BadRequest(new { Message = "ID mismatch" });
            //}
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
