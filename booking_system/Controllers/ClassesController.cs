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

public class ClassesController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<ClassesController> _logger;

    public ClassesController(DataContext dbContext, ILogger<ClassesController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<Class> Get()
    {
        return _db.Classes;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<Class> Get([FromODataUri] Guid key)
    {
        var result = _db.Classes.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] Class Class)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Classes.Add(Class);
        await _db.SaveChangesAsync();
        return Created(Class);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<Class> Class)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingClassBooking = await _db.Classes.FindAsync(key);
        if (existingClassBooking == null)
        {
            return NotFound();
        }

        Class.Patch(existingClassBooking);
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
        var existingClassBooking = await _db.Classes.FindAsync(key);
        if (existingClassBooking == null)
        {
            return NotFound();
        }

        _db.Classes.Remove(existingClassBooking);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool ClassBookingExists(Guid key)
    {
        try
        {
            return _db.Classes.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}