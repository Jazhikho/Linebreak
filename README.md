## Linebreak

Linebreak is a narrative hacking simulation built as a standalone C# console application. The design is guided by `Docs/GDD.md` and additional project rules in `Docs/ProjectRules.md`.

### Repository Layout

- `Docs` – Game design documentation and team process guides.
- `Src` – Application code organised into modules (`Core`, `UI`, `Simulation`, `Conspiracies`, `Commands`, `Saves`, `Data`).
- `Tests` – Automated test projects covering core systems and branching logic.
- `Build` – Output artifacts, packaging scripts, and deployment configuration.
- `Tools` – Development tooling, generators, and auxiliary scripts.

### Getting Started

1. Ensure .NET 8 SDK is installed.
2. Restore dependencies and compile:
   ```bash
   dotnet restore
   dotnet build Linebreak.sln
   ```
3. Execute the automated tests before committing:
   ```bash
   dotnet test
   ```
4. Run the terminal experience from the application project:
   ```bash
   dotnet run --project Src/App/Linebreak.App.csproj
   ```
5. Prototype features according to the roadmap in the GDD.

