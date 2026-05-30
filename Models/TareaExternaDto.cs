using System;

namespace PRACTICA_03.Models
{
    public class TareaExternaDto
    {
        public int ExternalId { get; set; }
        public string Titulo { get; set; } = null!;
        public bool Completado { get; set; }
    }
}
