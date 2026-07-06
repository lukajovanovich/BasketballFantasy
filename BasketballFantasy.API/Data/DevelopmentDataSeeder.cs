using BasketballFantasy.Domain.Entities;
using BasketballFantasy.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.API.Data
{
    public static class DevelopmentDataSeeder
    {
        public static async Task SeedAsync(BasketballFantasyContext context)
        {
            if (await context.Teams.AnyAsync())
            {
                return;
            }

            var team1 = new Team { Name = "Belgrade Falcons", City = "Beograd" };
            var team2 = new Team { Name = "Novi Sad Wolves", City = "Novi Sad" };

            context.Teams.AddRange(team1, team2);
            await context.SaveChangesAsync();

            var coach1 = new Coach
            {
                FirstName = "Milan",
                LastName = "Jovanovic",
                ExperienceYears = 12,
                TeamId = team1.Id
            };

            var coach2 = new Coach
            {
                FirstName = "Nikola",
                LastName = "Petrovic",
                ExperienceYears = 9,
                TeamId = team2.Id
            };

            context.Coaches.AddRange(coach1, coach2);
            await context.SaveChangesAsync();

            var players = new List<Player>
            {
                new() { FirstName = "Luka", LastName = "Markovic", Position = "PG", Height = 191.5m, JerseyNumber = 4, TeamId = team1.Id },
                new() { FirstName = "Stefan", LastName = "Ilic", Position = "SG", Height = 196.0m, JerseyNumber = 9, TeamId = team1.Id },
                new() { FirstName = "Viktor", LastName = "Savic", Position = "SF", Height = 201.0m, JerseyNumber = 11, TeamId = team1.Id },
                new() { FirstName = "Ognjen", LastName = "Kovacevic", Position = "PF", Height = 206.5m, JerseyNumber = 14, TeamId = team1.Id },
                new() { FirstName = "Andrej", LastName = "Ristic", Position = "C", Height = 211.0m, JerseyNumber = 15, TeamId = team1.Id },
                new() { FirstName = "Danilo", LastName = "Pavlovic", Position = "PG", Height = 189.5m, JerseyNumber = 5, TeamId = team2.Id },
                new() { FirstName = "Filip", LastName = "Djordjevic", Position = "SG", Height = 194.0m, JerseyNumber = 7, TeamId = team2.Id },
                new() { FirstName = "Relja", LastName = "Stankovic", Position = "C", Height = 209.0m, JerseyNumber = 13, TeamId = team2.Id }
            };

            context.Players.AddRange(players);
            await context.SaveChangesAsync();

            var match = new Match
            {
                HomeTeamId = team1.Id,
                AwayTeamId = team2.Id,
                MatchDate = DateTime.UtcNow.AddDays(-3),
                HomeScore = 94,
                AwayScore = 88
            };

            context.Matches.Add(match);
            await context.SaveChangesAsync();

            context.PlayerStatistics.AddRange(
                new PlayerStatistic
                {
                    PlayerId = players[0].Id,
                    MatchId = match.Id,
                    Points = 24,
                    Assists = 9,
                    Rebounds = 4,
                    Minutes = 34,
                    Steals = 2,
                    Blocks = 0,
                    Turnovers = 3
                },
                new PlayerStatistic
                {
                    PlayerId = players[1].Id,
                    MatchId = match.Id,
                    Points = 19,
                    Assists = 4,
                    Rebounds = 5,
                    Minutes = 31,
                    Steals = 1,
                    Blocks = 0,
                    Turnovers = 2
                },
                new PlayerStatistic
                {
                    PlayerId = players[5].Id,
                    MatchId = match.Id,
                    Points = 21,
                    Assists = 7,
                    Rebounds = 3,
                    Minutes = 33,
                    Steals = 1,
                    Blocks = 0,
                    Turnovers = 4
                },
                new PlayerStatistic
                {
                    PlayerId = players[7].Id,
                    MatchId = match.Id,
                    Points = 16,
                    Assists = 1,
                    Rebounds = 11,
                    Minutes = 29,
                    Steals = 0,
                    Blocks = 3,
                    Turnovers = 2
                });

            context.Injuries.Add(new Injury
            {
                PlayerId = players[2].Id,
                Description = "Istegnuce zadnje loze",
                InjuryDate = DateTime.UtcNow.AddDays(-6),
                RecoveryDate = DateTime.UtcNow.AddDays(5),
                IsActive = true
            });

            await context.SaveChangesAsync();

            var fantasyTeam = new FantasyTeam
            {
                Name = "Weekend Winners",
                CoachId = coach1.Id
            };

            context.FantasyTeams.Add(fantasyTeam);
            await context.SaveChangesAsync();

            context.FantasyTeamPlayers.AddRange(
                new FantasyTeamPlayer { FantasyTeamId = fantasyTeam.Id, PlayerId = players[0].Id, Role = FantasyRole.Starter },
                new FantasyTeamPlayer { FantasyTeamId = fantasyTeam.Id, PlayerId = players[1].Id, Role = FantasyRole.Starter },
                new FantasyTeamPlayer { FantasyTeamId = fantasyTeam.Id, PlayerId = players[5].Id, Role = FantasyRole.Starter },
                new FantasyTeamPlayer { FantasyTeamId = fantasyTeam.Id, PlayerId = players[7].Id, Role = FantasyRole.Bench });

            await context.SaveChangesAsync();
        }
    }
}
