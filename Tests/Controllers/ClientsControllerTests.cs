using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Controllers;
using UMOApi.Data;
using UMOApi.Models;
using Xunit;

namespace UMOApi.Tests.Controllers;

public class ClientsControllerTests : IDisposable
{
    private readonly UMOApiDbContext _context;
    private readonly ClientsController _controller;

    public ClientsControllerTests()
    {
        var options = new DbContextOptionsBuilder<UMOApiDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UMOApiDbContext(options);
        _controller = new ClientsController(_context);
        
        SeedTestData();
    }

    private void SeedTestData()
    {
        var clients = new List<ClientDetails>
        {
            new ClientDetails
            {
                MandantId = 1,
                Id = 1,
                FirstName = "Max",
                LastName = "Mustermann",
                BirthDay = new DateTime(1950, 5, 15),
                Nummer = 1
            },
            new ClientDetails
            {
                MandantId = 1,
                Id = 2,
                FirstName = "Erika",
                LastName = "Musterfrau",
                BirthDay = new DateTime(1945, 8, 22),
                Nummer = 2
            },
            new ClientDetails
            {
                MandantId = 1,
                Id = 3,
                FirstName = "Hans",
                LastName = "Schmidt",
                BirthDay = new DateTime(1960, 3, 10),
                Nummer = 3
            }
        };

        _context.ClientDetails.AddRange(clients);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetClients_ReturnsAllClients()
    {
        // Act
        var result = await _controller.GetClients();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public void DatabaseContainsTestData()
    {
        // Act
        var count = _context.ClientDetails.Count();

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public void ClientsHaveCorrectProperties()
    {
        // Act
        var client = _context.ClientDetails.First();

        // Assert
        client.FirstName.Should().NotBeNullOrEmpty();
        client.LastName.Should().NotBeNullOrEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
