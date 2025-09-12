using Microsoft.EntityFrameworkCore;
using RankingPadelAPI.Models;

namespace RankingPadelAPI;

public sealed class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

  public DbSet<Jugador> Jugadores { get; set; }
}