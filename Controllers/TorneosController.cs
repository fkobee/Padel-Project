using Microsoft.AspNetCore.Mvc;
using RankingPadelAPI.Models;
using RankingPadelAPI.Data;

namespace RankingPadelAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class TorneosController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public TorneosController(ApplicationDbContext context)
    {
      _context = context;
    }

    [HttpGet]
    public IActionResult GetTorneos()
    {
      var torneos = _context.Torneos.ToList();
      return Ok(torneos);
    }

    [HttpGet("{id}")]
    public IActionResult GetTorneo(int id)
    {
      var torneo = _context.Torneos.Find(id);
      if (torneo == null)
        return NotFound();
      return Ok(torneo);
    }

    [HttpPost]
    public IActionResult CreateTorneo([FromBody] Torneo torneo)
    {
      _context.Torneos.Add(torneo);
      _context.SaveChanges();
      return CreatedAtAction(nameof(GetTorneo), new { id = torneo.Id }, torneo);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTorneo(int id, [FromBody] Torneo torneo)
    {
      var existing = _context.Torneos.Find(id);
      if (existing == null)
        return NotFound();

      existing.Nombre = torneo.Nombre;
      existing.Fecha = torneo.Fecha;
      existing.Ubicacion = torneo.Ubicacion;

      _context.SaveChanges();
      return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTorneo(int id)
    {
      var torneo = _context.Torneos.Find(id);
      if (torneo == null)
        return NotFound();

      _context.Torneos.Remove(torneo);
      _context.SaveChanges();
      return NoContent();
    }
  }
}