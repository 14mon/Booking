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

public class PackagesController : ODataController
{
    private readonly DataContext _db;

    private readonly ILogger<PackagesController> _logger;

    public PackagesController(DataContext dbContext, ILogger<PackagesController> logger)
    {
        _logger = logger;
        _db = dbContext;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public IQueryable<Package> Get()
    {
        return _db.Packages;
    }

    [EnableQuery(MaxExpansionDepth = 50, MaxAnyAllExpressionDepth = 5, MaxNodeCount = 200, PageSize = 15)]
    public SingleResult<Package> Get([FromODataUri] Guid key)
    {
        var result = _db.Packages.Where(c => c.Id == key);
        return SingleResult.Create(result);
    }

    [EnableQuery]
    public async Task<IActionResult> Post([FromBody] Package Package)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.Packages.Add(Package);
        await _db.SaveChangesAsync();
        return Created(Package);
    }

    [EnableQuery]
    public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<Package> Package)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var existingPackage = await _db.Packages.FindAsync(key);
        if (existingPackage == null)
        {
            return NotFound();
        }

        Package.Patch(existingPackage);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PackageExists(key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Updated(existingPackage);
    }

    [EnableQuery]
    public async Task<IActionResult> Delete([FromODataUri] Guid key)
    {
        var existingPackage = await _db.Packages.FindAsync(key);
        if (existingPackage == null)
        {
            return NotFound();
        }

        _db.Packages.Remove(existingPackage);
        await _db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }

    private bool PackageExists(Guid key)
    {
        try
        {
            return _db.Packages.Any(p => p.Id == key);
        }
        catch (System.Exception ex)
        {
            throw;
        }

    }
}