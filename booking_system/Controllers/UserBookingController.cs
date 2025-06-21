using booking_system.Data;
using booking_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("v1/[controller]")]
public class UserBookingController : ControllerBase
{
    private readonly DataContext _db;
    private readonly ILogger<UserBookingController> _logger;

    public UserBookingController(DataContext dbContext, ILogger<UserBookingController> logger)
    {
        _logger = logger;
        _db = dbContext;

    }

    [HttpPost]
    [Route("Class")]
    public async Task<IActionResult> Booking(BookRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var classCredit = await _db.Classes
            .FirstOrDefaultAsync(c => c.Id == request.ClassId && c.Status == ClassStatus.active);

        if (classCredit == null)
        {
            return NotFound("Class not found or not active.");
        }

        if (user.CreditBalance < classCredit.RequiredCredit)
        {
            return BadRequest("Insufficient credit to book this class.");
        }

        // Prepare credit history entry
        UserCreditHistory creditHistory = new()
        {
            UserId = request.UserId,
            CreditAmount = classCredit.RequiredCredit,
            Type = CreditType.book,
        };

        // Determine booking status based on class fullness
        BookingStatus bookingStatus = classCredit.IsFull ? BookingStatus.waiting : BookingStatus.booked;

        ClassBooking booking = new()
        {
            UserId = request.UserId,
            UserCreditId = creditHistory.Id,
            Status = bookingStatus,
            BookAt = DateTime.UtcNow,
            ClassId = request.ClassId
        };

        // Save changes: add credit history and booking to db context
        _db.UserCreditHistories.Add(creditHistory);
        _db.ClassBookings.Add(booking);

        // Optionally, reduce user credit balance if booking confirmed (not waiting)
        if (!classCredit.IsFull)
        {
            user.CreditBalance -= classCredit.RequiredCredit;
        }

        await _db.SaveChangesAsync();

        return Ok($"Booking {(bookingStatus == BookingStatus.booked ? "successful" : "waiting list")}.");
    }

    [HttpPost]
    [Route("Cancel")]
    public async Task<IActionResult> Cancel(CancelRequest request)
    {
        var booking = await _db.ClassBookings
            .Include(b => b.User)                  // Include User for updating credit balance
            .Include(b => b.UserCreditHistory)     // Include UserCreditHistory for refund amount
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && b.UserId == request.UserId);

        if (booking == null)
            return NotFound("Booking not found.");

        if (booking.Status == BookingStatus.cancelledbyClass || booking.Status == BookingStatus.cancelledbyUser)
            return BadRequest("Booking is already cancelled.");

        var classEntity = await _db.Classes.FirstOrDefaultAsync(c => c.Id == booking.ClassId);
        if (classEntity == null)
            return NotFound("Class not found.");

        DateTime classStartTime = classEntity.StartTime;
        var hoursBeforeStart = (classStartTime - request.CancelTime).TotalHours;
        bool isEligibleForRefund = hoursBeforeStart >= 4;

        booking.CancelledAt = request.CancelTime;
        booking.Status = BookingStatus.cancelledbyUser;

        // Save the booking update first
        await _db.SaveChangesAsync();

        if (isEligibleForRefund)
        {
            // Make sure UserCreditHistory exists and CreditAmount is valid
            if (booking.UserCreditHistory == null)
            {
                return BadRequest("No credit history available for refund.");
            }

            int refundAmount = booking.UserCreditHistory.CreditAmount;

            // Create Refund record
            var refund = new Refund
            {
                BookingId = booking.Id,
                Type = RedundType.cancelled_before_4hr,  // Fix typo: RedundType -> RefundType?
                RefundAmount = refundAmount,
                RefundCurrency = null, // Ensure your DB allows null or provide a currency code
                RefundedAt = DateTime.UtcNow,
                Note = "Refund due to cancellation 4 hours before class start."
            };
            _db.Refunds.Add(refund);

            // Save here to generate refund.Id (if Identity PK)
            await _db.SaveChangesAsync();

            // Create UserCreditHistory record for refund
            var refundCreditHistory = new UserCreditHistory
            {
                UserId = booking.UserId,
                RefundId = refund.Id,
                CreditAmount = refundAmount,
                Type = CreditType.refund,
                IsExpired = false,
                CreatedAt = DateTime.UtcNow,  // Add CreatedAt if your model requires it
            };
            _db.UserCreditHistories.Add(refundCreditHistory);

            // Update user's credit balance
            booking!.User!.CreditBalance += refundAmount;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Booking cancelled and refund processed."
            });
        }

        return Ok(new
        {
            Message = "Booking cancelled, no refund due to cancellation within 4 hours of class start."
        });
    }


    public class BookRequest
    {
        public Guid UserId { get; set; }
        public Guid ClassId { get; set; }

    }

    public class CancelRequest
    {
        public Guid UserId { get; set; }
        public Guid BookingId { get; set; }
        public DateTime CancelTime { get; set; }

    }
}