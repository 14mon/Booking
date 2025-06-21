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

public class PaymentGatewaysController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<PaymentGatewaysController> _logger;

    public PaymentGatewaysController(DataContext dbContext, ILogger<PaymentGatewaysController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<PaymentGateway> Get()
    {
        return _db.PaymentGateways;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<PaymentGateway> Get([FromODataUri] Guid key)
    {
        var result = _db.PaymentGateways.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] PaymentGateway PaymentGateway)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.PaymentGateways.Add(PaymentGateway);
        await _db.SaveChangesAsync();
        return Created(PaymentGateway);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<PaymentGateway> PaymentGateway)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingPaymentGateway = await _db.PaymentGateways.FindAsync(key);
        if (existingPaymentGateway == null)
        {
            return NotFound();
        }

        PaymentGateway.Patch(existingPaymentGateway);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PaymentGatewayExists(key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Updated(existingPaymentGateway);
    }

    [EnableQuery]
    public async Task<IActionResult> Delete([FromODataUri] Guid key)
    {
        var existingPaymentGateway = await _db.PaymentGateways.FindAsync(key);
        if (existingPaymentGateway == null)
        {
            return NotFound();
        }

        _db.PaymentGateways.Remove(existingPaymentGateway);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool PaymentGatewayExists(Guid key)
    {
        try
        {
            return _db.PaymentGateways.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}