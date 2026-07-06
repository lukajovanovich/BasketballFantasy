# Basketball Fantasy

ASP.NET Core Web API projekat za pracenje statistike kosarkasa i formiranje fantasy tima.
Arhitektura je dopunjena sa `ASP.NET Identity`, `JWT` autentikacijom, `MediatR` CQRS pristupom i `AutoMapper` mapiranjem DTO modela.

## Struktura foldera

```text
projekat/
|- BasketballFantasy.slnx
|- dotnet-tools.json
|- README.md
|- BasketballFantasy.Domain/
|  |- Entities/
|  |- Repositories/
|- BasketballFantasy.Infrastructure/
|  |- Repositories/
|  |- Migrations/
|  |- BasketballFantasyContext.cs
|  |- UnitOfWork.cs
|- BasketballFantasy.API/
|  |- Controllers/
|  |- DTOs/
|  |- Auth/
|  |- Features/
|  |- Mapping/
|  |- Services/
|  |- Validators/
|  |- wwwroot/
|  |- Program.cs
|  |- appsettings.json
|  |- BasketballFantasy.API.http
|- .config/
|  |- dotnet-tools.json
```

## Entiteti i veze

- `Team` ima vise `Player` i vise `Coach`.
- `Player` pripada jednom `Team`.
- `Coach` pripada jednom `Team`.
- `Match` ima `HomeTeam` i `AwayTeam`.
- `PlayerStatistic` pripada jednom `Player` i jednoj `Match`.
- `Injury` pripada jednom `Player`.
- `FantasyTeam` ima jednog `Coach`.
- `FantasyTeamPlayer` povezuje `FantasyTeam` i `Player` i cuva ulogu `Starter` ili `Bench`.

## Formula za fantasy poene

```text
FantasyPoints = Points + Rebounds * 1.2 + Assists * 1.5 + Steals * 2 + Blocks * 2 - Turnovers
```

## Glavni endpoint-i

### CRUD

- `GET/POST /teams`
- `GET/PUT/DELETE /teams/{id}`
- `GET/POST /players`
- `GET/PUT/DELETE /players/{id}`
- `GET/POST /coaches`
- `GET/PUT/DELETE /coaches/{id}`
- `GET/POST /matches`
- `GET/PUT/DELETE /matches/{id}`
- `GET/POST /player-statistics`
- `GET/PUT/DELETE /player-statistics/{id}`
- `GET/POST /injuries`
- `GET/PUT/DELETE /injuries/{id}`
- `GET/POST /fantasy-teams`
- `GET/PUT/DELETE /fantasy-teams/{id}`

### Auth

- `POST /auth/register`
- `POST /auth/login`

### Dodatni endpoint-i

- `GET /teams/{id}/players`
- `GET /players/{id}/statistics`
- `GET /players/{id}/totals`
- `GET /injuries/active`
- `GET /player-statistics/top-scorers?count=10`
- `POST /fantasy-teams/{id}/players`
- `GET /fantasy-teams/{id}/points`
- `GET /fantasy-teams/{id}/export`
- `GET /matches/{id}/statistics/export`

## Pokretanje projekta

1. Pokreni restore:

```powershell
dotnet restore BasketballFantasy.slnx
```

2. Ako treba, vrati lokalni EF alat:

```powershell
dotnet tool restore
```

3. Kreiraj bazu iz migracije:

```powershell
dotnet tool run dotnet-ef database update --project "BasketballFantasy.Infrastructure\BasketballFantasy.Infrastructure.csproj" --startup-project "BasketballFantasy.API\BasketballFantasy.API.csproj"
```

4. Pokreni aplikaciju:

```powershell
dotnet run --project "BasketballFantasy.API\BasketballFantasy.API.csproj"
```

5. Otvori frontend u browser-u:

- `http://localhost:5126/` prema podrazumevanom `launchSettings.json` profilu

6. Prijava za administraciju:

- korisnicko ime: `admin`
- lozinka: `Admin123!`

Posle login-a koristi JWT token kroz `Authorization: Bearer <token>`.

Frontend je staticki hostovan iz `BasketballFantasy.API/wwwroot` i sluzi za laksi rad sa svim glavnim resursima:

- prijava i registracija korisnika
- pregled i CRUD nad timovima, igracima, trenerima, utakmicama, statistikama, povredama i fantasy timovima
- dodatni pregledi kao sto su top scoreri, aktivne povrede, ukupna statistika igraca i export u Excel

Napomena za development:

- `appsettings.Development.json` koristi `InMemory` provider da bi aplikacija i frontend mogli odmah da se pokrenu bez LocalDB instance
- pri prvom startu development seed ubacuje demo timove, igrace, statistike, povredu i fantasy tim
- `appsettings.json` i SQL Server migracije ostaju dostupni za varijantu sa pravom bazom

## Sta napisati u dokumentaciji

### Verbalni model

Opisati da sistem prati timove, igrace, trenere, utakmice, individualne statistike, povrede i fantasy timove. Posebno naglasiti da fantasy tim koristi stvarne igrace i stvarne statistike iz meceva za racunanje fantasy poena.

### Use case-ovi

- Administracija timova
- Administracija igraca
- Administracija trenera
- Administracija utakmica
- Unos statistike igraca po utakmici
- Evidencija povreda
- Formiranje fantasy tima
- Dodavanje igraca u fantasy tim
- Racunanje ukupnih fantasy poena
- Eksport statistike u Excel

### Domen model

Opisati sve klase iz domena i navesti relacije:

- `Team 1:N Player`
- `Team 1:N Coach`
- `Team 1:N Match` kao domaci tim
- `Team 1:N Match` kao gostujuci tim
- `Player 1:N PlayerStatistic`
- `Match 1:N PlayerStatistic`
- `Player 1:N Injury`
- `Coach 1:N FantasyTeam`
- `FantasyTeam 1:N FantasyTeamPlayer`
- `Player 1:N FantasyTeamPlayer`

### Ogranicenja

- igrac mora pripadati postojecem timu
- broj na dresu mora biti veci od 0
- visina mora biti izmedju 150 i 260 cm
- statisticki atributi ne mogu biti negativni
- `RecoveryDate` ne moze biti pre `InjuryDate`
- u istoj utakmici jedan igrac ne moze imati duplu statistiku
- fantasy tim ne moze imati vise od 5 startera
- fantasy tim ne moze imati vise od 4 igraca na klupi
- isti igrac ne moze biti dodat dva puta u isti fantasy tim

### ClosedXML

Opisati da je `ClosedXML` .NET biblioteka za jednostavno kreiranje i izmenu Excel `.xlsx` fajlova bez rucnog rada sa niskonivojskim OpenXML API-jem. U ovom projektu koristi se za eksport statistike fantasy tima i statistike utakmice u Excel fajl.
