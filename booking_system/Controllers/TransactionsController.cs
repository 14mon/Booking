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

public class TransactionsController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(DataContext dbContext, ILogger<TransactionsController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<Transaction> Get()
    {
        return _db.Transactions;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<Transaction> Get([FromODataUri] Guid key)
    {
        var result = _db.Transactions.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] Transaction Transaction)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Transactions.Add(Transaction);
        await _db.SaveChangesAsync();
        return Created(Transaction);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<Transaction> Transaction)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingTransaction = await _db.Transactions.FindAsync(key);
        if (existingTransaction == null)
        {
            return NotFound();
        }

        Transaction.Patch(existingTransaction);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TransactionExists(key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Updated(existingTransaction);
    }

    [EnableQuery]
    public async Task<IActionResult> Delete([FromODataUri] Guid key)
    {
        var existingTransaction = await _db.Transactions.FindAsync(key);
        if (existingTransaction == null)
        {
            return NotFound();
        }

        _db.Transactions.Remove(existingTransaction);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool TransactionExists(Guid key)
    {
        try
        {
            return _db.Transactions.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}