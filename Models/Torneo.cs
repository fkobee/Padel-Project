using System;
using System.ComponentModel.DataAnnotations;

namespace RankingPadelAPI.Models
{
    public class Torneo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public string Ubicacion { get; set; }
    }
}