using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using booking_system.Data;
using Microsoft.EntityFrameworkCore;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace class_schedule;

public class Function
{
    private readonly DataContext _db;

    public Function()
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseNpgsql(connectionString);
        _db = new DataContext(optionsBuilder.Options);
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task FunctionHandler(ILambdaContext context)
    {
        var endedClasses = await _db.Classes.Where(c => c.EndTime <= DateTime.UtcNow).Select(c => c.Id).ToListAsync();

        if (!endedClasses.Any())
            return;

        // Optional: check all bookings for ended classes (not just waiting ones)
        var checkClassId = await _db.ClassBookings.Where(cb => endedClasses.Contains(cb.ClassId)).ToListAsync();

        // Required: get only waitlist bookings for ended classes
        var waitlistBookings = await _db.ClassBookings
            .Where(cb =>
                endedClasses.Contains(cb.ClassId) &&
                cb.Status == BookingStatus.waiting &&
                cb.Refund == null)
            .ToListAsync();

        foreach (var booking in waitlistBookings)
        {
            var creditAmount = booking.UserCreditHistory?.CreditAmount ?? 0;

            if (creditAmount > 0 && booking.User != null)
            {
                // 1. Create Refund
                var refund = new booking_system.Models.Refund
                {
                    BookingId = booking.Id,
                    Type = RedundType.waitlist_refunded,
                    RefundAmount = creditAmount,
                    RefundedAt = DateTime.UtcNow,
                    Note = "Auto refund for waitlist booking."
                };

                // 2. Update User's credit balance
                booking.User.CreditBalance += creditAmount;

                _db.Refunds.Add(refund);
                await _db.SaveChangesAsync(); // Save first so we get refund.Id

                // 3. Create UserCreditHistory entry
                var creditHistory = new booking_system.Models.UserCreditHistory
                {
                    UserId = booking.User.Id,
                    RefundId = refund.Id,
                    CreditAmount = creditAmount,
                    Type = CreditType.refund, // Make sure this enum exists
                    IsExpired = false
                };

                _db.UserCreditHistories.Add(creditHistory);

                await _db.SaveChangesAsync(); // Save the credit history
            }
        }



    }

}
