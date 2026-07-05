using BasketballFantasy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasketballFantasy.Infrastructure
{
    public class BasketballFantasyContext : DbContext
    {
        public BasketballFantasyContext(DbContextOptions<BasketballFantasyContext> options)
            : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Injury> Injuries { get; set; }
        public DbSet<FantasyTeam> FantasyTeams { get; set; }
        public DbSet<FantasyTeamPlayer> FantasyTeamPlayers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().Property(t => t.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Team>().Property(t => t.City).HasMaxLength(100).IsRequired();

            modelBuilder.Entity<Player>().Property(p => p.FirstName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Player>().Property(p => p.LastName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Player>().Property(p => p.Position).HasMaxLength(30).IsRequired();
            modelBuilder.Entity<Player>().Property(p => p.Height).HasPrecision(5, 2);

            modelBuilder.Entity<Coach>().Property(c => c.FirstName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Coach>().Property(c => c.LastName).HasMaxLength(100).IsRequired();

            modelBuilder.Entity<Injury>().Property(i => i.Description).HasMaxLength(300).IsRequired();

            modelBuilder.Entity<FantasyTeam>().Property(ft => ft.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<FantasyTeamPlayer>().Property(ftp => ftp.Role).HasConversion<string>();

            modelBuilder.Entity<Team>()
                .HasMany(t => t.Players)
                .WithOne(p => p.Team)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.Coaches)
                .WithOne(c => c.Team)
                .HasForeignKey(c => c.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerStatistic>()
                .HasOne(ps => ps.Player)
                .WithMany(p => p.PlayerStatistics)
                .HasForeignKey(ps => ps.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerStatistic>()
                .HasOne(ps => ps.Match)
                .WithMany(m => m.PlayerStatistics)
                .HasForeignKey(ps => ps.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Injury>()
                .HasOne(i => i.Player)
                .WithMany(p => p.Injuries)
                .HasForeignKey(i => i.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FantasyTeam>()
                .HasOne(ft => ft.Coach)
                .WithMany(c => c.FantasyTeams)
                .HasForeignKey(ft => ft.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FantasyTeamPlayer>()
                .HasOne(ftp => ftp.FantasyTeam)
                .WithMany(ft => ft.FantasyTeamPlayers)
                .HasForeignKey(ftp => ftp.FantasyTeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FantasyTeamPlayer>()
                .HasOne(ftp => ftp.Player)
                .WithMany(p => p.FantasyTeamPlayers)
                .HasForeignKey(ftp => ftp.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FantasyTeamPlayer>()
                .HasIndex(ftp => new { ftp.FantasyTeamId, ftp.PlayerId })
                .IsUnique();

            modelBuilder.Entity<PlayerStatistic>()
                .HasIndex(ps => new { ps.PlayerId, ps.MatchId })
                .IsUnique();
        }
    }
}
