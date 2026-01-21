using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;

namespace UMOApi.Controllers;

[ApiController]
[Route("")]
public class AddressController : ControllerBase
{
    private readonly UMOApiDbContext _context;

    public AddressController(UMOApiDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated list of addresses.
    /// </summary>
    [HttpGet("addresses")]
    public async Task<ActionResult<PaginatedResponse<AddressDto>>> GetAddresses(
        [FromQuery] int startIndex = 0,
        [FromQuery] int count = 20)
    {
        var totalCount = await _context.Addresses.CountAsync();
        var pageCount = (int)Math.Ceiling((double)totalCount / count);

        var addresses = await _context.Addresses
            .Include(a => a.City)
            .Include(a => a.District)
            .Include(a => a.Country)
            .OrderBy(a => a.City!.Name)
            .ThenBy(a => a.Street)
            .Skip(startIndex)
            .Take(count)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                MandantId = a.MandantId,
                Street = a.Street,
                HouseNumber = a.HouseNumber,
                ZipCode = a.ZipCode,
                City = a.City != null ? a.City.Name : null,
                District = a.District != null ? a.District.Name : null,
                Country = a.Country != null ? a.Country.Name : null,
                AdditionalInfo = a.AdditionalInfo
            })
            .ToListAsync();

        return Ok(new PaginatedResponse<AddressDto>
        {
            Count = totalCount,
            Start = startIndex,
            PageCount = pageCount,
            Results = addresses
        });
    }

    /// <summary>
    /// Creates a new address.
    /// </summary>
    [HttpPost("address")]
    public async Task<ActionResult<AddressDto>> CreateAddress([FromBody] AddressCreateDto createDto)
    {
        var address = new Address
        {
            MandantId = createDto.MandantId,
            Street = createDto.Street,
            HouseNumber = createDto.HouseNumber,
            ZipCode = createDto.ZipCode,
            CityId = createDto.CityId,
            DistrictId = createDto.DistrictId,
            CountryId = createDto.CountryId,
            AdditionalInfo = createDto.AdditionalInfo,
            CreateId = "system",
            CreateDate = DateTime.UtcNow
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        // Reload with includes
        var createdAddress = await _context.Addresses
            .Include(a => a.City)
            .Include(a => a.District)
            .Include(a => a.Country)
            .FirstOrDefaultAsync(a => a.Id == address.Id);

        return CreatedAtAction(nameof(GetAddresses), new { id = address.Id }, new AddressDto
        {
            Id = createdAddress!.Id,
            MandantId = createdAddress.MandantId,
            Street = createdAddress.Street,
            HouseNumber = createdAddress.HouseNumber,
            ZipCode = createdAddress.ZipCode,
            City = createdAddress.City?.Name,
            District = createdAddress.District?.Name,
            Country = createdAddress.Country?.Name,
            AdditionalInfo = createdAddress.AdditionalInfo
        });
    }

    /// <summary>
    /// Retrieves a list of cities.
    /// </summary>
    [HttpGet("cities")]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities([FromQuery] int? clientId = null)
    {
        var query = _context.Cities
            .Include(c => c.District)
            .Include(c => c.Country)
            .AsQueryable();

        var cities = await query
            .OrderBy(c => c.Name)
            .Select(c => new CityDto
            {
                Id = c.Id,
                MandantId = c.MandantId,
                Name = c.Name,
                ZipCode = c.ZipCode,
                District = c.District != null ? c.District.Name : null,
                Country = c.Country != null ? c.Country.Name : null
            })
            .ToListAsync();

        return Ok(cities);
    }

    /// <summary>
    /// Retrieves a list of districts.
    /// </summary>
    [HttpGet("districts")]
    public async Task<ActionResult<IEnumerable<DistrictDto>>> GetDistricts([FromQuery] int? id = null)
    {
        var query = _context.Districts
            .Include(d => d.Country)
            .AsQueryable();

        if (id.HasValue)
        {
            query = query.Where(d => d.Id == id.Value);
        }

        var districts = await query
            .OrderBy(d => d.Name)
            .Select(d => new DistrictDto
            {
                Id = d.Id,
                MandantId = d.MandantId,
                Name = d.Name,
                Code = d.Code,
                Country = d.Country != null ? d.Country.Name : null
            })
            .ToListAsync();

        return Ok(districts);
    }

    /// <summary>
    /// Retrieves a list of countries.
    /// </summary>
    [HttpGet("countries")]
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
    {
        var countries = await _context.Countries
            .OrderBy(c => c.Name)
            .Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
                IsoCode = c.IsoCode,
                PhoneCode = c.PhoneCode
            })
            .ToListAsync();

        return Ok(countries);
    }
}
