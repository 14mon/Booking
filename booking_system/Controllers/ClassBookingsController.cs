using booking_system.Data;
using booking_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace booking_system.Controller;

public class ClassBookingsController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<ClassBookingsController> _logger;

    public ClassBookingsController(DataContext dbContext, ILogger<ClassBookingsController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<ClassBooking> Get()
    {
        return _db.ClassBookings;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<ClassBooking> Get([FromODataUri] Guid key)
    {
        var result = _db.ClassBookings.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] ClassBooking ClassBooking)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.ClassBookings.Add(ClassBooking);
        await _db.SaveChangesAsync();
        return Created(ClassBooking);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<ClassBooking> ClassBooking)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingClassBooking = await _db.ClassBookings.FindAsync(key);
        if (existingClassBooking == null)
        {
            return NotFound();
        }

        ClassBooking.Patch(existingClassBooking);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClassBookingExists(key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Updated(existingClassBooking);
    }

    [EnableQuery]
    public async Task<IActionResult> Delete([FromODataUri] Guid key)
    {
        var existingClassBooking = await _db.ClassBookings.FindAsync(key);
        if (existingClassBooking == null)
        {
            return NotFound();
        }

        _db.ClassBookings.Remove(existingClassBooking);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool ClassBookingExists(Guid key)
    {
        try
        {
            return _db.ClassBookings.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}