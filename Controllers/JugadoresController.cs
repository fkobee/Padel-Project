using Microsoft.AspNetCore.Mvc;
using RankingPadelAPI.Models;

namespace RankingPadelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JugadoresController : ControllerBase
    {
        private static List<Jugador> jugadores = new();

        // GET: api/jugadores
        [HttpGet]
        public ActionResult<IEnumerable<Jugador>> Get()
        {
            return jugadores.OrderByDescending(j => j.Puntos).ToList();
        }

        // POST: api/jugadores
        [HttpPost]
        public ActionResult<Jugador> Post(Jugador jugador)
        {
            jugador.Id = jugadores.Count + 1;
            jugadores.Add(jugador);
            return jugador;
        }

        // POST: api/jugadores/partido?ganadorId=1&perdedorId=2
        [HttpPost("partido")]
        public ActionResult RegistrarPartido(int ganadorId, int perdedorId)
        {
            var ganador = jugadores.FirstOrDefault(j => j.Id == ganadorId);
            var perdedor = jugadores.FirstOrDefault(j => j.Id == perdedorId);

            if (ganador == null || perdedor == null)
                return NotFound("Jugador no encontrado");

            ganador.Puntos += 3;   // puntos por ganar
            perdedor.Puntos += 1;   // puntos por participar/perder

            return Ok(new { Mensaje = "Partido registrado", Ganador = ganador, Perdedor = perdedor });
        }
    }
}
