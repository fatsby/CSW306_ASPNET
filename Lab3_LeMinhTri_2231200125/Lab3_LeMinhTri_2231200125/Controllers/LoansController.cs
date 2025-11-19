using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.DTOs.LoanDTOs;
using Lab3_LeMinhTri_2231200125.Models;
using Lab3_LeMinhTri_2231200125.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lab3_LeMinhTri_2231200125.Controllers {
    [Route("api/loans")]
    [ApiController]
    public class LoansController : ControllerBase {
        private readonly AppDbContext _dbContext;
        public LoansController(AppDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] int? userId, [FromQuery] int? status, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate) {
            var query = _dbContext.Loans.AsQueryable();

            if (userId.HasValue) {
                query = query.Where(loan => loan.UserId == userId.Value);
            }

            if (status.HasValue) {
                query = query.Where(loan => (int)loan.Status == status.Value);
            }

            if (startDate.HasValue) {
                query = query.Where(loan => loan.LoanDate >= startDate.Value);
            }

            if (endDate.HasValue) {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1);
                query = query.Where(loan => loan.LoanDate < inclusiveEndDate);
            }

            var loans = await query.ToListAsync();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        [ActionName("GetByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id) {
            var loan = await _dbContext.Loans.FindAsync(id);
            if (loan == null) {
                return NotFound(new { Message = "Loan not found" });
            }
            return Ok(loan);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateLoanDTO loanRequest) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var requestedBook = await _dbContext.Books.FindAsync(loanRequest.BookId);

            if (requestedBook == null) {
                return BadRequest(new { Message = "BookId does not exist" });
            }

            if (requestedBook.AvailableCopies <= 0) {
                return BadRequest(new { Message = "No available copies for this book" });
            }

            var userExists = await _dbContext.User.AnyAsync(u => u.UserId == loanRequest.UserId);
            if (!userExists) {
                return BadRequest(new { Message = "UserId does not exist" });
            }

            requestedBook.AvailableCopies -= 1;
            _dbContext.Books.Update(requestedBook);
            await _dbContext.SaveChangesAsync();

            var newLoan = new Loan {
                UserId = loanRequest.UserId,
                BookId = loanRequest.BookId,
                DueDate = loanRequest.DueDate,
                LoanDate = DateTime.Now,
                Status = 0 // 0 = active
            };

            _dbContext.Loans.Add(newLoan);
            await _dbContext.SaveChangesAsync();
            return Ok(newLoan);
        }

        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnBookAsync(int id) {
            var loan = await _dbContext.Loans.FindAsync(id);
            if (loan == null) {
                return NotFound(new { Message = "Loan not found" });
            }
            if (loan.Status != 0) {
                return BadRequest(new { Message = "Loan is not active" });
            }
            loan.ReturnDate = DateTime.Now;
            loan.Status = 1; // 1 = returned
            var book = await _dbContext.Books.FindAsync(loan.BookId);
            if (book != null) {
                book.AvailableCopies += 1;
                _dbContext.Books.Update(book);
            }
            _dbContext.Loans.Update(loan);
            await _dbContext.SaveChangesAsync();
            return Ok(loan);
        }

        [HttpGet("/history")]
        [Authorize(Policy = "MinimumMembership")]
        public async Task<IActionResult> GetLoanHistoryAsync() {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString)) {
                return Unauthorized("User ID claim not found.");
            }

            if (!int.TryParse(userIdString, out int userId)) {
                return BadRequest("Invalid User ID format.");
            }

            var loanHistory = await _dbContext.Loans
                .Where(loan => loan.UserId == userId && loan.Status == 1) // 1 = returned
                .ToListAsync();

            return Ok(loanHistory);
        }
    }
}
