using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Controllers;
using UMOApi.Data;
using UMOApi.Models;
using Xunit;

namespace UMOApi.Tests.Controllers;

public class DevicesControllerTests : IDisposable
{
    private readonly UMOApiDbContext _context;
    private readonly DevicesController _controller;

    public DevicesControllerTests()
    {
        var options = new DbContextOptionsBuilder<UMOApiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UMOApiDbContext(options);
        _controller = new DevicesController(_context);
        
        SeedTestData();
    }

    private void SeedTestData()
    {
        var devices = new List<DeviceDetails>
        {
            new DeviceDetails
            {
                MandantId = 1,
                Id = 1,
                SerialNumber = "TUN-2024-001",
                DeviceType = "Hausnotrufgerät",
                Status = "In Benutzung",
                Manufacturer = "Tunstall"
            },
            new DeviceDetails
            {
                MandantId = 1,
                Id = 2,
                SerialNumber = "NEA-2024-002",
                DeviceType = "Mobiler Notruf",
                Status = "Verfügbar",
                Manufacturer = "Neat"
            },
            new DeviceDetails
            {
                MandantId = 1,
                Id = 3,
                SerialNumber = "TEL-2024-003",
                DeviceType = "Rauchmelder",
                Status = "Defekt",
                Manufacturer = "Telealarm"
            }
        };

        _context.DeviceDetails.AddRange(devices);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetDevices_ReturnsAllDevices()
    {
        // Act
        var result = await _controller.GetDevices();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public void DatabaseContainsTestData()
    {
        // Act
        var count = _context.DeviceDetails.Count();

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public void DevicesHaveCorrectProperties()
    {
        // Act
        var device = _context.DeviceDetails.First();

        // Assert
        device.SerialNumber.Should().NotBeNullOrEmpty();
        device.DeviceType.Should().NotBeNullOrEmpty();
        device.Manufacturer.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void DevicesHaveValidStatus()
    {
        // Act
        var statuses = _context.DeviceDetails.Select(d => d.Status).ToList();

        // Assert
        statuses.Should().NotBeEmpty();
        statuses.Should().OnlyContain(s => !string.IsNullOrEmpty(s));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
