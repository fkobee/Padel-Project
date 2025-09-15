using Microsoft.EntityFrameworkCore;
using RankingPadelAPI.Models;

namespace RankingPadelAPI.Data;

public sealed class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

  public DbSet<Jugador> Jugadores { get; set; }
}