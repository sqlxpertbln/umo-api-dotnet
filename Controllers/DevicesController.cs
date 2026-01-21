using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;

namespace UMOApi.Controllers;

[ApiController]
[Route("")]
public class DevicesController : ControllerBase
{
    private readonly UMOApiDbContext _context;

    public DevicesController(UMOApiDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated list of devices.
    /// </summary>
    [HttpGet("devices")]
    public async Task<ActionResult<PaginatedResponse<DeviceListDto>>> GetDevices(
        [FromQuery] int startIndex = 0,
        [FromQuery] int count = 20)
    {
        var totalCount = await _context.DeviceDetails.CountAsync();
        var pageCount = (int)Math.Ceiling((double)totalCount / count);

        var devices = await _context.DeviceDetails
            .OrderBy(d => d.DeviceType)
            .ThenBy(d => d.SerialNumber)
            .Skip(startIndex)
            .Take(count)
            .Select(d => new DeviceListDto
            {
                Id = d.Id,
                MandantId = d.MandantId,
                DeviceType = d.DeviceType,
                SerialNumber = d.SerialNumber,
                Description = d.Description,
                Status = d.Status,
                Manufacturer = d.Manufacturer,
                Model = d.Model
            })
            .ToListAsync();

        return Ok(new PaginatedResponse<DeviceListDto>
        {
            Count = totalCount,
            Start = startIndex,
            PageCount = pageCount,
            Results = devices
        });
    }

    /// <summary>
    /// Retrieves a device by MandantId and Id.
    /// </summary>
    [HttpGet("device/{mandantId}-{id}")]
    public async Task<ActionResult<DeviceListDto>> GetDevice(int mandantId, int id)
    {
        var device = await _context.DeviceDetails
            .FirstOrDefaultAsync(d => d.MandantId == mandantId && d.Id == id);

        if (device == null)
        {
            return NotFound($"Device with MandantId {mandantId} and Id {id} not found.");
        }

        return Ok(new DeviceListDto
        {
            Id = device.Id,
            MandantId = device.MandantId,
            DeviceType = device.DeviceType,
            SerialNumber = device.SerialNumber,
            Description = device.Description,
            Status = device.Status,
            Manufacturer = device.Manufacturer,
            Model = device.Model
        });
    }

    /// <summary>
    /// Retrieves detailed device information.
    /// </summary>
    [HttpGet("deviceDetails/{mandantId}-{id}")]
    public async Task<ActionResult<DeviceDetailsDto>> GetDeviceDetails(int mandantId, int id)
    {
        var device = await _context.DeviceDetails
            .FirstOrDefaultAsync(d => d.MandantId == mandantId && d.Id == id);

        if (device == null)
        {
            return NotFound($"Device with MandantId {mandantId} and Id {id} not found.");
        }

        return Ok(MapToDeviceDetailsDto(device));
    }

    /// <summary>
    /// Creates a new device.
    /// </summary>
    [HttpPost("devicedetails")]
    public async Task<ActionResult<DeviceDetailsDto>> CreateDevice([FromBody] DeviceCreateDto createDto)
    {
        var device = new DeviceDetails
        {
            MandantId = createDto.MandantId,
            DeviceType = createDto.DeviceType,
            SerialNumber = createDto.SerialNumber,
            Description = createDto.Description,
            Status = createDto.Status,
            Manufacturer = createDto.Manufacturer,
            Model = createDto.Model,
            PurchaseDate = createDto.PurchaseDate,
            WarrantyEndDate = createDto.WarrantyEndDate,
            PurchasePrice = createDto.PurchasePrice,
            Location = createDto.Location,
            Notes = createDto.Notes,
            AssignedClientId = createDto.AssignedClientId,
            CreateId = "system",
            CreateDate = DateTime.UtcNow
        };

        _context.DeviceDetails.Add(device);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeviceDetails),
            new { mandantId = device.MandantId, id = device.Id },
            MapToDeviceDetailsDto(device));
    }

    /// <summary>
    /// Updates an existing device.
    /// </summary>
    [HttpPut("devicedetails")]
    public async Task<ActionResult<DeviceDetailsDto>> UpdateDevice([FromBody] DeviceCreateDto updateDto, [FromQuery] int id)
    {
        var device = await _context.DeviceDetails.FindAsync(id);
        if (device == null)
        {
            return NotFound($"Device with Id {id} not found.");
        }

        device.DeviceType = updateDto.DeviceType ?? device.DeviceType;
        device.SerialNumber = updateDto.SerialNumber ?? device.SerialNumber;
        device.Description = updateDto.Description ?? device.Description;
        device.Status = updateDto.Status ?? device.Status;
        device.Manufacturer = updateDto.Manufacturer ?? device.Manufacturer;
        device.Model = updateDto.Model ?? device.Model;
        device.PurchaseDate = updateDto.PurchaseDate ?? device.PurchaseDate;
        device.WarrantyEndDate = updateDto.WarrantyEndDate ?? device.WarrantyEndDate;
        device.PurchasePrice = updateDto.PurchasePrice ?? device.PurchasePrice;
        device.Location = updateDto.Location ?? device.Location;
        device.Notes = updateDto.Notes ?? device.Notes;
        device.AssignedClientId = updateDto.AssignedClientId ?? device.AssignedClientId;
        device.UpdateId = "system";
        device.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(MapToDeviceDetailsDto(device));
    }

    /// <summary>
    /// Deletes a device.
    /// </summary>
    [HttpDelete("devicedetails")]
    public async Task<IActionResult> DeleteDevice([FromQuery] int mandantId, [FromQuery] int id)
    {
        var device = await _context.DeviceDetails
            .FirstOrDefaultAsync(d => d.MandantId == mandantId && d.Id == id);

        if (device == null)
        {
            return NotFound($"Device with MandantId {mandantId} and Id {id} not found.");
        }

        _context.DeviceDetails.Remove(device);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Device deleted successfully." });
    }

    private DeviceDetailsDto MapToDeviceDetailsDto(DeviceDetails device)
    {
        return new DeviceDetailsDto
        {
            Id = device.Id,
            MandantId = device.MandantId,
            DeviceType = device.DeviceType,
            SerialNumber = device.SerialNumber,
            Description = device.Description,
            Status = device.Status,
            Manufacturer = device.Manufacturer,
            Model = device.Model,
            PurchaseDate = device.PurchaseDate,
            WarrantyEndDate = device.WarrantyEndDate,
            PurchasePrice = device.PurchasePrice,
            Location = device.Location,
            Notes = device.Notes,
            AssignedClientId = device.AssignedClientId,
            CreateId = device.CreateId,
            CreateDate = device.CreateDate,
            UpdateId = device.UpdateId,
            UpdateDate = device.UpdateDate
        };
    }
}
