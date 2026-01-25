// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Presentation Layer)
// This controller is part of the Presentation Layer in Clean Architecture. It handles HTTP requests
// related to SystemEntry entities and translates them into application-level commands or queries.
//
// META-DATA:
//   - Layer: Presentation (API Controller)
//   - Responsibility: Expose SystemEntry-related functionality via a RESTful API.
// =================================================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;

namespace UMOApi.Controllers;

[ApiController]
[Route("")]
public class SystemEntryController : ControllerBase
{
    private readonly UMOApiDbContext _context;

    public SystemEntryController(UMOApiDbContext context)
    {
        _context = context;
    }

    #region Title Endpoints

    /// <summary>
    /// Retrieves a list of client titles.
    /// </summary>
    [HttpGet("title")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetTitles()
    {
        return await GetSystemEntriesByType("T");
    }

    /// <summary>
    /// Creates a new title.
    /// </summary>
    [HttpPost("title")]
    public async Task<ActionResult<SystemEntryDto>> CreateTitle([FromBody] SystemEntryCreateDto createDto)
    {
        createDto.Type = "T";
        return await CreateSystemEntry(createDto);
    }

    /// <summary>
    /// Updates a title.
    /// </summary>
    [HttpPut("title")]
    public async Task<ActionResult<SystemEntryDto>> UpdateTitle([FromBody] SystemEntryCreateDto updateDto, [FromQuery] int id)
    {
        updateDto.Type = "T";
        return await UpdateSystemEntry(updateDto, id);
    }

    #endregion

    #region Prefix Endpoints

    /// <summary>
    /// Retrieves a list of prefixes.
    /// </summary>
    [HttpGet("prefix")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetPrefixes()
    {
        return await GetSystemEntriesByType("P");
    }

    /// <summary>
    /// Creates a new prefix.
    /// </summary>
    [HttpPost("prefix")]
    public async Task<ActionResult<SystemEntryDto>> CreatePrefix([FromBody] SystemEntryCreateDto createDto)
    {
        createDto.Type = "P";
        return await CreateSystemEntry(createDto);
    }

    /// <summary>
    /// Updates a prefix.
    /// </summary>
    [HttpPut("prefix")]
    public async Task<ActionResult<SystemEntryDto>> UpdatePrefix([FromBody] SystemEntryCreateDto updateDto, [FromQuery] int id)
    {
        updateDto.Type = "P";
        return await UpdateSystemEntry(updateDto, id);
    }

    #endregion

    #region Client Status Endpoints

    /// <summary>
    /// Retrieves a list of client statuses.
    /// </summary>
    [HttpGet("clientStatus")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetClientStatuses()
    {
        return await GetSystemEntriesByType("S");
    }

    /// <summary>
    /// Creates a new client status.
    /// </summary>
    [HttpPost("clientStatus")]
    public async Task<ActionResult<SystemEntryDto>> CreateClientStatus([FromBody] SystemEntryCreateDto createDto)
    {
        createDto.Type = "S";
        return await CreateSystemEntry(createDto);
    }

    #endregion

    #region Marital Status Endpoints

    /// <summary>
    /// Retrieves a list of marital statuses.
    /// </summary>
    [HttpGet("maritalStatus")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetMaritalStatuses()
    {
        return await GetSystemEntriesByType("M");
    }

    /// <summary>
    /// Retrieves a list of client marital statuses.
    /// </summary>
    [HttpGet("clientMaritalStatus")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetClientMaritalStatuses()
    {
        return await GetSystemEntriesByType("M");
    }

    /// <summary>
    /// Creates a new client marital status.
    /// </summary>
    [HttpPost("clientMaritalStatus")]
    public async Task<ActionResult<SystemEntryDto>> CreateClientMaritalStatus([FromBody] SystemEntryCreateDto createDto)
    {
        createDto.Type = "M";
        return await CreateSystemEntry(createDto);
    }

    /// <summary>
    /// Deletes a client marital status.
    /// </summary>
    [HttpDelete("clientMaritalStatus")]
    public async Task<IActionResult> DeleteClientMaritalStatus([FromQuery] int id)
    {
        var entry = await _context.SystemEntries.FindAsync(id);
        if (entry == null || entry.Type != "M")
        {
            return NotFound($"Marital Status with Id {id} not found.");
        }

        _context.SystemEntries.Remove(entry);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Marital Status deleted successfully." });
    }

    #endregion

    #region Invoice Method Endpoints

    /// <summary>
    /// Retrieves a list of invoice methods.
    /// </summary>
    [HttpGet("invoiceMethod")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetInvoiceMethods()
    {
        return await GetSystemEntriesByType("I");
    }

    #endregion

    #region Reason Endpoints

    /// <summary>
    /// Retrieves a list of reasons.
    /// </summary>
    [HttpGet("reason")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetReasons()
    {
        return await GetSystemEntriesByType("R");
    }

    #endregion

    #region Priority Endpoints

    /// <summary>
    /// Retrieves a list of medical priorities.
    /// </summary>
    [HttpGet("priority")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetPriorities()
    {
        return await GetSystemEntriesByType("PR");
    }

    #endregion

    #region Language Endpoints

    /// <summary>
    /// Retrieves a list of languages.
    /// </summary>
    [HttpGet("language")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetLanguages()
    {
        return await GetSystemEntriesByType("L");
    }

    #endregion

    #region Insurance Class Endpoints

    /// <summary>
    /// Retrieves a list of insurance classifications.
    /// </summary>
    [HttpGet("insuranceclass")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetInsuranceClasses()
    {
        return await GetSystemEntriesByType("IC");
    }

    #endregion

    #region Insurance Care Endpoints

    /// <summary>
    /// Retrieves a list of insurance care entries.
    /// </summary>
    [HttpGet("insurancecare")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetInsuranceCare()
    {
        return await GetSystemEntriesByType("ICR");
    }

    #endregion

    #region Financial Group Endpoints

    /// <summary>
    /// Retrieves a list of financial groups.
    /// </summary>
    [HttpGet("financialgroup")]
    public async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetFinancialGroups()
    {
        return await GetSystemEntriesByType("FG");
    }

    #endregion

    #region Generic Reference Endpoints

    /// <summary>
    /// Retrieves a single reference entry by kind and id.
    /// </summary>
    [HttpGet("reference/{kind}/{id}")]
    public async Task<ActionResult<SystemEntryDto>> GetReference(string kind, int id)
    {
        var typeCode = GetTypeCodeFromKind(kind);
        var entry = await _context.SystemEntries
            .FirstOrDefaultAsync(e => e.Type == typeCode && e.Id == id);

        if (entry == null)
        {
            return NotFound($"Reference {kind} with Id {id} not found.");
        }

        return Ok(MapToSystemEntryDto(entry));
    }

    /// <summary>
    /// Creates a new reference entry.
    /// </summary>
    [HttpPost("reference/{kind}")]
    public async Task<ActionResult<SystemEntryDto>> CreateReference(string kind, [FromBody] SystemEntryCreateDto createDto)
    {
        createDto.Type = GetTypeCodeFromKind(kind);
        return await CreateSystemEntry(createDto);
    }

    /// <summary>
    /// Updates a reference entry.
    /// </summary>
    [HttpPut("reference/{kind}")]
    public async Task<ActionResult<SystemEntryDto>> UpdateReference(string kind, [FromBody] SystemEntryCreateDto updateDto, [FromQuery] int id)
    {
        updateDto.Type = GetTypeCodeFromKind(kind);
        return await UpdateSystemEntry(updateDto, id);
    }

    #endregion

    #region Tarif Endpoints

    /// <summary>
    /// Retrieves a list of tariffs with VAT tax.
    /// </summary>
    [HttpGet("Tarifs")]
    public async Task<ActionResult<IEnumerable<TarifDto>>> GetTarifs([FromQuery] int? id = null)
    {
        var query = _context.Tarifs
            .Include(t => t.VatTax)
            .AsQueryable();

        if (id.HasValue)
        {
            query = query.Where(t => t.Id == id.Value);
        }

        var tarifs = await query
            .OrderBy(t => t.Name)
            .Select(t => new TarifDto
            {
                Id = t.Id,
                MandantId = t.MandantId,
                Name = t.Name,
                Description = t.Description,
                BasePrice = t.BasePrice,
                VatPercentage = t.VatTax != null ? t.VatTax.Percentage : null,
                TotalPrice = t.TotalPrice,
                ValidFrom = t.ValidFrom,
                ValidTo = t.ValidTo,
                IsActive = t.IsActive
            })
            .ToListAsync();

        return Ok(tarifs);
    }

    /// <summary>
    /// Creates a new tariff with VAT tax.
    /// </summary>
    [HttpPost("Tarifs")]
    public async Task<ActionResult<TarifDto>> CreateTarif([FromBody] TarifCreateDto createDto)
    {
        // Handle VAT Tax
        VatTax? vatTax = null;
        if (createDto.VatTaxId.HasValue)
        {
            vatTax = await _context.VatTaxes.FindAsync(createDto.VatTaxId.Value);
        }
        else if (createDto.VatPercentage.HasValue)
        {
            // Find or create VAT tax by percentage
            vatTax = await _context.VatTaxes
                .FirstOrDefaultAsync(v => v.Percentage == createDto.VatPercentage.Value);
            
            if (vatTax == null)
            {
                vatTax = new VatTax
                {
                    MandantId = createDto.MandantId,
                    Name = $"VAT {createDto.VatPercentage}%",
                    Percentage = createDto.VatPercentage,
                    Code = $"V{createDto.VatPercentage}",
                    IsActive = true,
                    CreateDate = DateTime.UtcNow
                };
                _context.VatTaxes.Add(vatTax);
                await _context.SaveChangesAsync();
            }
        }

        var tarif = new Tarif
        {
            MandantId = createDto.MandantId,
            Name = createDto.Name,
            Description = createDto.Description,
            BasePrice = createDto.BasePrice,
            VatTaxId = vatTax?.Id,
            TotalPrice = createDto.BasePrice * (1 + (vatTax?.Percentage ?? 0) / 100),
            ValidFrom = createDto.ValidFrom,
            ValidTo = createDto.ValidTo,
            IsActive = createDto.IsActive,
            CreateId = "system",
            CreateDate = DateTime.UtcNow
        };

        _context.Tarifs.Add(tarif);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTarifs), new { id = tarif.Id }, new TarifDto
        {
            Id = tarif.Id,
            MandantId = tarif.MandantId,
            Name = tarif.Name,
            Description = tarif.Description,
            BasePrice = tarif.BasePrice,
            VatPercentage = vatTax?.Percentage,
            TotalPrice = tarif.TotalPrice,
            ValidFrom = tarif.ValidFrom,
            ValidTo = tarif.ValidTo,
            IsActive = tarif.IsActive
        });
    }

    /// <summary>
    /// Updates a tariff and its associated VAT tax.
    /// </summary>
    [HttpPut("Tarifs")]
    public async Task<ActionResult<TarifDto>> UpdateTarif([FromBody] TarifCreateDto updateDto, [FromQuery] int id)
    {
        var tarif = await _context.Tarifs
            .Include(t => t.VatTax)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tarif == null)
        {
            return NotFound($"Tarif with Id {id} not found.");
        }

        // Handle VAT Tax update
        VatTax? vatTax = tarif.VatTax;
        if (updateDto.VatTaxId.HasValue && updateDto.VatTaxId != tarif.VatTaxId)
        {
            vatTax = await _context.VatTaxes.FindAsync(updateDto.VatTaxId.Value);
        }
        else if (updateDto.VatPercentage.HasValue && updateDto.VatPercentage != tarif.VatTax?.Percentage)
        {
            vatTax = await _context.VatTaxes
                .FirstOrDefaultAsync(v => v.Percentage == updateDto.VatPercentage.Value);
            
            if (vatTax == null)
            {
                vatTax = new VatTax
                {
                    MandantId = updateDto.MandantId,
                    Name = $"VAT {updateDto.VatPercentage}%",
                    Percentage = updateDto.VatPercentage,
                    Code = $"V{updateDto.VatPercentage}",
                    IsActive = true,
                    CreateDate = DateTime.UtcNow
                };
                _context.VatTaxes.Add(vatTax);
                await _context.SaveChangesAsync();
            }
        }

        tarif.Name = updateDto.Name ?? tarif.Name;
        tarif.Description = updateDto.Description ?? tarif.Description;
        tarif.BasePrice = updateDto.BasePrice ?? tarif.BasePrice;
        tarif.VatTaxId = vatTax?.Id ?? tarif.VatTaxId;
        tarif.TotalPrice = tarif.BasePrice * (1 + (vatTax?.Percentage ?? 0) / 100);
        tarif.ValidFrom = updateDto.ValidFrom ?? tarif.ValidFrom;
        tarif.ValidTo = updateDto.ValidTo ?? tarif.ValidTo;
        tarif.IsActive = updateDto.IsActive;
        tarif.UpdateId = "system";
        tarif.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new TarifDto
        {
            Id = tarif.Id,
            MandantId = tarif.MandantId,
            Name = tarif.Name,
            Description = tarif.Description,
            BasePrice = tarif.BasePrice,
            VatPercentage = vatTax?.Percentage,
            TotalPrice = tarif.TotalPrice,
            ValidFrom = tarif.ValidFrom,
            ValidTo = tarif.ValidTo,
            IsActive = tarif.IsActive
        });
    }

    #endregion

    #region Report/View Endpoint

    /// <summary>
    /// Retrieves data from a database view.
    /// </summary>
    [HttpGet("view")]
    public async Task<ActionResult<object>> GetView([FromQuery] string viewName)
    {
        // For demo purposes, return data based on view name
        // In production, this would query actual database views
        return viewName.ToLower() switch
        {
            "clients" => Ok(await _context.ClientDetails.Take(100).ToListAsync()),
            "devices" => Ok(await _context.DeviceDetails.Take(100).ToListAsync()),
            "providers" => Ok(await _context.ProfessionalProviderDetails.Take(100).ToListAsync()),
            _ => NotFound($"View \'{viewName}\' not found.")
        };
    }

    #endregion

    #region Helper Methods

    private async Task<ActionResult<IEnumerable<SystemEntryDto>>> GetSystemEntriesByType(string type)
    {
        var entries = await _context.SystemEntries
            .Where(e => e.Type == type && e.IsActive)
            .OrderBy(e => e.SortOrder)
            .ThenBy(e => e.Description)
            .Select(e => MapToSystemEntryDto(e))
            .ToListAsync();

        return Ok(entries);
    }

    private async Task<ActionResult<SystemEntryDto>> CreateSystemEntry(SystemEntryCreateDto createDto)
    {
        var entry = new SystemEntry
        {
            MandantId = createDto.MandantId,
            Type = createDto.Type,
            Description = createDto.Description,
            BooleanOptions = createDto.BooleanOptions,
            Code = createDto.Code,
            SortOrder = createDto.SortOrder,
            IsActive = createDto.IsActive,
            CreateId = "system",
            CreateDate = DateTime.UtcNow
        };

        _context.SystemEntries.Add(entry);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReference), 
            new { kind = GetKindFromTypeCode(entry.Type!), id = entry.Id }, 
            MapToSystemEntryDto(entry));
    }

    private async Task<ActionResult<SystemEntryDto>> UpdateSystemEntry(SystemEntryCreateDto updateDto, int id)
    {
        var entry = await _context.SystemEntries.FindAsync(id);
        if (entry == null)
        {
            return NotFound($"System Entry with Id {id} not found.");
        }

        entry.Description = updateDto.Description ?? entry.Description;
        entry.BooleanOptions = updateDto.BooleanOptions ?? entry.BooleanOptions;
        entry.Code = updateDto.Code ?? entry.Code;
        entry.SortOrder = updateDto.SortOrder ?? entry.SortOrder;
        entry.IsActive = updateDto.IsActive;
        entry.UpdateId = "system";
        entry.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(MapToSystemEntryDto(entry));
    }

    private SystemEntryDto MapToSystemEntryDto(SystemEntry entry)
    {
        return new SystemEntryDto
        {
            Id = entry.Id,
            MandantId = entry.MandantId,
            Type = entry.Type,
            Description = entry.Description,
            BooleanOptions = entry.BooleanOptions,
            Code = entry.Code,
            SortOrder = entry.SortOrder,
            IsActive = entry.IsActive
        };
    }

    private string GetTypeCodeFromKind(string kind)
    {
        return kind.ToUpper() switch
        {
            "TITLE" => "T",
            "PREFIX" => "P",
            "CLIENTSTATUS" => "S",
            "MARITALSTATUS" => "M",
            "INVOICEMETHOD" => "I",
            "REASON" => "R",
            "PRIORITY" => "PR",
            "LANGUAGE" => "L",
            "INSURANCECLASS" => "IC",
            "INSURANCECARE" => "ICR",
            "FINANCIALGROUP" => "FG",
            _ => ""
        };
    }

    private string GetKindFromTypeCode(string typeCode)
    {
        return typeCode switch
        {
            "T" => "title",
            "P" => "prefix",
            "S" => "clientStatus",
            "M" => "maritalStatus",
            "I" => "invoiceMethod",
            "R" => "reason",
            "PR" => "priority",
            "L" => "language",
            "IC" => "insuranceClass",
            "ICR" => "insuranceCare",
            "FG" => "financialGroup",
            _ => "unknown"
        };
    }

    #endregion
}
