using Pokedex.Interfaces;
using Pokedex.Services;
using Pokedex.Models;


var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRAZIONE SERVIZI (Dependency Injection)
builder.Services.AddHttpClient("PokeApi", c => c.BaseAddress = new Uri("https://pokeapi.co/api/v2/"));
builder.Services.AddHttpClient("FunTranslations", c => c.BaseAddress = new Uri("https://funtranslations.mercxry.me/"));

// Registriamo il servizio: IPokemonService verrà risolto con PokemonService
builder.Services.AddScoped<IPokemonService, PokemonService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. MIDDLEWARE CONFIGURATION
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. ENDPOINTS
app.MapGet("/pokemon/{name}", async (string name, IPokemonService service) => 
{
    var pokemon = await service.GetPokemonDetailsAsync(name);
    return pokemon is not null ? Results.Ok(pokemon) : Results.NotFound();
});

app.MapGet("/pokemon/translated/{name}", async (string name, IPokemonService service) => 
{
    var translated = await service.GetTranslatedPokemonAsync(name);
    return translated is not null ? Results.Ok(translated) : Results.NotFound();
});

app.Run();

public partial class Program { }