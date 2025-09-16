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
      _partidoService.Add(partido);
      return Ok(partido);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Partido partido)
    {
      if (id != partido.Id)
        return BadRequest();

      var partidoExistente = _partidoService.GetById(id);
      if (partidoExistente == null)
        return NotFound();

      _partidoService.Update(partido);
      return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var partidoExistente = _partidoService.GetById(id);
      if (partidoExistente == null)
        return NotFound();

      _partidoService.Delete(id);
      return NoContent();
    }
  }
}