using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;

namespace UMOApi.Controllers;

[ApiController]
[Route("")]
public class ProvidersController : ControllerBase
{
    private readonly UMOApiDbContext _context;

    public ProvidersController(UMOApiDbContext context)
    {
        _context = context;
    }

    #region Direct Provider

    /// <summary>
    /// Retrieves a paginated list of direct providers.
    /// </summary>
    [HttpGet("directProvider")]
    public async Task<ActionResult<PaginatedResponse<DirectProviderListDto>>> GetDirectProviders(
        [FromQuery] int startIndex = 0,
        [FromQuery] int count = 20)
    {
        var totalCount = await _context.DirectProviderDetails.CountAsync();
        var pageCount = (int)Math.Ceiling((double)totalCount / count);

        var providers = await _context.DirectProviderDetails
            .OrderBy(p => p.Name)
            .Skip(startIndex)
            .Take(count)
            .Select(p => new DirectProviderListDto
            {
                Id = p.Id,
                MandantId = p.MandantId,
                Name = p.Name,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Phone = p.Phone,
                Email = p.Email,
                Status = p.Status
            })
            .ToListAsync();

        return Ok(new PaginatedResponse<DirectProviderListDto>
        {
            Count = totalCount,
            Start = startIndex,
            PageCount = pageCount,
            Results = providers
        });
    }

    /// <summary>
    /// Retrieves detailed direct provider information.
    /// </summary>
    [HttpGet("directProviderDetails")]
    public async Task<ActionResult<DirectProviderDetailsDto>> GetDirectProviderDetails(
        [FromQuery] int mandantId,
        [FromQuery] int id)
    {
        var provider = await _context.DirectProviderDetails
            .FirstOrDefaultAsync(p => p.MandantId == mandantId && p.Id == id);

        if (provider == null)
        {
            return NotFound($"Direct Provider with MandantId {mandantId} and Id {id} not found.");
        }

        return Ok(MapToDirectProviderDetailsDto(provider));
    }

    /// <summary>
    /// Creates a new direct provider.
    /// </summary>
    [HttpPost("directProviderDetails")]
    public async Task<ActionResult<DirectProviderDetailsDto>> CreateDirectProvider([FromBody] DirectProviderCreateDto createDto)
    {
        var provider = new DirectProviderDetails
        {
            MandantId = createDto.MandantId,
            Name = createDto.Name,
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Street = createDto.Street,
            HouseNumber = createDto.HouseNumber,
            ZipCode = createDto.ZipCode,
            City = createDto.City,
            Country = createDto.Country,
            Phone = createDto.Phone,
            Mobile = createDto.Mobile,
            Fax = createDto.Fax,
            Email = createDto.Email,
            Website = createDto.Website,
            Status = createDto.Status,
            Notes = createDto.Notes,
            CreateId = "system",
            CreateDate = DateTime.UtcNow
        };

        _context.DirectProviderDetails.Add(provider);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDirectProviderDetails),
            new { mandantId = provider.MandantId, id = provider.Id },
            MapToDirectProviderDetailsDto(provider));
    }

    /// <summary>
    /// Updates an existing direct provider.
    /// </summary>
    [HttpPut("directProviderDetails")]
    public async Task<ActionResult<DirectProviderDetailsDto>> UpdateDirectProvider([FromBody] DirectProviderCreateDto updateDto, [FromQuery] int id)
    {
        var provider = await _context.DirectProviderDetails.FindAsync(id);
        if (provider == null)
        {
            return NotFound($"Direct Provider with Id {id} not found.");
        }

        provider.Name = updateDto.Name ?? provider.Name;
        provider.FirstName = updateDto.FirstName ?? provider.FirstName;
        provider.LastName = updateDto.LastName ?? provider.LastName;
        provider.Street = updateDto.Street ?? provider.Street;
        provider.HouseNumber = updateDto.HouseNumber ?? provider.HouseNumber;
        provider.ZipCode = updateDto.ZipCode ?? provider.ZipCode;
        provider.City = updateDto.City ?? provider.City;
        provider.Country = updateDto.Country ?? provider.Country;
        provider.Phone = updateDto.Phone ?? provider.Phone;
        provider.Mobile = updateDto.Mobile ?? provider.Mobile;
        provider.Fax = updateDto.Fax ?? provider.Fax;
        provider.Email = updateDto.Email ?? provider.Email;
        provider.Website = updateDto.Website ?? provider.Website;
        provider.Status = updateDto.Status ?? provider.Status;
        provider.Notes = updateDto.Notes ?? provider.Notes;
        provider.UpdateId = "system";
        provider.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(MapToDirectProviderDetailsDto(provider));
    }

    /// <summary>
    /// Deletes a direct provider.
    /// </summary>
    [HttpDelete("directProviderDetails")]
    public async Task<IActionResult> DeleteDirectProvider([FromQuery] int providerId, [FromQuery] int mandantId)
    {
        var provider = await _context.DirectProviderDetails
            .FirstOrDefaultAsync(p => p.MandantId == mandantId && p.Id == providerId);

        if (provider == null)
        {
            return NotFound($"Direct Provider with MandantId {mandantId} and Id {providerId} not found.");
        }

        _context.DirectProviderDetails.Remove(provider);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Direct Provider deleted successfully." });
    }

    #endregion

    #region Professional Provider

    /// <summary>
    /// Retrieves a paginated list of professional providers.
    /// </summary>
    [HttpGet("professionalProvider")]
    public async Task<ActionResult<PaginatedResponse<ProfessionalProviderListDto>>> GetProfessionalProviders(
        [FromQuery] int startIndex = 0,
        [FromQuery] int count = 20)
    {
        var totalCount = await _context.ProfessionalProviderDetails.CountAsync();
        var pageCount = (int)Math.Ceiling((double)totalCount / count);

        var providers = await _context.ProfessionalProviderDetails
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip(startIndex)
            .Take(count)
            .Select(p => new ProfessionalProviderListDto
            {
                Id = p.Id,
                MandantId = p.MandantId,
                Name = p.Name,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Specialty = p.Specialty,
                Phone = p.Phone,
                Email = p.Email,
                Status = p.Status
            })
            .ToListAsync();

        return Ok(new PaginatedResponse<ProfessionalProviderListDto>
        {
            Count = totalCount,
            Start = startIndex,
            PageCount = pageCount,
            Results = providers
        });
    }

    /// <summary>
    /// Retrieves detailed professional provider information.
    /// </summary>
    [HttpGet("professionalProviderDetail")]
    public async Task<ActionResult<ProfessionalProviderDetailsDto>> GetProfessionalProviderDetails(
        [FromQuery] int mandantId,
        [FromQuery] int id)
    {
        var provider = await _context.ProfessionalProviderDetails
            .FirstOrDefaultAsync(p => p.MandantId == mandantId && p.Id == id);

        if (provider == null)
        {
            return NotFound($"Professional Provider with MandantId {mandantId} and Id {id} not found.");
        }

        return Ok(MapToProfessionalProviderDetailsDto(provider));
    }

    /// <summary>
    /// Creates a new professional provider.
    /// </summary>
    [HttpPost("professionalProviderDetails")]
    public async Task<ActionResult<ProfessionalProviderDetailsDto>> CreateProfessionalProvider([FromBody] ProfessionalProviderCreateDto createDto)
    {
        var provider = new ProfessionalProviderDetails
        {
            MandantId = createDto.MandantId,
            Name = createDto.Name,
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Specialty = createDto.Specialty,
            Street = createDto.Street,
            HouseNumber = createDto.HouseNumber,
            ZipCode = createDto.ZipCode,
            City = createDto.City,
            Country = createDto.Country,
            Phone = createDto.Phone,
            Mobile = createDto.Mobile,
            Fax = createDto.Fax,
            Email = createDto.Email,
            Website = createDto.Website,
            Status = createDto.Status,
            LicenseNumber = createDto.LicenseNumber,
            Notes = createDto.Notes,
            Qualifications = createDto.Qualifications,
            CreateId = "system",
            CreateDate = DateTime.UtcNow
        };

        _context.ProfessionalProviderDetails.Add(provider);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProfessionalProviderDetails),
            new { mandantId = provider.MandantId, id = provider.Id },
            MapToProfessionalProviderDetailsDto(provider));
    }

    /// <summary>
    /// Updates an existing professional provider.
    /// </summary>
    [HttpPut("professionalProviderDetails")]
    public async Task<ActionResult<ProfessionalProviderDetailsDto>> UpdateProfessionalProvider([FromBody] ProfessionalProviderCreateDto updateDto, [FromQuery] int id)
    {
        var provider = await _context.ProfessionalProviderDetails.FindAsync(id);
        if (provider == null)
        {
            return NotFound($"Professional Provider with Id {id} not found.");
        }

        provider.Name = updateDto.Name ?? provider.Name;
        provider.FirstName = updateDto.FirstName ?? provider.FirstName;
        provider.LastName = updateDto.LastName ?? provider.LastName;
        provider.Specialty = updateDto.Specialty ?? provider.Specialty;
        provider.Street = updateDto.Street ?? provider.Street;
        provider.HouseNumber = updateDto.HouseNumber ?? provider.HouseNumber;
        provider.ZipCode = updateDto.ZipCode ?? provider.ZipCode;
        provider.City = updateDto.City ?? provider.City;
        provider.Country = updateDto.Country ?? provider.Country;
        provider.Phone = updateDto.Phone ?? provider.Phone;
        provider.Mobile = updateDto.Mobile ?? provider.Mobile;
        provider.Fax = updateDto.Fax ?? provider.Fax;
        provider.Email = updateDto.Email ?? provider.Email;
        provider.Website = updateDto.Website ?? provider.Website;
        provider.Status = updateDto.Status ?? provider.Status;
        provider.LicenseNumber = updateDto.LicenseNumber ?? provider.LicenseNumber;
        provider.Notes = updateDto.Notes ?? provider.Notes;
        provider.Qualifications = updateDto.Qualifications ?? provider.Qualifications;
        provider.UpdateId = "system";
        provider.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(MapToProfessionalProviderDetailsDto(provider));
    }

    /// <summary>
    /// Links a professional provider to a client.
    /// </summary>
    [HttpPost("professionalProviderDetailsLink")]
    public async Task<IActionResult> LinkProfessionalProviderToClient([FromBody] ProfessionalProviderLinkDto linkDto)
    {
        // Check if provider exists
        var provider = await _context.ProfessionalProviderDetails
            .FirstOrDefaultAsync(p => p.Id == linkDto.ProfessionalProviderId);
        if (provider == null)
        {
            return NotFound($"Professional Provider with Id {linkDto.ProfessionalProviderId} not found.");
        }

        // Check if client exists
        var client = await _context.ClientDetails
            .FirstOrDefaultAsync(c => c.Id == linkDto.ClientId);
        if (client == null)
        {
            return NotFound($"Client with Id {linkDto.ClientId} not found.");
        }

        // Create link
        var link = new ProfessionalProviderClientLink
        {
            ProfessionalProviderId = linkDto.ProfessionalProviderId,
            ClientId = linkDto.ClientId,
            MandantId = linkDto.MandantId,
            Role = linkDto.Role,
            LinkDate = DateTime.UtcNow,
            Notes = linkDto.Notes
        };

        _context.ProfessionalProviderClientLinks.Add(link);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Professional Provider linked to Client successfully.", linkId = link.Id });
    }

    #endregion

    #region Helper Methods

    private DirectProviderDetailsDto MapToDirectProviderDetailsDto(DirectProviderDetails provider)
    {
        return new DirectProviderDetailsDto
        {
            Id = provider.Id,
            MandantId = provider.MandantId,
            Name = provider.Name,
            FirstName = provider.FirstName,
            LastName = provider.LastName,
            Street = provider.Street,
            HouseNumber = provider.HouseNumber,
            ZipCode = provider.ZipCode,
            City = provider.City,
            Country = provider.Country,
            Phone = provider.Phone,
            Mobile = provider.Mobile,
            Fax = provider.Fax,
            Email = provider.Email,
            Website = provider.Website,
            Status = provider.Status,
            Notes = provider.Notes,
            CreateId = provider.CreateId,
            CreateDate = provider.CreateDate,
            UpdateId = provider.UpdateId,
            UpdateDate = provider.UpdateDate
        };
    }

    private ProfessionalProviderDetailsDto MapToProfessionalProviderDetailsDto(ProfessionalProviderDetails provider)
    {
        return new ProfessionalProviderDetailsDto
        {
            Id = provider.Id,
            MandantId = provider.MandantId,
            Name = provider.Name,
            FirstName = provider.FirstName,
            LastName = provider.LastName,
            Specialty = provider.Specialty,
            Street = provider.Street,
            HouseNumber = provider.HouseNumber,
            ZipCode = provider.ZipCode,
            City = provider.City,
            Country = provider.Country,
            Phone = provider.Phone,
            Mobile = provider.Mobile,
            Fax = provider.Fax,
            Email = provider.Email,
            Website = provider.Website,
            Status = provider.Status,
            LicenseNumber = provider.LicenseNumber,
            Notes = provider.Notes,
            Qualifications = provider.Qualifications,
            CreateId = provider.CreateId,
            CreateDate = provider.CreateDate,
            UpdateId = provider.UpdateId,
            UpdateDate = provider.UpdateDate
        };
    }

    #endregion
}
