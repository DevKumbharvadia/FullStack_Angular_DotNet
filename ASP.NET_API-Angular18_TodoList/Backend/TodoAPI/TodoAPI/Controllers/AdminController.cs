using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Models.Entity;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/audits
        [HttpGet]
        public IActionResult GetUserAudits()
        {
            try
            {
                // Include the User data in the result
                var audits = _context.UserAudits
            .Include(a => a.User)
            .Select(a => new AuditWithUserDto
            {
                UserAuditId = a.UserAuditId,
                UserId = a.UserId,
                Username = a.User.Username, // Make sure to include this property
                LoginTime = a.LoginTime,
                LogoutTime = a.LogoutTime
            })
            .ToList();

                if (audits == null)
                {
                    return NotFound("No audits found.");
                }

                return Ok(audits);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        // GET: api/audits/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<UserAudit>>> GetUserAuditsByUserId(Guid userId)
        {
            try
            {
                var audits = await _context.UserAudits
                    .Where(a => a.UserId == userId)
                    .Include(a => a.User)
                    .ToListAsync();

                if (audits == null || !audits.Any())
                {
                    return NotFound($"No audits found for user with ID: {userId}");
                }

                return Ok(audits);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}