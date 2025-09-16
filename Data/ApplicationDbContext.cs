using Microsoft.EntityFrameworkCore;
using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Data;

public sealed class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

  public DbSet<Jugador> Jugadores { get; set; }
  public DbSet<Torneo> Torneos { get; set; }
  public DbSet<Equipo> Equipos { get; set; }
  public DbSet<Partido> Partidos { get; set; }
}