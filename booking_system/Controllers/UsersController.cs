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

public class UsersController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<UsersController> _logger;

    public UsersController(DataContext dbContext, ILogger<UsersController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<User> Get()
    {
        return _db.Users;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<User> Get([FromODataUri] Guid key)
    {
        var result = _db.Users.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] User User)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Users.Add(User);
        await _db.SaveChangesAsync();
        return Created(User);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<User> User)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingUser = await _db.Users.FindAsync(key);
        if (existingUser == null)
        {
            return NotFound();
        }

        User.Patch(existingUser);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Updated(existingUser);
    }

    [EnableQuery]
    public async Task<IActionResult> Delete([FromODataUri] Guid key)
    {
        var existingUser = await _db.Users.FindAsync(key);
        if (existingUser == null)
        {
            return NotFound();
        }

        _db.Users.Remove(existingUser);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool UserExists(Guid key)
    {
        try
        {
            return _db.Users.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}