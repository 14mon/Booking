using booking_system.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployerService.Controllers
{
    [ApiController]
    [Route("v1")]
    public class VerifyController : ControllerBase
    {
        private readonly DataContext _db;
        private readonly ILogger<VerifyController> _logger;

        public VerifyController(DataContext dbContext, ILogger<VerifyController> logger)
        {
            _logger = logger;
            _db = dbContext;
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify(Guid token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(e => e.VerificationToken == token);

            if (user == null)
            {

                return BadRequest("Invalid verification token.");
            }

            if (user.IsEmailVerified)
            {

                return BadRequest("Email already verified.");
            }

            try
            {
                user.IsEmailVerified = true;
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    userId = user.Id,
                    UserEmail = user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
