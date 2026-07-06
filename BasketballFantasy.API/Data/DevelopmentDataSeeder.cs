using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.API.Data
{
    public static class DevelopmentDataSeeder
    {
        public static async Task SeedAsync(BasketballFantasyContext context)
        {
            var teamDefinitions = GetTeamDefinitions();
            var matchDefinitions = GetMatchDefinitions();
            var injuryDefinitions = GetInjuryDefinitions();
            var fantasyTeamDefinitions = GetFantasyTeamDefinitions();

            var teamsByName = await EnsureTeamsAsync(context, teamDefinitions);
            var coachesByKey = await EnsureCoachesAsync(context, teamDefinitions, teamsByName);
            var playersByKey = await EnsurePlayersAsync(context, teamDefinitions, teamsByName);
            var matchesByKey = await EnsureMatchesAsync(context, matchDefinitions, teamsByName);

            await EnsurePlayerStatisticsAsync(context, matchDefinitions, matchesByKey, playersByKey);
            await EnsureInjuriesAsync(context, injuryDefinitions, playersByKey);
            await EnsureFantasyTeamsAsync(context, fantasyTeamDefinitions, coachesByKey, playersByKey);
        }

        private static async Task<Dictionary<string, Team>> EnsureTeamsAsync(
            BasketballFantasyContext context,
            IReadOnlyList<SeedTeamDefinition> definitions)
        {
            var teamNames = definitions.Select(item => item.Name).ToList();
            var existingTeams = await context.Teams
                .Where(team => teamNames.Contains(team.Name))
                .ToListAsync();

            var teamsByName = existingTeams.ToDictionary(team => team.Name, StringComparer.OrdinalIgnoreCase);
            var missingTeams = definitions
                .Where(definition => !teamsByName.ContainsKey(definition.Name))
                .Select(definition => new Team
                {
                    Name = definition.Name,
                    City = definition.City
                })
                .ToList();

            if (missingTeams.Count > 0)
            {
                context.Teams.AddRange(missingTeams);
                await context.SaveChangesAsync();

                foreach (var team in missingTeams)
                {
                    teamsByName[team.Name] = team;
                }
            }

            return teamsByName;
        }

        private static async Task<Dictionary<string, Coach>> EnsureCoachesAsync(
            BasketballFantasyContext context,
            IReadOnlyList<SeedTeamDefinition> definitions,
            IReadOnlyDictionary<string, Team> teamsByName)
        {
            var teamIds = teamsByName.Values.Select(team => team.Id).ToList();
            var existingCoaches = await context.Coaches
                .Where(coach => teamIds.Contains(coach.TeamId))
                .ToListAsync();

            var coachesByKey = existingCoaches.ToDictionary(
                coach => CreateCoachKey(teamsByName.Values.First(team => team.Id == coach.TeamId).Name, coach.FirstName, coach.LastName),
                StringComparer.OrdinalIgnoreCase);

            var missingCoaches = new List<Coach>();

            foreach (var definition in definitions)
            {
                var key = CreateCoachKey(definition.Name, definition.Coach.FirstName, definition.Coach.LastName);
                if (coachesByKey.ContainsKey(key))
                {
                    continue;
                }

                var coach = new Coach
                {
                    FirstName = definition.Coach.FirstName,
                    LastName = definition.Coach.LastName,
                    ExperienceYears = definition.Coach.ExperienceYears,
                    TeamId = teamsByName[definition.Name].Id
                };

                missingCoaches.Add(coach);
                coachesByKey[key] = coach;
            }

            if (missingCoaches.Count > 0)
            {
                context.Coaches.AddRange(missingCoaches);
                await context.SaveChangesAsync();
            }

            return coachesByKey;
        }

        private static async Task<Dictionary<string, Player>> EnsurePlayersAsync(
            BasketballFantasyContext context,
            IReadOnlyList<SeedTeamDefinition> definitions,
            IReadOnlyDictionary<string, Team> teamsByName)
        {
            var teamIds = teamsByName.Values.Select(team => team.Id).ToList();
            var existingPlayers = await context.Players
                .Where(player => teamIds.Contains(player.TeamId))
                .ToListAsync();

            var teamNameById = teamsByName.Values.ToDictionary(team => team.Id, team => team.Name);
            var playersByKey = existingPlayers.ToDictionary(
                player => CreatePlayerKey(teamNameById[player.TeamId], player.FirstName, player.LastName),
                StringComparer.OrdinalIgnoreCase);

            var missingPlayers = new List<Player>();

            foreach (var definition in definitions)
            {
                foreach (var playerDefinition in definition.Players)
                {
                    var key = CreatePlayerKey(definition.Name, playerDefinition.FirstName, playerDefinition.LastName);
                    if (playersByKey.ContainsKey(key))
                    {
                        continue;
                    }

                    var player = new Player
                    {
                        FirstName = playerDefinition.FirstName,
                        LastName = playerDefinition.LastName,
                        Position = playerDefinition.Position,
                        Height = playerDefinition.Height,
                        JerseyNumber = playerDefinition.JerseyNumber,
                        TeamId = teamsByName[definition.Name].Id
                    };

                    missingPlayers.Add(player);
                    playersByKey[key] = player;
                }
            }

            if (missingPlayers.Count > 0)
            {
                context.Players.AddRange(missingPlayers);
                await context.SaveChangesAsync();
            }

            return playersByKey;
        }

        private static async Task<Dictionary<string, Match>> EnsureMatchesAsync(
            BasketballFantasyContext context,
            IReadOnlyList<SeedMatchDefinition> definitions,
            IReadOnlyDictionary<string, Team> teamsByName)
        {
            var teamIds = teamsByName.Values.Select(team => team.Id).ToList();
            var existingMatches = await context.Matches
                .Where(match => teamIds.Contains(match.HomeTeamId) || teamIds.Contains(match.AwayTeamId))
                .ToListAsync();

            var matchesByKey = new Dictionary<string, Match>(StringComparer.OrdinalIgnoreCase);
            foreach (var match in existingMatches)
            {
                var homeTeamName = teamsByName.Values.First(team => team.Id == match.HomeTeamId).Name;
                var awayTeamName = teamsByName.Values.First(team => team.Id == match.AwayTeamId).Name;
                matchesByKey[CreateMatchKey(homeTeamName, awayTeamName, match.MatchDate)] = match;
            }

            var missingMatches = new List<Match>();

            foreach (var definition in definitions)
            {
                var key = CreateMatchKey(definition.HomeTeamName, definition.AwayTeamName, definition.MatchDate);
                if (matchesByKey.ContainsKey(key))
                {
                    continue;
                }

                var match = new Match
                {
                    HomeTeamId = teamsByName[definition.HomeTeamName].Id,
                    AwayTeamId = teamsByName[definition.AwayTeamName].Id,
                    MatchDate = definition.MatchDate,
                    HomeScore = definition.HomeScore,
                    AwayScore = definition.AwayScore
                };

                missingMatches.Add(match);
                matchesByKey[key] = match;
            }

            if (missingMatches.Count > 0)
            {
                context.Matches.AddRange(missingMatches);
                await context.SaveChangesAsync();
            }

            return matchesByKey;
        }

        private static async Task EnsurePlayerStatisticsAsync(
            BasketballFantasyContext context,
            IReadOnlyList<SeedMatchDefinition> matchDefinitions,
            IReadOnlyDictionary<string, Match> matchesByKey,
            IReadOnlyDictionary<string, Player> playersByKey)
        {
            var matchIds = matchesByKey.Values.Select(match => match.Id).ToList();
            var existingStatistics = await context.PlayerStatistics
                .Where(statistic => matchIds.Contains(statistic.MatchId))
                .ToListAsync();

            var existingKeys = existingStatistics
                .Select(statistic => $"{statistic.PlayerId}|{statistic.MatchId}")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingStatistics = new List<PlayerStatistic>();

            foreach (var matchDefinition in matchDefinitions)
            {
                var match = matchesByKey[CreateMatchKey(matchDefinition.HomeTeamName, matchDefinition.AwayTeamName, matchDefinition.MatchDate)];

                foreach (var statisticDefinition in matchDefinition.Statistics)
                {
                    var player = playersByKey[CreatePlayerKey(
                        statisticDefinition.TeamName,
                        statisticDefinition.FirstName,
                        statisticDefinition.LastName)];

                    var key = $"{player.Id}|{match.Id}";
                    if (existingKeys.Contains(key))
                    {
                        continue;
                    }

                    missingStatistics.Add(new PlayerStatistic
                    {
                        PlayerId = player.Id,
                        MatchId = match.Id,
                        Points = statisticDefinition.Points,
                        Assists = statisticDefinition.Assists,
                        Rebounds = statisticDefinition.Rebounds,
                        Minutes = statisticDefinition.Minutes,
                        Steals = statisticDefinition.Steals,
                        Blocks = statisticDefinition.Blocks,
                        Turnovers = statisticDefinition.Turnovers
                    });

                    existingKeys.Add(key);
                }
            }

            if (missingStatistics.Count > 0)
            {
                context.PlayerStatistics.AddRange(missingStatistics);
                await context.SaveChangesAsync();
            }
        }

        private static async Task EnsureInjuriesAsync(
            BasketballFantasyContext context,
            IReadOnlyList<SeedInjuryDefinition> definitions,
            IReadOnlyDictionary<string, Player> playersByKey)
        {
            var playerIds = playersByKey.Values.Select(player => player.Id).ToList();
            var existingInjuries = await context.Injuries
                .Where(injury => playerIds.Contains(injury.PlayerId))
                .ToListAsync();

            var existingKeys = existingInjuries
                .Select(injury => $"{injury.PlayerId}|{injury.InjuryDate:O}|{injury.Description}")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingInjuries = new List<Injury>();

            foreach (var definition in definitions)
            {
                var player = playersByKey[CreatePlayerKey(definition.TeamName, definition.FirstName, definition.LastName)];
                var key = $"{player.Id}|{definition.InjuryDate:O}|{definition.Description}";

                if (existingKeys.Contains(key))
                {
                    continue;
                }

                missingInjuries.Add(new Injury
                {
                    PlayerId = player.Id,
                    Description = definition.Description,
                    InjuryDate = definition.InjuryDate,
                    RecoveryDate = definition.RecoveryDate,
                    IsActive = definition.IsActive
                });

                existingKeys.Add(key);
            }

            if (missingInjuries.Count > 0)
            {
                context.Injuries.AddRange(missingInjuries);
                await context.SaveChangesAsync();
            }
        }

        private static async Task EnsureFantasyTeamsAsync(
            BasketballFantasyContext context,
            IReadOnlyList<SeedFantasyTeamDefinition> definitions,
            IReadOnlyDictionary<string, Coach> coachesByKey,
            IReadOnlyDictionary<string, Player> playersByKey)
        {
            var fantasyTeamNames = definitions.Select(item => item.Name).ToList();
            var existingFantasyTeams = await context.FantasyTeams
                .Where(team => fantasyTeamNames.Contains(team.Name))
                .ToListAsync();

            var fantasyTeamsByName = existingFantasyTeams.ToDictionary(team => team.Name, StringComparer.OrdinalIgnoreCase);
            var missingFantasyTeams = new List<FantasyTeam>();

            foreach (var definition in definitions)
            {
                if (fantasyTeamsByName.ContainsKey(definition.Name))
                {
                    continue;
                }

                var fantasyTeam = new FantasyTeam
                {
                    Name = definition.Name,
                    CoachId = coachesByKey[CreateCoachKey(
                        definition.CoachTeamName,
                        definition.CoachFirstName,
                        definition.CoachLastName)].Id
                };

                missingFantasyTeams.Add(fantasyTeam);
                fantasyTeamsByName[definition.Name] = fantasyTeam;
            }

            if (missingFantasyTeams.Count > 0)
            {
                context.FantasyTeams.AddRange(missingFantasyTeams);
                await context.SaveChangesAsync();
            }

            var fantasyTeamIds = fantasyTeamsByName.Values.Select(team => team.Id).ToList();
            var existingLinks = await context.FantasyTeamPlayers
                .Where(link => fantasyTeamIds.Contains(link.FantasyTeamId))
                .ToListAsync();

            var existingKeys = existingLinks
                .Select(link => $"{link.FantasyTeamId}|{link.PlayerId}")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missingLinks = new List<FantasyTeamPlayer>();

            foreach (var definition in definitions)
            {
                var fantasyTeam = fantasyTeamsByName[definition.Name];

                foreach (var playerDefinition in definition.Players)
                {
                    var player = playersByKey[CreatePlayerKey(
                        playerDefinition.TeamName,
                        playerDefinition.FirstName,
                        playerDefinition.LastName)];

                    var key = $"{fantasyTeam.Id}|{player.Id}";
                    if (existingKeys.Contains(key))
                    {
                        continue;
                    }

                    missingLinks.Add(new FantasyTeamPlayer
                    {
                        FantasyTeamId = fantasyTeam.Id,
                        PlayerId = player.Id,
                        Role = playerDefinition.Role
                    });

                    existingKeys.Add(key);
                }
            }

            if (missingLinks.Count > 0)
            {
                context.FantasyTeamPlayers.AddRange(missingLinks);
                await context.SaveChangesAsync();
            }
        }

        private static IReadOnlyList<SeedTeamDefinition> GetTeamDefinitions() =>
            new[]
            {
                new SeedTeamDefinition(
                    "Real Madrid",
                    "Madrid",
                    new SeedCoachDefinition("Chus", "Mateo", 18),
                    new[]
                    {
                        new SeedPlayerDefinition("Facundo", "Campazzo", "PG", 181.0m, 7),
                        new SeedPlayerDefinition("Mario", "Hezonja", "SF", 203.0m, 11),
                        new SeedPlayerDefinition("Walter", "Tavares", "C", 220.0m, 22)
                    }),
                new SeedTeamDefinition(
                    "Panathinaikos",
                    "Athens",
                    new SeedCoachDefinition("Ergin", "Ataman", 25),
                    new[]
                    {
                        new SeedPlayerDefinition("Kostas", "Sloukas", "PG", 190.0m, 10),
                        new SeedPlayerDefinition("Kendrick", "Nunn", "SG", 191.0m, 25),
                        new SeedPlayerDefinition("Mathias", "Lessort", "C", 206.0m, 26)
                    }),
                new SeedTeamDefinition(
                    "Olympiacos",
                    "Piraeus",
                    new SeedCoachDefinition("Georgios", "Bartzokas", 22),
                    new[]
                    {
                        new SeedPlayerDefinition("Thomas", "Walkup", "PG", 193.0m, 6),
                        new SeedPlayerDefinition("Sasha", "Vezenkov", "PF", 206.0m, 14),
                        new SeedPlayerDefinition("Nikola", "Milutinov", "C", 213.0m, 33)
                    }),
                new SeedTeamDefinition(
                    "Fenerbahce",
                    "Istanbul",
                    new SeedCoachDefinition("Sarunas", "Jasikevicius", 16),
                    new[]
                    {
                        new SeedPlayerDefinition("Marko", "Guduric", "SG", 198.0m, 23),
                        new SeedPlayerDefinition("Nigel", "Hayes-Davis", "SF", 203.0m, 11),
                        new SeedPlayerDefinition("Devon", "Hall", "PG", 196.0m, 22)
                    }),
                new SeedTeamDefinition(
                    "Crvena zvezda",
                    "Belgrade",
                    new SeedCoachDefinition("Ioannis", "Sfairopoulos", 24),
                    new[]
                    {
                        new SeedPlayerDefinition("Milos", "Teodosic", "PG", 196.0m, 4),
                        new SeedPlayerDefinition("Nemanja", "Nedovic", "SG", 192.0m, 26),
                        new SeedPlayerDefinition("Joel", "Bolomboy", "C", 206.0m, 21)
                    }),
                new SeedTeamDefinition(
                    "Partizan",
                    "Belgrade",
                    new SeedCoachDefinition("Zeljko", "Obradovic", 30),
                    new[]
                    {
                        new SeedPlayerDefinition("Carlik", "Jones", "PG", 185.0m, 2),
                        new SeedPlayerDefinition("Sterling", "Brown", "SG", 196.0m, 12),
                        new SeedPlayerDefinition("Tyrique", "Jones", "C", 206.0m, 9)
                    })
            };

        private static IReadOnlyList<SeedMatchDefinition> GetMatchDefinitions() =>
            new[]
            {
                new SeedMatchDefinition(
                    "Partizan",
                    "Crvena zvezda",
                    new DateTime(2026, 3, 7, 19, 0, 0, DateTimeKind.Utc),
                    88,
                    82,
                    new[]
                    {
                        new SeedPlayerStatisticDefinition("Partizan", "Carlik", "Jones", 18, 7, 4, 32, 2, 0, 3),
                        new SeedPlayerStatisticDefinition("Partizan", "Sterling", "Brown", 16, 3, 5, 29, 1, 0, 2),
                        new SeedPlayerStatisticDefinition("Crvena zvezda", "Nemanja", "Nedovic", 20, 2, 3, 31, 1, 0, 2),
                        new SeedPlayerStatisticDefinition("Crvena zvezda", "Joel", "Bolomboy", 12, 1, 8, 28, 0, 2, 1)
                    }),
                new SeedMatchDefinition(
                    "Real Madrid",
                    "Panathinaikos",
                    new DateTime(2026, 3, 12, 19, 45, 0, DateTimeKind.Utc),
                    90,
                    86,
                    new[]
                    {
                        new SeedPlayerStatisticDefinition("Real Madrid", "Facundo", "Campazzo", 17, 10, 3, 31, 2, 0, 2),
                        new SeedPlayerStatisticDefinition("Real Madrid", "Mario", "Hezonja", 21, 3, 6, 30, 1, 1, 2),
                        new SeedPlayerStatisticDefinition("Panathinaikos", "Kendrick", "Nunn", 24, 4, 3, 33, 2, 0, 3),
                        new SeedPlayerStatisticDefinition("Panathinaikos", "Mathias", "Lessort", 14, 2, 7, 29, 1, 1, 2)
                    }),
                new SeedMatchDefinition(
                    "Olympiacos",
                    "Fenerbahce",
                    new DateTime(2026, 3, 19, 18, 15, 0, DateTimeKind.Utc),
                    84,
                    79,
                    new[]
                    {
                        new SeedPlayerStatisticDefinition("Olympiacos", "Thomas", "Walkup", 11, 8, 4, 30, 1, 0, 2),
                        new SeedPlayerStatisticDefinition("Olympiacos", "Sasha", "Vezenkov", 22, 2, 6, 32, 1, 0, 2),
                        new SeedPlayerStatisticDefinition("Fenerbahce", "Marko", "Guduric", 19, 4, 4, 31, 1, 0, 3),
                        new SeedPlayerStatisticDefinition("Fenerbahce", "Nigel", "Hayes-Davis", 17, 2, 5, 34, 2, 1, 1)
                    }),
                new SeedMatchDefinition(
                    "Panathinaikos",
                    "Partizan",
                    new DateTime(2026, 3, 26, 18, 30, 0, DateTimeKind.Utc),
                    93,
                    87,
                    new[]
                    {
                        new SeedPlayerStatisticDefinition("Panathinaikos", "Kostas", "Sloukas", 13, 9, 2, 28, 1, 0, 1),
                        new SeedPlayerStatisticDefinition("Panathinaikos", "Kendrick", "Nunn", 27, 5, 4, 34, 2, 0, 2),
                        new SeedPlayerStatisticDefinition("Partizan", "Carlik", "Jones", 20, 6, 3, 33, 1, 0, 2),
                        new SeedPlayerStatisticDefinition("Partizan", "Tyrique", "Jones", 15, 1, 10, 30, 0, 2, 2)
                    })
            };

        private static IReadOnlyList<SeedInjuryDefinition> GetInjuryDefinitions() =>
            new[]
            {
                new SeedInjuryDefinition(
                    "Panathinaikos",
                    "Mathias",
                    "Lessort",
                    "Povreda kolena",
                    new DateTime(2026, 3, 29, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 4, 22, 0, 0, 0, DateTimeKind.Utc),
                    true),
                new SeedInjuryDefinition(
                    "Crvena zvezda",
                    "Joel",
                    "Bolomboy",
                    "Istegnuce lista",
                    new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 3, 28, 0, 0, 0, DateTimeKind.Utc),
                    false),
                new SeedInjuryDefinition(
                    "Fenerbahce",
                    "Devon",
                    "Hall",
                    "Problemi sa ramenom",
                    new DateTime(2026, 3, 21, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc),
                    true)
            };

        private static IReadOnlyList<SeedFantasyTeamDefinition> GetFantasyTeamDefinitions() =>
            new[]
            {
                new SeedFantasyTeamDefinition(
                    "EuroLeague Stars",
                    "Partizan",
                    "Zeljko",
                    "Obradovic",
                    new[]
                    {
                        new SeedFantasyPlayerDefinition("Real Madrid", "Facundo", "Campazzo", FantasyRole.Starter),
                        new SeedFantasyPlayerDefinition("Panathinaikos", "Kendrick", "Nunn", FantasyRole.Starter),
                        new SeedFantasyPlayerDefinition("Olympiacos", "Sasha", "Vezenkov", FantasyRole.Starter),
                        new SeedFantasyPlayerDefinition("Crvena zvezda", "Joel", "Bolomboy", FantasyRole.Bench)
                    }),
                new SeedFantasyTeamDefinition(
                    "Belgrade Legends",
                    "Crvena zvezda",
                    "Ioannis",
                    "Sfairopoulos",
                    new[]
                    {
                        new SeedFantasyPlayerDefinition("Partizan", "Carlik", "Jones", FantasyRole.Starter),
                        new SeedFantasyPlayerDefinition("Crvena zvezda", "Nemanja", "Nedovic", FantasyRole.Starter),
                        new SeedFantasyPlayerDefinition("Real Madrid", "Mario", "Hezonja", FantasyRole.Starter),
                        new SeedFantasyPlayerDefinition("Panathinaikos", "Mathias", "Lessort", FantasyRole.Bench)
                    })
            };

        private static string CreateCoachKey(string teamName, string firstName, string lastName) =>
            $"{teamName}|{firstName}|{lastName}";

        private static string CreatePlayerKey(string teamName, string firstName, string lastName) =>
            $"{teamName}|{firstName}|{lastName}";

        private static string CreateMatchKey(string homeTeamName, string awayTeamName, DateTime matchDate) =>
            $"{homeTeamName}|{awayTeamName}|{matchDate:O}";

        private sealed record SeedTeamDefinition(
            string Name,
            string City,
            SeedCoachDefinition Coach,
            IReadOnlyList<SeedPlayerDefinition> Players);

        private sealed record SeedCoachDefinition(
            string FirstName,
            string LastName,
            int ExperienceYears);

        private sealed record SeedPlayerDefinition(
            string FirstName,
            string LastName,
            string Position,
            decimal Height,
            int JerseyNumber);

        private sealed record SeedMatchDefinition(
            string HomeTeamName,
            string AwayTeamName,
            DateTime MatchDate,
            int HomeScore,
            int AwayScore,
            IReadOnlyList<SeedPlayerStatisticDefinition> Statistics);

        private sealed record SeedPlayerStatisticDefinition(
            string TeamName,
            string FirstName,
            string LastName,
            int Points,
            int Assists,
            int Rebounds,
            int Minutes,
            int Steals,
            int Blocks,
            int Turnovers);

        private sealed record SeedInjuryDefinition(
            string TeamName,
            string FirstName,
            string LastName,
            string Description,
            DateTime InjuryDate,
            DateTime RecoveryDate,
            bool IsActive);

        private sealed record SeedFantasyTeamDefinition(
            string Name,
            string CoachTeamName,
            string CoachFirstName,
            string CoachLastName,
            IReadOnlyList<SeedFantasyPlayerDefinition> Players);

        private sealed record SeedFantasyPlayerDefinition(
            string TeamName,
            string FirstName,
            string LastName,
            FantasyRole Role);
    }
}
