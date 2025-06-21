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

public class CountriesController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<CountriesController> _logger;

    public CountriesController(DataContext dbContext, ILogger<CountriesController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<Country> Get()
    {
        return _db.Countries;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<Country> Get([FromODataUri] Guid key)
    {
        var result = _db.Countries.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] Country Country)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Countries.Add(Country);
        await _db.SaveChangesAsync();
        return Created(Country);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<Country> Country)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingClassBooking = await _db.Countries.FindAsync(key);
        if (existingClassBooking == null)
        {
            return NotFound();
        }

        Country.Patch(existingClassBooking);
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
        var existingClassBooking = await _db.Countries.FindAsync(key);
        if (existingClassBooking == null)
        {
            return NotFound();
        }

        _db.Countries.Remove(existingClassBooking);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool ClassBookingExists(Guid key)
    {
        try
        {
            return _db.Countries.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}