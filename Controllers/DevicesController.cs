// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Presentation Layer)
// This controller is part of the Presentation Layer in Clean Architecture. It handles HTTP requests
// related to Device entities and translates them into application-level commands or queries.
//
// META-DATA:
//   - Layer: Presentation (API Controller)
//   - Responsibility: Expose Device-related functionality via a RESTful API.
// =================================================================================================

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
