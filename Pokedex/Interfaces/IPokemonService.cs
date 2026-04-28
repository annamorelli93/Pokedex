using Pokedex.Models;

namespace Pokedex.Interfaces;

/// <summary>
/// Interface of the service responsible for retrieving Pokémon data and applying translations to descriptions.
/// </summary>
public interface IPokemonService
{
    /// <summary>
    /// Retrieves Pokémon details from the external API.
    /// </summary>
    /// <param name="name">The Pokémon name (case-insensitive).</param>
    /// <returns>
    /// A <see cref="PokemonResponseDto"/> containing basic Pokémon information,
    /// or <c>null</c> if the request fails or the Pokémon is not found.
    /// </returns>
    Task<PokemonResponseDto?> GetPokemonDetailsAsync(string name);

    /// <summary>
    /// Retrieves Pokémon details and returns a translated version of its description.
    /// </summary>
    /// <param name="name">The Pokémon name.</param>
    /// <returns>
    /// A <see cref="PokemonResponseDto"/> with translated description,
    /// or <c>null</c> if the Pokémon cannot be retrieved.
    /// </returns>
    Task<PokemonResponseDto?> GetTranslatedPokemonAsync(string name);
}