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

public class GatewayRawEventsController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<GatewayRawEventsController> _logger;

    public GatewayRawEventsController(DataContext dbContext, ILogger<GatewayRawEventsController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<GatewayRawEvent> Get()
    {
        return _db.GatewayRawEvents;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<GatewayRawEvent> Get([FromODataUri] Guid key)
    {
        var result = _db.GatewayRawEvents.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] GatewayRawEvent GatewayRawEvent)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.GatewayRawEvents.Add(GatewayRawEvent);
        await _db.SaveChangesAsync();
        return Created(GatewayRawEvent);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<GatewayRawEvent> GatewayRawEvent)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingGatewayRawEvent = await _db.GatewayRawEvents.FindAsync(key);
        if (existingGatewayRawEvent == null)
        {
            return NotFound();
        }

        GatewayRawEvent.Patch(existingGatewayRawEvent);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GatewayRawEventExists(key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Updated(existingGatewayRawEvent);
    }

    [EnableQuery]
    public async Task<IActionResult> Delete([FromODataUri] Guid key)
    {
        var existingGatewayRawEvent = await _db.GatewayRawEvents.FindAsync(key);
        if (existingGatewayRawEvent == null)
        {
            return NotFound();
        }

        _db.GatewayRawEvents.Remove(existingGatewayRawEvent);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool GatewayRawEventExists(Guid key)
    {
        try
        {
            return _db.GatewayRawEvents.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}