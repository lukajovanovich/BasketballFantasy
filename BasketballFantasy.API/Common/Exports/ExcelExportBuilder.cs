using BasketballFantasy.API.Common.Calculations;
using BasketballFantasy.Domain.Entities;
using ClosedXML.Excel;

namespace BasketballFantasy.API.Common.Exports
{
    public static class ExcelExportBuilder
    {
        public static byte[] ExportFantasyTeamStatistics(FantasyTeam fantasyTeam)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("FantasyTeam");

            worksheet.Cell(1, 1).Value = "Fantasy tim";
            worksheet.Cell(1, 2).Value = fantasyTeam.Name;
            worksheet.Cell(2, 1).Value = "Trener";
            worksheet.Cell(2, 2).Value = $"{fantasyTeam.Coach?.FirstName} {fantasyTeam.Coach?.LastName}";

            worksheet.Cell(4, 1).Value = "Igrac";
            worksheet.Cell(4, 2).Value = "Tim";
            worksheet.Cell(4, 3).Value = "Uloga";
            worksheet.Cell(4, 4).Value = "Ukupno poena";
            worksheet.Cell(4, 5).Value = "Ukupno skokova";
            worksheet.Cell(4, 6).Value = "Ukupno asistencija";
            worksheet.Cell(4, 7).Value = "Fantasy poeni";

            var row = 5;

            foreach (var fantasyPlayer in fantasyTeam.FantasyTeamPlayers.OrderBy(x => x.Role).ThenBy(x => x.Player!.LastName))
            {
                var playerStatistics = fantasyPlayer.Player?.PlayerStatistics ?? new List<PlayerStatistic>();

                worksheet.Cell(row, 1).Value = $"{fantasyPlayer.Player?.FirstName} {fantasyPlayer.Player?.LastName}";
                worksheet.Cell(row, 2).Value = fantasyPlayer.Player?.Team?.Name ?? string.Empty;
                worksheet.Cell(row, 3).Value = fantasyPlayer.Role.ToString();
                worksheet.Cell(row, 4).Value = playerStatistics.Sum(x => x.Points);
                worksheet.Cell(row, 5).Value = playerStatistics.Sum(x => x.Rebounds);
                worksheet.Cell(row, 6).Value = playerStatistics.Sum(x => x.Assists);
                worksheet.Cell(row, 7).Value = FantasyPointsCalculator.CalculateTotalFantasyPoints(playerStatistics);

                row++;
            }

            worksheet.Cell(row + 1, 1).Value = "Ukupni fantasy poeni";
            worksheet.Cell(row + 1, 2).Value = fantasyTeam.FantasyTeamPlayers.Sum(x =>
                FantasyPointsCalculator.CalculateTotalFantasyPoints(x.Player?.PlayerStatistics ?? new List<PlayerStatistic>()));

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public static byte[] ExportMatchStatistics(Match match)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("MatchStatistics");

            worksheet.Cell(1, 1).Value = "Utakmica";
            worksheet.Cell(1, 2).Value = $"{match.HomeTeam?.Name} - {match.AwayTeam?.Name}";
            worksheet.Cell(2, 1).Value = "Datum";
            worksheet.Cell(2, 2).Value = match.MatchDate;

            worksheet.Cell(4, 1).Value = "Igrac";
            worksheet.Cell(4, 2).Value = "Tim";
            worksheet.Cell(4, 3).Value = "Poeni";
            worksheet.Cell(4, 4).Value = "Asistencije";
            worksheet.Cell(4, 5).Value = "Skokovi";
            worksheet.Cell(4, 6).Value = "Minuti";
            worksheet.Cell(4, 7).Value = "Ukradene";
            worksheet.Cell(4, 8).Value = "Blokade";
            worksheet.Cell(4, 9).Value = "Turnovers";
            worksheet.Cell(4, 10).Value = "Fantasy poeni";

            var row = 5;

            foreach (var statistic in match.PlayerStatistics.OrderBy(x => x.Player!.Team!.Name).ThenBy(x => x.Player!.LastName))
            {
                worksheet.Cell(row, 1).Value = $"{statistic.Player?.FirstName} {statistic.Player?.LastName}";
                worksheet.Cell(row, 2).Value = statistic.Player?.Team?.Name ?? string.Empty;
                worksheet.Cell(row, 3).Value = statistic.Points;
                worksheet.Cell(row, 4).Value = statistic.Assists;
                worksheet.Cell(row, 5).Value = statistic.Rebounds;
                worksheet.Cell(row, 6).Value = statistic.Minutes;
                worksheet.Cell(row, 7).Value = statistic.Steals;
                worksheet.Cell(row, 8).Value = statistic.Blocks;
                worksheet.Cell(row, 9).Value = statistic.Turnovers;
                worksheet.Cell(row, 10).Value = FantasyPointsCalculator.CalculateFantasyPoints(statistic);

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
