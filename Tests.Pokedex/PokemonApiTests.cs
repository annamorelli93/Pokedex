using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pokedex.Interfaces;
using Pokedex.Models;
using Xunit;

namespace Tests.Pokedex;

public class PokemonEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PokemonEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetPokemon_ShouldReturnOk_WhenPokemonExists()
    {
        // Arrange
        var mockService = new Mock<IPokemonService>();

        var pokemon = new PokemonResponseDto(
            Name: "mewtwo",
            Description: "A powerful Pokémon",
            Habitat: "rare",
            IsLegendary: true
        );

        mockService
            .Setup(s => s.GetPokemonDetailsAsync("mewtwo"))
            .ReturnsAsync(pokemon);

        var client = CreateClient(mockService);

        // Act
        var response = await client.GetAsync("/pokemon/mewtwo", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<PokemonResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
        content.Should().BeEquivalentTo(pokemon);
    }

    [Fact]
    public async Task GetPokemon_ShouldReturnNotFound_WhenPokemonDoesNotExist()
    {
        // Arrange
        var mockService = new Mock<IPokemonService>();

        mockService
            .Setup(s => s.GetPokemonDetailsAsync("missing"))
            .ReturnsAsync((PokemonResponseDto?)null);

        var client = CreateClient(mockService);

        // Act
        var response = await client.GetAsync("/pokemon/missing", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTranslated_ShouldReturnOk_WhenPokemonExists()
    {
        // Arrange
        var mockService = new Mock<IPokemonService>();

        var pokemon = new PokemonResponseDto(
            Name: "pikachu",
            Description: "Translated text",
            Habitat: "forest",
            IsLegendary: false
        );

        mockService
            .Setup(s => s.GetTranslatedPokemonAsync("pikachu"))
            .ReturnsAsync(pokemon);

        var client = CreateClient(mockService);

        // Act
        var response = await client.GetAsync("/pokemon/translated/pikachu", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<PokemonResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
        content.Should().BeEquivalentTo(pokemon);
    }

    [Fact]
    public async Task GetTranslated_ShouldReturnNotFound_WhenServiceReturnsNull()
    {
        // Arrange
        var mockService = new Mock<IPokemonService>();

        mockService
            .Setup(s => s.GetTranslatedPokemonAsync("missing"))
            .ReturnsAsync((PokemonResponseDto?)null);

        var client = CreateClient(mockService);

        // Act
        var response = await client.GetAsync("/pokemon/translated/missing", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private HttpClient CreateClient(Mock<IPokemonService> mockService)
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IPokemonService));

                if (descriptor != null)
                    services.Remove(descriptor);
                
                services.AddSingleton(mockService.Object);
            });
        });

        return factory.CreateClient();
    }
}