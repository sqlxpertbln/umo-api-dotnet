using FluentAssertions;
using System.Net;
using System.Net.Http;
using Xunit;

namespace UMOApi.Tests.Integration;

/// <summary>
/// Integration tests for the UMO API endpoints.
/// These tests verify that the API endpoints respond correctly.
/// </summary>
public class ApiIntegrationTests
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public ApiIntegrationTests()
    {
        _client = new HttpClient();
        // Use environment variable or default to local development URL
        _baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") 
            ?? "https://umo-api-dev.azurewebsites.net";
    }

    [Fact]
    public async Task GetClients_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/api/clients");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDevices_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/api/devices");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDirectProviders_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/api/directProvider");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProfessionalProviders_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/api/professionalProvider");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetServiceHubDashboard_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/api/servicehub/dashboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetServiceHubAlerts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/api/servicehub/alerts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SwaggerEndpoint_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/swagger/v1/swagger.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthCheck_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync($"{_baseUrl}/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
