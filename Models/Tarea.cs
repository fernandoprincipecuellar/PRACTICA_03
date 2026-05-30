using System;
using System.ComponentModel.DataAnnotations;

namespace PRACTICA_03.Models
{
    public enum Estado
    {
        Pendiente,
        EnProceso,
        Completada
    }

    public enum Prioridad
    {
        Baja,
        Media,
        Alta
    }

    public class Tarea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }

        [Required]
        public Estado Estado { get; set; }

        [Required]
        public Prioridad Prioridad { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime FechaVencimiento { get; set; }
    }
}
