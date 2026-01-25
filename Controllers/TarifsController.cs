// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Presentation Layer)
// This controller is part of the Presentation Layer in Clean Architecture. It handles HTTP requests
// related to Tarif entities and translates them into application-level commands or queries.
//
// META-DATA:
//   - Layer: Presentation (API Controller)
//   - Responsibility: Expose Tarif-related functionality via a RESTful API.
// =================================================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;

namespace UMOApi.Controllers;

[ApiController]
[Route("tarifs")]
public class TarifsController : ControllerBase
{
    private readonly UMOApiDbContext _context;

    public TarifsController(UMOApiDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a list of tariffs.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TarifDto>>> GetTarifs()
    {
        var tarifs = await _context.Tarifs
            .Include(t => t.VatTax)
            .Select(t => new TarifDto
            {
                Id = t.Id,
                Name = t.Name,
                BasePrice = t.BasePrice,
                VatPercentage = t.VatTax != null ? t.VatTax.Percentage : 0,
                TotalPrice = t.TotalPrice
            })
            .ToListAsync();

        return Ok(tarifs);
    }
}
