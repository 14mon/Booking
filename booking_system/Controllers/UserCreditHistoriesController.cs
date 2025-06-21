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

public class UserCreditHistoriesController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<UserCreditHistoriesController> _logger;

    public UserCreditHistoriesController(DataContext dbContext, ILogger<UserCreditHistoriesController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<UserCreditHistory> Get()
    {
        return _db.UserCreditHistories;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<UserCreditHistory> Get([FromODataUri] Guid key)
    {
        var result = _db.UserCreditHistories.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] UserCreditHistory UserCreditHistory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.UserCreditHistories.Add(UserCreditHistory);
        await _db.SaveChangesAsync();
        return Created(UserCreditHistory);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<UserCreditHistory> UserCreditHistory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingClassBooking = await _db.UserCreditHistories.FindAsync(key);
        if (existingClassBooking == null)
        {
            return NotFound();
        }

        UserCreditHistory.Patch(existingClassBooking);
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
        var existingClassBooking = await _db.UserCreditHistories.FindAsync(key);
        if (existingClassBooking == null)
        {
            return NotFound();
        }

        _db.UserCreditHistories.Remove(existingClassBooking);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool ClassBookingExists(Guid key)
    {
        try
        {
            return _db.UserCreditHistories.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}