**How to run the project**

**Prerequisites**

Before running the project, make sure you have installed:

.NET SDK 8.0
A code editor or IDE (recommended: Rider, Visual Studio, or VS Code)
Git (to clone the repository)

To verify installation:

dotnet --version

You should see 8.0.x.

**Clone the repository**

git clone <repository-url>
cd <project-folder>

**Run the API**

From the root of the solution:

dotnet restore
dotnet build
dotnet run --project Pokedex

The API will start and display a URL such as:

https://localhost:5001
http://localhost:5000

**Run tests**

To execute all unit and integration tests:

dotnet test

**Test the API manually**

You can use:

Rider HTTP Client (proj.http),
Postman,
curl

Example request:

GET https://localhost:5001/pokemon/pikachu

**What I would improve for a production API**

If this project were moved to production, I would make the following improvements:

1. Security
Add rate limiting to prevent abuse of external APIs
Add API authentication (JWT or API keys)
Validate all incoming input more strictly
2. Resilience & reliability
Replace direct HttpClient usage with:
Polly (retry, circuit breaker, timeout policies)
Add timeout handling for external APIs
Add fallback caching when external services fail
3. Caching layer
Cache Pokémon responses (they rarely change)
Cache translation results (external API dependency is slow/rate-limited)
4. Testing strategy
Separate:
Unit tests (mocked services)
Integration tests (real HTTP pipeline)
Mock external APIs (PokéAPI, translation API)
Add test coverage reports (e.g. Coverlet)
5. Observability
Add structured logging (Serilog or built-in logging improvements)
Add metrics (latency, failure rate)
6. API design improvements
Versioning (/api/v1/pokemon)
Standardized error responses (ProblemDetails)
Consistent naming conventions
7. Performance improvements
Reduce repeated external API calls
Parallelize independent requests when possible
Avoid blocking JSON parsing patterns where async alternatives exist