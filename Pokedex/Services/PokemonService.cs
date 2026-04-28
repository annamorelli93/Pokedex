using System.Text.Json;
using Pokedex.Interfaces;
using Pokedex.Models;

namespace Pokedex.Services;

/// <summary>
/// Service responsible for retrieving Pokémon data and applying translations to descriptions.
/// </summary>
public class PokemonService : IPokemonService
{
    private static readonly HttpClient HttpClient = new();
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PokemonService"/> class.
    /// </summary>
    /// <param name="httpClientFactory">Factory used to create configured HTTP clients.</param>
    public PokemonService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Retrieves Pokémon details from the external API.
    /// </summary>
    /// <param name="name">The Pokémon name (case-insensitive).</param>
    /// <returns>
    /// A <see cref="PokemonResponseDto"/> containing basic Pokémon information,
    /// or <c>null</c> if the request fails or the Pokémon is not found.
    /// </returns>
    public async Task<PokemonResponseDto?> GetPokemonDetailsAsync(string name)
    {
        var client = _httpClientFactory.CreateClient("PokeApi");

        try
        {
            // PokéAPI is case-sensitive 
            var response = await client.GetAsync($"pokemon-species/{name.ToLower()}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            // Read stream
            await using var contentStream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(contentStream);
            var root = jsonDoc.RootElement;

            var description = root.GetProperty("flavor_text_entries")
                .EnumerateArray()
                .FirstOrDefault(e => e.GetProperty("language").GetProperty("name").GetString() == "en")
                .GetProperty("flavor_text")
                .GetString();

            // Clean up: PokéAPI can use \f or \n in description
            description = description?.Replace("\n", " ").Replace("\f", " ") ?? "No description available.";

            string habitat = "unknown";
            if (root.TryGetProperty("habitat", out var habitatElement) &&
                habitatElement.ValueKind != JsonValueKind.Null)
            {
                habitat = habitatElement.GetProperty("name").GetString() ?? "unknown";
            }

            var isLegendary = root.GetProperty("is_legendary").GetBoolean();

            // Mapping to DTO
            return new PokemonResponseDto(
                Name: root.GetProperty("name").GetString() ?? name,
                Description: description,
                Habitat: habitat,
                IsLegendary: isLegendary
            );
        }
        catch
        {
            // Logging could be added here
            return null;
        }
    }

    /// <summary>
    /// Retrieves Pokémon details and returns a translated version of its description.
    /// </summary>
    /// <param name="name">The Pokémon name.</param>
    /// <returns>
    /// A <see cref="PokemonResponseDto"/> with translated description,
    /// or <c>null</c> if the Pokémon cannot be retrieved.
    /// </returns>
    public async Task<PokemonResponseDto?> GetTranslatedPokemonAsync(string name)
    {
        var pokemon = await GetPokemonDetailsAsync(name);
        if (pokemon == null) return null;
        
        var translatorType =
            (pokemon.Habitat.Equals("cave", StringComparison.OrdinalIgnoreCase) || pokemon.IsLegendary)
                ? "yoda"
                : "shakespeare";
        
        var translatedDescription = await TranslateTextAsync(pokemon.Description, translatorType);
        
        return pokemon with
        {
            Description = translatedDescription
        };
    }

    /// <summary>
    /// Translates a text using the specified translation style.
    /// Falls back to the original text if the translation fails.
    /// </summary>
    /// <param name="text">The text to translate.</param>
    /// <param name="translatorType">The translation style (e.g., "yoda", "shakespeare").</param>
    /// <returns>The translated text, or the original text if an error occurs.</returns>
    private async Task<string> TranslateTextAsync(string text, string translatorType)
    {
        var url = $"https://api.funtranslations.mercxry.me/v1/translate/{translatorType}";

        var payload = new
        {
            text
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("accept", "application/json");
        request.Content = content;

        try
        {
            var response = await HttpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return text;

            var responseContent = await response.Content.ReadAsStringAsync();
            
            using var doc = JsonDocument.Parse(responseContent);
            var translatedText = doc
                .RootElement
                .GetProperty("contents")
                .GetProperty("translated")
                .GetString();

            return translatedText ?? text;
        }
        catch
        {
            return text;
        }
    }
}