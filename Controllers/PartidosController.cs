using Microsoft.AspNetCore.Mvc;
using RankingPadelAPI.Domain;
using RankingPadelAPI.Services;

namespace RankingPadelAPI.Controllers
{
  [ApiController]
  [Route("partidos")]
  public class PartidosController : ControllerBase
  {
    private readonly IPartidoService _partidoService;

    public PartidosController(IPartidoService partidoService)
    {
      _partidoService = partidoService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Partido>> GetAll()
    {
      var partidos = _partidoService.GetAll();
      return Ok(partidos);
    }

    [HttpGet("{id}")]
    public ActionResult<Partido> GetById(int id)
    {
      var partido = _partidoService.GetById(id);
      if (partido == null)
        return NotFound();
      return Ok(partido);
    }

    [HttpPost]
    public ActionResult<Partido> Create(Partido partido)
    {
      var created = _partidoService.Create(partido);
      return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Partido partido)
    {
      if (id != partido.Id)
        return BadRequest();

      var updated = _partidoService.Update(id, partido);
      if (updated == null)
        return NotFound();

      return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var deleted = _partidoService.Delete(id);
      if (!deleted)
        return NotFound();

      return NoContent();
    }
  }
}