# CLAUDE.md — Basketball Fantasy

ASP.NET Core Web API (.NET 10) za praćenje statistike košarkaša i formiranje fantasy timova, sa statičkim frontendom u `wwwroot`. Detalji o entitetima, endpointima i ograničenjima su u `README.md` — pročitaj ga pre većih izmena.

## Stack
- .NET 10, ASP.NET Core Web API, EF Core 10 (SQL Server + InMemory provider)
- ASP.NET Identity + JWT (role: `Admin`, `User` u `Auth/ApplicationRoles.cs`)
- MediatR (CQRS), AutoMapper, FluentValidation, ClosedXML (Excel export)
- Frontend: čist HTML/CSS/JS u `BasketballFantasy.API/wwwroot` (bez frameworka i build koraka)

## Struktura (Clean Architecture, 3 projekta)
- `BasketballFantasy.Domain` — entiteti (`Entities/`) i interfejsi repozitorijuma + `IUnitOfWork` (`Repositories/`). Bez zavisnosti od drugih projekata.
- `BasketballFantasy.Infrastructure` — `BasketballFantasyContext`, implementacije repozitorijuma, `UnitOfWork`, migracije, `ApplicationUser`.
- `BasketballFantasy.API` — kontroleri, `Features/<Resurs>/` (Commands + Queries), `DTOs/`, `Validators/`, `Mapping/`, `Auth/`, `Middleware/`, `Common/`, `Data/DevelopmentDataSeeder.cs`, `wwwroot/`.

## Komande
```bash
dotnet tool restore
dotnet build BasketballFantasy.slnx
# pokretanje bez SQL Servera (obavezno u cloud/Linux okruženju):
DatabaseProvider=InMemory dotnet run --project BasketballFantasy.API/BasketballFantasy.API.csproj
# nova migracija (samo za SQL Server varijantu):
dotnet tool run dotnet-ef migrations add <Ime> --project BasketballFantasy.Infrastructure --startup-project BasketballFantasy.API
```
- App radi na `http://localhost:5126`. Demo admin: `admin` / `Admin123!` (seed iz `IdentitySeeder`).
- U cloud okruženju nema SQL Servera ni LocalDB-a — uvek koristi `DatabaseProvider=InMemory`. Ne menjaj `appsettings*.json` zbog toga.
- Ako `dotnet` nije instaliran, instaliraj .NET 10 SDK pre build-a.
- Trenutno ne postoji test projekat. Posle svake izmene minimum je da `dotnet build` prođe bez grešaka i novih upozorenja.

## Kako se dodaje nova funkcionalnost (prati postojeći obrazac)
1. Entitet u `Domain/Entities`, po potrebi interfejs repozitorijuma i property u `IUnitOfWork`.
2. Implementacija u `Infrastructure/Repositories`, registracija u `UnitOfWork` i `DbSet` + konfiguracija u `BasketballFantasyContext`.
3. DTO-ovi u `API/DTOs/<Resurs>/<Resurs>Dtos.cs` (`Create...Request`, `Update...Request`, `...Dto`).
4. `sealed record` komanda/upit + handler u `API/Features/<Resurs>/<Resurs>Commands.cs` / `<Resurs>Queries.cs`. Handleri rade preko `IUnitOfWork`, ne direktno preko `DbContext`.
5. Validator u `API/Validators/<Resurs>RequestValidators.cs`; validacija se poziva ručno u kontroleru i vraća `ValidationError(...)`.
6. Mapiranje u `Mapping/MappingProfile.cs`.
7. Kontroler nasleđuje `ApiControllerBase`, koristi `ISender`, `[Authorize]` na klasi, a `[Authorize(Roles = ApplicationRoles.Admin)]` na izmenama.
8. Ako resurs treba u UI-u, dopuni `wwwroot/app.js` i `index.html`, i ažuriraj `README.md` (endpointi, ograničenja).

## Konvencije
- Kod, imena klasa i promenljivih na engleskom; poruke grešaka korisniku na srpskom latinicom (npr. `"Tim sa Id={id} nije pronadjen."`).
- Greške bacaj kao `NotFoundException` / `BadRequestException` (`Common/Exceptions`) — `ApiExceptionMiddleware` ih pretvara u HTTP odgovore.
- Fantasy poeni se računaju isključivo kroz `Common/Calculations/FantasyPointsCalculator.cs` (formula je u README-u) — ne dupliraj formulu.
- Poštuj poslovna ograničenja iz README-a (max 5 startera, max 4 na klupi, bez duplih igrača/statistika itd.).
- Ne menjaj postojeće migracije; za promenu šeme dodaj novu.
- Ne dodaji NuGet pakete bez razloga; ako dodaješ, objasni zašto u opisu PR-a.

## Način rada
- Za veće zadatke prvo napiši kratak plan, pa implementiraj.
- Male, fokusirane izmene; jedan PR = jedna funkcionalnost.
- Commit poruke kratke, na engleskom, u imperativu (npr. `Add injury filter by team`).
- Pre završetka: `dotnet build` prolazi, a ako si menjao API, ukratko u PR-u navedi koje endpointe treba ručno isprobati.
