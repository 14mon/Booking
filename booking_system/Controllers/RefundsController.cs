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

public class RefundsController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<RefundsController> _logger;

    public RefundsController(DataContext dbContext, ILogger<RefundsController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<Refund> Get()
    {
        return _db.Refunds;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<Refund> Get([FromODataUri] Guid key)
    {
        var result = _db.Refunds.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] Refund Refund)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Refunds.Add(Refund);
        await _db.SaveChangesAsync();
        return Created(Refund);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<Refund> Refund)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingRefund = await _db.Refunds.FindAsync(key);
        if (existingRefund == null)
        {
            return NotFound();
        }

        Refund.Patch(existingRefund);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RefundExists(key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Updated(existingRefund);
    }

    [EnableQuery]
    public async Task<IActionResult> Delete([FromODataUri] Guid key)
    {
        var existingRefund = await _db.Refunds.FindAsync(key);
        if (existingRefund == null)
        {
            return NotFound();
        }

        _db.Refunds.Remove(existingRefund);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool RefundExists(Guid key)
    {
        try
        {
            return _db.Refunds.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}