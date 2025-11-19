using Lab3_LeMinhTri_2231200125.Data;
using Lab3_LeMinhTri_2231200125.DTOs.BookDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3_LeMinhTri_2231200125.Controllers {
    [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase {
        private readonly AppDbContext _dbContext;
        public ReportController(AppDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet("top-borrowed")]
        [Authorize(Policy = "AdminOrLibrarian")]
        public async Task<IActionResult> GetTopBorrowedBooks(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int top = 10) {

            var topBorrowedBooksQuery = _dbContext.Loans
                .Where(l => l.LoanDate >= fromDate && l.LoanDate <= toDate)
                .GroupBy(l => l.BookId)
                .Select(g => new {
                    BookId = g.Key,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(top);

            var report = await topBorrowedBooksQuery
                .Join(_dbContext.Books,
                      loanGroup => loanGroup.BookId,
                      book => book.BookId,
                      (loanGroup, book) => new TopBorrowedBookDTO {
                          BookId = loanGroup.BookId,
                          Title = book.Title,
                          BorrowCount = loanGroup.BorrowCount
                      })
                .ToListAsync();

            return Ok(report);
        }
    }
}
