namespace Pokedex.Models;

public record PokemonResponseDto(
    string Name,
    string Description,
    string Habitat,
    bool IsLegendary);
    
    