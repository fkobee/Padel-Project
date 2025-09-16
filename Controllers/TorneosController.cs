using Microsoft.AspNetCore.Mvc;
using RankingPadelAPI.Services;
using RankingPadelAPI.Domain.Arguments;
using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Controllers;

[ApiController]
[Route("torneos")]
public class TorneosController : ControllerBase
{
    private readonly ITorneosService _torneosService;

    public TorneosController(ITorneosService torneosService)
    {
        _torneosService = torneosService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TorneoDto>> GetTorneos()
    {
        var torneos = _torneosService.GetAllTorneos();
        var dto = torneos.Select(t => new TorneoDto
        {
            Id = t.Id,
            Nombre = t.Nombre,
            Fecha = t.Fecha,
            Ubicacion = t.Ubicacion
        });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TorneoDto>> GetTorneo(int id)
    {
        var torneo = _torneosService.GetTorneoById(id);
        if (torneo == null) return NotFound();

        return Ok(new TorneoDto
        {
            Id = torneo.Id,
            Nombre = torneo.Nombre,
            Fecha = torneo.Fecha,
            Ubicacion = torneo.Ubicacion
        });
    }

    [HttpPost]
    public IActionResult CreateTorneo([FromBody] CreateTorneoDto dto)
    {
        var torneo = new Torneo
        {
            Nombre = dto.Nombre,
            Fecha = dto.FechaInicio,
            Ubicacion = dto.Ubicacion
        };
        _torneosService.AddTorneo(torneo);

        return CreatedAtAction(nameof(GetTorneo), new { id = torneo.Id },
        new CreateTorneoResponse(torneo.Id, torneo.Nombre, torneo.Fecha));
    }

}
